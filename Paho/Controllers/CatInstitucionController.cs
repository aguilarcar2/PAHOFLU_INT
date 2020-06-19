using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatInstitucionController : ControllerBase
    {
        private int _pageSize = 10;

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;
            //var userInstitutionId = user.InstitutionID;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullNameSortParm = string.IsNullOrEmpty(sortOrder) ? "fullname_desc" : "";
            ViewBag.NameParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.InstIdSortParm = sortOrder == "instid" ? "instid_desc" : "instid";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //var catalogo = from c in db.Institutions where c.CountryID == countryId select c;
            var catalogo = from c in db.Institutions select c;

            if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution is Hospital)
                {
                    catalogo = catalogo.OfType<Hospital>().Where(j => j.ID == user.InstitutionID || j.Father_ID == user.InstitutionID);
                }
                else if (user.Institution is Lab)
                {
                    catalogo = catalogo.OfType<Lab>().Where(j => j.ID == user.InstitutionID);
                }
            }
            else if (user.Institution.AccessLevel == AccessLevel.Country)
            {
                catalogo = catalogo.Where(j => j.CountryID == countryId);       //.OrderBy(j => j.CountryID).ThenBy(j => j.FullName);
            }

            else if (user.Institution.AccessLevel == AccessLevel.Area)
            {
                if (user.Institution.cod_region_salud != null)
                {
                    catalogo = catalogo.Where(j => j.CountryID == countryId && j.cod_region_salud == user.Institution.cod_region_salud);
                }
                else if ( user.Institution.cod_region_institucional != null)
                {
                    catalogo = catalogo.Where(j => j.CountryID == countryId && j.cod_region_salud == user.Institution.cod_region_institucional);
                }
                else if (user.Institution.cod_region_pais != null)
                {
                    catalogo = catalogo.Where(j => j.CountryID == countryId && j.cod_region_salud == user.Institution.cod_region_pais);
                }


            }

            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.FullName.Contains(searchString) || s.Name.Contains(searchString) || s.Country.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "fullname_desc":
                    catalogo = catalogo.OrderByDescending(s => s.FullName);
                    break;
                case "name":
                    catalogo = catalogo.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    catalogo = catalogo.OrderByDescending(s => s.Name);
                    break;
                case "instid":
                    catalogo = catalogo.OrderBy(s => s.InstID);
                    break;
                case "instid_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstID);
                    break;
                default:
                    //catalogo = catalogo.OrderBy(s => s.FullName);
                    catalogo = catalogo.OrderBy(s => s.CountryID).ThenBy(s => s.FullName);
                    break;
            }



            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);

            //**** Link Dashboard
            string dashbUrl = "", dashbTitle = "";
            List<CatDashboardLink> lista = (from tg in db.CatDashboarLinks
                                            where tg.id_country == user.Institution.CountryID
                                            select tg).ToList();
            if (lista.Count > 0)
            {
                dashbUrl = lista[0].link;
                dashbTitle = lista[0].title;
            }

            ViewBag.DashbUrl = dashbUrl;
            ViewBag.DashbTitle = dashbTitle;
            //****

            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        // GET: CatAgeGroup/Create
        public ActionResult Create()
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //var countryId = user.Institution.CountryID ?? 0;

            //Institution model = new Institution();
            ////string Language = user.Institution.Country.Language;        //#### CAFQ: 180911
            ////ViewBag.UserCountry = countryId;
            //if (countryId != 32)
            //    selectedCountryID = countryId;

            PopulateDepartmentsDropDownList(); 
            return View();
        }

        // POST: CatAgeGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
            "AreaID, FullName, Name, AccessLevel, cod_institution_type, InstID, Father_ID, sentinel, SARI, ILI, surv_unusual, PCR, IFI, Active, orig_country, " + 
            "sentinel, cod_region_institucional, cod_region_salud, cod_region_pais, InstType, OrdenPrioritybyLab, NPHL, LocationTypeID, ForeignCountryID, " +
            "ForeignInstitutionAddress, LabNIC, CountryID, FlowFree, latitude, longitude")]
            Hospital catalog)
        {
            try
            {   
                if (ModelState.IsValid)
                {
                    //var user = UserManager.FindById(User.Identity.GetUserId());
                    //var countryId = user.Institution.CountryID ?? 0;

                    //catalog.CountryID = countryId;

                    if (catalog.InstType == InstitutionType.Admin) {
                        db.Institutions.Add(new AdminInstitution(catalog));
                    } else if (catalog.InstType == InstitutionType.Lab) {
                        db.Institutions.Add(new Lab(catalog));
                    } else {
                        db.Institutions.Add(catalog);
                    }
                                        
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException dex)
            {
                ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador." + "\n" + dex.Message);
            }

            return View(catalog);
        }

        // GET: CatAgeGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var catalogo = db.Institutions.Find(id);
            if (catalogo == null) {
                return HttpNotFound();
            }

            if (catalogo is Hospital) {
                catalogo.InstType = InstitutionType.Hospital;
            } else if (catalogo is Lab) {
                catalogo.InstType = InstitutionType.Lab;
            } else {
                catalogo.InstType = InstitutionType.Admin;
            }

            PopulateDepartmentsDropDownList(catalogo.CountryID, catalogo.AreaID,
                catalogo.AccessLevel, catalogo.InstType, catalogo.cod_region_institucional, 
                catalogo.cod_region_salud, catalogo.cod_region_pais, catalogo.Father_ID, catalogo.InstType,
                catalogo.LocationTypeID, catalogo.ForeignCountryID, catalogo.ForeignInstitutionAddress, catalogo.cod_institution_type);
            
            return View(catalogo);
        }

        /* // POST: CatAgeGroup/Edit/5
        [HttpPost]      ///, ActionName("Edit")]
        [ValidateAntiForgeryToken] */
        //public ActionResult EditPost(int? id, InstitutionType InstType)
        //{
        //    if (id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    var catalog = db.Institutions.Find(id);

        //    if ((int)InstType == 1)
        //        catalog.InstType = InstitutionType.Hospital;
        //    else if ((int)InstType == 2)
        //        catalog.InstType = InstitutionType.Lab;
        //    else if ((int)InstType == 3)
        //        catalog.InstType = InstitutionType.Admin;

        //    //    //,"InstType"
        //    if (TryUpdateModel(catalog, "",
        //           new string[] { "AreaID", "FullName", "Name", "AccessLevel", "InstID", "Father_ID", "SARI", "ILI", "surv_unusual", "PCR", "IFI",
        //               "Active", "sentinel", "orig_country","cod_region_institucional","cod_region_salud","cod_region_pais", "cod_institution_type",
        //               "OrdenPrioritybyLab", "NPHL", "LocationTypeID", "ForeignCountryID", "ForeignInstitutionAddress", "LabNIC", "CountryID" }))
        //    {
        //        try
        //        {


        //            db.SaveChanges();

        //            return RedirectToAction("Index");
        //        }
        //        catch (RetryLimitExceededException /* dex */)
        //        {
        //            ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
        //        }
        //    }
        //    return View(catalog);
        //}
        /*
            [Bind(Include =
            "AreaID, FullName, Name, AccessLevel, cod_institution_type, InstID, Father_ID, sentinel, SARI, ILI, surv_unusual, PCR, IFI, Active, orig_country, " + 
            "sentinel, cod_region_institucional, cod_region_salud, cod_region_pais, InstType, OrdenPrioritybyLab, NPHL, LocationTypeID, ForeignCountryID, " + 
            "ForeignInstitutionAddress, LabNIC, CountryID")]
         **/
        /*
       public ActionResult Edit([Bind(Include = "ID, AreaID, FullName, Name, AccessLevel, cod_institution_type, InstID, Father_ID, sentinel, SARI, ILI, surv_unusual, PCR, IFI, Active, orig_country, " +
           "sentinel, cod_region_institucional, cod_region_salud, cod_region_pais, InstType, OrdenPrioritybyLab, NPHL, LocationTypeID, ForeignCountryID, " +
           "ForeignInstitutionAddress, LabNIC, CountryID, FlowFree, latitude, longitude")] Lab catalog)
       {
           if (ModelState.IsValid)
           {
               //...
               // Empleando area capturado podemos ajustar los valores del modelo a guardar
               //...

               ////if (catalog is Hospital)
               //if (catalog.InstType == InstitutionType.Hospital)
               //{
               //    //catalog.InstType = InstitutionType.Hospital;
               //    catalog.InstitutionType = (int)InstitutionType.Hospital;
               //}
               //else if (catalog.InstType == InstitutionType.Lab)
               //{
               //    //catalog.InstType = InstitutionType.Lab;
               //    catalog.InstitutionType = (int)InstitutionType.Lab;
               //}
               //else
               //{
               //    //catalog.InstType = InstitutionType.Admin;
               //    catalog.InstitutionType = (int)InstitutionType.Admin;
               //}

               db.Entry(catalog).State = EntityState.Modified;
               db.SaveChanges();
               return RedirectToAction("Index");
           }
           //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);

           return View(catalog);
       }
       */

        // POST: CatAgeGroup/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catalogo = db.Institutions.Find(id);
            if (TryUpdateModel(catalogo, "",
               new string[] { "AreaID", "FullName", "Name", "AccessLevel", "InstID", "Father_ID", "SARI", "ILI", "surv_unusual", "PCR", "IFI",
                   "Active", "sentinel", "orig_country","cod_region_institucional","cod_region_salud","cod_region_pais","InstType", "cod_institution_type",
                   "OrdenPrioritybyLab", "NPHL", "LocationTypeID", "ForeignCountryID", "ForeignInstitutionAddress", "LabNIC", "CountryID", "FlowFree", "latitude", "longitude" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
                }
            }

            return View(catalogo);
        }


        // GET: CatAgeGroup/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            var catalogo = db.Institutions.Find(id);
            if (catalogo == null) {
                return HttpNotFound();
            }

            if (catalogo is Hospital)
            {
                catalogo.InstType = InstitutionType.Hospital;
            }
            else if (catalogo is Lab)
            {
                catalogo.InstType = InstitutionType.Lab;
            }
            else
            {
                catalogo.InstType = InstitutionType.Admin;
            }

            PopulateDepartmentsDropDownList(catalogo.CountryID, catalogo.AreaID,
                catalogo.AccessLevel, catalogo.InstType, catalogo.cod_region_institucional,
                catalogo.cod_region_salud, catalogo.cod_region_pais, catalogo.Father_ID, catalogo.InstType,
                catalogo.LocationTypeID, catalogo.InstitutionTypeID, catalogo.LabNIC);

            return View(catalogo);
        }

        // POST: CatAgeGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var catalog  = db.Institutions.Find(id);
                db.Institutions.Remove(catalog);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetInstituciones(string tipo)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            List<SelectListItem> City = new List<SelectListItem>();
            switch (tipo)
            {
                case "Lab":
                    var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
                    instQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
                    return Json(new SelectList(instQuery, "ID", "Name"));
                default:
                    var hospQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId && x.AccessLevel == AccessLevel.SelfOnly).OrderBy(x => x.Name).ToList();
                    hospQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
                    return Json(new SelectList(hospQuery, "ID", "Name"));
            }
        }


        public JsonResult GetAreaOfCountry(int countryID)
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //var countryId = user.Institution.CountryID ?? 0;
            //List<SelectListItem> City = new List<SelectListItem>();

            //switch (tipo)
            //{
            //    case "Lab":
            //        var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
            //        instQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            //        return Json(new SelectList(instQuery, "ID", "Name"));
            //    default:
            //        var hospQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId && x.AccessLevel == AccessLevel.SelfOnly).OrderBy(x => x.Name).ToList();
            //        hospQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            //        return Json(new SelectList(hospQuery, "ID", "Name"));
            //}

            var instQuery = db.Areas.Where(x => x.CountryID == countryID).OrderBy(x => x.Name).ToList();
            if(instQuery.Count > 1)
                instQuery.Insert(0, new Area { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });

            return Json(new SelectList(instQuery, "ID", "Name"));
        }

        private void PopulateDepartmentsDropDownList(
            object selectedCountryID = null,
            object selectedArea = null, 
            object selectedNivel = null, 
            object selectedTipo = null,
            object selectedRegInst = null,
            object selectedRegSalud = null,
            object selectedRePais = null,
            int? selectedFather = null, 
            InstitutionType? instType = null,
            object selectedLocationTypeID = null,
            object selectedForeignCountriesID = null,
            object SelectedForeignInstitutionAddress = null,
            object selectedcodInstitutionTypeID = null)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            var countryId = 0;
            if (selectedCountryID == null)
                countryId = user.Institution.CountryID ?? 0;
            else
                countryId = (Int32)selectedCountryID;

            string Language = user.Institution.Country.Language; 
            ViewBag.UserCountry = countryId;

            //**** Paises
            var countries = db.Countries
                    .Where(c => (countryId == 32) ? c.Active == true : c.ID == countryId)
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = (Language == "SPA") ? c.Name : c.ENG,
                    })
                    .OrderBy(d => d.Name)
                    .ToList();

            if(countries.Count > 1)
                countries.Insert(0, new CountryView { Id = "0", Name = "-- " + getMsg("msgSelect") + " -- " });

            ViewBag.Countries = new SelectList(countries, "ID", "Name", selectedCountryID);

            //**** Areas
            var areasQuery = (from d in db.Areas where d.CountryID == countryId orderby d.Name select d)
                             .ToList();
            if (areasQuery.Count > 1)
                areasQuery.Insert(0, new Area { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            ViewBag.AreaID = new SelectList(areasQuery, "ID", "Name", selectedArea);

            //**** Nivel de acceso: SelfOnly, Country...etc
            var nivelQuery = from AccessLevel e in Enum.GetValues(typeof(AccessLevel)) select new { Id = e, Name = (countryId == 17 && e.ToString() == "Area") ? "Parish":  e.ToString() };
            ViewBag.AccessLevel = new SelectList(nivelQuery, "Id", "Name", selectedNivel);

            //if (user.Institution.CountryID == 17)
            //{
            //    var nivelQuery_JAM = from AccessLevel_JAM e in Enum.GetValues(typeof(AccessLevel)) select new { Id = e, Name = e.ToString() };
            //    ViewBag.AccessLevel = new SelectList(nivelQuery_JAM, "Id", "Name", selectedNivel);
            //}

            //**** Tipo: Hospital, Lab, Admin
            var tipoQuery = from InstitutionType e in Enum.GetValues(typeof(InstitutionType)) select new { Id = e, Name = e.ToString() };
            ViewBag.InstitutionType = new SelectList(tipoQuery, "Id", "Name", selectedTipo);

            //**** 
            var regInstQuery = (from d in db.Regions where (d.tipo_region == 1 || d.tipo_region == null) && d.CountryID == countryId orderby d.Name select d).ToList();
            regInstQuery.Insert(0, new Region { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            ViewBag.cod_region_institucional = new SelectList(regInstQuery, "orig_country", "Name", selectedRegInst);

            var regSaludQuery = (from d in db.Regions where d.tipo_region == 2 && d.CountryID == countryId orderby d.Name select d).ToList();
            regSaludQuery.Insert(0, new Region { ID = 0, Name = "-- " + getMsg("msgSelect") +" -- " });
            ViewBag.cod_region_salud = new SelectList(regSaludQuery, "orig_country", "Name", selectedRegSalud);

            var regPaisQuery = (from d in db.Regions where d.tipo_region == 3 && d.CountryID == countryId orderby d.Name select d).ToList();
            regPaisQuery.Insert(0, new Region { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            ViewBag.cod_region_pais = new SelectList(regPaisQuery, "orig_country", "Name", selectedRePais);

            ViewBag.Father_ID_value = selectedFather;
            if (instType == InstitutionType.Lab)
            {
                var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
                instQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
                ViewBag.Father_ID = new SelectList(instQuery, "ID", "Name", selectedFather);
            } else {
                var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId && x.AccessLevel == AccessLevel.SelfOnly).OrderBy(x => x.Name).ToList();
                instQuery.Insert(0, new Hospital { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
                ViewBag.Father_ID = new SelectList(instQuery, "ID", "Name", selectedFather);
            }

            //**** Ubicacion: Local, Exterior
            if (selectedLocationTypeID == null)
                selectedLocationTypeID = 1;
            var regInstTypeLocaQuery = (from d in db.InstitutionLocationType
                                        select new InstitutionLocationTypeView
                                        {
                                            ID = d.ID,
                                            Name = (Language == "SPA") ? d.Name_SPA : d.Name_ENG
                                        }).ToList();

            regInstTypeLocaQuery.Insert(0, new InstitutionLocationTypeView { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });
            ViewBag.cod_institution_location_type = new SelectList(regInstTypeLocaQuery, "ID", "Name", selectedLocationTypeID);

            //**** Tipo de institución para Honduras
            var regInstTypeHON = db.CatInstitutionTypeHON
                                .Select(c => new InstitutionLocationTypeView()
                                {
                                    ID = c.ID,
                                    Name = (user.Institution.Country.Language == "SPA") ? c.SPA : c.ENG,
                                })
                                .OrderBy(d => d.Name)
                                .ToList();

            regInstTypeHON.Insert(0, new InstitutionLocationTypeView { ID = 0, Name = "-- " + getMsg("msgSelect") + " -- " });

            ViewBag.cod_institution_type = new SelectList(regInstTypeHON, "ID", "Name", selectedcodInstitutionTypeID);

            //**** Paises for ForeignCountries
            var regCountries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = (Language == "SPA") ? c.Name : c.ENG,
                    })
                    .OrderBy(d => d.Name)
                    .ToList();
            regCountries.Insert(0, new CountryView { Id = "0", Name = "-- " + getMsg("msgSelect") + " -- " });
            ViewBag.ForeignCountries = new SelectList(regCountries, "ID", "Name", selectedForeignCountriesID);
        }

        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }
    }
}

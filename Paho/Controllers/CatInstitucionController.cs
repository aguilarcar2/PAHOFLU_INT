using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using System;
using System.Collections.Generic;
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

            var catalogo = from c in db.Institutions where c.CountryID == countryId select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.FullName.Contains(searchString) || s.Name.Contains(searchString));
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
                    catalogo = catalogo.OrderBy(s => s.FullName);
                    break;
            }

            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        // GET: CatAgeGroup/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList(); 
            return View();
        }

        // POST: CatAgeGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = 
            "AreaID, FullName, Name, AccessLevel, InstID, Father_ID, SARI, ILI, PCR, IFI, Active, orig_country, cod_region_institucional, cod_region_salud, cod_region_pais, InstType")]
            Hospital catalog)
        {
            try
            {   
                if (ModelState.IsValid)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    var countryId = user.Institution.CountryID ?? 0;

                    catalog.CountryID = countryId;

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
                ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
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

            PopulateDepartmentsDropDownList(catalogo.AreaID, 
                catalogo.AccessLevel, catalogo.InstType, catalogo.cod_region_institucional, 
                catalogo.cod_region_salud, catalogo.cod_region_pais, catalogo.Father_ID, catalogo.InstType);
            
            return View(catalogo);
        }

        // POST: CatAgeGroup/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catalog = db.Institutions.Find(id);
            if (TryUpdateModel(catalog, "",
               new string[] { "AreaID", "FullName", "Name", "AccessLevel", "InstID", "Father_ID", "SARI", "ILI", "PCR", "IFI",
                   "Active", "orig_country","cod_region_institucional","cod_region_salud","cod_region_pais","InstType" }))
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
            return View(catalog);
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

            PopulateDepartmentsDropDownList(catalogo.AreaID,
                catalogo.AccessLevel, catalogo.InstType, catalogo.cod_region_institucional,
                catalogo.cod_region_salud, catalogo.cod_region_pais, catalogo.Father_ID, catalogo.InstType);

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
                    instQuery.Insert(0, new Hospital { ID = 0, Name = "-- Seleccione -- " });
                    return Json(new SelectList(instQuery, "ID", "Name"));
                default:
                    var hospQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId && x.AccessLevel == AccessLevel.SelfOnly).OrderBy(x => x.Name).ToList();
                    hospQuery.Insert(0, new Hospital { ID = 0, Name = "-- Seleccione -- " });
                    return Json(new SelectList(hospQuery, "ID", "Name"));
            }
        }

        private void PopulateDepartmentsDropDownList(
            object selectedArea = null, 
            object selectedNivel = null, 
            object selectedTipo = null,
            object selectedRegInst = null,
            object selectedRegSalud = null,
            object selectedRePais = null,
            int? selectedFather = null, InstitutionType? instType = null)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            var areasQuery = from d in db.Areas where d.CountryID == countryId orderby d.Name select d;
            ViewBag.AreaID = new SelectList(areasQuery, "ID", "Name", selectedArea);

            var nivelQuery = from AccessLevel e in Enum.GetValues(typeof(AccessLevel)) select new { Id = e, Name = e.ToString() };
            ViewBag.AccessLevel = new SelectList(nivelQuery, "Id", "Name", selectedNivel);

            var tipoQuery = from InstitutionType e in Enum.GetValues(typeof(InstitutionType)) select new { Id = e, Name = e.ToString() };
            ViewBag.InstitutionType = new SelectList(tipoQuery, "Id", "Name", selectedTipo);

            var regInstQuery = (from d in db.Regions where (d.tipo_region == 1 || d.tipo_region == null) && d.CountryID == countryId orderby d.Name select d).ToList();
            regInstQuery.Insert(0, new Region { ID = 0, Name = "-- Seleccione -- " });
            ViewBag.cod_region_institucional = new SelectList(regInstQuery, "ID", "Name", selectedRegInst);

            var regSaludQuery = (from d in db.Regions where d.tipo_region == 2 && d.CountryID == countryId orderby d.Name select d).ToList();
            regSaludQuery.Insert(0, new Region { ID = 0, Name = "-- Seleccione -- " });
            ViewBag.cod_region_salud = new SelectList(regSaludQuery, "ID", "Name", selectedRegSalud);

            var regPaisQuery = (from d in db.Regions where d.tipo_region == 3 && d.CountryID == countryId orderby d.Name select d).ToList();
            regPaisQuery.Insert(0, new Region { ID = 0, Name = "-- Seleccione -- " });
            ViewBag.cod_region_pais = new SelectList(regPaisQuery, "ID", "Name", selectedRePais);
            
            if (instType == InstitutionType.Lab) {
                var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
                instQuery.Insert(0, new Hospital { ID = 0, Name = "-- Seleccione -- " });
                ViewBag.Father_ID = new SelectList(instQuery, "ID", "Name", selectedFather);
            } else {
                var instQuery = db.Institutions.OfType<Hospital>().Where(x => x.CountryID == countryId && x.AccessLevel == AccessLevel.SelfOnly).OrderBy(x => x.Name).ToList();
                instQuery.Insert(0, new Hospital { ID = 0, Name = "-- Seleccione -- " });
                ViewBag.Father_ID = new SelectList(instQuery, "ID", "Name", selectedFather);
            }
        }
    }
}

using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatInstitucionConfController : ControllerBase
    {
        private int _pageSize = 10;

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.FullNameParentSortParm = string.IsNullOrEmpty(sortOrder) ? "fullnameparent_desc" : "fullnameparent";
            ViewBag.FullNameFromSortParm = sortOrder == "fullname" ? "fullname_desc" : "fullname";
            ViewBag.FullNameToSortParm = sortOrder == "fullnameto" ? "fullnameto_desc" : "fullnameto";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.InstitutionsConfiguration
                           where c.InstitutionFrom.CountryID == countryId && 
                           c.InstitutionTo.CountryID == countryId && 
                           c.InstitutionParent.CountryID == countryId
                           select c;

            if (user.Institution.AccessLevel == AccessLevel.Area)
            {
                if (user.Institution.cod_region_salud != null)
                {
                    catalogo = catalogo.Where(j => j.InstitutionParent.cod_region_salud == user.Institution.cod_region_salud);
                }
                else if (user.Institution.cod_region_institucional != null)
                {
                    catalogo = catalogo.Where(j =>  j.InstitutionParent.cod_region_institucional == user.Institution.cod_region_institucional);
                }
                else if (user.Institution.cod_region_pais != null)
                {
                    catalogo = catalogo.Where(j => j.InstitutionParent.cod_region_pais == user.Institution.cod_region_pais);
                }


            }


            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => 
                s.InstitutionFrom.FullName.Contains(searchString) || 
                s.InstitutionTo.FullName.Contains(searchString) ||
                s.InstitutionParent.FullName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "fullname_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionFrom.FullName);
                    break;
                case "fullnameto":
                    catalogo = catalogo.OrderBy(s => s.InstitutionTo.FullName);
                    break;
                case "fullnameto_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionTo.FullName);
                    break;
                case "fullnameparent":
                    catalogo = catalogo.OrderBy(s => s.InstitutionParent.FullName);
                    break;
                case "fullnameparent_desc":
                    catalogo = catalogo.OrderByDescending(s => s.InstitutionParent.FullName);
                    break;
                default:
                    catalogo = catalogo.OrderBy(s => s.InstitutionParent.FullName).ThenBy(w => w.Priority);
                    break;
            }

            if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                catalogo = catalogo.Where(j => j.InstitutionParentID == user.InstitutionID);
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

        // GET: CatInstitucionConf/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: CatInstitucionConf/Create
        [HttpPost]
        public ActionResult Create([Bind(Include = "InstitutionFromID, InstitutionToID, InstitutionParentID, Priority, Conclusion, OpenAlways")] InstitutionConfiguration catalog)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.InstitutionsConfiguration.Add(catalog);
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

        // GET: CatInstitucionConf/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var catalogo = db.InstitutionsConfiguration.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentsDropDownList(catalogo.InstitutionFromID, catalogo.InstitutionToID, catalogo.InstitutionParentID);

            return View(catalogo);
        }

        // POST: CatInstitucionConf/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id, long? InstitutionFromID, long? InstitutionToID, long? InstitutionParentID , int Priority, bool Conclusion, bool OpenAlways)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var count_records_exist = db.InstitutionsConfiguration.Where(z => z.InstitutionParentID == InstitutionParentID && z.InstitutionToID == InstitutionToID && z.InstitutionFromID == InstitutionFromID && z.ID != id).Count();

            var catalog = db.InstitutionsConfiguration.Find(id);
            var InstitutionFromID_original = catalog.InstitutionFromID;
            var InstitutionToID_original = catalog.InstitutionToID;
            var InstitutionParentID_original = catalog.InstitutionParentID;
            var Priority_original = catalog.Priority;
            var Conclusion_original = catalog.Conclusion;
            var OpenAlways_original = catalog.OpenAlways;

            if (count_records_exist >= 1)
            {
                ModelState.AddModelError("", "No es posible guardar los datos. El flujo se encuentra repetido.");
                return View(catalog);
            }

            if (TryUpdateModel(catalog, "",
               new string[] { "InstitutionFromID","InstitutionToID","InstitutionParentID","Priority","Conclusion", "OpenAlways" }))
            {
                try
                {

                    db.SaveChanges();

                    var consStringFlowCorrection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (var conFlowCorrection = new SqlConnection(consStringFlowCorrection))
                    {
                        using (var commandFlowCorrection = new SqlCommand("FlowCorrection", conFlowCorrection) { CommandType = CommandType.StoredProcedure })
                        {
                            var user = UserManager.FindById(User.Identity.GetUserId());
                            commandFlowCorrection.Parameters.Clear();
                            commandFlowCorrection.Parameters.Add("@InstitutionParentID", SqlDbType.Int).Value = catalog.InstitutionParentID;
                            commandFlowCorrection.Parameters.Add("@InstitutionToID", SqlDbType.Int).Value = catalog.InstitutionToID;
                            //commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                            var returnParameter = commandFlowCorrection.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;

                            conFlowCorrection.Open();

                            using (var reader2 = commandFlowCorrection.ExecuteReader())
                            {
                                if (returnParameter.Value.ToString() == "1")
                                {
                                    ViewBag.Message = $"Correctamente ejecutado.";
                                }
                            }
                        }
                    }

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "No es posible guardar los datos. Intente de nuevo, si el problema persiste contacte al administrador.");
                }
            }
            return View(catalog);
        }

        // GET: CatInstitucionConf/Delete/5
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
            var catalogo = db.InstitutionsConfiguration.Find(id);
            if (catalogo == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentsDropDownList(catalogo.InstitutionFromID, catalogo.InstitutionToID, catalogo.InstitutionParentID);

            return View(catalogo);
        }

        // POST: CatAgeGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var catalog = db.InstitutionsConfiguration.Find(id);
                db.InstitutionsConfiguration.Remove(catalog);
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

        private void PopulateDepartmentsDropDownList(
            object selectedFrom = null,
            object selectedTo = null,
            object selectedParent = null)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = user.Institution.CountryID ?? 0;

            var instQuery = db.Institutions.OfType<Institution>().Where(x => x.CountryID == countryId).OrderBy(x => x.Name).ToList();
            ViewBag.InstitutionFromID = new SelectList(instQuery, "ID", "Name", selectedFrom);
            ViewBag.InstitutionToID = new SelectList(instQuery, "ID", "Name", selectedTo);
            ViewBag.InstitutionParentID = new SelectList(instQuery, "ID", "Name", selectedParent);

            if (selectedParent != null)
            {
                int IDParent = Convert.ToInt32(selectedParent);
                var instParent = db.Institutions.OfType<Institution>().Where(x => x.CountryID == countryId && x.ID == IDParent).FirstOrDefault();
                ViewBag.ParentActive = instParent.Active;
            } 
        }
    }
}

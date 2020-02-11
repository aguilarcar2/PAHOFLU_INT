using Microsoft.AspNet.Identity;
using PagedList;
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CatCountryController : ControllerBase
    {
        private int _pageSize = 10;

        // GET: Country
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string language = user.Institution.Country.Language;
            //var countryId = user.Institution.CountryID ?? 0;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeParm = "Code";
            ViewBag.NameParm = "Name";
            ViewBag.ENGParm = "ENG";
            ViewBag.ActiveParm = "Active";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var catalogo = from c in db.Countries select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                catalogo = catalogo.Where(s => s.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Code":
                    catalogo = catalogo.OrderBy(s => s.Code);
                    break;
                case "Name":
                    catalogo = catalogo.OrderBy(s => s.Name);
                    break;
                case "ENG":
                    catalogo = catalogo.OrderBy(s => s.ENG);
                    break;
                case "Active":
                    catalogo = catalogo.OrderByDescending(s => s.Active).ThenBy(s => (language == "SPA") ? s.Name : s.ENG);
                    break;
                default:
                    //catalogo = catalogo.OrderBy(s => (language == "SPA") ? s.Name : s.ENG);
                    catalogo = catalogo.OrderByDescending(s => s.Active).ThenBy(s => (language == "SPA") ? s.Name : s.ENG);
                    break;
            }

            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);

            //return View(areas.ToList());
            return View(catalogo.ToPagedList(pageNumber, pageSize));
        }

        // GET: Country/Create
        public ActionResult Create()
        {
            Country country = new Country();

            List<SelectListItem> languageList = new List<SelectListItem>();
            languageList.Add(new SelectListItem { Text = "Español", Value = "SPA" });
            languageList.Add(new SelectListItem { Text = "Ingles", Value = "ENG" });

            country.LanguageList = languageList;

            return View(country);
        }

        // POST: Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, Code, Name, ENG, NumberAdminisDivision, Active, Language")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(country);
        }

        // GET: Country/Edit/5
        public ActionResult Edit(int? id)
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //var UsrLang = user.Institution.Country.Language;
            //****
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Country country = db.Countries.Find(id);
            if (country == null)
                return HttpNotFound();

            //****
            List<SelectListItem> languageList = new List<SelectListItem>();
            languageList.Add(new SelectListItem { Text = "Español", Value = "SPA" });
            languageList.Add(new SelectListItem { Text = "Ingles", Value = "ENG" });

            country.LanguageList = languageList;

            return View(country);
        }
        
        // POST: Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID, Code, Name, ENG, NumberAdminisDivision, Active, Language")] Country country)
        public ActionResult Edit([Bind(Include = "Code, Name, ENG, NumberAdminisDivision, Active, Language")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //??????
            return View(country);
        }


        // GET: Country/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Country country = db.Countries.Find(id);
            if (country == null)
                return HttpNotFound();

            return View(country);
        }

        // POST: Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country country = db.Countries.Find(id);
            db.Countries.Remove(country);
            db.SaveChanges();

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


    }
}
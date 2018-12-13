using Microsoft.AspNet.Identity;
using PagedList;
using Resources.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ColonyController : ControllerBase
    {
        // GET: Colony
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var colonies = db.Colonies.Include(p => p.Hamlet);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    colonies = colonies.Where(s => s.Hamlet.Neighborhood.State.Area.CountryID == user.Institution.CountryID)
                                        .OrderBy(o => o.Hamlet.Neighborhood.State.Area.Country.Name).ThenBy(o => o.Hamlet.Neighborhood.State.Area.Name)
                                        .ThenBy(o => o.Hamlet.Neighborhood.State.Name).ThenBy(o => o.Hamlet.Neighborhood.Name)
                                        .ThenBy(o => o.Hamlet.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    colonies = colonies.Where(s => s.Hamlet.Neighborhood.State.Area.CountryID == user.Institution.CountryID)
                                        .OrderBy(o => o.Hamlet.Neighborhood.State.Area.Country.Name).ThenBy(o => o.Hamlet.Neighborhood.State.Area.Name)
                                        .ThenBy(o => o.Hamlet.Neighborhood.State.Name).ThenBy(o => o.Hamlet.Neighborhood.Name)
                                        .ThenBy(o => o.Hamlet.Name).ThenBy(o => o.Name);
                }
            }

            return View(colonies.ToList());
        }

        // GET: Hamlet/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;

            //****
            var cat_countries = from c in db.Countries select c;
            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
            }

            ViewBag.Countries = new SelectList(cat_countries, "ID", "Name");

            //****
            var areas = db.Areas.Include(a => a.Country);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    areas = areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    areas = areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
            }

            ViewBag.Areas = new SelectList(areas, "ID", "Name");

            //****
            return View();
        }

        // POST: Colony/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, HamletID, Name, orig_country")] Colony colony)
        {
            if (ModelState.IsValid)
            {
                db.Colonies.Add(colony);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //???????
            return View(colony);
        }

        // GET: Colony/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;
            //****
            var cat_countries = from c in db.Countries select c;
            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_countries = cat_countries.Where(s => s.ID == user.Institution.CountryID);
                }
            }

            ViewBag.Countries = new SelectList(cat_countries, "ID", "Name");

            //****
            var cat_areas = db.Areas.Include(a => a.Country);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    cat_areas = cat_areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    cat_areas = cat_areas.Where(s => s.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Country.Name).ThenBy(o => o.Name);
                }
            }

            ViewBag.Areas = new SelectList(cat_areas, "ID", "Name");

            //****
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Colony colony = db.Colonies.Find(id); 
            if (colony == null)
                return HttpNotFound();

            return View(colony);
        }

        // POST: Colony/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID, HamletID, Name, orig_country")] Colony colony)
        {
            if (ModelState.IsValid)
            {
                db.Entry(colony).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //??????
            return View(colony);
        }

        // GET: Colony/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Colony colony = db.Colonies.Find(id);
            if (colony == null)
                return HttpNotFound();

            return View(colony);
        }

        // POST: Colony/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Colony colony = db.Colonies.Find(id);
            db.Colonies.Remove(colony);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Colony/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Colony colony = db.Colonies.Find(id);
            if (colony == null)
            {
                return HttpNotFound();
            }

            return View(colony);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);

            return searchedMsg;
        }
    }
}
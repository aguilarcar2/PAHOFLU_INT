﻿using Microsoft.AspNet.Identity;
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
    public class StateController : ControllerBase
    {
        // GET: States
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var states = db.States.Include(p => p.Area);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    states = states.Where(s => s.Area.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Area.Country.Name).ThenBy(o => o.Area.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    states = states.Where(s => s.Area.CountryID == user.Institution.CountryID)
                                    .OrderBy(o => o.Area.Country.Name).ThenBy(o => o.Area.Name).ThenBy(o => o.Name);
                }
            }

            return View(states.ToList());
        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrLang = user.Institution.Country.Language;
            /*var cat_countries = from c in db.Countries select c;
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
            }*/

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

            ViewBag.CountryID = new SelectList(cat_countries, "ID", "Name");

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

        // POST: States/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, AreaID, Name, orig_country")] State state)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(state);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //???????
            return View(state);
        }

        // GET: State/Edit/5
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

            ViewBag.CountryID = new SelectList(cat_countries, "ID", "Name");

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
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            State state = db.States.Find(id);
            if (state == null)
                return HttpNotFound();

            return View(state);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID, AreaID, Name, orig_country")] State state)
        {
            if (ModelState.IsValid)
            {
                db.Entry(state).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.CountryID = new SelectList(db.Countries, "ID", "Code", area.CountryID);           //??????
            return View(state);
        }

        // GET: States/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            State state = db.States.Find(id);
            if (state == null)
                return HttpNotFound();

            return View(state);
        }

        // POST: States/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            State state = db.States.Find(id);
            db.States.Remove(state);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: State/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = db.States.Find(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
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
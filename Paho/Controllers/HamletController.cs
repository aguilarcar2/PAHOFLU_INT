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
    public class HamletController : ControllerBase
    {
        // GET: Hamlet
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var hamlets = db.Hamlets.Include(p => p.Neighborhood);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    hamlets = hamlets.Where(s => s.Neighborhood.State.Area.CountryID == user.Institution.CountryID)
                                        .OrderBy(o => o.Neighborhood.State.Area.Country.Name).ThenBy(o => o.Neighborhood.State.Area.Name)
                                        .ThenBy(o => o.Neighborhood.State.Name).ThenBy(o => o.Neighborhood.Name)
                                        .ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    hamlets = hamlets.Where(s => s.Neighborhood.State.Area.CountryID == user.Institution.CountryID)
                                        .OrderBy(o => o.Neighborhood.State.Area.Country.Name).ThenBy(o => o.Neighborhood.State.Area.Name)
                                        .ThenBy(o => o.Neighborhood.State.Name).ThenBy(o => o.Neighborhood.Name)
                                        .ThenBy(o => o.Name);
                }
            }

            return View(hamlets.ToList());
        }

        // GET: Hamlet/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Hamlet hamlet = db.Hamlets.Find(id);
            if (hamlet == null)
            {
                return HttpNotFound();
            }

            return View(hamlet);
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
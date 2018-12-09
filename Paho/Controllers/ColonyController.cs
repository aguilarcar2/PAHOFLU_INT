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
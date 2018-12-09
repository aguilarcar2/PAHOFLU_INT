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
    public class NeighborhoodController : ControllerBase
    {
        // GET: Neighborhood
        public ActionResult Index()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var neighborhood = db.Neighborhoods.Include(p => p.State);

            if (user.Institution.AccessLevel == AccessLevel.Country || user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
            {
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    neighborhood = neighborhood.Where(s => s.State.Area.CountryID == user.Institution.CountryID)
                                                .OrderBy(o => o.State.Area.Country.Name).ThenBy(o => o.State.Area.Name)
                                                .ThenBy(o => o.State.Name).ThenBy(o => o.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.SelfOnly || user.Institution.AccessLevel == AccessLevel.Service)
                {
                    neighborhood = neighborhood.Where(s => s.State.Area.CountryID == user.Institution.CountryID)
                                                .OrderBy(o => o.State.Area.Country.Name).ThenBy(o => o.State.Area.Name)
                                                .ThenBy(o => o.State.Name).ThenBy(o => o.Name);
                }
            }

            return View(neighborhood.ToList());
        }

        // GET: State/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Neighborhood neighborhood = db.Neighborhoods.Find(id);
            if (neighborhood == null)
            {
                return HttpNotFound();
            }

            return View(neighborhood);
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

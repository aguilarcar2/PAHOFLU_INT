using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DiccionarioController : ControllerBase
    {
        
        // GET: Administracion
        public ActionResult Index()
        {
            //**** Link Dashboard
            var user = UserManager.FindById(User.Identity.GetUserId());

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

            return View();
        }
    }
}

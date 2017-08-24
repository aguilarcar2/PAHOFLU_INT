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
        
            return View();
        }
   
        
        
    }
}

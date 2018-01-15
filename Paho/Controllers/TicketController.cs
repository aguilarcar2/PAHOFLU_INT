using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace Paho.Controllers
{
    [Authorize]
    public class TicketController : ControllerBase
    {
        // GET: Ticket
        public ActionResult Index()
        {
            var SummaryViewModel = new SummaryViewModel();
            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var DoS = DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-GT"));
            SummaryViewModel.DatePickerConfig = DoS;
            //SummaryViewModel.UsrCtry = user.Institution.CountryID;

            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                SummaryViewModel.DisplayCountries = true;
                SummaryViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                SummaryViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }


            return View(SummaryViewModel);
        }

        public JsonResult CreateTicket(string UsrCountry, string ticketSubject, string ticketMsg)
        {
            string ticketResult="";

            string userTicket = User.Identity.GetUserId();
            //---------------------------Para setear los tickets en la base de datos---------------------------------
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                string jsonGraphData = "";
                
                using (var command = new SqlCommand("SetTicket", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@subject", SqlDbType.Text).Value = ticketSubject;
                    command.Parameters.Add("@description", SqlDbType.Text).Value = ticketMsg;
                    command.Parameters.Add("@status", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@ticketDate", SqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@userTicket", SqlDbType.Text).Value = userTicket;
                    
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ticketResult = reader["ResultInsert"].ToString().Trim();

                        }
                    }
                }
            }
            //--------------envio de correo----------------------
            var mailSupport = ConfigurationManager.AppSettings["supportMail"]; 
            var mailSupportCC1 = ConfigurationManager.AppSettings["supportMailCC1"];
            var mailSupportCC2 = ConfigurationManager.AppSettings["supportMailCC2"];
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("pahoflu@gmail.com", "Chachas1");

            MailMessage mm = new MailMessage("pahoflu@gmail.com", mailSupport, ticketSubject, ticketMsg);
            mm.CC.Add(mailSupportCC1);
            mm.CC.Add(mailSupportCC2);
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
            //--------------fin envio correo---------------------

            return Json(ticketResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTicket()
        {
            string userTicket = User.Identity.GetUserId();
            List<Dictionary<string, string>> ticketList = new List<Dictionary<string, string>>();
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("GetTicket", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@userTicket", SqlDbType.Text).Value = userTicket;                    
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, string> ticketItem = new Dictionary<string, string>();
                            ticketItem.Add("ID", reader["ID"].ToString().Trim());
                            ticketItem.Add("subject", reader["subject"].ToString().Trim());
                            ticketItem.Add("description", reader["description"].ToString().Trim());
                            ticketItem.Add("status", reader["status"].ToString().Trim());
                            ticketItem.Add("ticketDate", reader["ticketDate"].ToString().Trim());
                            ticketList.Add(ticketItem);
                        }
                    }
                }
            }
            return Json(ticketList, JsonRequestBehavior.AllowGet);            
        }
        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }
    }
}
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Paho.Utility;

namespace Paho.Controllers
{
    [Authorize]
    public class SummaryJMController : ControllerBase
    {
        // GET: SummaryJM
        public ActionResult Index()
        {
            var SummaryViewModel = new SummaryJMViewModel();

            IQueryable<Institution> institutions = null;
            //IQueryable<CatAgeGroupJM> CatAgeGroupQuery = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            SummaryViewModel.DatePickerConfig = DoS;
            SummaryViewModel.DateFormatDP = date_format;
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
                    institutions = db.Institutions
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.AccessLevel == AccessLevel.Area).OrderBy(n => n.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    //institutions = db.Institutions
                    //               .Where(i => i.AreaID == user.Institution.AreaID && i.AccessLevel == AccessLevel.Area).OrderBy(n => n.FullName);
                    institutions = db.Institutions
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(n => n.FullName);
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID).OrderBy(n => n.FullName);
                }
            }

            SummaryViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (institutions != null)
            {
                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Institution> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }

                SummaryViewModel.Institutions = institutionsDisplay;
            };

            var AgeGroupDisplay = db.CatAgeGroupJM.Where(y => y.id_country == user.Institution.CountryID).OrderBy(z => z.id_conf_country).ToList();
            //.Select(x => x.AgeGroup)

            //SummaryViewModel.CatAgeGroup = AgeGroupDisplay;
            SummaryViewModel.CatAgeGroup = AgeGroupDisplay;

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
            ViewBag.UsrCtry = user.Institution.CountryID;
            //****
            return View(SummaryViewModel);
        }

        public JsonResult GetSummaryDetailsJM(int hospitalId, string hospitalDate, int EpiWeek, int EpiYear)
        {
            var HospitalDate = DateTime.Parse(hospitalDate);
            //var casesummary =  db.CaseSummaries.FirstOrDefault(s=>s.HosiptalId == hospitalId && s.StartDateOfWeek == HospitalDate);
            var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.HospitalId == hospitalId && s.EpiYear == EpiYear && s.EW == EpiWeek);
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (casesummary == null)
            {
                casesummary = new CaseSummaryJM()
                {
                    WeekendDate = HospitalDate,
                    HospitalId = hospitalId,
                    EW = EpiWeek,
                    EpiYear = EpiYear,
                    CaseSummaryDetailJM = new List<CaseSummaryDetailJM>()
                };
                db.Entry(casesummary).State = EntityState.Added;
                var AgeGroupbyCountry = db.CatAgeGroupJM.Where(i => i.id_country == user.Institution.CountryID).ToList();
                foreach (CatAgeGroupJM agegroup in AgeGroupbyCountry)
                {
                    casesummary.CaseSummaryDetailJM.Add(
                        new CaseSummaryDetailJM()
                        {
                            AgeGroup = agegroup.id_conf_country,
                            ILICases = 0,
                            ILISamplesTaken = 0,
                            TotalVisits = 0,
                        }
                    );
                }
                db.SaveChanges();
            }
            var casesummaryDetails = casesummary.CaseSummaryDetailJM.ToArray();


            //     (
            //      from casesummary in db.CaseSummaries as IEnumerable<CaseSummary>
            //      where casesummary
            //      select casesummary.CaseSummaryDetails
            //      );

            //      CaseSummaryDetails = db.FluCases.GroupBy(o=>o.AgeGroup).Select( f => new {

            //      AgeGroup = f.Key,
            //      Hospfem = f.Sum(i => i.Gender == Gender.Female && i.CaseHospital.HospAmDate.HasValue ? 1 : 0),
            //      HospMaso = f.Sum(i => i.Gender == Gender.Male && i.CaseHospital.HospAmDate.HasValue ? 1 : 0),
            //      ICUfem = f.Sum(i => i.Gender == Gender.Female &&  i.CaseHospital.ICUAmDate.HasValue ? 1 : 0),
            //      ICUMaso = f.Sum(i => i.Gender == Gender.Male &&  i.CaseHospital.ICUAmDate.HasValue ? 1 : 0),
            //      Deffem = f.Sum(i => i.Gender == Gender.Female && i.CaseHospital.Destin.Equals("D") ? 1 : 0),
            //      DefMaso = f.Sum(i => i.Gender == Gender.Male && i.CaseHospital.Destin.Equals("D") ? 1 : 0)              
            //      }               
            return Json(casesummaryDetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSummaryJM(CaseSummaryDetailJM[] casesummaryDetails)
        {
            var casesummaryid = casesummaryDetails[0].CaseSummaryId;
            var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.Id == casesummaryid);

            foreach (CaseSummaryDetailJM casesummaryDetail in casesummaryDetails)
            {
                var dbcasesummaryDetail = casesummary.CaseSummaryDetailJM.First(d => d.Id == casesummaryDetail.Id);
                
                dbcasesummaryDetail.ILICases = casesummaryDetail.ILICases;
                dbcasesummaryDetail.ILISamplesTaken = casesummaryDetail.ILISamplesTaken;
                dbcasesummaryDetail.TotalVisits = casesummaryDetail.TotalVisits;
            }
            db.SaveChanges();

            return Json(getMsg("viewValidateSavedRecordSummary"));
        }

        public JsonResult GetSummaryForYearJM(int hospitalId)
        {
            var epiYear = DateTime.Now.Year.ToString();
            int intEpiYear = Int32.Parse(epiYear);
            List<Dictionary<string, int>> summaryPerYear = new List<Dictionary<string, int>>();
            ///////DateTime dFech = fechaInicioPrimeraSemanaEpidemiologica(DateTime.UtcNow.Year);
            DateTime dFech = PAHOClassUtilities.fechaInicioPrimeraSemanaEpidemiologica(DateTime.Today.Year);
            int nSemaActual = PAHOClassUtilities.NumeroActualSE();
            //dFech = dFech.AddDays(6);
            DateTime dSEHoy;
            dSEHoy = dFech.AddDays((nSemaActual - 1) * 7).AddDays(6);

            //#### Fecha fin de SE del dia de hoy
            /*DateTime dSEHoy;
            DateTime dHoy = DateTime.Now.Date;
            dSEHoy = dHoy;

            for (int i = 52; i >= 1; i--)
            {
                DateTime vSeIn = dFech.AddDays(7 * (i - 1));
                DateTime vSeFi = vSeIn.AddDays(6);
                if (dHoy >= vSeIn && dHoy <= vSeFi)
                {
                    dSEHoy = vSeFi;
                    break;
                }
            }*/
            //#### 

            //for (int i = 1; i <= 52; i++)
            //for (int i = 52; i >= 1; i--)
            for (int i = nSemaActual; i >= 1; i--)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                var ColILICasesST = 0;
                var ColILISamplesTakenST = 0;
                var ColTotalVisitsST = 0;

                DateTime WeekendDate = DateTime.UtcNow;

                var casesummary = db.CaseSummariesJM.FirstOrDefault(s => s.HospitalId == hospitalId && s.EW == i && s.EpiYear == intEpiYear);
                if (casesummary == null)
                {
                    DateTime vFeIn = dFech.AddDays(7 * (i - 1));
                    DateTime vFeFi = vFeIn.AddDays(6);

                    if (WeekendDate >= vFeIn && WeekendDate <= vFeFi)
                        WeekendDate = vFeFi;
                    else
                    {
                        if (vFeFi < WeekendDate)
                            WeekendDate = dFech.AddDays(7 * (i - 1) + 6);
                        else
                            WeekendDate = dSEHoy;
                    }
                }
                else
                {
                    //var casesummaryDetails = casesummary.CaseSummaryDetailJM.ToArray();
                    var casesummaryDetails = casesummary.CaseSummaryDetailJM.ToArray();
                    WeekendDate = casesummary.WeekendDate;
                    foreach (CaseSummaryDetailJM casesummaryDetail in casesummaryDetails)
                    {//casesummaryDetails
                        ColILICasesST += casesummaryDetail.ILICases;
                        ColILISamplesTakenST += casesummaryDetail.ILISamplesTaken;
                        ColTotalVisitsST += casesummaryDetail.TotalVisits;
                    }
                }
                dictionary.Add("EpiWeek", i);
                dictionary.Add("ColILICasesST", ColILICasesST);
                dictionary.Add("ColILISamplesTakenST", ColILISamplesTakenST);
                dictionary.Add("ColTotalVisitsST", ColTotalVisitsST);
                Int32 unixTimestamp = (Int32)(WeekendDate.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dictionary.Add("WeekendDate", unixTimestamp);
                dictionary.Add("EpiYear", intEpiYear);
                summaryPerYear.Add(dictionary);
            }
            return Json(summaryPerYear, JsonRequestBehavior.AllowGet);
        }

        /*private DateTime fechaInicioPrimeraSemanaEpidemiologica(int nYear)
        {
            DateTime vFeIn, vTemp, vSaba;
            //****
            vTemp = new DateTime(nYear, 1, 1);                  // 1er día del anio
            int nWeDa = (int)vTemp.DayOfWeek + 1;               // Domingo 1er día   
            vSaba = vTemp.AddDays(7 - nWeDa);                   // 1er sábado

            int nDife = (int)vSaba.Subtract(vTemp).TotalDays;
            if (nDife >= 3)
                vFeIn = vSaba.AddDays(-(7 - 1));
            else
                vFeIn = vSaba.AddDays(1);
            //****
            return vFeIn;
        }*/

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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Paho.Models;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Globalization;
using Microsoft.AspNet.Identity;
using System.Data.Entity.SqlServer;
using System.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Paho.Controllers
{
    [Authorize]
    public class CasesController : ControllerBase
    {

        public void HistoryRecord(int? RecordId, int Action_history, int? flow, int? state)
        {
            RecordHistory history;
            history = new RecordHistory();
            history.Action = Action_history;
            history.Recordid = RecordId;
            history.Userid = User.Identity.GetUserId();
            history.flow = flow;
            history.state = state;
            history.DateAction = DateTime.Now;
            db.Entry(history).State = EntityState.Added;
            db.SaveChanges();
        }

        public ActionResult GetCountries()
        {
            var countries = db.Countries
                  .Select(c => new
                  {
                      Id = c.ID.ToString(),
                      Name = c.Name,
                      Active = c.Active
                  })
                  .OrderBy(c => c.Name)
                  .ToArray();

            return Json(countries, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Institution> GetInstitutionByRegion(long institutionId)
        {
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == institutionId && i.Priority == 1);

            var institutionLabs = institutionsConfiguration.Select(i => i.InstitutionTo);
            if (!institutionLabs.Any())
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                institutionLabs = db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID);
            }

            return institutionLabs;
        }

        // GET: Regiones
        public JsonResult GetRegions(int CountryID, int? AreaID, int? RegionID)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = CountryID > 0 ? CountryID : user.Institution.CountryID;
            //Refrescar institucion en el combo segun la region a la que pertenecen
            var regions =
             (
              from Region in db.Regions as IQueryable<Region>
              where Region.CountryID == UsrCtry && (Region.tipo_region == 1 || Region.tipo_region == null)
              select new
              {
                  Id = Region.ID,
                  //Id = Region.orig_country,
                  Name = Region.Name
                  }).ToArray();

            if (user.type_region != 1 && user.type_region != null)
            {
                regions =
             (
              from Region in db.Regions as IQueryable<Region>
              where Region.CountryID == UsrCtry && (Region.tipo_region == user.type_region )
              select new
              {
                  Id = Region.ID,
                  //Id = Region.orig_country,
                  Name = Region.Name
              }).ToArray();
            }


                return Json(regions, JsonRequestBehavior.AllowGet);
        }

        // GET: GetHospitals
        public JsonResult GetInstitutions(int? CountryID, int? AreaID, int? RegionID)
        {

            if (RegionID != null )
            {

                var user = UserManager.FindById(User.Identity.GetUserId());
                var UsrCtry = user.Institution.CountryID;
                //Refrescar institucion en el combo segun la region a la que pertenecen

                var institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry 
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();

                if ((user.type_region == 1 || user.type_region == null) && RegionID > 0)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_institucional == RegionID
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }
                

                if ( user.type_region == 2)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_salud == RegionID
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }
                else if (user.type_region == 3)
                {
                    institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == UsrCtry && institution.cod_region_pais == RegionID
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Hospital ? InstitutionType.Hospital : InstitutionType.Lab
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();
                }

                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutions.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                } else if (institutions.Count() == 0)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = "-- Sin unidades centinela --" };
                    institutionsDisplay.Insert(0, all);
                }
                return Json(institutionsDisplay, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var user = UserManager.FindById(User.Identity.GetUserId());
                //var UsrCtry = user.Institution.CountryID;

                //CountryID = CountryID != null ? CountryID : UsrCtry;

                var institutions =
                 (
                  from institution in db.Institutions as IQueryable<Institution>
                  where institution.CountryID == CountryID
                  select new
                  {
                      Id = institution.ID,
                      Name = institution.Name,
                      InstitutionType = institution is Lab ? InstitutionType.Lab : InstitutionType.Hospital
                      //InstitutionType = InstitutionType.Hospital
                  }).ToArray();

                var institutionsDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.Id.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutions.Count() > 1)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }
                else if (institutions.Count() == 0)
                {
                    var all = new LookupView<Institution> { Id = "0", Name = "-- Sin unidades centinela --" };
                    institutionsDisplay.Insert(0, all);
                }


                return Json(institutionsDisplay, JsonRequestBehavior.AllowGet);
            }




        }

        public ActionResult GetAreas(int CountryID)
        {
            var areas =
                     (
                      from area in db.Areas as IQueryable<Area>
                      where area.CountryID == CountryID
                      orderby area.Name
                      select new
                      {
                          Id = area.ID,
                          Name = area.Name
                      }).ToArray();

            return Json(areas, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStates(int AreaID)
        {
            var states =
                   (
                    from state in db.States as IQueryable<State>
                    where state.AreaID == AreaID
                    select new
                    {
                        Id = state.ID,
                        Name = state.Name
                    })
                    .OrderBy(c => c.Name)
                    .ToArray();

            return Json(states, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocals(int StateID)
        {
            var locals =
                (
                 from local in db.Locals as IQueryable<Local>
                 where local.StateID == StateID
                 select new
                 {
                     Id = local.ID,
                     Name = local.Code
                 }).ToArray();

            return Json(locals, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNeighborhoods(int StateID)
        {
            var neighborhoods =
                (
                 from neighborhood in db.Neighborhoods as IQueryable<Neighborhood>
                 where neighborhood.StateID == StateID
                 select new
                 {
                     Id = neighborhood.ID,
                     Name = neighborhood.Name
                 })
                 .OrderBy (d=> d.Name)
                 .ToArray();

            return Json(neighborhoods, JsonRequestBehavior.AllowGet);
        }
        // GET: FluCases
        public ActionResult GetFluCases(string sidx, string sord, int page, int rows,
                                        int institutionId,
                                        int? regionId,
                                        string ScaseNo,
                                        string ShospReg,
                                        string Sstatus,
                                        string SName,
                                        string SLastName,
                                        int? SRecordId,
                                        string SNotiDateS,
                                        string SNotiDateE,
                                        bool SPend,
                                        int ITy
                                        )
        {
            //IEnumerable<FluCase> flucases;
            IQueryable<FluCase> flucases = null;
            var InstitutionDB = institutionId;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var UsrCtry = user.Institution.CountryID;
            var RecordId = SRecordId;
            var UsrAccessLevel = user.Institution.AccessLevel;
            var UsrRegion = user.Institution.cod_region_institucional;
            var UsrInstitution = user.Institution.ID;

            var Access_Lever_IT = false;

            if (InstitutionDB > 0)
                Access_Lever_IT = db.Institutions.Where(f => f.ID == InstitutionDB).FirstOrDefault().AccessLevel == AccessLevel.Service ? true : false;

            if ((InstitutionDB == 0 && user.Institution is Hospital) || (InstitutionDB > 0 && Access_Lever_IT && user.Institution is Hospital))
            {
                if (InstitutionDB == 0)
                    InstitutionDB = Convert.ToInt32(user.InstitutionID);

                var institution_type = db.Institutions.Where(f => f.ID == InstitutionDB).ToList();

                if (institution_type.Any())
                    if (institution_type[0].Father_ID.ToString() != "" && institution_type[0].Father_ID.ToString() != "0")
                        Int32.TryParse(institution_type[0].Father_ID.ToString(), out InstitutionDB);

                if (institution_type[0] is Hospital && institution_type[0].AccessLevel != AccessLevel.Service)
                {
                    var institutions_service = db.Institutions.OfType<Hospital>()
                                      .Where(i => i.Father_ID == InstitutionDB || i.ID == InstitutionDB).Select(t => t.ID).ToList();
                    if (institutions_service.Any())
                    {
                        flucases = db.FluCases.Where(y => institutions_service.Contains(y.HospitalID));
                    }
                }
            } else if (InstitutionDB == 0 ) {
                //flucases = db.FluCases.Where(f => f.CountryID == UsrCtry) as IQueryable<FluCase>;
                flucases = db.FluCases.Where(f => f.CountryID == UsrCtry);
            } else {
                //flucases = db.FluCases.Where(f => f.HospitalID == InstitutionDB) as IQueryable<FluCase>;
                flucases = db.FluCases.Where(f => f.HospitalID == InstitutionDB);
            }


            if ((regionId > 0 || UsrAccessLevel == AccessLevel.Regional) && institutionId == 0)
            {
                var RegionId_ = (UsrAccessLevel == AccessLevel.Regional) ? UsrRegion : regionId;
                var Regions = db.Institutions.Where(l => l.cod_region_institucional == RegionId_);
                var list_regions = Regions.Select(i => i.ID).ToList();

                if (list_regions.Any())
                {
                    flucases = flucases.Where(k => list_regions.Contains(k.HospitalID));
                }
            } 

            var pageIndex = Convert.ToInt32(page) - 1;
            var pageSize = rows;

            if (ITy == 2)
            {
                //if ( InstitutionDB != 0) var user = UserManager.FindById(User.Identity.GetUserId());
                //var usrCtry = user.Institution.CountryID;
                var labs = db.Institutions.Where(f => f.ID == institutionId);
                var list_labs = labs.ToList();
                var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                                        .Where(i => i.InstitutionToID == user.Institution.ID);

                if (InstitutionDB == 0 && institutionsConfiguration.Any())
                {
                    var Listlabs = institutionsConfiguration.Select(i => i.InstitutionParentID).ToList();
                    flucases = flucases.Where(f => Listlabs.Contains(f.HospitalID));
                }

                if (!institutionsConfiguration.Any())
                {
                    flucases = flucases.Where(f => f.IsSample == true);
                    //InstitutionDB = labs;

                    if (list_labs.Count() > 0) {
                        InstitutionDB = int.Parse(list_labs[0].Father_ID.ToString());
                        flucases = flucases.Where(f => f.HospitalID == InstitutionDB);
                    }
                }

                if (SPend == true)
                {
                    flucases = flucases.Where(f => f.CaseStatus == null);
                    if (institutionsConfiguration.Any())
                    {
                        //i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID
                        flucases = flucases.Where(h => (h.IsSample == true && h.Processed != false && (h.FinalResult == null || h.FinalResult != "N")) && ( ((h.flow == (institutionsConfiguration.Where(i=> i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j=> j.Priority).ToList().FirstOrDefault() - 1)) && (h.statement == 2 || h.statement == null) ) || ((h.flow == (institutionsConfiguration.Where(i => i.InstitutionParentID == h.HospitalID && i.InstitutionToID == user.Institution.ID).Select(j => j.Priority).ToList().FirstOrDefault())) && (h.statement == 1 || h.statement == null))) );
                    }
                    else
                    {
                        flucases = flucases.Where(f => f.IsSample == true && f.Processed != false && f.FinalResult == null);
                        flucases = flucases.Where(h => h.flow == 0);
                    }

                    //flucases = flucases.Where(h => !h.CaseLabTests.Where(i => i.LabID == UsrInstitution).Any());
                    //flucases = flucases.Where(f => f.CaseStatus != null);
                }


            }

            if (RecordId > 0)
            {
                flucases = flucases.Where(t => t.ID == RecordId);
            }

            if (SPend == true && ITy != 2)
            {
                flucases = flucases.Where(f => (f.Surv== 1 && (f.Destin == "" || f.Destin == null)) || f.CaseStatus == null );
                //flucases = flucases.Where(f => f.CaseStatus != null);
            }

            if (SNotiDateS != "" && SNotiDateS != "undefined")
            {
                DateTime NotiDateS;

                NotiDateS = DateTime.ParseExact(SNotiDateS, "d", CultureInfo.CreateSpecificCulture("es-GT"));
                flucases = flucases.Where(i => i.RegDate >= NotiDateS);
            }

            if (Sstatus != "" && Sstatus != "undefined")
            {
                flucases = flucases.Where(f => f.Destin == Sstatus);
                //flucases = flucases.Where(f => f.CaseStatus != null);
            }


            if (ShospReg != "" && ShospReg != "undefined") {
                flucases = flucases.Where(g => g.NationalId == ShospReg);
            }

            if (SName != "" && SName != "undefined")
            {
                flucases = flucases.Where(z => string.Concat(z.FName1.Trim().ToUpper(), " ", z.FName2.Trim().ToUpper()).Contains(SName.Trim().ToUpper()));
                //flucases = flucases.Where(g => g.FName1.ToUpper().Contains(SName.ToUpper()) || g.FName2.ToUpper().Contains(SName.ToUpper()));
            }

            if (SLastName != "" && SLastName != "undefined")
            {
                flucases = flucases.Where(z => string.Concat(z.LName1.Trim().ToUpper(), " ", z.LName2.Trim().ToUpper()).Contains(SLastName.Trim().ToUpper()));

                //flucases = flucases.Where(g => g.LName1.ToUpper().Contains(SLastName.ToUpper()) || g.LName2.ToUpper().Contains(SLastName.ToUpper()));
            }

            if (ScaseNo != "")
            {
                flucases = flucases.Where(h => h.NoExpediente == ScaseNo.ToUpper());
            }

            flucases = flucases.OrderByDescending(s => s.HospitalDate);

            var totalRecords = flucases.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            flucases = flucases.Skip(pageIndex * pageSize).Take(1500).Take(pageSize);

            //flucases = flucases.Skip(pageIndex * pageSize).Take(pageSize);
            var jsondata = new List<Object>();

            //var Arrayrows = (from flucase in flucases
            //                 select new
            //                 {
            //                     id_D = flucase.ID,
            //                     H_D = flucase.HospitalDate,
            //                     LN_D = flucase.LName1 + " " + flucase.LName2 ?? "",
            //                     FN_D = flucase.FName1 + " " + flucase.FName2 ?? "",
            //                     NE_D = flucase.NoExpediente ?? "",
            //                     IS_D = flucase.IsSample,
            //                     FR_D = flucase.FinalResult,
            //                     D_D = flucase.Destin,
            //                     FRVT_D = flucase.FinalResultVirusTypeID,
            //                     P_D = flucase.Processed,
            //                     CS_D = flucase.CaseStatus,
            //                     //CLT_D = flucase.CaseLabTests.Where(z => z.LabID == 60).ToList()
            //                 }).AsEnumerable()
            //                    .Select(x => new
            //                    {
            //                        id = x.id_D.ToString(),
            //                        cell = new string[]
            //                        {
            //                            x.id_D.ToString(),
            //                            x.H_D.ToString("d", CultureInfo.CreateSpecificCulture("es-GT")),
            //                            x.LN_D,
            //                            x.FN_D,
            //                            x.NE_D ?? "",
            //                            (x.IS_D == true) ? BooleanType.Si.ToString() : (x.IS_D == false) ? BooleanType.No.ToString(): "",
            //                            x.FR_D == "U" ? "No realizado" : x.FR_D == "N" ? "Negativo" : x.FR_D == "I" ? "Indeterminado": (x.FRVT_D == 1) ? "Influenza A" : (x.FRVT_D == 2) ? "Influenza B" : (x.FRVT_D == 3) ? "Parainfluenza I" : (x.FRVT_D == 4) ? "Parainfluenza II" : (x.FRVT_D == 5) ? "Parainfluenza III" : (x.FRVT_D == 6) ? "VSR" : (x.FRVT_D == 7) ? "Adenovirus" : (x.FRVT_D == 8) ? "Metapneumovirus" :  (x.FRVT_D == 9) ? "Otro" : (x.P_D == false) ? "No procesado" :""  ,
            //                            x.D_D == "A" ? "Dado de Alta" : x.D_D == "D" ? "Fallecido" : x.D_D == "R" ? "Referido" : "",
            //                            //x.CLT_D.Select(z => new { Id = z..ID.ToString(), x.Name }).ToList()
            //                        }
            //                    }).ToArray();
            //if (UsrCtry >= 1)
            //{

            var  Arrayrows = (from flucase in flucases
                                 select new
                                 {
                                     surv_ID = flucase.Surv,
                                     id_D = flucase.ID,
                                     H_D = flucase.HospitalDate,
                                     LN_D = flucase.LName1 + " " + flucase.LName2 ?? "",
                                     FN_D = flucase.FName1 + " " + flucase.FName2 ?? "",
                                     NE_D = flucase.NoExpediente ?? "",
                                     IS_D = flucase.IsSample,
                                     FR_D = flucase.FinalResult,
                                     D_D = flucase.Destin,
                                     FRVT_D = flucase.FinalResultVirusTypeID,
                                     P_D = flucase.Processed,
                                     CS_D = flucase.CaseStatus,
                                     VR_IF_D = flucase.CaseLabTests.Where(e => e.TestType == (TestType)1 && e.Processed != null).OrderBy(d => d.SampleNumber).ThenBy(f => f.CatTestResult.orden).ThenBy(u => u.TestDate).ThenBy(y => y.CatVirusType.orden).FirstOrDefault(),
                                     VR_PCR_D = flucase.CaseLabTests.Where(e => e.TestType == (TestType)2 && e.Processed != null).OrderBy(d => d.SampleNumber).ThenBy(f => f.CatTestResult.orden).ThenBy(u => u.TestDate).ThenBy(y => y.CatVirusType.orden).FirstOrDefault(),
                                     HEALTH_INST = flucase.Hospital.Name ?? ""
                                 }).AsEnumerable()
                                   .Select(x => new
                                   {
                                       id = x.id_D.ToString(),
                                       cell = new string[]
                                     {
                                         "<img src='/Content/themes/base/images/" + x.surv_ID.ToString() + "_" + user.Institution.Country.Language.ToString() + ".png' alt='"+ (x.surv_ID == 1 ? "SARI":"ILI") + "'/>",
                                         x.id_D.ToString(),
                                         x.H_D.ToString("d", CultureInfo.CreateSpecificCulture("es-GT")),
                                         x.LN_D,
                                         x.FN_D,
                                         x.NE_D ?? "",
                                         x.VR_IF_D == null ? "" :  x.VR_IF_D.TestResultID == null ? "": x.VR_IF_D.TestResultID.ToString() == "N" ? "Negative":   x.VR_IF_D.TestResultID.ToString() == "I" ? "Indeterminate":  x.VR_IF_D.TestResultID.ToString() == "U" ? "Unrealized": x.VR_IF_D.CatVirusType == null ? "" : (user.Institution.Country.Language == "SPA" ? x.VR_IF_D.CatVirusType.SPA : x.VR_IF_D.CatVirusType.ENG) ,
                                         x.VR_PCR_D == null ? "" : x.VR_PCR_D.TestResultID == null ? "": x.VR_PCR_D.TestResultID.ToString() == "N" ? "Negative":   x.VR_PCR_D.TestResultID.ToString() == "I" ? "Indeterminate":   x.VR_PCR_D.TestResultID.ToString() == "U" ? "Unrealized": x.VR_PCR_D.CatVirusType == null ? "" : x.VR_PCR_D.CatVirusType.SPA.Contains("Influenza A") == true ? x.VR_PCR_D.CatVirusSubType == null ? "" : (user.Institution.Country.Language == "SPA" ?  x.VR_PCR_D.CatVirusSubType.SPA : x.VR_PCR_D.CatVirusSubType.ENG ): (user.Institution.Country.Language == "SPA" ? x.VR_PCR_D.CatVirusType.SPA : x.VR_PCR_D.CatVirusType.ENG) ,
                                         x.IS_D == false ? "No sample" : x.FR_D == "U" ? "Unrealized" : x.FR_D == "N" ? "Negative" : x.FR_D == "I" ? "Indeterminate": (x.FRVT_D == 1) ? "Influenza A" : (x.FRVT_D == 2) ? "Influenza B" : (x.FRVT_D == 3) ? "Parainfluenza I" : (x.FRVT_D == 4) ? "Parainfluenza II" : (x.FRVT_D == 5) ? "Parainfluenza III" : (x.FRVT_D == 6) ? "VRS" : (x.FRVT_D == 7) ? "Adenovirus" : (x.FRVT_D == 8) ? "Metapneumovirus" :  (x.FRVT_D == 9) ? "Other" : (x.P_D == false) ? "Not processed" :""  ,
                                         x.HEALTH_INST ?? "",
                                         x.CS_D == 1 ?  "<img src='/Content/themes/base/images/open.png' alt='En estudio'/>" + " Under study" : x.CS_D == 2 ? "<img src='/Content/themes/base/images/close.png' alt='Descartado'/>" + " Discarded" : x.CS_D == 3 ? "<img src='/Content/themes/base/images/close.png' alt='Cerrado'/>" + " Closed"  : "<img src='/Content/themes/base/images/open.png' alt='Sin estatus'/>" + " No status"
                                     }
                                   }).ToArray();
            //}

                jsondata.Add(new
                {
                    total = totalPages,
                    page = page,
                    records = totalRecords,
                    rows = Arrayrows
                });

            return Json(jsondata[0], JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCIE10( string term, int max, string code)
        {
            var search = term;

            if (search == "" || search == null) search = "-A";

            var diags = ( db.CIE10.Where(f => f.Diag.Contains(search)) as IQueryable<CatDiag>);

            if (code != "" && code!= null)
            {
                diags = diags.Where(h => h.Diag.Contains(code));
            }

            diags = diags.Take(10);

            var jsondata = (
                    from diags_list in diags
                    select new
                    {
                        value = diags_list.ID,
                        label = diags_list.Diag
                    }).ToArray();


            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSalon(string term, int max)
        {
            var search = term;

            if (search == "" || search == null) search = "A";

            var salones = (db.Salones.Where(f => f.description_service.Contains(search)) as IQueryable<CatServicios>);

            salones = salones.Take(10);

            var jsondata = (
                    from salons_list in salones
                    select new
                    {
                        value = salons_list.ID,
                        label = salons_list.description_service
                    }).ToArray();

            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetSalonID(int ID)
        {
            CatServicios salones = db.Salones.Find(ID);
            if (salones == null)
            {
                return HttpNotFound();
            }

            var result =
                 new
                 {
                     value = salones.ID.ToString(),
                     label = salones.description_service
                 };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCIE10ID( int ID)
        {
            CatDiag diag = db.CIE10.Find(ID);
            if (diag == null)
            {
                return HttpNotFound();
            }

            var result =
                 new
                  {
                      value = diag.ID.ToString(),
                      label = diag.Diag
                  };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        // GET: Cases/Delete/5
        public ActionResult GetContact(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }


            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            institutions = db.Institutions.OfType<Hospital>().Where(i => i.ID == flucase.HospitalID);
            var institutionsName = institutions.Select(x => (string)x.FullName).ToList();
            
            var region_institutional = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID  && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_institucional && d.tipo_region == 1).Select(j => (string)j.Name).ToList(); //.Select(x => (string)x.Name)
            var region_salud = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_salud && d.tipo_region == 2).Select(j => (string)j.Name).ToList();
            var region_pais = db.Regions.Where(d => d.CountryID == institutions.ToList().FirstOrDefault().CountryID && d.orig_country == institutions.ToList().FirstOrDefault().cod_region_pais && d.tipo_region == 3).Select(j => (string)j.Name).ToList();
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
            var institutionsConfigurationMax = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionParentID == flucase.HospitalID).OrderByDescending(j => j.Priority);
            var flow_local_lab = 0;
            var flow_max = 1;
            var flow_open_always = false;
            if (institutionsConfiguration.Any())
            {
                flow_local_lab = institutionsConfiguration.First().Priority;
                flow_open_always = institutionsConfiguration.First().OpenAlways;
            }

            if (institutionsConfigurationMax.Any())
            {
                flow_max = institutionsConfigurationMax.First().Priority;
            }

            var result =
                 new
                 {
                     Id = flucase.ID.ToString(),
                     Surv = flucase.Surv.ToString(),
                     SurvInusual = flucase.SurvInusual,
                     Brote = flucase.Brote,
                     LName1 = flucase.LName1,
                     LName2 = flucase.LName2,
                     FName1 = flucase.FName1,
                     FName2 = flucase.FName2,
                     DocumentType = flucase.DocumentType,
                     NoExpediente = flucase.NoExpediente,
                     NationalId = flucase.NationalId,
                     DOB = flucase.DOB,
                     Age = flucase.Age,
                     AMeasure = flucase.AMeasure.ToString(),
                     Gender = flucase.Gender.ToString(),
                     HospitalDate = flucase.HospitalDate,
                     RegDate = flucase.RegDate,
                     nationality = flucase.nationality,
                     nativepeople = flucase.nativepeople,
                     hospitalIDRecord = flucase.HospitalID,
                     hospitalName = institutionsName[0],
                     DataStatement = flucase.statement,
                     flow_record = flucase.flow,
                     flow_institution = flow_local_lab,
                     flow_max = flow_max,
                     flow_open_always = flow_open_always,
                     region_institucional = region_institutional.FirstOrDefault(),
                     region_salud = region_salud.FirstOrDefault(),
                     region_pais = region_pais.FirstOrDefault(),
                     selectedServiceId = (db.Institutions.Where(j => j.ID == flucase.HospitalID).FirstOrDefault().AccessLevel == (AccessLevel)6) ? flucase.HospitalID : 0
                 };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DeleteCase(int id)
        {
            try {
                var flucase = db.FluCases.Find(id);
                var flow = flucase.flow;
                var statement = flucase.statement;
                db.Entry(flucase).State = EntityState.Deleted;
                db.FluCases.Remove(flucase);
                db.SaveChanges();
                HistoryRecord(id, 3, flow, statement);
                return Json("Registo borrado!", JsonRequestBehavior.AllowGet);
            } catch (Exception) {
                return Json("Fallo borrar el registro !", JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cases/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveContact(
            int? id,
            int? Surv,
            bool? SInusual,
            bool? Brote,
            string LName1,
            string LName2,
            string FName1,
            string FName2,
            int? DocumentType,
            string NoExpediente,
            string NationalId,
            DateTime? DOB,
            int? Age,
            AMeasure? AMeasure,
            Gender Gender,
            DateTime HospitalDate,
            DateTime RegDate,
            int HospitalId,
            int? nativepeople,
            int? nationality,
            DateTime DateFeverDummy
            )
        {
            FluCase flucase;
            var DateDummyRange1 = DateFeverDummy.Date.AddDays(1);
            var DateDummyRange2 = DateFeverDummy.Date.AddDays(-10);

                if (!id.HasValue)
            {
                var flucases = db.FluCases.Where(f => f.NoExpediente == NoExpediente.ToUpper() && (f.FeverDate < DateDummyRange1 && f.FeverDate >= DateDummyRange2)).ToList();

                if (flucases.Any())
                {
                    //id = flucases.First().ID;
                    //flucase = db.FluCases.Find(id);
                    //db.Entry(flucase).State = EntityState.Modified;
                    flucase = db.FluCases.Find(id);
                    return Json("El registro ya existe por favor verifique. Último registro: " + flucases.First().ID.ToString() );

                }
                else
                {
                    flucase = new FluCase();
                    flucase.HospitalID = HospitalId;
                    flucase.flow = 0;
                    db.Entry(flucase).State = EntityState.Added;

                }
            }
            else
            {
                flucase = db.FluCases.Find(id);
                flucase.HospitalID = HospitalId;
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.Surv = Surv;
            flucase.SurvInusual = SInusual;
            flucase.Brote = Brote;
            flucase.LName1 = LName1 ?? "";
            flucase.LName2 = LName2 ?? "";
            flucase.FName1 = FName1 ?? "";
            flucase.FName2 = FName2 ?? "";
            flucase.FullName = (FName1 ?? "" )+ ( FName2 == "" ? "" : " " )+ (FName2 ?? "");
            flucase.DocumentType = DocumentType;
            flucase.NoExpediente = NoExpediente ?? "";
            flucase.NationalId = NationalId ?? "";
            flucase.DOB = DOB;
            flucase.Age = Age;
            flucase.AMeasure = AMeasure;
            flucase.Gender = Gender;
            flucase.HospitalDate = HospitalDate;
            flucase.RegDate = RegDate;
            flucase.nativepeople = nativepeople;
            flucase.nationality = nationality;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;
            try
            {
                db.SaveChanges();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Grabar en la bitacora

            if (!id.HasValue)
            {
                HistoryRecord(flucase.ID, 1, flucase.flow, 1);
                HistoryRecord(flucase.ID, 1, flucase.flow, 3);
            }
            else
            {
                HistoryRecord(id, 2, flucase.flow, 5);
            }

            return Json(flucase.ID);
        }

        private JsonResult SaveChanges()
        {
            db.SaveChanges();
            return Json("Success");
        }

        // GET: Cases/GetRisk/5
        public ActionResult GetRisk(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    Vaccin = flucase.Vaccin,
                    HDisease = flucase.HDisease,
                    Diabetes = flucase.Diabetes,
                    Neuro = flucase.Neuro,
                    Asthma = flucase.Asthma,
                    Pulmonary = flucase.Pulmonary,
                    Liver = flucase.Liver,
                    Renal = flucase.Renal,
                    Immunsupp = flucase.Immunsupp,
                    ParaCerebral= flucase.ParaCerebral,
                    Indigena = flucase.Indigena,
                    TrabSalud = flucase.TrabSalud,
                    Desnutricion = flucase.Desnutricion,
                    Prematuridad = flucase.Prematuridad,
                    BajoPesoNacer = flucase.BajoPesoNacer,
                    AusLacMat = flucase.AusLacMat,
                    Pregnant = flucase.Pregnant,
                    Pperium = flucase.Pperium,
                    Trimester = flucase.Trimester,
                    PregnantWeek = flucase.PregnantWeek,
                    Smoking = flucase.Smoking,
                    Alcohol = flucase.Alcohol,
                    DownSyn = flucase.DownSyn,
                    Obesity = flucase.Obesity,
                    OtherRisk = flucase.OtherRisk,
                    RiskFactors = flucase.RiskFactors,
                    VacInfluenza = flucase.VacInfluenza,
                    VacInfluenzaDateFirst = flucase.VacInfluenzaDateFirst,
                    VacInfluenzaDateSecond = flucase.VacInfluenzaDateSecond,
                    VacBcg = flucase.VacBcg,
                    VacBcgDate = flucase.VacBcgDate,
                    VacBcgDosis = flucase.VacBcgDosis,
                    VacNeumococo = flucase.VacNeumococo,
                    VacNeumococoDate = flucase.VacNeumococoDate,
                    VacNeumococoDosis = flucase.VacNeumococoDosis,
                    VacTosFerina = flucase.VacTosFerina,
                    VacTosFerinaDate = flucase.VacTosFerinaDate,
                    VacTosFerinaDosis = flucase.VacTosFerinaDosis,
                    VacHaemophilus = flucase.VacHaemophilus,
                    VacHaemophilusDate = flucase.VacHaemophilusDate,
                    VaccinFuente = flucase.VaccinFuente,
                    AntiViral = flucase.AntiViral,
                    AntiViralDate = flucase.AntiViralDate,
                    AntiViralDateEnd = flucase.AntiViralDateEnd,
                    AntiViralType = flucase.AntiViralType,
                    OseltaDose = flucase.OseltaDose,
                    AntiViralDose = flucase.AntiViralDose
                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
                 {
                     id = "",
                     Vaccin = 9,
                     HDisease = false,
                     Diabetes = false,
                     Neuro = false,
                     Asthma = false,
                     Pulmonary = false,
                     Liver = false,
                     Renal = false,
                     Immunsupp = false,
                     Pregnant = 0,
                     Pperium = false,
                     Trimester = 0,
                     Smoking = false,
                     Alcohol = false,
                     DownSyn = false,
                     Obesity = 0,
                     OtherRisk = "",
                     AntiViralDose = ""
            },
                 JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveRisk/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveRisk(
                int Id,
                Vaccin? Vaccin,
                bool? HDisease,
                bool? Diabetes,
                bool? Neuro,
                bool? Asthma,
                bool? Pulmonary,
                bool? Liver,
                bool? Renal,
                bool? Immunsupp,
                bool? ParaCerebral,
                bool? Indigena,
                bool? TrabSalud,
                bool? Desnutricion,
                bool? Prematuridad,
                bool? BajoPesoNacer,
                bool? AusLacMat,
                Pregnant? Pregnant,
                bool? Pperium,
                Trimester? Trimester,
                int? PregnantWeek,
                bool? Smoking,
                bool? Alcohol,
                bool? DownSyn,
                Obesity? Obesity,
                string OtherRisk,
                int? VacInfluenza,
                DateTime? VacInfluenzaDateFirst,
                DateTime? VacInfluenzaDateSecond,
                int? VacBcg,
                DateTime? VacBcgDate,
                int? VacBcgDosis,
                int? VacNeumococo,
                DateTime? VacNeumococoDate,
                int? VacNeumococoDosis,
                int? VacTosFerina,
                DateTime? VacTosFerinaDate,
                int? VacTosFerinaDosis,
                int? VacHaemophilus,
                DateTime? VacHaemophilusDate,
                int? VaccinFuente,
                int? AntiViral,
                DateTime? AntiViralDate,
                DateTime? AntiViralDateEnd,
                int? AntiViralType,
                int? OseltaDose,
                string AntiViralDose,
                int? RiskFactors
            )
        {
            FluCase flucase;

            flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.Vaccin = Vaccin;
            flucase.HDisease = HDisease;
            flucase.Diabetes = Diabetes;
            flucase.Neuro = Neuro;
            flucase.Asthma = Asthma;
            flucase.Pulmonary = Pulmonary;
            flucase.Liver = Liver;
            flucase.Renal = Renal;
            flucase.Immunsupp = Immunsupp;

            flucase.ParaCerebral = ParaCerebral;
            flucase.Indigena = Indigena;
            flucase.TrabSalud = TrabSalud;
            flucase.Desnutricion = Desnutricion;
            flucase.Prematuridad = Prematuridad;
            flucase.BajoPesoNacer = BajoPesoNacer;
            flucase.AusLacMat = AusLacMat;

            flucase.Pregnant = Pregnant;
            flucase.Pperium = Pperium;
            flucase.Trimester = Trimester;
            flucase.PregnantWeek = PregnantWeek;
            flucase.Smoking = Smoking;
            flucase.Alcohol = Alcohol;
            flucase.DownSyn = DownSyn;
            flucase.Obesity = Obesity;
            flucase.OtherRisk = OtherRisk;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;

            //flucase.VacInfluenza = (VacInfluenza.ToString() == null) ? VacInfluenza : (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacInfluenza.ToString());
            flucase.VacInfluenza = VacInfluenza;
            flucase.VacInfluenzaDateFirst = VacInfluenzaDateFirst;
            flucase.VacInfluenzaDateSecond = VacInfluenzaDateSecond;
            if (VacBcg != null) flucase.VacBcg = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacBcg.ToString());
            flucase.VacBcgDate = VacBcgDate;
            flucase.VacBcgDosis = VacBcgDosis;
            if (VacNeumococo != null) flucase.VacNeumococo = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacNeumococo.ToString());
            flucase.VacNeumococoDate = VacNeumococoDate;
            flucase.VacNeumococoDosis = VacNeumococoDosis;
            if (VacTosFerina != null) flucase.VacTosFerina = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacTosFerina.ToString());
            flucase.VacTosFerinaDate = VacTosFerinaDate;
            flucase.VacTosFerinaDosis = VacTosFerinaDosis;
            if (VacHaemophilus != null) flucase.VacHaemophilus = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), VacHaemophilus.ToString());
            flucase.VacHaemophilusDate = VacHaemophilusDate;
            flucase.VaccinFuente = VaccinFuente;
            //if (AntiViral != null)  flucase.AntiViral = (VaccineOptions)Enum.Parse(typeof(VaccineOptions), AntiViral.ToString());
            if (AntiViral != null) flucase.AntiViral = AntiViral;
            flucase.AntiViralDate = AntiViralDate;
            flucase.AntiViralDateEnd = AntiViralDateEnd;
            flucase.AntiViralType = AntiViralType;
            flucase.OseltaDose = OseltaDose;
            flucase.AntiViralDose = AntiViralDose;
            if (AntiViral != null)  flucase.RiskFactors = (RiskFactor)Enum.Parse(typeof(RiskFactor), RiskFactors.ToString());

            db.SaveChanges();

            return Json("Success");
        }

        public ActionResult GetLabsHospital(long? institutionId)
        {
            if (institutionId == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            return Json(new
            {
                LabsHospital = GetLabsByInstitution(Convert.ToInt32(institutionId)).Select(x => new { Id = x.ID, x.Name }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private IQueryable<Institution> GetLabsByInstitution(long institutionId) {
            if (institutionId == 0)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                var service_inst = db.Institutions.Where(j => j.Father_ID == user.InstitutionID && j.AccessLevel == AccessLevel.Service).Any();

                if (user.Institution.AccessLevel == AccessLevel.Service || (user.Institution.AccessLevel == AccessLevel.SelfOnly && user.Institution is Hospital && service_inst == true))
                {
                    institutionId = (long)user.InstitutionID;
                } 

            }
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //if (institutionId == 0) { institutionId = Convert.ToInt32(user.InstitutionID); }
            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>()
                    .Where(i => i.InstitutionFromID == institutionId && i.Priority == 1);

            var institutionLabs = institutionsConfiguration.Select(i => i.InstitutionTo);
            if (!institutionLabs.Any()) {
                var user = UserManager.FindById(User.Identity.GetUserId());
                institutionLabs = db.Institutions.OfType<Lab>().Where(i => i.CountryID == user.Institution.CountryID);
            }

            return institutionLabs;
        }

        // GET: Cases/GetHospital/5
        public ActionResult GetHospital(int Id, long institutionId)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            //if ( institutionId <= 0 ) { institutionId = flucase.HospitalID; }

            var institutionLabs = GetLabsByInstitution(institutionId);

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    CHNum = flucase.CHNum,
                    FeverDate = flucase.FeverDate,
                    DiagDate = flucase.DiagDate,
                    SalonVal = flucase.Salon,
                    DiagPrinAdmVal = flucase.DiagAdm,
                    DiagOtroAdm = flucase.DiagOtroAdm,
                    //Antiviral = flucase.Antiviral,
                    AStartDate = flucase.AStartDate,
                    HospAmDate = flucase.HospAmDate,
                    HospExDate = flucase.HospExDate,
                    ICU = flucase.ICU,
                    ICUAmDate = flucase.ICUAmDate,
                    ICUExDate = flucase.ICUExDate,
                    FalleDate = flucase.FalleDate,
                    InstReferName = flucase.InstReferName,
                    Destin = flucase.Destin,
                    DestinICU = flucase.DestinICU,
                    HallRadio = flucase.HallRadio,
                    UCInt = flucase.UCInt,
                    UCri = flucase.UCri,
                    MecVent = flucase.MecVent,
                    MecVentNoInv = flucase.MecVentNoInv,
                    ECMO = flucase.ECMO,
                    VAFO = flucase.VAFO,
                    DiagEgVal = flucase.DiagEg,
                    IsSample = flucase.IsSample,
                    SampleDate = flucase.SampleDate,
                    SampleType = flucase.SampleType,
                    ShipDate = flucase.ShipDate,
                    LabId = flucase.LabID,
                    SampleDate2 = flucase.SampleDate2,
                    SampleType2 = flucase.SampleType2,
                    ShipDate2 = flucase.ShipDate2,
                    LabId2 = flucase.LabID2,
                    SampleDate3 = flucase.SampleDate3,
                    SampleType3 = flucase.SampleType3,
                    ShipDate3 = flucase.ShipDate3,
                    LabId3 = flucase.LabID3,
                    Adenopatia = flucase.Adenopatia,
                    AntecedentesFiebre = flucase.AntecedentesFiebre,
                    Rinorrea = flucase.Rinorrea,
                    Malestar = flucase.Malestar,
                    Nauseas = flucase.Nauseas,
                    DolorMuscular = flucase.DolorMuscular,
                    Disnea = flucase.Disnea,
                    DolorCabeza = flucase.DolorCabeza,
                    Estridor = flucase.Estridor,
                    Tos = flucase.Tos,
                    DifResp = flucase.DifResp,
                    MedSatOxig = flucase.MedSatOxig,
                    SatOxigPor = flucase.SatOxigPor,
                    Tiraje = flucase.Tiraje,
                    Odinofagia = flucase.Odinofagia,
                    CaseStatus = flucase.CaseStatus,
                    CloseDate = flucase.CloseDate,
                    ObservationCase = flucase.ObservationCase,
                    LabsHospital = institutionLabs.Select(x => new { Id = x.ID, x.Name }).ToList(),
                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
            {
                id = "",
                CHNum = "",
                FeverDate = new DateTime?(),
                Antiviral = "N",
                AStartDate = new DateTime?(),
                HospAmDate = new DateTime?(),
                HospExDate = new DateTime?(),
                ICUAmDate = new DateTime?(),
                ICUExDate = new DateTime?(),
                Destin = "",
                IsSample = false,
                SampleDate = new DateTime?(),
                SampleType = "",
                ShipDate = new DateTime?(),
                LabId = "",
                LabsHospital = institutionLabs.Select(x => new { x.ID, x.Name }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveHospital/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveHospital(
                int id,
                string CHNum,
                DateTime? FeverDate,
                int? FeverEW,
                int? FeverEY,
                DateTime? DiagDate,
                int? DiagEW,
                string Antiviral,
                DateTime? AStartDate,
                DateTime? HospAmDate,
                int? HospEW,
                DateTime? HospExDate,
                int? ICU,
                DateTime? ICUAmDate,
                int? ICUEW,
                DateTime? ICUExDate,
                string Destin,
                bool? IsSample,
                DateTime? SampleDate,
                string SampleType,
                DateTime? ShipDate,
                long? LabId,
                DateTime? SampleDate2,
                string SampleType2,
                DateTime? ShipDate2,
                long? LabId2,
                DateTime? SampleDate3,
                string SampleType3,
                DateTime? ShipDate3,
                long? LabId3,
                bool? Adenopatia,
                bool? AntecedentesFiebre,
                bool? Rinorrea,
                bool? Malestar,
                bool? Nauseas,
                bool? DolorMuscular,
                bool? Disnea,
                bool? DolorCabeza,
                bool? Estridor,
                bool? Tos,
                bool? DifResp,
                bool? MedSatOxig,
                int? SatOxigPor,
                int? SalonVal,
                int? DiagPrinAdmVal,
                string DiagOtroAdm,
                string DestinICU,
                DateTime? FalleDate,
                string InstReferName,
                bool? HallRadio,
                bool? UCInt,
                bool? UCri,
                bool? MecVent,
                bool? MecVentNoInv,
                bool? ECMO,
                bool? VAFO,
                int? DiagEgVal,
                bool? Tiraje,
                bool? Odinofagia,
                int? CaseStatus,
                DateTime? CloseDate,
                string ObservationCase,
                int? DataStatement
            )
        {
            FluCase flucase;

            int CaseStatusD = CaseStatus ?? 0;

            flucase = db.FluCases.Find(id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }

            //inicializar llaves
            if (Destin == "") Destin = null;

            flucase.CHNum = CHNum;
            flucase.FeverDate = FeverDate;
            flucase.FeverEW = FeverEW;
            flucase.FeverEY = FeverEY;
            flucase.DiagDate = DiagDate;
            flucase.DiagEW = DiagEW;
            //flucase.Antiviral = Antiviral;
            flucase.AStartDate = AStartDate;
            flucase.HospAmDate = HospAmDate;
            flucase.HospEW = HospEW;
            flucase.HospExDate = HospExDate;
            flucase.ICU = ICU;
            flucase.ICUAmDate = ICUAmDate;
            flucase.ICUEW = ICUEW;
            flucase.ICUExDate = ICUExDate;
            flucase.Destin = Destin;
            flucase.FalleDate = FalleDate;
            flucase.InstReferName = InstReferName;
            flucase.IsSample = IsSample;
            flucase.SampleDate = SampleDate;
            flucase.SampleType = SampleType;
            flucase.ShipDate = ShipDate;
            flucase.LabID = LabId;
            flucase.SampleDate2 = SampleDate2;
            flucase.SampleType2 = SampleType2;
            flucase.ShipDate2 = ShipDate2;
            flucase.LabID2 = LabId2;
            flucase.SampleDate3 = SampleDate3;
            flucase.SampleType3 = SampleType3;
            flucase.ShipDate3 = ShipDate3;
            flucase.LabID3 = LabId3;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;
            flucase.Adenopatia = Adenopatia;
            flucase.AntecedentesFiebre = AntecedentesFiebre;
            flucase.Rinorrea = Rinorrea;
            flucase.Malestar = Malestar;
            flucase.Nauseas = Nauseas;
            flucase.DolorMuscular = DolorMuscular;
            flucase.Disnea = Disnea;
            flucase.DolorCabeza = DolorCabeza;
            flucase.Estridor = Estridor;
            flucase.Tos = Tos;
            flucase.DifResp = DifResp;
            flucase.MedSatOxig = MedSatOxig;
            flucase.SatOxigPor = SatOxigPor;
            flucase.Salon = SalonVal;
            flucase.DiagAdm = DiagPrinAdmVal;
            flucase.DiagOtroAdm = DiagOtroAdm;
            flucase.DestinICU = DestinICU;
            flucase.HallRadio = HallRadio;
            flucase.UCInt = UCInt;
            flucase.UCri = UCri;
            flucase.MecVent = MecVent;
            flucase.MecVentNoInv = MecVentNoInv;
            flucase.ECMO = ECMO;
            flucase.VAFO = VAFO;
            flucase.DiagEg = DiagEgVal;
            flucase.Tiraje = Tiraje;
            flucase.Odinofagia = Odinofagia;
            flucase.CaseStatus = CaseStatus;
            if (CaseStatusD > 0)
                flucase.flow = 99 ;
            flucase.statement = DataStatement;
            flucase.CloseDate = CloseDate;
            flucase.ObservationCase = ObservationCase;
            db.SaveChanges();

            if (flucase.flow == 99 )
                HistoryRecord(flucase.ID, 1, flucase.flow, 9);

            return Json("Success");
        }

        //// GET: Cases/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Case @case = db.FluCases.Find(id);
        //    if (@case == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(@case);
        //}

        //// POST: Cases/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Case @case = db.FluCases.Find(id);
        //    db.FluCases.Remove(@case);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        // GET: Cases/GetGEO/5
        public ActionResult GetGEO(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    CountryID = flucase.CountryID,
                    AreaID = flucase.AreaID,
                    LocalID = flucase.LocalID,
                    StateID = flucase.StateID,
                    NeighborhoodID = flucase.NeighborhoodID,
                    UrbanRural = flucase.UrbanRural,
                    CountryID2weeks = flucase.CountryID2weeks,
                    AreaID2weeks = flucase.AreaID2weeks,
                    StateID2weeks = flucase.StateID2weeks,
                    NeighborhoodID2weeks = flucase.NeighborhoodID2weeks,
                    Address = flucase.Address,
                    PhoneNumber = flucase.PhoneNumber,
                    Latitude = flucase.Latitude,
                    Longitude = flucase.Longitude,
                    CountryOrigin = flucase.CountryOrigin
                }, JsonRequestBehavior.AllowGet);
            };
            return Json(new
            {
                id = "",
                CountryID = "",
                AreaID = "",
                StateID = "",
                LocalID = "",
                NeighborhoodID = "",
                UrbanRural = UrbanRural.Unknow,
                Address = "",
                CountryOrigin = ""
            },
            JsonRequestBehavior.AllowGet);
        }

        // POST: Cases/SaveGEO/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveGEO(
                int id,
                int? CountryId,
                int? AreaId,
                int? StateId,
                int? LocalId,
                int? NeighborhoodId,
                UrbanRural UrbanRural,
                int? CountryId2weeks,
                int? AreaId2weeks,
                int? StateId2weeks,
                int? LocalId2weeks,
                int? NeighborhoodId2weeks,
                string Address,
                int? CountryOrigin,
                string PhoneNumber,
                string Latitude,
                string Longitude
            )
        {
            FluCase flucase;

            flucase = db.FluCases.Find(id);
            if (flucase == null)
            {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            }
            else
            {
                db.Entry(flucase).State = EntityState.Modified;
            }
            flucase.CountryID = CountryId;
            flucase.AreaID = AreaId;
            flucase.StateID = StateId;
            flucase.LocalID = LocalId;
            flucase.NeighborhoodID = NeighborhoodId;
            flucase.UrbanRural = UrbanRural;
            flucase.CountryID2weeks = CountryId2weeks;
            flucase.AreaID2weeks = AreaId2weeks;
            flucase.StateID2weeks = StateId2weeks;
            flucase.NeighborhoodID2weeks = NeighborhoodId2weeks;
            flucase.Address = Address;
            flucase.PhoneNumber = PhoneNumber;
            flucase.Latitude = Latitude;
            flucase.Longitude = Longitude;
            flucase.CountryOrigin = CountryOrigin;
            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;

            db.SaveChanges();

            return Json("Success");
        }

        // GET: Cases/GetGEO/5
        public ActionResult GetLab(int Id)
        {
            FluCase flucase = db.FluCases.Find(Id);
            if (flucase == null)
            {
                return HttpNotFound();
            }

            IQueryable<Institution> institutions = null;


            var user = UserManager.FindById(User.Identity.GetUserId());
            institutions = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID);
            var institutionsIds = institutions.Select(x => (long?)x.ID).ToArray();

            var canConclude = true;
            var CanPCRLab = db.Institutions.Where(i => i.ID == user.Institution.ID).First()?.PCR;
            var CanIFILab = db.Institutions.Where(i => i.ID == user.Institution.ID).First()?.IFI;

            var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
            var institutionActualFlow = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.Priority == flucase.flow && i.InstitutionParentID == flucase.HospitalID);
            var flow_local_lab = 0;
            var flow_statement = flucase.statement ?? 1;
            var flow_open_always = false;
            if (institutionsConfiguration.Any()) {
                canConclude = institutionsConfiguration.Count(x => x.Conclusion == true) > 0;
                CanPCRLab = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID).First()?.PCR;
                CanIFILab = db.Institutions.OfType<Lab>().Where(i => i.ID == user.Institution.ID).First()?.IFI;
                flow_local_lab = institutionsConfiguration.First().Priority;
                flow_open_always = institutionsConfiguration.First().OpenAlways;
                //canConclude = institutionsConfiguration.Count(x => x.Conclusion == 1) > 0; // Original - modificado por AM
            }



            if (flucase != null)
            {
                return Json(new
                {
                    id = flucase.ID.ToString(),
                    RecDate = flucase.RecDate,
                    Processed = flucase.Processed,
                    RecDate2 = flucase.RecDate2,
                    Processed2 = flucase.Processed2,
                    RecDate3 = flucase.RecDate3,
                    Processed3 = flucase.Processed3,
                    EndLabDate = flucase.EndLabDate,
                    FResult = flucase.FResult,
                    NoProRen = flucase.NoProRen,
                    NoProRenId = flucase.NoProRenId,
                    TempSample1 = flucase.TempSample1,

                    NoProRen2 = flucase.NoProRen2,
                    NoProRenId2 = flucase.NoProRenId2,
                    TempSample2 = flucase.TempSample2,

                    NoProRen3 = flucase.NoProRen3,
                    NoProRenId3 = flucase.NoProRenId3,
                    TempSample3 = flucase.TempSample3,

                    Comments = flucase.Comments,
                    FinalResult = flucase.FinalResult,
                    FinalResultVirusTypeID = flucase.FinalResultVirusTypeID,
                    FinalResultVirusSubTypeID = flucase.FinalResultVirusSubTypeID,
                    FinalResultVirusLineageID = flucase.FinalResultVirusLineageID,
                    FinalResult_2 = flucase.FinalResult_2,
                    FinalResultVirusTypeID_2 = flucase.FinalResultVirusTypeID_2,
                    FinalResultVirusSubTypeID_2 = flucase.FinalResultVirusSubTypeID_2,
                    FinalResultVirusLineageID_2 = flucase.FinalResultVirusLineageID_2,
                    FinalResult_3 = flucase.FinalResult_3,
                    FinalResultVirusTypeID_3 = flucase.FinalResultVirusTypeID_3,
                    FinalResultVirusSubTypeID_3 = flucase.FinalResultVirusSubTypeID_3,
                    FinalResultVirusLineageID_3 = flucase.FinalResultVirusLineageID_3,
                    flow_record = flucase.flow,
                    flow_institution = flow_local_lab,
                    flow_open_always = flow_open_always,
                    DataStatement = flow_statement,
                    CanPCRLab = CanPCRLab,
                    CanIFILab = CanIFILab,
                    LabTests = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 1 || caselabtest.SampleNumber == null
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              CTRLOtherVirusType = caselabtest.CTRLOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              TestResultID_VirusSubType = caselabtest.TestResultID_VirusSubType,
                              VirusSubTypeID_2 = caselabtest.VirusSubTypeID_2,
                              CTSubType_2 = caselabtest.CTSubType_2,
                              CTRLSubType_2 = caselabtest.CTRLSubType_2,
                              TestResultID_VirusSubType_2 = caselabtest.TestResultID_VirusSubType_2,

                              InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              ParaInfI = caselabtest.ParaInfI,
                              ParaInfII = caselabtest.ParaInfII,
                              ParaInfIII = caselabtest.ParaInfIII,
                              RSV = caselabtest.RSV,
                              Adenovirus = caselabtest.Adenovirus,
                              Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              //CanEdit = flucase.flow == 99 ? false : (((flucase.flow - 1) == flow_local_lab || flucase.flow == flow_local_lab) & flow_local_lab > 0  && flow_statement == 1) ? true : false, //institutionsIds.Contains(caselabtest.LabID),
                              CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN"
                          }
                      )
                      .OrderBy(d => d.TestDate)
                      .ThenBy(c=> c.ProcLab)
                      .ToArray(),
                    LabTests_Sample2 = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 2
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              CTRLOtherVirusType = caselabtest.CTRLOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              ParaInfI = caselabtest.ParaInfI,
                              ParaInfII = caselabtest.ParaInfII,
                              ParaInfIII = caselabtest.ParaInfIII,
                              RSV = caselabtest.RSV,
                              Adenovirus = caselabtest.Adenovirus,
                              Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN"
                          }
                      )
                      .OrderBy(c => c.ProcLab)
                      .ThenBy(d => d.TestDate)
                      .ToArray(),
                    LabTests_Sample3 = (
                          from caselabtest in flucase.CaseLabTests
                          where caselabtest.SampleNumber == 3
                          select new
                          {
                              Id = caselabtest.ID,
                              CaseLabID = caselabtest.FluCaseID,
                              ProcLab = caselabtest.LabID.ToString(),
                              ProcLabName = GetLabName(caselabtest.LabID),
                              ProcessLab = caselabtest.Processed,
                              SampleNumber = caselabtest.SampleNumber,
                              TestType = caselabtest.TestType,
                              TestDate = caselabtest.TestDate,
                              TestResultID = caselabtest.TestResultID,
                              VirusTypeID = caselabtest.VirusTypeID,
                              CTVirusType = caselabtest.CTVirusType,
                              CTRLVirusType = caselabtest.CTRLVirusType,
                              OtherVirusTypeID = caselabtest.OtherVirusTypeID,
                              CTOtherVirusType = caselabtest.CTOtherVirusType,
                              OtherVirus = caselabtest.OtherVirus,
                              InfA = caselabtest.InfA,
                              VirusSubTypeID = caselabtest.VirusSubTypeID,
                              CTSubType = caselabtest.CTSubType,
                              CTRLSubType = caselabtest.CTRLSubType,
                              InfB = caselabtest.InfB,
                              VirusLineageID = caselabtest.VirusLineageID,
                              CTLineage = caselabtest.CTLineage,
                              CTRLLineage = caselabtest.CTRLLineage,
                              ParaInfI = caselabtest.ParaInfI,
                              ParaInfII = caselabtest.ParaInfII,
                              ParaInfIII = caselabtest.ParaInfIII,
                              RSV = caselabtest.RSV,
                              Adenovirus = caselabtest.Adenovirus,
                              Metapneumovirus = caselabtest.Metapneumovirus,
                              RNP = caselabtest.RNP,
                              CTRLRNP = caselabtest.CTRLRNP,
                              CTRLNegative = caselabtest.CTRLNegative,
                              TestEndDate = caselabtest.TestEndDate,
                              CanEdit = institutionsIds.Contains(caselabtest.LabID),
                              CanPCR = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.PCR,
                              CanIFI = db.Institutions.OfType<Lab>().Where(i => i.ID == caselabtest.LabID).First()?.IFI,
                              EndFlow = institutionActualFlow.Any() ? ((caselabtest.TestResultID == "N") ? db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID).Any().ToString().ToUpper() : db.InstitutionConfEndFlowByVirus.Where(j => j.id_InstCnf == institutionActualFlow.FirstOrDefault().ID && j.id_Cat_TestType == caselabtest.TestType && j.value_Cat_TestResult == caselabtest.TestResultID && j.id_Cat_VirusType == caselabtest.VirusTypeID).Any().ToString().ToUpper()) : "UNKNOWN"
                          }
                      )
                      .OrderBy(c => c.ProcLab)
                      .ThenBy(d => d.TestDate)
                      .ToArray(),
                    LabsResult = institutions.Select(x => new { Id = x.ID.ToString(), x.Name }).ToList(),
                    SubTypeByLabRes = GetSubTypebyLab(user.InstitutionID),
                    CanConclude = canConclude

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    id = "",
                    RecDate = new DateTime?(),
                    Processed = "",
                    EndLabDate = new DateTime?(),
                    FResult = "",
                    NoProRen ="",
                    Comments = "",
                    LabTests = ""
                },
                     JsonRequestBehavior.AllowGet);
            }
        }

        public string GetLabName(long? id)
        {
            if (id == null) return "";
            var labName = db.Institutions.OfType<Lab>().Where(i => i.ID == id).First()?.Name;
            return labName;
        }

        public object GetSubTypebyLab(long? id)
        {
            IQueryable<CatVirusSubType> CVST_Test = null;
            var user = UserManager.FindById(User.Identity.GetUserId());

            var CatVirusSubtypeConfiguration = db.CatVirusSubTypeConfByLab.OfType<CatVirusSubTypeConfByLab>()
        .Where(i => i.id_Institution == id);

            if (!CatVirusSubtypeConfiguration.Any())
            {
                CVST_Test = db.CatVirusSubType.OrderBy(i => i.orden);
            }
            else
            {
                var ListVaccines = CatVirusSubtypeConfiguration.Select(i => i.id_CatSubType).ToList();
                CVST_Test = db.CatVirusSubType.Where(k => ListVaccines.Contains(k.ID));
            }

            return (user.Institution.Country.Language == "SPA" ?  CVST_Test.Select(x => new { Id = x.ID.ToString(), Name = x.SPA }).ToList() : CVST_Test.Select(x => new { Id = x.ID.ToString(), Name = x.ENG }).ToList());
        }

        // POST: Cases/SaveGEO/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult SaveLab(
                int id,
                DateTime? RecDate,
                bool? Processed,
                DateTime? RecDate2,
                bool? Processed2,
                DateTime? RecDate3,
                bool? Processed3,
                DateTime? EndLabDate,
                string FResult,
                int? NoProRenId,
                string NoProRen,
                Decimal? TempSample1,
                int? NoProRenId2,
                string NoProRen2,
                Decimal? TempSample2,
                int? NoProRenId3,
                string NoProRen3,
                Decimal? TempSample3,
                string Comments,
                string FinalResult,
                int? FinalResultVirusTypeID,
                int? FinalResultVirusSubTypeID,
                int? FinalResultVirusLineageID,
                string FinalResult_2,
                int? FinalResultVirusTypeID_2,
                int? FinalResultVirusSubTypeID_2,
                int? FinalResultVirusLineageID_2,
                string FinalResult_3,
                int? FinalResultVirusTypeID_3,
                int? FinalResultVirusSubTypeID_3,
                int? FinalResultVirusLineageID_3,
                int? DataStatement,
                List<LabTestViewModel> LabTests
            )
        {
            FluCase flucase;
            var IFI_RecordHistory = false;
            var PCR_RecordHistory = false;
            var IFI_Count = 0;
            var PCR_Count = 0;
            var user = UserManager.FindById(User.Identity.GetUserId());

            flucase = db.FluCases.Find(id);

            if (flucase == null) {
                flucase = new FluCase();
                db.Entry(flucase).State = EntityState.Added;
            } else {
                db.Entry(flucase).State = EntityState.Modified;
            }

            IFI_Count = flucase.CaseLabTests.Where(e => e.FluCaseID == id && e.TestType == (TestType)1 && e.Processed != null).Count();
            PCR_Count = flucase.CaseLabTests.Where(e => e.FluCaseID == id && e.TestType == (TestType)2 && e.Processed != null).Count();

            flucase.RecDate = RecDate;
            flucase.Processed = Processed;
            flucase.RecDate2 = RecDate2;
            flucase.Processed2 = Processed2;
            flucase.RecDate3 = RecDate3;
            flucase.Processed3 = Processed3;
            flucase.EndLabDate = EndLabDate;
            flucase.FResult = FResult;
            flucase.NoProRen = NoProRen;
            flucase.NoProRenId = NoProRenId;
            flucase.TempSample1 = TempSample1;

            flucase.NoProRen2 = NoProRen2;
            flucase.NoProRenId2 = NoProRenId2;
            flucase.TempSample2 = TempSample2;

            flucase.NoProRen3 = NoProRen3;
            flucase.NoProRenId3 = NoProRenId3;
            flucase.TempSample3 = TempSample3;

            flucase.Comments = Comments;
            flucase.FinalResult = FinalResult;
            flucase.FinalResultVirusTypeID = FinalResultVirusTypeID;
            flucase.FinalResultVirusSubTypeID = FinalResultVirusSubTypeID;
            flucase.FinalResultVirusLineageID = FinalResultVirusLineageID;
            flucase.FinalResult_2 = FinalResult_2;
            flucase.FinalResultVirusTypeID_2 = FinalResultVirusTypeID_2;
            flucase.FinalResultVirusSubTypeID_2 = FinalResultVirusSubTypeID_2;
            flucase.FinalResultVirusLineageID_2 = FinalResultVirusLineageID_2;
            flucase.FinalResult_3 = FinalResult_3;
            flucase.FinalResultVirusTypeID_3 = FinalResultVirusTypeID_3;
            flucase.FinalResultVirusSubTypeID_3 = FinalResultVirusSubTypeID_3;
            flucase.FinalResultVirusLineageID_3 = FinalResultVirusLineageID_3;
            //flucase.flow = 2;

            //if (db.CaseLabTests.Count() > 0) PCR_IFI_RecordHistory = true;

            db.CaseLabTests.RemoveRange(flucase.CaseLabTests);
            var existrecordlabtest = false;
            flucase.CaseLabTests = new List<CaseLabTest>();
            if (LabTests != null) {
                foreach (LabTestViewModel labTestViewModel in LabTests.OrderBy(x=>x.SampleNumber).ThenBy(z => z.TestDate).ThenBy(y=>y.LabID)) {
                    if (labTestViewModel.LabID == user.InstitutionID)
                        existrecordlabtest = true;
                    if (labTestViewModel.TestType == 1) IFI_RecordHistory = true;
                    if (labTestViewModel.TestType == 2) PCR_RecordHistory = true;

                    flucase.CaseLabTests.Add(
                        new CaseLabTest {

                            LabID = labTestViewModel.LabID,
                            Processed = labTestViewModel.Processed,
                            SampleNumber = labTestViewModel.SampleNumber,
                            TestDate = labTestViewModel.TestDate,
                            TestResultID = labTestViewModel.TestResultID,
                            TestEndDate = labTestViewModel.TestEndDate,
                            TestType = (TestType)labTestViewModel.TestType,
                            VirusTypeID = labTestViewModel.VirusTypeID,
                            CTVirusType = labTestViewModel.CTVirusType,
                            CTRLVirusType = labTestViewModel.CTRLVirusType,
                            OtherVirusTypeID = labTestViewModel.OtherVirusTypeID,
                            CTOtherVirusType = labTestViewModel.CTOtherVirusType,
                            CTRLOtherVirusType = labTestViewModel.CTRLOtherVirusType,
                            OtherVirus = labTestViewModel.OtherVirus,
                            InfA = labTestViewModel.InfA,
                            VirusSubTypeID = labTestViewModel.VirusSubTypeID,
                            CTSubType = labTestViewModel.CTSubType,
                            CTRLSubType = labTestViewModel.CTRLSubType,
                            TestResultID_VirusSubType = labTestViewModel.TestResultID_VirusSubType,
                            VirusSubTypeID_2 = labTestViewModel.VirusSubTypeID_2,
                            CTSubType_2 = labTestViewModel.CTSubType_2,
                            CTRLSubType_2 = labTestViewModel.CTRLSubType_2,
                            TestResultID_VirusSubType_2 = labTestViewModel.TestResultID_VirusSubType_2,

                            InfB = labTestViewModel.InfB,
                            VirusLineageID = labTestViewModel.VirusLineageID,
                            CTLineage = labTestViewModel.CTLineage,
                            CTRLLineage = labTestViewModel.CTRLLineage,
                            ParaInfI = labTestViewModel.ParaInfI,
                            ParaInfII = labTestViewModel.ParaInfII,
                            ParaInfIII = labTestViewModel.ParaInfIII,
                            RNP = labTestViewModel.RNP,
                            CTRLRNP = labTestViewModel.CTRLRNP,
                            CTRLNegative = labTestViewModel.CTRLNegative,
                            RSV = labTestViewModel.RSV,
                            Adenovirus = labTestViewModel.Adenovirus,
                            Metapneumovirus = labTestViewModel.Metapneumovirus,
                            FluCaseID = flucase.ID
                        }
                    );
                }
            }

            if (user.Institution is Lab && existrecordlabtest == true)
            {
                var institutionsConfiguration = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(i => i.InstitutionToID == user.Institution.ID && i.InstitutionParentID == flucase.HospitalID);
                if (institutionsConfiguration.Any())
                {
                    var flow_temp = institutionsConfiguration.First().Priority;
                    if (flow_temp > flucase.flow || flucase.flow == null)
                        flucase.flow = flow_temp;
                }
                else
                {
                    flucase.flow = 1;
                }
                if (flucase.flow != 99)
                    flucase.statement = DataStatement;
            }

            flucase.InsertDate = DateTime.Now;
            //flucase.UserID = User.Identity.Name;

            var result = SaveChanges();

            var tests = flucase.CaseLabTests.ToList();
            if (tests.Count <= 0) return result;

            if (tests.Count >= 1) flucase.MuestraID1 = tests[0].ID;
            if (tests.Count >= 2) flucase.MuestraID2 = tests[1].ID;
            if (tests.Count >= 3) flucase.MuestraID3 = tests[2].ID;
            if (tests.Count >= 4) flucase.MuestraID4 = tests[3].ID;
            if (tests.Count >= 5) flucase.MuestraID5 = tests[4].ID;
            if (tests.Count >= 6) flucase.MuestraID6 = tests[5].ID;

            if (tests.Count >= 7) flucase.MuestraID7 = tests[6].ID;
            if (tests.Count >= 8) flucase.MuestraID8 = tests[7].ID;
            if (tests.Count >= 9) flucase.MuestraID9 = tests[8].ID;
            if (tests.Count >= 10) flucase.MuestraID10 = tests[9].ID;
            if (tests.Count >= 11) flucase.MuestraID11 = tests[10].ID;
            if (tests.Count >= 12) flucase.MuestraID12 = tests[11].ID;
            if (tests.Count >= 13) flucase.MuestraID13 = tests[12].ID;
            if (tests.Count >= 14) flucase.MuestraID14 = tests[13].ID;
            if (tests.Count >= 15) flucase.MuestraID15 = tests[14].ID;

            result = SaveChanges();

            //Bitacora
            if (IFI_Count == 0 && PCR_Count == 0)
            HistoryRecord(flucase.ID, 4, flucase.flow, 1);

            if (IFI_Count > 0 || PCR_Count > 0)
                HistoryRecord(flucase.ID, 4, flucase.flow, 5);

            if (IFI_RecordHistory && IFI_Count == 0)
            HistoryRecord(flucase.ID, 4, flucase.flow, 7);

            if (PCR_RecordHistory && PCR_Count == 0)
                HistoryRecord(flucase.ID, 4, flucase.flow, 8);

            

            return result;
        }
        public JsonResult GetRecordHistory(int caseId)
        {
            List<Dictionary<string, string>> logsPerUser = new List<Dictionary<string, string>>();

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var user_lang = user.Institution.Country.Language.ToString() ?? "SPA";

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("B1", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Record_ID", SqlDbType.Int).Value = caseId;
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int caseNumber = 0;
                            int operationNumber = 0;
                            string actionDesc = "";
                            string userOperator = "";
                            string healthUnit = "";
                            string operationDate = "";
                            //x = reader.GetValue(1);
                            Dictionary<string, string> userRecord = new Dictionary<string, string>();
                            userRecord.Add("caseNumber", reader["caseNumber"].ToString());
                            userRecord.Add("operationNumber", reader["operationNumber"].ToString());
                            userRecord.Add("actionDesc", reader["actionDesc"].ToString());
                            userRecord.Add("userOperator", reader["userOperator"].ToString());
                            userRecord.Add("healthUnit", reader["healthUnit"].ToString());
                            userRecord.Add("stateRecord", (user_lang == "ENG") ? reader["stateRecord_ENG"].ToString() : reader["stateRecord"].ToString());
                            userRecord.Add("operationDate", reader["operationDate"].ToString());
                            logsPerUser.Add(userRecord);

                        }
                    }
                }
                return Json(logsPerUser, JsonRequestBehavior.AllowGet);
            }
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

        [Authorize]
        public JsonResult GetPatientInformation(int DTP, string DNP)
        {
            var dataforpatron = false;
            var user = UserManager.FindById(User.Identity.GetUserId());
            List<Dictionary<string, string>> PatientInformation_ = new List<Dictionary<string, string>>();
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                if (user.Institution.CountryID == 9) {
                    var number_ = (int)(Int64) Convert.ToDouble(DNP);
                    using (var command = new SqlCommand("PatientInformationCR", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@DocumentNumberP", SqlDbType.BigInt).Value = number_;
                        command.Parameters.Add("@DocumentTypeP", SqlDbType.Int).Value = DTP;
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //x = reader.GetValue(1);
                                Dictionary<string, string> PatientInformation = new Dictionary<string, string>();
                                var nombre = reader["NOM_PERSONA"].ToString().Split(' ');
                                PatientInformation.Add("nombre1", nombre[0].ToString());
                                PatientInformation.Add("nombre2", nombre.Length > 1 ? nombre[1].ToString():"");
                                PatientInformation.Add("apellido1", reader["NOM_APELLIDO1"].ToString());
                                PatientInformation.Add("apellido2", reader["NOM_APELLIDO2"].ToString());
                                PatientInformation.Add("sexo", reader["IND_SEXO"].ToString() == "M" ? "Male" : reader["IND_SEXO"].ToString() == "F" ? "Female" : "Unknown");
                                PatientInformation.Add("DOB", Convert.ToDateTime(reader["FEC_NACIMIENTO"]).ToString("yyyy-MM-dd"));
                                //PatientInformation.Add("value", reader["Val_Padron"].ToString( ));
                                PatientInformation_.Add(PatientInformation);
                                dataforpatron = true;

                            }
                        }
                        con.Close();
                    }
                }

                if (dataforpatron == false)
                {
                    using (var command = new SqlCommand("PatientInformation", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@DocumentNumberP", SqlDbType.NVarChar).Value = DNP;
                        command.Parameters.Add("@DocumentTypeP", SqlDbType.Int).Value = DTP;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = user.InstitutionID;
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //x = reader.GetValue(1);
                                Dictionary<string, string> PatientInformation = new Dictionary<string, string>();
                                PatientInformation.Add("nombre1", reader["FName1"].ToString());
                                PatientInformation.Add("nombre2", reader["FName2"].ToString());
                                PatientInformation.Add("apellido1", reader["LName1"].ToString());
                                PatientInformation.Add("apellido2", reader["LName2"].ToString());
                                PatientInformation.Add("sexo", reader["Gender"].ToString() == "1" ? "Male" : reader["Gender"].ToString() == "2" ? "Female" : "Unknown");
                                //PatientInformation.Add("DOB", JsonConvert.SerializeObject(reader["DOB"], new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat }));
                                PatientInformation.Add("DOB", Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd"));
                                PatientInformation_.Add(PatientInformation);
                                dataforpatron = true;

                            }
                        }
                        con.Close();
                    }
                }

                return Json(PatientInformation_, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

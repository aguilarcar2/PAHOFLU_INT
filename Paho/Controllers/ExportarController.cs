using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using Paho.Reports.Entities;
using System.Drawing;
using System.Net;
using System.Data.Entity;
using System.Globalization;
using System.Collections;
using System.Web.Script.Serialization;
using Paho.Utility;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Report")]
    public class ExportarController : ControllerBase
    {
        // GET:Exportar
        public ActionResult Index()
        {
            var ExportarViewModel = new ExportarViewModel();
            IQueryable<Institution> institutions = null;
            IQueryable<Paho.Models.Region> regions = null;
            //IQueryable<Paho.Models.Area> areas = null;
            IQueryable<Area> areas = null;                          //#### CAFQ: 180703 
            IQueryable<ReportCountry> ReportsCountries = null;

            var user = UserManager.FindById(User.Identity.GetUserId());
            ExportarViewModel.CountryID = user.Institution.CountryID ?? 0;
            ReportsCountries = db.ReportsCountries.Where(i => i.CountryID == user.Institution.CountryID && i.active == true);

            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                ExportarViewModel.DisplayCountries = true;
                ExportarViewModel.DisplayAreas = true;              //#### CAFQ: 180703   
                ExportarViewModel.DisplayRegionals = true;
                ExportarViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                ExportarViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(n => n.FullName);

                    if (user.type_region == null)
                    {     // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);
                    }
                    else
                    {
                        // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == user.type_region).OrderBy(i => i.Name);
                    }

                    areas = db.Areas.Where(d => d.CountryID == user.Institution.CountryID).OrderBy(i => i.Name);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(n => n.FullName);
                }
                else if (user.Institution is Hospital && user.Institution.AccessLevel != AccessLevel.Service)//línea agregada de la versión en inglés. Según AM, sirve para los servicios
                {

                    institutions = db.Institutions.OfType<Hospital>()
                                .Where(i => i.Father_ID == user.InstitutionID || i.ID == user.InstitutionID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Regional)
                {
                    if (user.type_region == 1 || user.type_region == null)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_institucional == user.Institution.cod_region_institucional).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 2)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_salud == user.Institution.cod_region_salud).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 3)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_pais == user.Institution.cod_region_pais).OrderBy(j => j.FullName);
                    }
                }
                else
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }
            else
            {
                ExportarViewModel.DisplayHospitals = true;

                var inst_by_lab = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(j => j.InstitutionToID == user.InstitutionID).Select(i => i.InstitutionParentID).ToList();
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.CountryID == user.Institution.CountryID).OrderBy(j => j.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID).OrderBy(j => j.FullName);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Regional)
                {
                    if (user.type_region == 1 || user.type_region == null)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_institucional == user.Institution.cod_region_institucional).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 2)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_salud == user.Institution.cod_region_salud).OrderBy(j => j.FullName);
                    }
                    else if (user.type_region == 3)
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID && i.cod_region_pais == user.Institution.cod_region_pais).OrderBy(j => j.FullName);
                    }
                }
                else
                {
                    if (inst_by_lab.Any())
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => inst_by_lab.Contains(i.ID));
                    }
                    else
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.ID == user.Institution.ID);
                    }

                }
            }

            var DoS = DateTime.Now.ToString(getMsg("msgDatePickerConfig"));
            var date_format = getMsg("msgDateFormatDP");

            //CaseViewModel.UsrCtry = user.Institution.CountryID;
            ExportarViewModel.DatePickerConfig = DoS;
            ExportarViewModel.DateFormatDP = date_format;

            ExportarViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (ReportsCountries != null)
            {
                ExportarViewModel.ReportsCountries = ReportsCountries.Select(i => new LookupView<ReportCountry>()
                {
                    Id = i.ID.ToString(),
                    Name = i.description,
                }).ToArray();
            }

            //**** 
            if (areas != null)
            {
                var areasDisplay = areas.Select(i => new LookupView<Area>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).OrderBy(d => d.Name).ToList();

                if (areasDisplay.Count() > 1)
                {
                    //var all = new LookupView<Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Area> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    areasDisplay.Insert(0, all);
                    ExportarViewModel.DisplayAreas = true;
                }

                ExportarViewModel.Areas = areasDisplay;
                //CaseViewModel.Regions = regions;
            }

            if (regions != null)
            {
                var regionsDisplay = regions.Select(i => new LookupView<Paho.Models.Region>()
                {
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                }).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Paho.Models.Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Paho.Models.Region> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    regionsDisplay.Insert(0, all);
                    ExportarViewModel.DisplayRegionals = true;
                }

                ExportarViewModel.Regions = regionsDisplay;
                //CaseViewModel.Regions = regions;
            }

            if (institutions != null)
            {
                var institutionsDisplay = institutions.Select(i => new LookupView<Paho.Models.Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Paho.Models.Institution> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Paho.Models.Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionsDisplay.Insert(0, all);
                }

                ExportarViewModel.Institutions = institutionsDisplay;
            };

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

            return View(ExportarViewModel);
        }

        private IQueryable<FluCase> GetQuery(int CountryID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate)
        {
            IQueryable<FluCase> query = db.Set<FluCase>();
            // assuming that you return all records when nothing is specified in the filter
            query = query.Where(t =>
                   t.CountryID == CountryID);

            if (HospitalID.HasValue)
                query = query.Where(t =>
                    t.HospitalID == HospitalID);

            if (Year.HasValue)
                query = query.Where(t =>
                    t.HospitalDate.Year == Year.Value);

            if (Month.HasValue)
                query = query.Where(t =>
                    t.HospitalDate.Month == Month.Value);

            if (SE.HasValue)
                query = query.Where(t =>
                    t.HospEW == SE.Value);

            if (StartDate.HasValue)
                query = query.Where(t =>
                   t.HospitalDate >= StartDate.Value);

            if (EndDate.HasValue)
                query = query.Where(t =>
                   t.HospitalDate >= EndDate.Value);

            return query;
        }

        [HttpGet]
        public ActionResult GetExcel(string Report, int CountryID, int? RegionID, int? HospitalID, int? Year, int? Month, int? SE, 
            DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? Inusual, 
            string CasosNPHL = "", int? Area = 0, int? Sentinel = null)        //#### CAFQ
        {
            try
            {
                var ms = new MemoryStream();

                if (Surv == 3)
                {
                    Surv = 0;
                    Inusual = true;
                }
                else if (Surv == 0)
                    Inusual = null;

                int AreaID_ = (int)Area;
                var user = UserManager.FindById(User.Identity.GetUserId());
                int CountryID_ = (CountryID >= 0) ? CountryID : (user.Institution.CountryID ?? 0);
                //int? HospitalID_ = (user.Institution.Father_ID > 0 || user.Institution.Father_ID == null) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                //int? HospitalID_ = (HospitalID >= 0) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                int? HospitalID_ = (HospitalID > 0) ? HospitalID : 0;
                int? RegionID_ = (RegionID >= 0) ? RegionID : (user.Institution.cod_region_institucional ?? 0);
                string Languaje_ = user.Institution.Country.Language ?? "SPA";
                string Country_Code = user.Institution.Country.Code;

                var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.ID == ReportCountry);
                int reportID = reportCountry.ReportID;//contiene la FK para obtener el ID del reporte en la tabla Report
                int reportStartCol = reportCountry.startCol;//contiene la startCol de la tabla ReportCountry, pasa saber en que columna se inicia
                int reportStartRow = reportCountry.startRow;//contiene la startRow de la tabla ReportCountry, pasa saber en que fila se inicia
                var report = db.Reports.FirstOrDefault(s => s.ID == reportID);
                string reportTemplate = report.Template;//contiene el nombre del template, que luego se relacionará al archivo template de Excel

                if (user.Institution.AccessLevel == AccessLevel.SelfOnly && HospitalID_ == 0) { HospitalID_ = Convert.ToInt32(user.Institution.ID); }
                if (user.Institution.AccessLevel == AccessLevel.Area && Area == 0) { AreaID_ = Convert.ToInt32(user.Institution.AreaID); }

                if (ReportCountry < 1)
                {
                    ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
                    return null;
                    ;
                }

                string templateToUse;

                switch (reportTemplate.ToUpper())
                {
                    case "D1":
                        templateToUse = "DenominadoresTemplate";
                        break;
                    case "B1":
                        templateToUse = "BitacoraTemplate";
                        break;
                    case "I1":
                        templateToUse = "IndicadoresDesempenio";            //#### CAFQ en web.config crear la variable 
                        break;
                    case "RE1":
                        templateToUse = "REVELAC-i";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "ML1":
                        templateToUse = "MuestrasLabTemplate";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "FM1":
                        templateToUse = "FormSariIliHospDeathTemplate";                        //#### CAFQ en web.config crear la variable 
                        break;
                    case "FLUID":
                        templateToUse = "FluIDTemplate";
                        break;
                    case "CC":
                        templateToUse = "ConsolidadoCargaTemplate";
                        break;
                    default:
                        templateToUse = "SariTemplate";
                        break;
                }

                var tempy = ConfigurationManager.AppSettings[templateToUse]
                    .Replace("{report}", reportTemplate)
                    .Replace("{countryId}", CountryID_.ToString());

                var tempy2 = System.IO.File.Exists(tempy);

                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings[templateToUse]
                    .Replace("{report}", reportTemplate)
                    .Replace("{countryId}", CountryID_.ToString())
                    ))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        bool insertRow = true;

                        //#### CAFQ: 180204
                        bool bVariosAnios = false;
                        if (YearFrom != null && YearTo != null)
                        {
                            bVariosAnios = (YearFrom != YearTo) ? true : false;
                        }
                        //#### 

                        if (reportTemplate == "R1" || reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R4" || reportTemplate == "D1" || reportTemplate == "B1")
                        {
                            insertRow = false;
                        }

                        if (reportTemplate == "I1")      //#### CAFQ
                            AppendDataToExcel_IndDes(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        else if (reportTemplate == "RE1")
                            AppendDataToExcel_REVELAC(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        else if (reportTemplate == "CC")
                            AppendDataToExcel_ConsolidadoCarga(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, Sentinel);        //#### CAFQ
                        else if (reportTemplate.ToUpper() == "FLUID")
                        {
                            var contador = 0;
                            var YearBegin = 0;
                            var YearEnd = 0;

                            if (YearFrom != null && YearTo != null)
                            {
                                YearBegin = (int)YearFrom;
                                YearEnd = (int)YearTo;
                            }
                            else if (Year != null)
                            {
                                YearBegin = (int)Year;
                                YearEnd = (int)Year;
                            }

                            var excelWs_VIRUSES_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "NATIONAL VIRUSES" : "Virus Identificados"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 1, excelWs_VIRUSES_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual);
                            var excelWs_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI" : "IRAG"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_IRAG", 8, 1, excelWs_IRAG.Index, false, ReportCountry, YearBegin, YearEnd, 1, Inusual);
                            var excelWs_DEATHS_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "DEATHS Sentinel Sites" : "Fallecidos IRAG"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_DEATHS_IRAG", 8, 1, excelWs_DEATHS_IRAG.Index, false, ReportCountry, YearEnd, YearEnd, 1, Inusual);
                            var excelWs_ILI = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI" : "ETI"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_ETI", 8, 1, excelWs_ILI.Index, false, ReportCountry, YearBegin, YearEnd, 2, Inusual);
                            var excelWs_VIRUSES_ILI = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "ILI VIRUSES - Sentinel" : "Virus ETI Identificados"];
                            AppendDataToExcel_FLUID(Languaje_, CountryID_, RegionID_, null, HospitalID, Month, SE, StartDate, EndDate, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 1, excelWs_VIRUSES_ILI.Index, false, ReportCountry, YearEnd, YearEnd, 2, Inusual);

                            // Leyendas
                            var excelWs_Leyendas = excelWorkBook.Worksheets["Leyendas"];
                            ConfigToExcel_FLUID(CountryID, Languaje_, RegionID, Year, HospitalID, excelWorkBook, "Leyendas", 1, excelWs_Leyendas.Index, false);

                            // Manejo de graficas 

                            contador = YearEnd - YearBegin;
                            if (contador > 0)
                            {
                                var excelWs_Graph_IRAG = excelWorkBook.Worksheets[(user.Institution.Country.Language == "ENG") ? "SARI Graphs" : "Gráficos IRAG"];
                                for (int i = contador; i >= 0; i--)
                                {
                                    ConfigGraph_FLUID(YearEnd - contador, excelWorkBook, excelWs_Graph_IRAG.Index);
                                }
                            }                   
                        }
                        else if (reportTemplate == "FM1")
                            AppendDataToExcel_FormSariIliHospDeath(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ
                        else if ((reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R1") && bVariosAnios)
                            AppendDataToExcel_R2_SeveralYears(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_);        //#### CAFQ: 180204
                        else
                        {
                            AppendDataToExcel(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual, AreaID_, CasosNPHL);        //#### CAFQ
                        }

                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;

                string nombFile = reportCountry.description == "" ? "Exportable_" : Country_Code+ "_" + reportCountry.description.ToString().Replace("%", "_").Replace(" ", "_") + "_";            //#### CAFQ

				return new FileStreamResult(ms, "application/xlsx")
                    {
                        FileDownloadName = nombFile + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"           //#### CAFQ
                    };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo: " + e.Message;
            }

            return null;
        }

        private void AppendDataToExcel_FormSariIliHospDeath(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string _storedProcedure = "", consString = "";

            var user = UserManager.FindById(User.Identity.GetUserId());
            bool ili = user.Institution.ILI;
            bool sari = user.Institution.SARI;

            if (sari)
            {
                _storedProcedure = "R6";
                consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;
                            int nFiLe = 0;

                            var n1 = 0;
                            var n2 = 0;
                            var n3 = 0;
                            var n4 = 0;
                            var n5 = 0;
                            var n6 = 0;

                            while (reader.Read())
                            {
                                ++nFiLe;                                // Fila leida
                                //************************************ Hospitalizaciones Denominadores
                                for (int i = 0; i < 53; i++)
                                    n5 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n5 += (int)reader.GetValue(i);      // Femenino

                                //**** Hospitalizaciones SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n1 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n1 += (int)reader.GetValue(i);      // Masculino

                                //************************************ ICU Denominadores
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n4 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n4 += (int)reader.GetValue(i);      // Femenino

                                //**** ICU SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n2 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n2 += (int)reader.GetValue(i);      // Masculino

                                //************************************ Death Denominadores
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n6 += (int)reader.GetValue(i);          // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n6 += (int)reader.GetValue(i);      // Femenino

                                //**** Death SARI
                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n3 += (int)reader.GetValue(i);      // Masculino

                                if (reader.Read())
                                    for (int i = 0; i < 53; i++)
                                        n3 += (int)reader.GetValue(i);      // Masculino

                                //************************************
                                excelWorksheet.Cells[row + 0, col].Value = n1;      // Admissions SARI
                                excelWorksheet.Cells[row + 1, col].Value = n3;      // Deaths SARI
                                excelWorksheet.Cells[row + 2, col].Value = n2;      // ICU admissions SARI

                                excelWorksheet.Cells[row + 3, col].Value = n4;      // ICU Denominadores
                                excelWorksheet.Cells[row + 7, col].Value = n5;      // Hospital admissions Denominadores
                                excelWorksheet.Cells[row + 8, col].Value = n6;      // Deaths Denominadores
                                //****
                                ++col;
                                n1 = 0;
                                n2 = 0;
                                n3 = 0;
                                n4 = 0;
                                n5 = 0;
                                n6 = 0;
                                //****
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }

                //****
                row = startRow;
                column = startColumn;
                _storedProcedure = "FLUID_IRAG_Total_Muestra_Analizadas_AgeGroup";

                using (var con2 = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con2) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con2.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;

                            while (reader.Read())
                            {
                                excelWorksheet.Cells[row + 4, col].Value = (int)reader.GetValue(2);
                                ++col;
                            }
                        }

                        command.Parameters.Clear();
                        con2.Close();
                    }
                }
            }       // End sari

            if (ili)
            {
                row = startRow;
                column = startColumn;
                _storedProcedure = "Summary_ILI_Cases_AgeGroup";

                using (var con2 = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(_storedProcedure, con2) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                        command.Parameters.Add("@Area_ID", SqlDbType.Int).Value = AreaId;                        //#### CAFQ

                        con2.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            var col = startColumn;
                            int nFiLe = 0;
                            int nGrup = 0;

                            int nCase1 = 0;
                            int nCase2 = 0;
                            int nCase3 = 0;
                            int nSamp1 = 0;
                            int nSamp2 = 0;
                            int nSamp3 = 0;
                            int nVisi1 = 0;
                            int nVisi2 = 0;
                            int nVisi3 = 0;

                            while (reader.Read())
                            {
                                ++nFiLe;
                                ++nGrup;

                                //for (int i = 0; i < 2; i++)
                                //{
                                if (nGrup == 1)
                                {
                                    nCase1 += (int)reader.GetValue(4);
                                    nSamp1 += (int)reader.GetValue(5);
                                    nVisi1 += (int)reader.GetValue(6);
                                }
                                else if (nGrup == 2)
                                {
                                    nCase2 += (int)reader.GetValue(4);
                                    nSamp2 += (int)reader.GetValue(5);
                                    nVisi2 += (int)reader.GetValue(6);
                                }
                                else if (nGrup == 3)
                                {
                                    nCase3 += (int)reader.GetValue(4);
                                    nSamp3 += (int)reader.GetValue(5);
                                    nVisi3 += (int)reader.GetValue(6);
                                }
                                //}

                                if (nFiLe % 3 == 0)
                                {
                                    nGrup = 0;
                                }
                            }
                            //****
                            row = row + 12;
                            excelWorksheet.Cells[row, col + 1].Value = nCase1;
                            excelWorksheet.Cells[row, col + 2].Value = nCase2;
                            excelWorksheet.Cells[row, col + 3].Value = nCase3;
                            ++row;
                            excelWorksheet.Cells[row, col + 1].Value = nSamp1;
                            excelWorksheet.Cells[row, col + 2].Value = nSamp2;
                            excelWorksheet.Cells[row, col + 3].Value = nSamp3;
                            ++row;
                            excelWorksheet.Cells[row, col + 1].Value = nVisi1;
                            excelWorksheet.Cells[row, col + 2].Value = nVisi2;
                            excelWorksheet.Cells[row, col + 3].Value = nVisi3;
                        }

                        command.Parameters.Clear();
                        con2.Close();
                    }
                }
            }       // End ili

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet, AreaId);
            //InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet);
        }

        private static void AppendDataToExcel_R2_SeveralYears(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            int nColumns = 0;

            //var xxx = excelWorksheet.Drawings;                // Coleccion de charts
            //var yyy = excelWorksheet.Drawings[0];             // Un chart especifico
            if (storedProcedure == "R2" || storedProcedure == "R3" || storedProcedure == "R1")
                excelWorksheet.Drawings.Remove(0);              // Eliminando grafico por defecto en plantilla

            string cLabelAxixY = "";
            if (storedProcedure == "R2")
                cLabelAxixY = (string)excelWorksheet.Cells[startRow - 1, startColumn + 1].Value;        // Label eje Y

            var row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
            {
                if (storedProcedure == "R2")
                {
                    row = startRow;
                    var col = startColumn + (nI - YearFrom.Value) + 1;

                    if (nI > YearFrom.Value)
                    {
                        excelWorksheet.Cells[row - 1, startColumn + 1].Copy(excelWorksheet.Cells[row - 1, col]);
                        excelWorksheet.Cells[row - 1, col].Value = nI;          // Year

                        excelWorksheet.Cells[startRow + 53, startColumn + 1].Copy(excelWorksheet.Cells[startRow + 53, col]);                // Total
                        excelWorksheet.Cells[startRow + 53, col].StyleID = excelWorksheet.Cells[startRow + 53, startColumn + 1].StyleID;

                        excelWorksheet.Cells[startRow + 52, col].StyleID = excelWorksheet.Cells[startRow + 52, startColumn + 1].StyleID;    // Fila 53
                    }
                    else
                    {
                        excelWorksheet.Cells[row - 1, col].Value = nI;          // Year
                    }
                }
                else if (storedProcedure == "R1" || storedProcedure == "R3")
                {
                    row = startRow + (nI - YearFrom.Value) * 54;
                }

                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = nI;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = nI;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            //int nColums;
                            nColumns = (storedProcedure == "R3") ? ((reader.FieldCount - 1) / 2) + 1 : reader.FieldCount;

                            while (reader.Read())
                            {
                                var col = column;
                                var cell = excelWorksheet.Cells[row, col];

                                if (row > startRow && insert_row == true)
                                    excelWorksheet.InsertRow(row, 1);

                                if (storedProcedure == "R2")
                                    col = column + (nI - YearFrom.Value) + ((nI == YearFrom) ? 0 : 1);

                                //for (var i = 0; i < reader.FieldCount; i++)
                                for (var i = 0; i < nColumns; i++)
                                {
                                    if (storedProcedure == "R2")
                                    {
                                        if (nI > YearFrom && i > 0)
                                            --col;
                                    }

                                    if (reader.GetValue(i) != null)
                                    {
                                        double numberD;
                                        bool isNumber = double.TryParse(reader.GetValue(i).ToString(), out numberD);

                                        DateTime dt;
                                        bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);

                                        if (isNumber)
                                        {
                                            excelWorksheet.Cells[row, col].Value = numberD;
                                        }
                                        else
                                        {
                                            if (isDate)
                                            {
                                                excelWorksheet.Cells[row, col].Value = dt;
                                            }
                                            else
                                            {
                                                excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                            }
                                        }
                                        excelWorksheet.Cells[row, col].StyleID = cell.StyleID;
                                    }
                                    col++;
                                }

                                if (storedProcedure == "R1")
                                {
                                    excelWorksheet.Cells[row, col].FormulaR1C1 = "IF(RC[-1]<=0, 0, (RC[-2]*100)/RC[-1])";
                                    excelWorksheet.Cells[row, col].Style.Numberformat.Format = "##0";
                                    excelWorksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                }

                                row++;
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }   // End for

            //**** INSERTAR GRAFICO
            if (storedProcedure == "R2")        // Total fallecidos por IRAG
            {
                var myChartCC = excelWorksheet.Drawings.AddChart("ChartColumnClustered", eChartType.ColumnClustered);

                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    int nCol = startColumn + (nI - YearFrom.Value) + 1;
                    var seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(7, nCol, 59, nCol), ExcelRange.GetAddress(7, 2, 59, 2));
                    seriesCC.Header = nI.ToString();
                }

                //myChartCC.Border.Fill.Color = System.Drawing.Color.Red;
                if (languaje_ == "ENG")
                    myChartCC.Title.Text = "NUMBERS OF DEATHS BY EPIDEMIOLOGICAL WEEK";
                else
                    myChartCC.Title.Text = "NÚMERO DE FALLECIDOS POR SEMANA EPIDEMIOLÓGICA";
                myChartCC.Title.Font.Bold = true;
                myChartCC.Legend.Position = eLegendPosition.Bottom;
                myChartCC.SetSize(920, 405);                    // Ancho, Alto in pixel
                myChartCC.SetPosition(startRow - 2, 0, (startColumn + (YearTo.Value - YearFrom.Value) + 1), 40);             // (int row, int rowoffset in pixel, int col, int coloffset in pixel)

                myChartCC.YAxis.Title.Text = cLabelAxixY;
                myChartCC.YAxis.Title.Font.Size = 9;
                myChartCC.YAxis.Title.Font.Bold = true;

                myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, startColumn].Value;    // Semana epidemiologica
                myChartCC.XAxis.Title.Font.Size = 9;
                myChartCC.XAxis.Title.Font.Bold = true;
            }
            else if (storedProcedure == "R3")       // Casos por IRAG y Hospitalizaciones Totales
            {
                int nFil = startRow;
                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    nFil = startRow + (nI - YearFrom.Value) * 54;
                    int nCol = startColumn;

                    var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStacked" + nI.ToString(), eChartType.ColumnStacked);

                    for (int nK = 1; nK < nColumns; ++nK)
                    {
                        var seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + nK, nFil + 52, nCol + nK), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                        seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + nK].Value.ToString();
                    }

                    if (languaje_ == "ENG")
                        myChartCC.Title.Text = "NUMBER OF SARI CASES BY AGE GROUP AND E. W. - " + nI.ToString();
                    else
                        myChartCC.Title.Text = "NÚMERO DE CASOS IRAG POR GRUPO DE EDAD Y S. E. - " + nI.ToString();
                    myChartCC.Title.Font.Bold = true;
                    myChartCC.SetSize(1090, 450);
                    myChartCC.SetPosition(startRow + (23 * (nI - YearFrom.Value)) - 1, 0, nColumns + 1, 40);
                    myChartCC.Legend.Position = eLegendPosition.Bottom;

                    myChartCC.YAxis.Title.Text = (languaje_ == "ENG") ? "Number of SARI cases" : "Número de casos SARI";
                    myChartCC.YAxis.Title.Font.Size = 9;
                    myChartCC.YAxis.Title.Font.Bold = true;

                    myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, startColumn].Value;    // Semana epidemiologica
                    myChartCC.XAxis.Title.Font.Size = 9;
                    myChartCC.XAxis.Title.Font.Bold = true;

                    // Formateo area de datos
                    var rowA = startRow + (nI - YearFrom.Value) * 54;

                    ExcelRange rRang = excelWorksheet.Cells[ExcelRange.GetAddress(rowA, startColumn, rowA + 53, startColumn + nColumns - 1)];
                    rRang.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    rRang.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    rRang.Style.Border.BorderAround(ExcelBorderStyle.Medium);

                    if (nI > YearFrom.Value)
                    {
                        rRang = excelWorksheet.Cells[ExcelRange.GetAddress(startRow + 53, startColumn, startRow + 53, startColumn + nColumns - 1)];
                        rRang.Copy(excelWorksheet.Cells[rowA + 53, startColumn]);                // Total
                    }
                }
            }
            else if (storedProcedure == "R1")       // Número de casos y % de hospitalizaciones por IRAG
            {
                int nFil = startRow;
                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    nFil = startRow + (nI - YearFrom.Value) * 54;
                    int nCol = startColumn;

                    //****
                    var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStackedLine" + nI.ToString(), eChartType.ColumnClustered);
                    var serieCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 1, nFil + 52, nCol + 1), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    serieCC.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;    // Leyenda

                    myChartCC.XAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol].Value;
                    myChartCC.XAxis.Title.Font.Size = 9;
                    myChartCC.XAxis.Title.Font.Bold = true;
                    myChartCC.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 1].Value;
                    myChartCC.YAxis.Title.Font.Size = 9;
                    myChartCC.YAxis.Title.Font.Bold = true;

                    //****
                    var myChartLI = myChartCC.PlotArea.ChartTypes.Add(eChartType.Line);
                    var serieLI = myChartLI.Series.Add(ExcelRange.GetAddress(nFil, nCol + 3, nFil + 52, nCol + 3), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    serieLI.Header = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;    // Leyenda;

                    myChartLI.UseSecondaryAxis = true;
                    myChartLI.YAxis.Title.Text = (string)excelWorksheet.Cells[startRow - 1, nCol + 3].Value;
                    myChartLI.YAxis.Title.Font.Size = 9;
                    myChartLI.YAxis.Title.Font.Bold = true;

                    //****
                    if (languaje_ == "ENG")
                        myChartCC.Title.Text = "CASES OF SARI BY EPIDEMIOLOGICAL WEEK  WITH % OF HOSPITALIZATIONS - " + nI.ToString();
                    else
                        myChartCC.Title.Text = "CASOS DE IRAG POR SEMANA EPIDEMIOLOGICA CON % DE HOSPITALIZACIONES - " + nI.ToString();
                    myChartCC.Title.Font.Bold = true;
                    myChartCC.SetSize(900, 420);
                    myChartCC.SetPosition(startRow + (23 * (nI - YearFrom.Value)) - 1, 0, 5, 40);
                    myChartCC.Legend.Position = eLegendPosition.Bottom;
                    myChartCC.Legend.Font.Bold = true;

                    //**** Formateo area de datos
                    if (nI > YearFrom.Value)
                    {
                        var rowA = startRow + (nI - YearFrom.Value) * 54;

                        ExcelRange rRang = excelWorksheet.Cells[ExcelRange.GetAddress(rowA, startColumn, rowA + 53, startColumn + 3)];
                        rRang.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        rRang.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rRang.Style.Border.BorderAround(ExcelBorderStyle.Medium);

                        rRang = excelWorksheet.Cells[ExcelRange.GetAddress(startRow + 53, startColumn, startRow + 53, startColumn + 3)];
                        rRang.Copy(excelWorksheet.Cells[rowA + 53, startColumn]);                // Total
                    }
                }
            }

            //**** inserción de labels y logo en el Excel
            if (ReportCountry != null)
            {
                //inserción de labels
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);
            }
        }

        private static void InsertarLogoExcel(string consString, ExcelWorksheet excelWorksheet, int? ReportCountry)
        {
            //inserción de logo
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountry where ID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = ReportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["logoRow"];
                            int insertcol = (int)reader["logoCol"];
                            String imagurl = (String)reader["logo"];
                            if (imagurl != "")
                            {
                                try
                                {
                                    WebClient wc = new WebClient();
                                    byte[] bytes = wc.DownloadData(imagurl);
                                    MemoryStream ms = new MemoryStream(bytes);
                                    Bitmap newImage = new Bitmap(System.Drawing.Image.FromStream(ms));
                                    excelWorksheet.Row(insertrow).Height = newImage.Height;
                                    var picture = excelWorksheet.Drawings.AddPicture("reportlogo", newImage);
                                    picture.SetPosition(insertrow, -(newImage.Height), insertcol, -100);
                                    //picture.SetPosition(insertrow, 0, insertcol, 0);
                                }
                                catch (Exception e)
                                {
                                    //ViewBag.Message = "No se insertó el logo, porque no es accesible";
                                }
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }   // End
        }

        //private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo)
        private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se,
            DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet,
            bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId, string CasosNPHL = "")               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string _storedProcedure;
            int excelColTota = 0, nPosiTipo = 0, nInicTip2 = 0, nPoSuViGr = 0;
            Dictionary<string, string> casosNHPLNumber = new Dictionary<string, string>();

            _storedProcedure = (storedProcedure == "ML1") ? "MuestrasLabNPHL" : storedProcedure;      //#### CAFQ
            if (storedProcedure == "R5")
            {
                if (countryId == 17 || countryId == 119 || countryId == 11)
                {
                    _storedProcedure = "R5_JM";
                    nPosiTipo = 21;                 // Posic. columna "Tipo" (tabla retornada x SP)
                    nInicTip2 = 11;                 // Inicio hospitalizados (tabla retornada x SP)
                    nPoSuViGr = 13;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                }
                else
                {
                    if (countryId == 25)
                    {
                        _storedProcedure = "R5_2";
                        nPosiTipo = 19;                 // Posic. columna "Tipo" (tabla retornada x SP)
                        nInicTip2 = 10;                 // Inicio hospitalizados (tabla retornada x SP)
                        nPoSuViGr = 12;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                    }
                    else
                    {
                        nPosiTipo = 15;                 // Posic. columna "Tipo" (tabla retornada x SP)
                        nInicTip2 = 8;                  // Inicio hospitalizados (tabla retornada x SP)
                        nPoSuViGr = 10;                 // Posic. Excel Sumatoria (Columna "Total hospitalized cases")
                    }
                }
            }

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                //using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                //using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure })     //**** CAFQ
                using (var command = new SqlCommand(_storedProcedure, con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })    //**** CAFQ
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                    if (storedProcedure == "ML1")                                                       // Muestras laboratorio NPHL
                    {
                        CasosNPHL = getIdentificarCasos(CasosNPHL, casosNHPLNumber);
                        command.Parameters.Add("@CasosNPHL", SqlDbType.Text).Value = CasosNPHL;
                    }
                    con.Open();
                    if ((storedProcedure == "R5"))
                    {
                        string VRS;
                        VRS = (languaje_ == "ENG") ? "RSV" : "VRS";

                        IDictionary<int, string> formulas1 = new Dictionary<int, string>();
                        formulas1[1] = "=" + VRS + "!B{{toreplace}}";
                        formulas1[2] = "=" + VRS + "!C{{toreplace}} + Ad!C{{toreplace}} + Parainfluenza!C{{toreplace}} + 'Inf A'!C{{toreplace}} + 'Inf B'!C{{toreplace}} + Metapnemovirus!C{{toreplace}}";
                        formulas1[3] = "=" + VRS + "!C{{toreplace}}";
                        formulas1[4] = "=Ad!C{{toreplace}}";
                        formulas1[5] = "=Parainfluenza!C{{toreplace}}";
                        formulas1[6] = "='Inf A'!C{{toreplace}}";
                        formulas1[7] = "='Inf B'!C{{toreplace}}";
                        formulas1[8] = "=Metapnemovirus!C{{toreplace}}";

                        if (countryId == 17 || countryId == 119 || countryId == 11)
                        {
                            formulas1[9] = "=" + VRS + "!M{{toreplace}}";
                            formulas1[10] = "=" + VRS + "!N{{toreplace}}+Ad!N{{toreplace}}+Parainfluenza!N{{toreplace}}+'Inf A'!N{{toreplace}}+'Inf B'!N{{toreplace}}+Metapnemovirus!N{{toreplace}}";

                            formulas1[11] = "=" + VRS + "!N{{toreplace}}";
                            formulas1[12] = "=Ad!N{{toreplace}}";
                            formulas1[13] = "=Parainfluenza!N{{toreplace}}";
                            formulas1[14] = "='Inf A'!N{{toreplace}}";
                            formulas1[15] = "='Inf B'!N{{toreplace}}";
                            formulas1[16] = "=Metapnemovirus!N{{toreplace}}";

                            formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}+J{{toreplace}}+K{{toreplace}}+L{{toreplace}}";
                            formulas1[18] = "=O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}+R{{toreplace}}+S{{toreplace}}+T{{toreplace}}+U{{toreplace}}+V{{toreplace}}+W{{toreplace}}";
                        }
                        else
                        {
                            if (countryId == 25)
                            {
                                formulas1[9] = "=" + VRS + "!L{{toreplace}}";
                                formulas1[10] = "=" + VRS + "!M{{toreplace}}+Ad!M{{toreplace}}+Parainfluenza!M{{toreplace}}+'Inf A'!M{{toreplace}}+'Inf B'!M{{toreplace}}+Metapnemovirus!M{{toreplace}}";

                                formulas1[11] = "=" + VRS + "!M{{toreplace}}";
                                formulas1[12] = "=Ad!M{{toreplace}}";
                                formulas1[13] = "=Parainfluenza!M{{toreplace}}";
                                formulas1[14] = "='Inf A'!M{{toreplace}}";
                                formulas1[15] = "='Inf B'!M{{toreplace}}";
                                formulas1[16] = "=Metapnemovirus!M{{toreplace}}";

                                formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}+J{{toreplace}}+K{{toreplace}}";
                                formulas1[18] = "=N{{toreplace}}+O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}+R{{toreplace}}+S{{toreplace}}+T{{toreplace}}+U{{toreplace}}";
                            }
                            else
                            {
                                formulas1[9] = "=" + VRS + "!J{{toreplace}}";
                                formulas1[10] = "=" + VRS + "!K{{toreplace}}+Ad!K{{toreplace}}+Parainfluenza!K{{toreplace}}+'Inf A'!K{{toreplace}}+'Inf B'!K{{toreplace}}+Metapnemovirus!K{{toreplace}}";

                                formulas1[11] = "=" + VRS + "!K{{toreplace}}";
                                formulas1[12] = "=Ad!K{{toreplace}}";
                                formulas1[13] = "=Parainfluenza!K{{toreplace}}";
                                formulas1[14] = "='Inf A'!K{{toreplace}}";
                                formulas1[15] = "='Inf B'!K{{toreplace}}";
                                formulas1[16] = "=Metapnemovirus!K{{toreplace}}";

                                formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}";
                                formulas1[18] = "=L{{toreplace}}+M{{toreplace}}+N{{toreplace}}+O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}";
                            }
                        }

                        for (int i = 1; i <= 7; i++)            // i: Hoja
                        {
                            var virustype = 0;
                            switch (i)
                            {
                                case 2:
                                    virustype = 6;
                                    break;
                                case 3:
                                    virustype = 7;
                                    break;
                                case 4:
                                    virustype = 13;
                                    break;
                                case 5:
                                    virustype = 1;
                                    break;
                                case 6:
                                    virustype = 2;
                                    break;
                                case 7:
                                    virustype = 8;
                                    break;
                            }
                            command.Parameters.Clear();
                            command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                            command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                            command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                            command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                            command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                            command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                            command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                            command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                            command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                            command.Parameters.Add("@Virus_type", SqlDbType.Int).Value = virustype;
                            command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                            command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                            command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                            command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                            var excelWorksheet2 = excelWorkBook.Worksheets[i];

                            using (var reader = command.ExecuteReader())
                            {
                                excelColTota = reader.FieldCount + 1;
                                row = 3;
                                column = 1;
                                var tipo_anterior = 1;
                                while (reader.Read())
                                {
                                    //if (Convert.ToInt32(reader.GetValue(15)) != tipo_anterior)
                                    if (Convert.ToInt32(reader.GetValue(nPosiTipo)) != tipo_anterior)
                                    {
                                        row++;
                                    }
                                    //tipo_anterior = Convert.ToInt32(reader.GetValue(15));
                                    tipo_anterior = Convert.ToInt32(reader.GetValue(nPosiTipo));
                                    var col = 1;
                                    var readercont = 0;
                                    int stylerow;
                                    if (tipo_anterior == 1)
                                    {
                                        stylerow = 2;
                                    }
                                    else
                                    {
                                        stylerow = row + 1;
                                    }
                                    excelWorksheet2.InsertRow(row, 1);
                                    //excelWorksheet.InsertRow(row, 1);
                                    //for (int j = 0; j < 17; j++)
                                    for (int j = 0; j < excelColTota; j++)                              // Total columnas retornadas x consulta
                                    {
                                        var cell = excelWorksheet2.Cells[stylerow, col + j];
                                        //if (tipo_anterior == 2 && j > 8)
                                        if (tipo_anterior == 2 && j > nInicTip2)            // nInicTip2: Inicio hospitalizados (8 o 10)
                                        {
                                        }
                                        else
                                        {
                                            if (i > 1 || j == 0)
                                            {
                                                if (j == 2)
                                                {
                                                    excelWorksheet2.Cells[row, col + j].Formula = formulas1[17].Replace("{{toreplace}}", row.ToString()); ;
                                                }
                                                else
                                                {
                                                    //if (j == 10)
                                                    if (j == nPoSuViGr)                 // Totales fila  de virus hospitalizados
                                                    {
                                                        excelWorksheet2.Cells[row, col + j].Formula = formulas1[18].Replace("{{toreplace}}", row.ToString()); ;
                                                    }
                                                    else
                                                    {
                                                        excelWorksheet2.Cells[row, col + j].Value = reader.GetValue(readercont);
                                                        readercont++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //if (formulas1.ContainsKey(j))
                                                if (j < 17 && formulas1.ContainsKey(j))
                                                {
                                                    //excelWorksheet2.Cells[row, col + j].Value = 8555;
                                                    excelWorksheet2.Cells[row, col + j].Formula = formulas1[j].Replace("{{toreplace}}", row.ToString()); ;
                                                    //excelWorksheet2.Cells[row, col + j].Formula = "=5+1";
                                                }
                                            }
                                        }
                                        excelWorksheet2.Cells[row, col + j].StyleID = cell.StyleID;
                                    }
                                    row++;
                                }
                                //Si se borran las rows auxiliares, las referencias de las funciones ya no funcionan?
                                //excelWorksheet2.DeleteRow(row);
                                //excelWorksheet2.DeleteRow(2);
                            }
                        }
                    }
                    else
                    {
                        if ((storedProcedure == "R6"))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                row = 9;
                                column = 6;
                                var contador_salto = 0;
                                while (reader.Read())
                                {
                                    var col = column;

                                    for (int i = 0; i < 53; i++)
                                    {
                                        var cell = excelWorksheet.Cells[startRow, col + i];
                                        excelWorksheet.Cells[row, col + i].Value = reader.GetValue(i);
                                    }
                                    contador_salto++;
                                    if (contador_salto % 4 == 2)
                                    {
                                        row += 2;
                                    }
                                    else
                                    {
                                        if (contador_salto % 4 == 0)
                                        {
                                            row += 5;
                                        }
                                        else
                                        {
                                            row++;
                                        }
                                    }

                                }
                            }
                            /*Inicia llenado de Tabla2. Este reporte, en el mismo tab tiene 2 tablas, con aspectos diferentes entre ellas*/
                            using (var command2 = new SqlCommand(storedProcedure + "_2", con) { CommandType = CommandType.StoredProcedure })
                            {
                                command2.Parameters.Clear();
                                command2.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                                command2.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                                command2.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                                command2.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                                command2.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                                command2.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                                command2.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                                command2.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                                command2.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                                command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                                command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                                command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                                command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ

                                var con2 = new SqlConnection(consString);
                                con2.Open();

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    int nAnDa = 0;
                                    if (countryId == 25)
                                    {
                                        row = row - 1 + (9 * 3) + 15;
                                        nAnDa = 8 * 8;                  // 8: Nº Age Group
                                    }
                                    else
                                    {
                                        if (countryId == 17 || countryId == 119 || countryId == 11)
                                        {
                                            row = row - 1 + (9 * 3) + 15;
                                            nAnDa = 9 * 8;              // 9: Nº Age Group
                                        }
                                        else
                                        {
                                            row = 212;
                                            nAnDa = 6 * 8;              // 6: Nº Age Group
                                        }
                                    }
                                    column = 6;

                                    var contador_salto2 = 0;
                                    while (reader2.Read())
                                    {
                                        var col2 = column;

                                        for (int i = 5; i < 58; i++)
                                        {
                                            var cell = excelWorksheet.Cells[startRow, col2 + i - 5];
                                            excelWorksheet.Cells[row, col2 + i - 5].Value = reader2.GetValue(i);
                                        }
                                        contador_salto2++;

                                        if (contador_salto2 % nAnDa == 0)
                                        {
                                            row += 5;
                                        }
                                        else
                                        {
                                            if (contador_salto2 % 2 == 0)
                                            {
                                                row += 2;
                                            }
                                            else
                                            {
                                                row++;
                                            }
                                        }
                                    }
                                }
                                /*Termina llenado de Tabla2*/
                            }
                        }
                        else        // R1, R2, R3, R4 etc
                        {
                            bool procesarFila;

                            using (var reader = command.ExecuteReader())
                            {
                                int nColums;
                                nColums = (storedProcedure == "R3") ? ((reader.FieldCount - 1) / 2) + 1 : reader.FieldCount;

                                while (reader.Read())
                                {
                                    var col = column;
                                    if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                                    procesarFila = true;
                                    if (storedProcedure == "ML1")
                                    {
                                        if (!casosNHPLNumber.ContainsKey((string)reader.GetValue(0)))
                                            procesarFila = false;
                                    }

                                    if (procesarFila)
                                    {
                                        for (var i = 0; i < nColums; i++)
                                        {
                                            var cell = excelWorksheet.Cells[startRow, col];
                                            if (reader.GetValue(i) != null)
                                            {
                                                var datoColu = reader.GetValue(i);

                                                double numberD;
                                                //bool isNumber = double.TryParse(reader.GetValue(i).ToString(), out numberD);
                                                bool isNumber = double.TryParse(datoColu.ToString(), out numberD);

                                                DateTime dt;
                                                //bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);
                                                bool isDate = DateTime.TryParse(datoColu.ToString(), out dt);

                                                if (isNumber)
                                                    excelWorksheet.Cells[row, col].Value = numberD;
                                                else
                                                {
                                                    if (isDate)
                                                        excelWorksheet.Cells[row, col].Value = dt;
                                                    else
                                                    {
                                                        //excelWorksheet.Cells[row, col].Style.Numberformat.Format = "@";
                                                        excelWorksheet.Cells[row, col].Value = (string)datoColu.ToString();
                                                    }
                                                }
                                                excelWorksheet.Cells[row, col].StyleID = cell.StyleID;
                                            }
                                            col++;
                                        }
                                        row++;
                                    }
                                }
                            }
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            /*Inserción de tablas en el tab  de antecedentes del reporte R6*/
            if (storedProcedure == "R6")
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand("R6_antecedentes", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                        command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                        command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                        command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                        command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            row = 11;
                            column = 2;
                            var contador_salto = 0;
                            var excelWorksheet2 = excelWorkBook.Worksheets[2];
                            while (reader.Read())
                            {
                                var col = column;
                                var colcont = 1;
                                for (int i = 0; i < 6; i++)
                                {
                                    var cell = excelWorksheet2.Cells[startRow, col + colcont];
                                    excelWorksheet2.Cells[row, col + colcont].Value = reader.GetValue(i);
                                    if (i == 1 || i == 3)
                                    {
                                        colcont += 4;
                                    }
                                    else
                                    {
                                        colcont += 2;
                                    }
                                }
                                contador_salto++;
                                if (contador_salto == 1 || contador_salto == 10)
                                {
                                    row += 3;
                                }
                                else
                                {
                                    if (contador_salto == 12 || contador_salto == 16 || contador_salto == 23)
                                    {
                                        row += 2;
                                    }
                                    else
                                    {
                                        row++;
                                    }
                                }
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }
            /*Fin inserción tablas*/

            /*-----------------------Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/
            //inserción de labels y logo en el Excel
            if (ReportCountry != null)
            {
                //inserción de labels
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);

                if (storedProcedure == "ML1")
                    setDatosAdicionales(excelWorksheet, startRow, startColumn);
            }

            /*-----------------------Fin de la Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        private static string getIdentificarCasos(string data, Dictionary<string, string> aData)
        {
            string cTemp = "";

            var array = data.Split('#');
            foreach (string element in array)
            {
                var array2 = element.Split(':');
                if (cTemp == "")
                    cTemp = array2[0];
                else
                    cTemp += '#' + array2[0];

                if (array2.Length > 1)
                    aData.Add(array2[1], array2[1]);
            }

            return cTemp;
        }

        private static void setDatosAdicionales(ExcelWorksheet excelWorksheet, int startRow, int startColumn)
        {
            excelWorksheet.Cells[startRow - 6, startColumn + 1].Value = DateTime.Today;
            //****
            int nFila = startRow;
            long nID, nPaci = 0;
            Dictionary<long, long> dicID = new Dictionary<long, long>();

            var vID = excelWorksheet.Cells[nFila, startColumn + 3].Value;
            while (vID != null && !(string.Compare(vID.ToString(), "") == 0))
            {
                nID = Convert.ToInt64(excelWorksheet.Cells[nFila, startColumn + 3].Value);
                if (dicID.ContainsKey(nID) == false)
                {
                    ++nPaci;
                    dicID.Add(nID, nID);
                }

                nFila++;
                vID = excelWorksheet.Cells[nFila, startColumn + 3].Value;
            }

            excelWorksheet.Cells[startRow - 5, startColumn + 1].Value = nPaci;
        }

        public ActionResult GetSummaryDetailsExcel(int hospitalId, string hospitalDate, int EpiWeek, int EpiYear)
        {
            var HospitalDate = DateTime.Parse(hospitalDate);
            var user = UserManager.FindById(User.Identity.GetUserId());
            var casesummary = db.CaseSummaries.FirstOrDefault(s => s.HosiptalId == hospitalId && s.StartDateOfWeek == HospitalDate);
            if (casesummary == null)
            {
                casesummary = new CaseSummary()
                {
                    StartDateOfWeek = HospitalDate,
                    HosiptalId = hospitalId,
                    EW = EpiWeek,
                    EpiYear = EpiYear,
                    CaseSummaryDetails = new List<CaseSummaryDetail>()
                };
                db.Entry(casesummary).State = EntityState.Added;
                var AgeGroupbyCountry = db.CatAgeGroup.Where(i => i.id_country == user.Institution.CountryID).ToList();
                foreach (CatAgeGroup agegroup in AgeGroupbyCountry)
                {
                    casesummary.CaseSummaryDetails.Add(
                        new CaseSummaryDetail()
                        {
                            AgeGroup = agegroup.id_conf_country,
                            ETINumFem = 0,
                            ETINumMaso = 0,
                            ETINumST = 0,
                            ETINumEmerST = 0,
                            ETIDenoFem = 0,
                            ETIDenoMaso = 0,
                            ETIDenoST = 0,
                            HospFem = 0,
                            HospMaso = 0,
                            HospST = 0,
                            UCIFem = 0,
                            UCIMaso = 0,
                            UCIST = 0,
                            DefFem = 0,
                            DefMaso = 0,
                            DefST = 0,
                            NeuFem = 0,
                            NeuMaso = 0,
                            NeuST = 0,
                            CCSARIFem = 0,
                            CCSARIMaso = 0,
                            CCSARIST = 0,
                            VentFem = 0,
                            VentMaso = 0,
                            VentST = 0
                        }
                    );
                }
                db.SaveChanges();
            }
            var casesummaryDetails = casesummary.CaseSummaryDetails.ToArray();


            try
            {
                var ms = new MemoryStream();
                var tempy = ConfigurationManager.AppSettings["DenominadoresTemplate"];

                var tempy2 = System.IO.File.Exists(tempy);

                using (var fs = System.IO.File.OpenRead(tempy))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        var excelWorksheet = excelWorkBook.Worksheets[1];
                        var row = 3;
                        var column = 2;
                        foreach (CaseSummaryDetail i in casesummaryDetails)
                        {
                            var col_cont = 0;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.ETINumST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.HospST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.UCIST;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefFem;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefMaso;
                            col_cont++;
                            excelWorksheet.Cells[row, column + col_cont].Value = i.DefST;
                            col_cont++;
                            row++;
                        }
                        //[{"Id":1799,"CaseSummaryId":297,"AgeGroup":1,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":10,"HospMaso":7,"HospST":17,"UCIFem":6,"UCIMaso":4,"UCIST":10,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1800,"CaseSummaryId":297,"AgeGroup":2,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":4,"HospMaso":5,"HospST":9,"UCIFem":0,"UCIMaso":1,"UCIST":1,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1801,"CaseSummaryId":297,"AgeGroup":3,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":20,"HospMaso":15,"HospST":35,"UCIFem":0,"UCIMaso":3,"UCIST":3,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1802,"CaseSummaryId":297,"AgeGroup":4,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":118,"HospMaso":26,"HospST":144,"UCIFem":5,"UCIMaso":1,"UCIST":6,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1803,"CaseSummaryId":297,"AgeGroup":5,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":41,"HospMaso":39,"HospST":80,"UCIFem":5,"UCIMaso":5,"UCIST":10,"DefFem":0,"DefMaso":0,"DefST":0,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0},{"Id":1804,"CaseSummaryId":297,"AgeGroup":6,"ETIDenoFem":0,"ETIDenoMaso":0,"ETIDenoST":0,"ETINumFem":0,"ETINumMaso":0,"ETINumST":0,"ETINumEmerST":0,"HospFem":41,"HospMaso":42,"HospST":83,"UCIFem":6,"UCIMaso":10,"UCIST":16,"DefFem":2,"DefMaso":6,"DefST":8,"NeuFem":0,"NeuMaso":0,"NeuST":0,"CCSARIFem":0,"CCSARIMaso":0,"CCSARIST":0,"VentFem":0,"VentMaso":0,"VentST":0}]



                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/xlsx")
                {
                    FileDownloadName = "Denominadores_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"
                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo" + "\n" + e.Message;
            }

            return null;
        }

        public ActionResult GetDataDictionaryExcel()
        {
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            List<String> tables = new List<String>();
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG=db_name()", con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetValue(0).ToString());
                        }
                    }
                    con.Close();
                }
            }

            try
            {
                var ms = new MemoryStream();
                var tempy = ConfigurationManager.AppSettings["DictionaryTemplate"];

                var tempy2 = System.IO.File.Exists(tempy);


                using (var fs = System.IO.File.OpenRead(tempy))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        foreach (String i in tables)
                        {
                            var row = 2;
                            var column = 1;
                            var excelWorksheet = excelWorkBook.Worksheets.Add(i, excelWorkBook.Worksheets[1]);
                            var col_cont = 0;
                            using (var con = new SqlConnection(consString))
                            {
                                using (var command = new SqlCommand("EXEC sp_columns " + i + ";", con))
                                {
                                    con.Open();
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(3).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(5).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(6).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(7).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(8).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(9).ToString();
                                            col_cont++;
                                            excelWorksheet.Cells[row, column + col_cont].Value = reader.GetValue(17).ToString();
                                            col_cont = 0;
                                            row++;
                                        }
                                    }
                                    con.Close();
                                }
                            }
                        }
                        excelWorkBook.Worksheets.Delete(1);
                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;
                return new FileStreamResult(ms, "application/xlsx")
                {
                    FileDownloadName = "Diccionario_de_datos_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"
                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo" + "\n" + e.Message;
            }

            return null;
        }

        /*public JsonResult GetReportsForCountry(int countryID)
        {
            ////var epiYear = DateTime.Now.Year.ToString();
            ////int intEpiYear = Int32.Parse(epiYear);
            List<Dictionary<string, int>> reportsPerCountry = new List<Dictionary<string, int>>();
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.CountryID == countryID && s.active == true);
            if (reportCountry == null)
            {

            }
            else
            {

                dictionary.Add(reportCountry.description, reportCountry.ID);
            }

                reportsPerCountry.Add(dictionary);

            return Json(reportsPerCountry, JsonRequestBehavior.AllowGet);
        }*/

        private static void AppendDataToExcel_REVELAC(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)         //#### CAFQ
        {
            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["DatosReporte"];
            var row = startRow;
            var column = startColumn;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("REVELAC_i", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            excelWorksheet1.Cells[row + nFila, 1].Value = reader.GetValue(0);       // ID
                            excelWorksheet1.Cells[row + nFila, 2].Value = reader.GetValue(5);       // Sexo
                            excelWorksheet1.Cells[row + nFila, 3].Value = reader.GetValue(8);       // Edad en anios
                            excelWorksheet1.Cells[row + nFila, 4].Value = reader.GetValue(9);       //
                            excelWorksheet1.Cells[row + nFila, 5].Value = reader.GetValue(10);      //

                            //excelWorksheet1.Cells[row + nFila, 6].Value = reader.GetValue(11);      // FachaAdministracionAntiviral
                            var vTemp = reader.GetValue(11);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 6].Value = vTemp;

                            excelWorksheet1.Cells[row + nFila, 7].Value = reader.GetValue(14);      //
                            excelWorksheet1.Cells[row + nFila, 8].Value = reader.GetValue(15);      //

                            //excelWorksheet1.Cells[row + nFila, 9].Value = reader.GetValue(16);      // VacunaInfluenzaFecha
                            vTemp = reader.GetValue(16);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 9].Value = vTemp;      // VacunaInfluenzaFecha

                            //excelWorksheet1.Cells[row + nFila, 10].Value = reader.GetValue(17);     // VacunaInfluenza2daDosis
                            vTemp = reader.GetValue(17);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 10].Value = vTemp;     // VacunaInfluenza2daDosis

                            //excelWorksheet1.Cells[row + nFila, 11].Value = reader.GetValue(18);     // VacunaInfluenza2daDosisFecha
                            vTemp = reader.GetValue(18);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 11].Value = vTemp;     // VacunaInfluenza2daDosisFecha

                            excelWorksheet1.Cells[row + nFila, 12].Value = reader.GetValue(19);     //
                            excelWorksheet1.Cells[row + nFila, 13].Value = reader.GetValue(21);     // FechaTomaMuestra


                            //excelWorksheet1.Cells[row + nFila, 14].Value = reader.GetValue(24);     // ResultadoRT-PCR
                            vTemp = reader.GetValue(24);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 14].Value = vTemp;     // ResultadoRT-PCR

                            var VABT = reader.GetValue(25).ToString();
                            var VABST = reader.GetValue(26).ToString();
                            var VABL = reader.GetValue(27).ToString();
                            if (VABT == "Influenza A" || VABT == "Influenza B")
                            {
                                excelWorksheet1.Cells[row + nFila, 15].Value = VABT;                    // Tipo Virus
                                excelWorksheet1.Cells[row + nFila, 16].Value = VABST;                   // Subtipo virus
                                excelWorksheet1.Cells[row + nFila, 17].Value = VABL;                    // Linaje virus
                            }

                            vTemp = reader.GetValue(28);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 18].Value = vTemp;     // Embarazada

                            vTemp = reader.GetValue(29);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 19].Value = vTemp;     // Semana embarazo

                            vTemp = reader.GetValue(30);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 20].Value = vTemp;     // Trimestre vacunacion

                            vTemp = reader.GetValue(31);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 21].Value = vTemp;     // Fecha ultima menstruac.

                            excelWorksheet1.Cells[row + nFila, 22].Value = "";     // Estado
                            excelWorksheet1.Cells[row + nFila, 23].Value = reader.GetValue(33);     // Region
                            excelWorksheet1.Cells[row + nFila, 24].Value = reader.GetValue(34);     // Hospital
                            excelWorksheet1.Cells[row + nFila, 25].Value = reader.GetValue(35);     // Tipo vigilancia

                            vTemp = reader.GetValue(36);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 26].Value = vTemp;     // Tipo vacuna

                            vTemp = reader.GetValue(37);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 27].Value = vTemp;     // Marca vacuna

                            vTemp = reader.GetValue(38);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 28].Value = vTemp;     // Fecha ingreso

                            vTemp = reader.GetValue(39);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 29].Value = vTemp;     // Fecha de egreso

                            vTemp = reader.GetValue(40);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 30].Value = vTemp;     // UCI

                            vTemp = reader.GetValue(41);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 31].Value = vTemp;     // Condicion de egreso

                            vTemp = reader.GetValue(82);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 32].Value = vTemp;        // Tipo de antiviral

                            vTemp = reader.GetValue(42);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 33].Value = vTemp;     // 1ra vacunacion niños 9 años

                            vTemp = reader.GetValue(43);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 34].Value = vTemp;     // Vacunacion temporada previa

                            vTemp = reader.GetValue(44);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 35].Value = vTemp;     // Vacunacion anterior temporada previa

                            vTemp = reader.GetValue(45);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 36].Value = vTemp;     // Vacunacion neumococos

                            /*excelWorksheet1.Cells[row + nFila, 37].Value = reader.GetValue(46);     // Otros virus
                            excelWorksheet1.Cells[row + nFila, 38].Value = reader.GetValue(47);     // Otros virus tipo*/
                            var OV1 = reader.GetValue(83);     // Otros virus 1
                            var OV2 = reader.GetValue(84);     // Otros virus 2
                            var OV3 = reader.GetValue(85);     // Otros virus 3
                            if (OV1.ToString() != "")
                            {
                                excelWorksheet1.Cells[row + nFila, 37].Value = "Si";                   // Otros virus
                                excelWorksheet1.Cells[row + nFila, 38].Value = OV1.ToString();      // Otros virus tipo
                            }
                            else if (OV2.ToString() != "")
                            {
                                excelWorksheet1.Cells[row + nFila, 37].Value = "Si";                   // Otros virus
                                excelWorksheet1.Cells[row + nFila, 38].Value = OV2.ToString();      // Otros virus tipo
                            }
                            else if (OV3.ToString() != "")
                            {
                                excelWorksheet1.Cells[row + nFila, 37].Value = "Si";                   // Otros virus
                                excelWorksheet1.Cells[row + nFila, 38].Value = OV3.ToString();      // Otros virus tipo
                            }

                            vTemp = reader.GetValue(48);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 39].Value = vTemp;     // Fiebre

                            vTemp = reader.GetValue(49);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 40].Value = vTemp;     // Tos

                            vTemp = reader.GetValue(50);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 41].Value = vTemp;     // Dolor de garganta

                            vTemp = reader.GetValue(51);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 42].Value = vTemp;     // Dificultatd respiratoria

                            if (reader.GetValue(52) != null && reader.GetValue(52) != DBNull.Value)
                                excelWorksheet1.Cells[row + nFila, 43].Value = Convert.ToInt32(reader.GetValue(52));     // Acortamiento de la respiracion

                            vTemp = reader.GetValue(53);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 44].Value = vTemp;     // Diagnostico egreso

                            vTemp = reader.GetValue(54);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 45].Value = vTemp;     // Diagnostico ingreso
                            //excelWorksheet1.Cells[row + nFila, 46].Value = "Calculado Asma + Bronquitis";     // Enfermedad Respiratoria (asma y/o Bronquitis cronica)

                            vTemp = reader.GetValue(55);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 47].Value = vTemp;     // Asma

                            vTemp = reader.GetValue(56);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 48].Value = vTemp;     // Bronquitis cronica o enfisema

                            var Dato1 = excelWorksheet1.Cells[row + nFila, 47].Value.ToString();
                            var Dato2 = excelWorksheet1.Cells[row + nFila, 48].Value.ToString();
                            if (Dato1 == "Si" || Dato2 == "Si")
                            {
                                excelWorksheet1.Cells[row + nFila, 46].Value = "Si";                // Enfermedad Respiratoria (asma y/o Bronquitis cronica)
                            }
                            else if (Dato1 == "No" || Dato2 == "No")
                            {
                                excelWorksheet1.Cells[row + nFila, 46].Value = "No";
                            }
                            else
                            {
                                excelWorksheet1.Cells[row + nFila, 46].Value = "Sin información";
                            }

                            vTemp = reader.GetValue(57);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 49].Value = vTemp;     // Otras enfermedades respiratorias

                            vTemp = reader.GetValue(58);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 50].Value = vTemp;     // Enfermedades cardiacas

                            vTemp = reader.GetValue(59);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 51].Value = vTemp;     // Ateroesclerosis

                            vTemp = reader.GetValue(60);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 52].Value = vTemp;     // Cardiomiopatia

                            vTemp = reader.GetValue(61);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 53].Value = vTemp;     // Desordenes del neurodesarrollo

                            vTemp = reader.GetValue(62);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 54].Value = vTemp;     // Paralisis cerebral

                            vTemp = reader.GetValue(63);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 55].Value = vTemp;     // Distrofias musculares

                            vTemp = reader.GetValue(64);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 56].Value = vTemp;     // Desordenes cogniticos

                            vTemp = reader.GetValue(66);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 58].Value = vTemp;     // Diabetes

                            Dato1 = excelWorksheet1.Cells[row + nFila, 58].Value.ToString();
                            excelWorksheet1.Cells[row + nFila, 57].Value = Dato1;                   // Desordenes metabolicos

                            vTemp = reader.GetValue(67);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 59].Value = vTemp;     // Desordenes del sistema inmune

                            vTemp = reader.GetValue(68);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 60].Value = vTemp;     // VIH/SIDA

                            vTemp = reader.GetValue(69);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 61].Value = vTemp;     // Quimioterapia

                            vTemp = reader.GetValue(70);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 62].Value = vTemp;     // Pacientes trasplantados tomando inmunosupresores

                            vTemp = reader.GetValue(71);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 63].Value = vTemp;     // Uso cronico corticoesteroides

                            vTemp = reader.GetValue(72);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 64].Value = vTemp;     // Insuficiencia renal cronica

                            vTemp = reader.GetValue(73);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 65].Value = vTemp;     // Enfermedad hepatica cronica

                            vTemp = reader.GetValue(74);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 66].Value = vTemp;     // Obesidad morbida

                            vTemp = reader.GetValue(75);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 67].Value = vTemp;     // Enfermedad hematologica

                            vTemp = reader.GetValue(76);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 68].Value = vTemp;     // Anemia falciforme

                            vTemp = reader.GetValue(77);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 69].Value = vTemp;     // Talasemia mayor

                            vTemp = reader.GetValue(78);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 70].Value = vTemp;     // Terapia cronica aspirina

                            vTemp = reader.GetValue(79);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 71].Value = vTemp;     // Tabaquismo

                            vTemp = reader.GetValue(80);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 72].Value = vTemp;     // Sindrome down

                            vTemp = reader.GetValue(81);
                            if (vTemp.ToString() == "") vTemp = "";
                            excelWorksheet1.Cells[row + nFila, 73].Value = vTemp;     // Indigena

                            ++nFila;
                        }
                        if (nFila > 0)
                        {
                            ExcelWorksheet excelWS = excelWorkBook.Worksheets["REVELAC_i"];
                            //**** Creando formulas en hoja REVELAC-i
                            for (int nY = startRow + 1; nY <= nFila + 1; nY++)
                            {
                                for (int nX = 1; nX <= 73; ++nX)
                                {
                                    string aa = excelWS.Cells[startRow, nX].FormulaR1C1;
                                    excelWS.Cells[nY, nX].FormulaR1C1 = aa;
                                }
                            }
                            //**** Recalculando formulas excel
                            excelWS.Calculate();

                            /*for(int col=1; col<= 73; col++)
                            {
                                //excelWS.Cells[startRow + 53, startColumn + 1].Copy(excelWS.Cells[startRow, col]);
                                ExcelRange rRang = excelWS.Cells[ExcelRange.GetAddress(startRow, col, nFila, col)];
                                rRang.Copy(excelWS.Cells[startRow, col]);
                            }*/

                            foreach (var cell in excelWS.Cells.Where(cell => cell.Formula != null))
                                cell.Value = cell.Value;
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        //private static void AppendDataToExcel_IndDes(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo)
        private static void AppendDataToExcel_IndDes(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? AreaId)         //#### CAFQ
        {
            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["DatosPie"];
            var row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            decimal[] nDato1 = new decimal[] { 0 };
            decimal[] nDato2 = new decimal[] { 0 };
            string[,] aDato3 = new string[6, 2];       // 6 Filas, 2 Columnas
            decimal[] nDato4 = new decimal[] { 0 };
            decimal[] nDato5 = new decimal[] { 0 };
            decimal[] nDato6 = new decimal[] { 0 };
            string[,] aDato7 = new string[6, 2];
            decimal[] nDato8 = new decimal[] { 0 };
            decimal[] nDato9 = new decimal[] { 0 };
            decimal[] nDato10 = new decimal[] { 0 };
            decimal[] nDato11 = new decimal[] { 0 };
            decimal[] nDato12 = new decimal[] { 0 };
            decimal[] nDato14 = new decimal[] { 0 };

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 1, nDato1);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 2, nDato2);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 3, nDato1, aDato3);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 4, nDato4);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 5, nDato5);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 6, nDato6);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 7, nDato1, aDato7);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 8, nDato8);

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 9, nDato9);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 10, nDato10);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 11, nDato11);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 12, nDato12);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, Surv, SurvInusual, 14, nDato14);
            //****
            var excelWorksheet2 = excelWorkBook.Worksheets[1];

            //**** Metas
            ArrayList aMetas = new ArrayList();
            ID_recuperarMetas(aMetas, countryId);

            double[] xx = (double[])aMetas[0];                      // Metas
            String[] yy = (String[])aMetas[1];                      // Unidades de la meta

            for (int nI = 0; nI < xx.Length; ++nI)
                excelWorksheet2.Cells[row + nI, column].Value = ID_formatearMeta(xx[nI], yy[nI], countryId, languaje_);

            //**** resultados
            double nTemp = 0;
            row = startRow;

            if (nDato1[0] != 0)
            {
                //nTemp = (double)(nDato4[0] / nDato1[0] * 100);
                nTemp = Math.Round((double)(nDato4[0] / nDato1[0] * 100), 0);
                excelWorksheet2.Cells[row, column + 1].Value = ID_formatearMeta(nTemp, yy[0], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[0], 2, yy[0]);
            }

            if (nDato8[0] != 0)
            {
                //nTemp = (double)(nDato5[0] / nDato8[0] * 100);
                nTemp = Math.Round((double)(nDato5[0] / nDato8[0] * 100), 0);
                excelWorksheet2.Cells[row + 1, column + 1].Value = ID_formatearMeta(nTemp, yy[1], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[1], 3, yy[1]);
            }

            if (nDato1[0] != 0)
            {
                //nTemp = (double)(nDato6[0] / nDato1[0] * 100);
                nTemp = Math.Round((double)(nDato6[0] / nDato1[0] * 100), 0);
                excelWorksheet2.Cells[row + 2, column + 1].Value = ID_formatearMeta(nTemp, yy[2], countryId, languaje_);
                ID_setResultados(excelWorksheet1, nTemp, xx[2], 4, yy[2]);
            }

            nTemp = (double)nDato9[0];
            excelWorksheet2.Cells[row + 3, column + 1].Value = ID_formatearMeta(nTemp, yy[3], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[3], 5, yy[3]);

            nTemp = (double)nDato10[0];
            excelWorksheet2.Cells[row + 4, column + 1].Value = ID_formatearMeta(nTemp, yy[4], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[4], 6, yy[4]);

            nTemp = (double)nDato11[0];
            excelWorksheet2.Cells[row + 5, column + 1].Value = ID_formatearMeta(nTemp, yy[5], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[5], 7, yy[5]);

            //nTemp = (double)nDato12[0] * 24.0;
            nTemp = Math.Round((double)nDato12[0], 0) * 24.0;
            excelWorksheet2.Cells[row + 6, column + 1].Value = ID_formatearMeta(nTemp, yy[6], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[6], 8, yy[6]);

            nTemp = (double)nDato14[0];
            excelWorksheet2.Cells[row + 7, column + 1].Value = ID_formatearMeta(nTemp, yy[7], countryId, languaje_);
            ID_setResultados(excelWorksheet1, nTemp, xx[7], 9, yy[7]);

            //****
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet2);
            InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet2);
        }

        private static void reportLabels(string consString, int countryId, string languaje_, int? ReportCountry, int? hospitalId, int? year, int? YearFrom, int? YearTo, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, ExcelWorksheet excelWorksheet, int? AreaId = 0)
        {
            //inserción de labels
            string labelWeekEpid = "";
            labelWeekEpid = getSemanasEpidemiologicasReporte(year, YearFrom, YearTo, se, startDate, endDate);

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountryConfig where ReportCountryID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = ReportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["row"];
                            int insertcol = (int)reader["col"];
                            int tab = Int32.Parse((string)reader["tab"]);           // Hoja
                            /**************Inicia inserción de labels automáticos*******************/
                            /*Si en la base de datos, el label se encierra dentro de dobles llaves {{parametro}}, el parámetro se cambiará por el parámetro correspondiente de búsqueda ingresado en el formulario*/
                            /*Llenado parámetros que vienen del formulario*/
                            string labelCountry = "";
                            string labelHospital = "";
                            string labelArea = "";
                            string labelYear = "";
                            string labelSE = "";
                            string labelStartDate = "";
                            string labelEndDate = "";
                            string labelCurrDate = "";
                            //string labelWeekEpid = "";

                            //Obtención del pais y llenado en la variable
                            if (countryId > 0)
                                labelCountry = getDescripcionDatoDesdeID(con, "select * from Country where ID = @ID", "Name", countryId);

                            if (hospitalId > 0)
                                labelHospital = getDescripcionDatoDesdeID(con, "select * from Institution where ID = @ID", "FullName", hospitalId);

                            if (AreaId > 0)
                                labelArea = getDescripcionDatoDesdeID(con, "select * from Area where ID = @ID", "Name", AreaId);

                            if (year > 0)
                            {
                                labelYear += year;
                            }
                            if (YearFrom > 0)
                            {
                                //labelYear += "Desde " + YearFrom + " ";
                                labelYear += SgetMsg("msgViewExportarLabelFromDesde", countryId, languaje_) + YearFrom + " ";
                            }
                            if (YearTo > 0)
                            {
                                //labelYear += "Hasta " + YearTo + " ";
                                labelYear += SgetMsg("msgViewExportarLabelToHasta", countryId, languaje_) + YearTo + " ";
                            }
                            if (se > 0)
                            {
                                labelSE += se;
                            }
                            if (startDate.HasValue)
                            {
                                labelStartDate += startDate.ToString();
                                DateTime oDate = Convert.ToDateTime(labelStartDate);
                                labelStartDate = oDate.Date.ToString();
                            }
                            if (endDate.HasValue)
                            {
                                labelEndDate += endDate.ToString();
                                DateTime oDate = Convert.ToDateTime(labelEndDate);
                                labelEndDate = oDate.Date.ToString();
                            }

                            //labelWeekEpid = getSemanasEpidemiologicasReporte(year, YearFrom, YearTo, se, startDate, endDate);
                            /*Fin llenado parámetros*/

                            string label = reader["label"].ToString();
                            if (label != "")
                            {
                                //if (label.StartsWith("{{") && label.EndsWith("}}"))
                                string cOriginal = label;               //#### CAFQ: 
                                int nResu1 = label.IndexOf("{{");       //#### CAFQ: 180415
                                int nResu2 = label.IndexOf("}}");       //#### CAFQ: 180415
                                if (nResu1 >= 0 && nResu2 >= 0)         //#### CAFQ: 180415
                                {
                                    label = cOriginal.Substring(nResu1, nResu2 - nResu1 + 2);       //#### CAFQ: 180415
                                    //string cMensa = getMsg("msgViewExportarLabelYear");
                                    //string cMensa = SgetMsg("msgViewExportarLabelYear", countryId, languaje_);
                                    //SgetMsg(string msgView, int? countryDisp, string langDisp)

                                    switch (label)
                                    {
                                        case "{{country}}":
                                            label = (labelCountry != "" ? (SgetMsg("msgViewExportarLabelCountry", countryId, languaje_) + ": " + labelCountry) : "");
                                            break;
                                        case "{{institution}}":
                                            label = (labelHospital != "" ? (SgetMsg("msgViewExportarLabelInstitutionShort", countryId, languaje_) + ": " + labelHospital) : "");
                                            break;
                                        case "{{Area}}":
                                            label = (labelArea != "" ? (SgetMsg("msgViewExportarLabelArea", countryId, languaje_) + ": " + labelArea) : "");
                                            break;
                                        case "{{year}}":
                                            //label = (labelYear != "" ? ("Año: " + labelYear) : "");
                                            //label = (labelYear != "" ? (SgetMsg("msgViewExportarLabelEpidemiologicalYear", countryId, languaje_) + ": " + labelYear) : "");
                                            label = (labelYear != "" ? (SgetMsg("msgViewExportarLabelYear", countryId, languaje_) + ": " + labelYear) : "");
                                            break;
                                        case "{{onlyYear}}":
                                            label = (labelYear != "" ? labelYear : "");
                                            break;
                                        case "{{se}}":
                                            //label = (labelSE != "" ? ("SE:" + labelSE) : "");
                                            label = (labelSE != "" ? (SgetMsg("msgViewExportarLabelEW", countryId, languaje_) + ": " + labelSE) : "");
                                            break;
                                        case "{{startDate}}":
                                            //label = (labelStartDate != "" ? ("Del:" + labelStartDate) : "");
                                            label = (labelStartDate != "" ? (SgetMsg("msgViewExportarLabelFrom", countryId, languaje_) + ": " + labelStartDate) : "");
                                            break;
                                        case "{{endDate}}":
                                            //label = (labelEndDate != "" ? ("Al:" + labelEndDate) : "");
                                            label = (labelEndDate != "" ? (SgetMsg("msgViewExportarLabelTo", countryId, languaje_) + ": " + labelEndDate) : "");
                                            break;
                                        case "{{fullinstitution}}":
                                            //label = (labelCountry != "" ? ("Pais:" + labelCountry) : "") + " " + (labelHospital != "" ? ("Inst:" + labelHospital) : "");
                                            label = (labelCountry != "" ? (SgetMsg("msgViewExportarLabelCountry", countryId, languaje_) + ": " + labelCountry) : "") + " " + (labelHospital != "" ? (SgetMsg("msgViewExportarLabelInstitutionShort", countryId, languaje_) + ": " + labelHospital) : "");
                                            break;
                                        case "{{fulldate}}":
                                            //label = (labelYear != "" ? ("Año:" + labelYear) : "") + " " + (labelSE != "" ? ("SE:" + labelSE) : "") + " " + (labelStartDate != "" ? ("Del:" + labelStartDate) : "") + " " + (labelEndDate != "" ? ("Al:" + labelEndDate) : "");
                                            label = (labelYear != "" ? (SgetMsg("msgViewExportarLabelEpidemiologicalYear", countryId, languaje_) + ": " + labelYear) : "") + " " + (labelSE != "" ? (SgetMsg("msgViewExportarLabelEW", countryId, languaje_) + ": " + labelSE) : "") + " " + (labelStartDate != "" ? (SgetMsg("msgViewExportarLabelFrom", countryId, languaje_) + ": " + labelStartDate) : "") + " " + (labelEndDate != "" ? (SgetMsg("msgViewExportarLabelTo", countryId, languaje_) + ": " + labelEndDate) : "");
                                            break;
                                        case "{{currentDate}}":
                                            DateTime oDate = DateTime.Today;
                                            labelCurrDate = String.Format("{0:" + SgetMsg("msgDateFormatReporting", countryId, languaje_) + "}", oDate);
                                            label = labelCurrDate;
                                            break;
                                        case "{{weekEpid}}":
                                            //label = (labelYear != "" ? ("Año:" + labelYear) : "") + " " + (labelSE != "" ? ("SE:" + labelSE) : "") + " " + (labelStartDate != "" ? ("Del:" + labelStartDate) : "") + " " + (labelEndDate != "" ? ("Al:" + labelEndDate) : "");
                                            //label = (labelWeekEpid != "" ? (SgetMsg("msgViewExportarLabelEpidemiologicalYear", countryId, languaje_) + ": " + labelWeekEpid) : "") + " " + (labelSE != "" ? (SgetMsg("msgViewExportarLabelEW", countryId, languaje_) + ": " + labelSE) : "") + " " + (labelStartDate != "" ? (SgetMsg("msgViewExportarLabelFrom", countryId, languaje_) + ": " + labelStartDate) : "") + " " + (labelEndDate != "" ? (SgetMsg("msgViewExportarLabelTo", countryId, languaje_) + ": " + labelEndDate) : "");
                                            label = (labelWeekEpid != "" ? (SgetMsg("msgViewExportarLabelWeekEpidReporting", countryId, languaje_) + ": " + labelWeekEpid) : "");
                                            break;
                                        default:
                                            label = "";
                                            break;
                                    }
                                }
                                else
                                    label = "";

                                var excelWs = excelWorksheet;
                                if (tab != 1)
                                {
                                    excelWs = excelWorkBook.Worksheets[tab];
                                }/*else
                                    {
                                        var excelWs = excelWorksheet;
                                    }*/
                                 //excelWorksheet.Cells[insertrow, insertcol].Value = label;
                                if (label != "")
                                    //excelWorksheet.Cells[insertrow, insertcol].Value = cOriginal;
                                    //excelWs.Cells[insertrow, insertcol].Value = cOriginal;
                                    //else
                                    //excelWorksheet.Cells[insertrow, insertcol].Value = cOriginal.Substring(0, nResu1) + label + cOriginal.Substring(nResu2 + 2, cOriginal.Length - nResu2 - 2);
                                    excelWs.Cells[insertrow, insertcol].Value = cOriginal.Substring(0, nResu1) + label + cOriginal.Substring(nResu2 + 2, cOriginal.Length - nResu2 - 2);
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static string getDescripcionDatoDesdeID(SqlConnection con, string query, string columnName, int? nID)
        {
            string dato = "";

            //using (var command2 = new SqlCommand("select * from Institution where ID = @InstitutionID", con))
            using (var command2 = new SqlCommand(query, con))
            {
                command2.Parameters.Clear();
                command2.Parameters.Add("@ID", SqlDbType.Int).Value = nID;
                using (var reader2 = command2.ExecuteReader())
                {
                    while (reader2.Read())
                    {
                        dato += reader2[columnName];
                    }
                }
                command2.Parameters.Clear();
            }

            return dato;
        }

        private static string getSemanasEpidemiologicasReporte(int? year, int? yearFrom, int? yearTo, int? se, DateTime? startDate, DateTime? endDate)
        {
            DateTime dtCurr = DateTime.Today;
            string labelSemanas = "";
            //****
            if (startDate.HasValue && endDate.HasValue)
            {

            }
            else if (year > 0 && se > 0)
            {
                if (year == dtCurr.Year)
                    labelSemanas = se.ToString();
                else
                    labelSemanas = year.ToString() + " - " + se.ToString();
            }
            else if (yearFrom > 0 && yearTo > 0)
            {
                int nUlSe = 0;

                if (yearTo == dtCurr.Year)
                    nUlSe = PAHOClassUtilities.semanasActualEpidemiologico();
                else
                    nUlSe = PAHOClassUtilities.semanasAnioEpidemiologico((int)yearTo);

                labelSemanas = yearFrom.ToString() + "." + "1" + " - " + yearTo.ToString() + "." + nUlSe.ToString();
            }
            else if (year > 0)
            {
                if (year == dtCurr.Year)
                {
                    int nSemanas = PAHOClassUtilities.semanasActualEpidemiologico();
                    labelSemanas = "1" + " - " + nSemanas.ToString();
                }
                else if (year < dtCurr.Year)
                {
                    int nSemanas = PAHOClassUtilities.semanasAnioEpidemiologico((int)year);
                    labelSemanas = year.ToString() + " " + "1" + " - " + nSemanas.ToString();
                }
            }

            return labelSemanas;
        }

        //private static void recuperarDatosIndDes(string consString, string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo, int IRAG, int opcion, decimal[] nResuOut, string[,] aResuOut = null)
        private static void recuperarDatosIndDes(string consString, string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int opcion, decimal[] nResuOut, string[,] aResuOut = null)         //#### CAFQ
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("IndicDesemp", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;      //#### CAFQ
                    command.Parameters.Add("@opcion", SqlDbType.Int).Value = opcion;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            if (opcion == 3 || opcion == 7)
                            {
                                aResuOut[nFila, 0] = reader.GetValue(0).ToString();
                                aResuOut[nFila, 1] = reader.GetValue(1).ToString();
                                ++nFila;
                            }
                            else
                            {
                                /*if (reader.GetValue(0).ToString() == "")
                                {
                                    nResuOut[0] = Convert.ToDecimal(0);
                                }
                                else
                                {
                                    nResuOut[0] = Convert.ToDecimal(reader.GetValue(0));
                                }*/
                                nResuOut[0] = (reader.GetValue(0).ToString() == "") ? nResuOut[0] = Convert.ToDecimal(0) : Convert.ToDecimal(reader.GetValue(0));
                            }
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static void InsertarImagenLogo(string consString, string reportTemplate, int? reportCountry, ExcelWorksheet excelWorksheet)
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("select * from ReportCountry where ID = @ReportCountryID", con))
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@ReportCountryID", SqlDbType.Int).Value = reportCountry;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int insertrow = (int)reader["logoRow"];
                            int insertcol = (int)reader["logoCol"];
                            String imagurl = (String)reader["logo"];
                            if (imagurl != "")
                            {
                                try
                                {
                                    /*
                                    //imagurl = "file:///C:/CAFQSource/PAHO/PAHOFLUCR_CLON/Paho/Content/themes/base/images/logo_pahowho_blue-en.png";     //#### SOLO DESARROLLO: CAFQ
                                    */
                                    WebClient wc = new WebClient();
                                    byte[] bytes = wc.DownloadData(imagurl);
                                    MemoryStream ms = new MemoryStream(bytes);
                                    Bitmap newImage = new Bitmap(System.Drawing.Image.FromStream(ms));
                                    excelWorksheet.Row(insertrow).Height = newImage.Height;
                                    var picture = excelWorksheet.Drawings.AddPicture("reportlogo", newImage);
                                    if (reportTemplate == "I1")
                                        picture.SetPosition(insertrow, -(newImage.Height), insertcol, 2);
                                    else
                                        picture.SetPosition(insertrow, -(newImage.Height), insertcol, -100);
                                    //picture.SetPosition(insertrow, 0, insertcol, 0);
                                }
                                catch (Exception e)
                                {
                                    //ViewBag.Message = "No se insertó el logo, porque no es accesible";
                                }
                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static void ID_recuperarMetas(ArrayList _aMetas, int _countryId)
        {
            double[] arr1 = new double[0];
            string[] arr2 = new string[0];

            string cMetas = ConfigurationManager.AppSettings["IndicadoresDesempenioMetas_" + _countryId.ToString()];
            if (cMetas == null)
                cMetas = "0:0:0:0:0:0:0:0";                         //"90%:100%:90%:2D:3D:1D:48H:7D"

            string[] aMetas = cMetas.Split(':');
            for (int nI = 0; nI < aMetas.Length; ++nI)
            {
                string cMeta = aMetas[nI].Substring(0, aMetas[nI].Length - 1);
                double nMeta = double.Parse(cMeta);
                string cUnid = aMetas[nI].Substring(aMetas[nI].Length - 1, 1);

                Array.Resize(ref arr1, nI + 1);
                Array.Resize(ref arr2, nI + 1);
                arr1[nI] = nMeta;
                arr2[nI] = cUnid;
            }

            _aMetas.Add(arr1);
            _aMetas.Add(arr2);
        }

        private static string ID_formatearMeta(double _Meta, string _Unid, int countryId, string languaje)
        {
            string cForma = "", cMeta = "";

            if (_Unid == "%")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = cMeta + _Unid;
            }

            if (_Unid == "D")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = (_Meta == 1) ? cMeta + " " + SgetMsg("msgViewExportarLabelDay", countryId, languaje) : cMeta + " " + SgetMsg("msgViewExportarLabelDays", countryId, languaje);
            }

            if (_Unid == "H")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                cForma = (_Meta == 1) ? cMeta + " " + SgetMsg("msgViewExportarLabelHour", countryId, languaje) : cMeta + " " + SgetMsg("msgViewExportarLabelHours", countryId, languaje);
            }

            return cForma;
        }

        private static void ID_setResultados(ExcelWorksheet excelWorksheet, double nResu, double nMeta, int nRow, string tipo)
        {
            if (tipo == "%")
            {
                if (nResu < nMeta)
                {
                    excelWorksheet.Cells[nRow, 1].Value = 1;                // Rojo
                    excelWorksheet.Cells[nRow, 2].Value = 0;
                }
                else
                {
                    excelWorksheet.Cells[nRow, 1].Value = 0;
                    excelWorksheet.Cells[nRow, 2].Value = 1;                // Verde
                }
            }
            else        // "D", "H"
            {
                if (nResu <= nMeta)
                {
                    excelWorksheet.Cells[nRow, 1].Value = 0;                // Verde
                    excelWorksheet.Cells[nRow, 2].Value = 1;
                }
                else
                {
                    excelWorksheet.Cells[nRow, 1].Value = 1;
                    excelWorksheet.Cells[nRow, 2].Value = 0;                // Rojo
                }
            }
        }

        public JsonResult ListaCasosPorEnviarNPHL(string Report, int CountryID, int? HospitalID, int? Year, int? Month, int? SE,
            DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? RegionID, int? YearFrom, int? YearTo, int? Surv, bool? Inusual)
        {
            List<string[]> casoDatosList = new List<string[]>();

            var user = UserManager.FindById(User.Identity.GetUserId());
            var countryId = CountryID;
            var languaje_ = user.Institution.Country.Language;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("MuestrasLabNPHL", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = Year;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = Month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = SE;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = StartDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = EndDate;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = RegionID;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = Inusual;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] casoDatos = new string[4];

                            casoDatos[0] = reader.GetValue(3).ToString();       // ID caso
                            casoDatos[1] = reader.GetValue(0).ToString();       // ID NPHL
                            casoDatos[2] = reader.GetValue(2).ToString();       // Nombre
                            casoDatos[3] = reader.GetValue(1).ToString();       // Apellido

                            casoDatosList.Add(casoDatos);
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }

            var jsonSerialiser = new JavaScriptSerializer();
            var jsonDatosLabNPHL = jsonSerialiser.Serialize(casoDatosList);

            return Json(jsonDatosLabNPHL);
        }

        private static void AppendDataToExcel_FLUID(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = 1;
                            //&& insert_row == true
                            if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var cell = excelWorksheet.Cells[startRow, col];
                                if (reader.GetValue(i) != null)
                                {
                                    int number;
                                    bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);

                                    if (isNumber)
                                    {
                                        excelWorksheet.Cells[row, col].Value = number;
                                    }
                                    else
                                    {
                                        excelWorksheet.Cells[row, col].Value = reader.GetValue(i).ToString();
                                    }
                                    excelWorksheet.Cells[row, col].StyleID = cell.StyleID;

                                }
                                col++;
                            }

                            row++;
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }
        private void ConfigToExcel_FLUID(int countryId, string languaje_country, int? regionId, int? year, int? hospitalId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int sheet, bool? insert_row)
        {
            var excelWorksheet = excelWorkBook.Worksheets["Leyendas"];
            //var excelWorksheet = excelWorkBook.Worksheets[sheet];

            if (year != null)
            {
                excelWorksheet.Cells[2, 1].Value = year.ToString();
            }

            if (hospitalId != null && hospitalId > 0)
            {
                var Institution = db.Institutions.Find(hospitalId);

                excelWorksheet.Cells[2, 5].Value = Institution.Name.ToString();

            }
            else if (regionId != null && regionId > 0)
            {
                var Region_report = db.Regions.Find(regionId);

                excelWorksheet.Cells[2, 4].Value = Region_report.Name.ToString();
            }
            else
            {
                var Pais = db.Countries.Find(countryId);

                excelWorksheet.Cells[2, 3].Value = Pais.Name.ToString();
            }

        }

        private void ConfigGraph_FLUID(int? year, ExcelWorkbook excelWorkBook, int sheet)
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];

            var LineChart = excelWorksheet.Drawings["GS1"] as ExcelLineChart;
            var series = LineChart.Series[0];

            series.Header = year.ToString();

            //var seriesCC = LineChart.Series.Add(ExcelRange.GetAddress(7, nCol, 59, nCol), ExcelRange.GetAddress(7, 2, 59, 2));

        }
        
        private void AppendDataToExcel_ConsolidadoCarga(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual, int? Sentinel)
        {

            //int count = nombres.Count(n => n.EndsWith("o"));
            int numAgeGroup = db.CatAgeGroup.Count(n => n.id_country == countryId);

            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["Hosp."];
            ExcelWorksheet excelWorksheet2 = excelWorkBook.Worksheets["UCI"];
            ExcelWorksheet excelWorksheet3 = excelWorkBook.Worksheets["Fallecidos"];
            var row = startRow;
            var column = startColumn;
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("R8_ConsolidadoCarga", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 600 })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = regionId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = languaje_;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    command.Parameters.Add("@yearFrom", SqlDbType.Int).Value = YearFrom;
                    command.Parameters.Add("@yearTo", SqlDbType.Int).Value = YearTo;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;
                    command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;
                    command.Parameters.Add("@Sentinel", SqlDbType.Int).Value = Sentinel;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        int nFila = 0;
                        while (reader.Read())
                        {
                            // Hospitalizaciones
                            excelWorksheet1.Cells[row + nFila, column + 0].Value = reader.GetValue(2);
                            excelWorksheet1.Cells[row + nFila, column + 1].Value = reader.GetValue(2 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 3].Value = reader.GetValue(3);
                            excelWorksheet1.Cells[row + nFila, column + 4].Value = reader.GetValue(3 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 6].Value = reader.GetValue(4);
                            excelWorksheet1.Cells[row + nFila, column + 7].Value = reader.GetValue(4 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 9].Value = reader.GetValue(5);
                            excelWorksheet1.Cells[row + nFila, column + 10].Value = reader.GetValue(5 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 12].Value = reader.GetValue(6);
                            excelWorksheet1.Cells[row + nFila, column + 13].Value = reader.GetValue(6 + numAgeGroup);

                            excelWorksheet1.Cells[row + nFila, column + 15].Value = reader.GetValue(7);
                            excelWorksheet1.Cells[row + nFila, column + 16].Value = reader.GetValue(7 + numAgeGroup);
                            // UCI
                            excelWorksheet2.Cells[row + nFila, column + 0].Value = reader.GetValue(14);
                            excelWorksheet2.Cells[row + nFila, column + 1].Value = reader.GetValue(14 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 3].Value = reader.GetValue(15);
                            excelWorksheet2.Cells[row + nFila, column + 4].Value = reader.GetValue(15 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 6].Value = reader.GetValue(16);
                            excelWorksheet2.Cells[row + nFila, column + 7].Value = reader.GetValue(16 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 9].Value = reader.GetValue(17);
                            excelWorksheet2.Cells[row + nFila, column + 10].Value = reader.GetValue(17 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 12].Value = reader.GetValue(18);
                            excelWorksheet2.Cells[row + nFila, column + 13].Value = reader.GetValue(18 + numAgeGroup);

                            excelWorksheet2.Cells[row + nFila, column + 15].Value = reader.GetValue(19);
                            excelWorksheet2.Cells[row + nFila, column + 16].Value = reader.GetValue(19 + numAgeGroup);
                            // Fallecidos
                            excelWorksheet3.Cells[row + nFila, column + 0].Value = reader.GetValue(26);
                            excelWorksheet3.Cells[row + nFila, column + 1].Value = reader.GetValue(26 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 3].Value = reader.GetValue(27);
                            excelWorksheet3.Cells[row + nFila, column + 4].Value = reader.GetValue(27 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 6].Value = reader.GetValue(28);
                            excelWorksheet3.Cells[row + nFila, column + 7].Value = reader.GetValue(28 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 9].Value = reader.GetValue(29);
                            excelWorksheet3.Cells[row + nFila, column + 10].Value = reader.GetValue(29 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 12].Value = reader.GetValue(30);
                            excelWorksheet3.Cells[row + nFila, column + 13].Value = reader.GetValue(30 + numAgeGroup);

                            excelWorksheet3.Cells[row + nFila, column + 15].Value = reader.GetValue(31);
                            excelWorksheet3.Cells[row + nFila, column + 16].Value = reader.GetValue(31 + numAgeGroup);
                            //####
                            ++nFila;
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
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

        private static string SgetMsg(string msgView, int? countryDisp, string langDisp)
        {
            string searchedMsg = ResourcesM.SgetMessage(msgView, countryDisp, langDisp);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }
    }
}

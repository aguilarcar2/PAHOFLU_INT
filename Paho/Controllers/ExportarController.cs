using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
//using System.Data.Entity;
using System.Data.SqlClient;
//using System.Globalization;
using System.Linq;
//using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.IO;
//using System.Web.UI;
//using System.Xml.Serialization;
//using System.Text;
using OfficeOpenXml;
using Paho.Reports.Entities;
using System.Drawing;
using System.Net;
using System.Data.Entity;
using System.Globalization;
using System.Collections;

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
            IQueryable<ReportCountry> ReportsCountries = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            ExportarViewModel.CountryID = user.Institution.CountryID ?? 0;
            ReportsCountries = db.ReportsCountries.Where(i => i.CountryID == user.Institution.CountryID && i.active == true);
            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                ExportarViewModel.DisplayCountries = true;
                ExportarViewModel.DisplayRegionals = true;
                ExportarViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                ExportarViewModel.DisplayHospitals = true;

                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                  .Where(i => i.CountryID == user.Institution.CountryID);

                    if (user.type_region == null)
                    {     // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == 1).OrderBy(i => i.Name);
                    }
                    else
                    {
                        // Regiones
                        regions = db.Regions.Where(c => c.CountryID == user.Institution.CountryID && c.tipo_region == user.type_region).OrderBy(i => i.Name);
                    }
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
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
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.CountryID == user.Institution.CountryID);
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    institutions = db.Institutions.OfType<Lab>()
                                   .Where(i => i.ID == user.Institution.ID);
                }
            }

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

            if (regions != null)
            {
                var regionsDisplay = regions.Select(i => new LookupView<Paho.Models.Region>()
                {
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                }).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    var all = new LookupView<Paho.Models.Region> { Id = "0", Name = "-- Todo(a)s --" };
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
                    var all = new LookupView<Paho.Models.Institution> { Id = "0", Name = "-- Todo(a)s --" };
                    institutionsDisplay.Insert(0, all);
                }

                ExportarViewModel.Institutions = institutionsDisplay;
            };

            return View(ExportarViewModel);
        }

        // POST: GetCasesInExcel
        public CsvActionResult<FluCaseExportViewModel> GetExcelCases(int CountryID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            IQueryable<FluCase> query = GetQuery( CountryID, HospitalID, Year, Month, SE, StartDate, EndDate);
            List<FluCaseExportViewModel> result = query.Select( f => new FluCaseExportViewModel(){
        ID              = f.ID              ,
        HospitalID      = f.Hospital.FullName,
        HospitalDate    = f.HospitalDate    ,
        RegDate         = f.RegDate         ,
        FName1          = f.FName1          ,
        FName2          = f.FName2          ,
        LName1          = f.LName1          ,
        LName2          = f.LName2          ,
        NoExpediente    = f.NoExpediente    ,
        NationalId      = f.NationalId      ,
        DOB             = f.DOB             ,
        Age             = f.Age             ,
        AMeasure        = f.AMeasure.ToString()        ,
        AgeGroup        = f.AgeGroup.ToString()        ,
        Gender          = f.Gender.ToString()          ,
        Country         = f.Country.Code         ,
        Area            = f.Area.Name            ,
        State           = f.State.Name           ,
        Local           = f.Local.Code.ToString()           ,
        Neighborhood    = f.Neighborhood.Code.ToString()    ,
        UrbanRural      = f.UrbanRural.ToString()      ,
        Address         = f.Address         ,
        Vaccin          = f.Vaccin.ToString()          ,
        RiskFactors     = f.RiskFactors.ToString()     ,
        HDisease        = f.HDisease        ,
        Diabetes        = f.Diabetes        ,
        Neuro           = f.Neuro           ,
        Asthma          = f.Asthma          ,
        Pulmonary       = f.Pulmonary       ,
        Liver           = f.Liver           ,
        Renal           = f.Renal           ,
        Immunsupp       = f.Immunsupp       ,
        Pregnant        = f.Pregnant.ToString()        ,
        Pperium         = f.Pperium         ,
        Trimester       = f.Trimester.ToString()       ,
        Smoking         = f.Smoking         ,
        Alcohol         = f.Alcohol         ,
        DownSyn         = f.DownSyn         ,
        Obesity         = f.Obesity.ToString()         ,
        OtherRisk       = f.OtherRisk       ,
        CHNum           = f.CHNum           ,
        FeverDate       = f.FeverDate       ,
        FeverEW         = f.FeverEW         ,
        AStartDate      = f.AStartDate      ,
        HospAmDate      = f.HospAmDate      ,
        HospEW          = f.HospEW          ,
        HospExDate      = f.HospExDate      ,
        ICUAmDate       = f.ICUAmDate       ,
        ICUEW           = f.ICUEW           ,
        ICUExDate       = f.ICUExDate       ,
        Destin          = f.Destin          ,
        IsSample        = f.IsSample        ,
        SampleDate      = f.SampleDate      ,
        SampleType      = f.SampleType      ,
        ShipDate        = f.ShipDate        ,
        Lab             = f.Lab.Name        ,
        Processed       = f.Processed       ,
        NoProRen        = f.NoProRen        ,
        EndLabDate      = f.EndLabDate      ,
        FResult         = f.FResult         ,
        Comments        = f.Comments
            }).ToList();
             //We return the XML from the memory as a .xls file
            //grid.DataSource = result;
            //grid.DataBind();
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //grid.RenderControl(htw);
            //return File(Encoding.GetBytes(sw.ToString), "application/ms-excel", "Marklist.xls");
            return new CsvActionResult<FluCaseExportViewModel> (result, "FluCases.csv");
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

        //private int GetWeek(DateTime date)
        //{
        //    DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
        //    Calendar cal = dfi.Calendar;
        //    return cal.GetWeekOfYear(date, dfi.CalendarWeekRule,
        //                                   dfi.FirstDayOfWeek);
        //}

        public CsvActionResult<Example> GetExcelExample(int CountryID, int HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate) {
            var query = new Example();
            var result = db.Database.SqlQuery<Example>(query.Query()).ToList();
            return new CsvActionResult<Example>(result, "example.csv");
        }

        [HttpGet]
        public ActionResult GetExcel(string Report, int CountryID, int? RegionID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? YearFrom, int? YearTo)
        {
            try
            {
                var ms = new MemoryStream();
                var user = UserManager.FindById(User.Identity.GetUserId());
                int CountryID_ = (CountryID >= 0) ? CountryID : (user.Institution.CountryID ?? 0);
                //int? HospitalID_ = (user.Institution.Father_ID > 0 || user.Institution.Father_ID == null) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                //int? HospitalID_ = (HospitalID >= 0) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                int? HospitalID_ = (HospitalID > 0) ? HospitalID : 0;
                int? RegionID_ = (RegionID >= 0) ? RegionID : (user.Institution.cod_region_institucional ?? 0);
                string Languaje_ = user.Institution.Country.Language ?? "SPA";

                var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.ID == ReportCountry);
                int reportID = reportCountry.ReportID;//contiene la FK para obtener el ID del reporte en la tabla Report
                int reportStartCol = reportCountry.startCol;//contiene la startCol de la tabla ReportCountry, pasa saber en que columna se inicia
                int reportStartRow = reportCountry.startRow;//contiene la startRow de la tabla ReportCountry, pasa saber en que fila se inicia
                var report = db.Reports.FirstOrDefault(s => s.ID == reportID);
                string reportTemplate = report.Template;//contiene el nombre del template, que luego se relacionará al archivo template de Excel

                if (user.Institution.AccessLevel == AccessLevel.SelfOnly) { HospitalID_ = Convert.ToInt32(user.Institution.ID); }


                if (ReportCountry < 1)
                {
                    ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
                    return null;
                    ;
                }

                string templateToUse;

                switch (reportTemplate)
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
                        //int startColumn = 1;  Ambos comentados, para eso está reportStartCol y reportStartRow
                        //int startRow = 2;
                        bool insertRow = true;

                        //if (Report != "Cases")
                        //{
                        //    startColumn = 2;
                        //    startRow = 8;
                        //}

                        if (reportTemplate == "R1" || reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R4" || reportTemplate == "D1" || reportTemplate == "B1")
                        {
                            insertRow = false;
                        }

                        if (reportTemplate == "I1")      //#### CAFQ
                            AppendDataToExcel_IndDes(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo);
                        else
                            AppendDataToExcel(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo);

                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;

                //return new FileStreamResult(ms, "application/xlsx")
                //ExcelPackage.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);

                string nombFile = "Exportable_";            //#### CAFQ
                if (reportTemplate == "I1")
                    nombFile = "IndicDesempenio_";

                return new FileStreamResult(ms, "application/xlsx")
                {
                    //FileDownloadName = "Exportable_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"
                    FileDownloadName = nombFile + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".xlsx"           //#### CAFQ
                };
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
            }

            return null;
        }

        private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo)
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;

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

                    con.Open();
					if ((storedProcedure == "R5"))
                    {
                        IDictionary<int, string> formulas1 = new Dictionary<int, string>();
                        formulas1[1] = "=VRS!B{{toreplace}}";
                        formulas1[2] = "=VRS!C{{toreplace}} + Ad!C{{toreplace}} + Parainfluenza!C{{toreplace}} + 'Inf A'!C{{toreplace}} + 'Inf B'!C{{toreplace}} + Metapnemovirus!C{{toreplace}}";
                        formulas1[3] = "=VRS!C{{toreplace}}";
                        formulas1[4] = "=Ad!C{{toreplace}}";
                        formulas1[5] = "=Parainfluenza!C{{toreplace}}";
                        formulas1[6] = "='Inf A'!C{{toreplace}}";
                        formulas1[7] = "='Inf B'!C{{toreplace}}";
                        formulas1[8] = "=Metapnemovirus!C{{toreplace}}";
                        formulas1[9] = "=VRS!J{{toreplace}}";
                        formulas1[10] = "=VRS!K{{toreplace}}+Ad!K{{toreplace}}+Parainfluenza!K{{toreplace}}+'Inf A'!K{{toreplace}}+'Inf B'!K{{toreplace}}+Metapnemovirus!K{{toreplace}}";
                        formulas1[11] = "=VRS!K{{toreplace}}";
                        formulas1[12] = "=Ad!K{{toreplace}}";
                        formulas1[13] = "=Parainfluenza!K{{toreplace}}";
                        formulas1[14] = "='Inf A'!K{{toreplace}}";
                        formulas1[15] = "='Inf B'!K{{toreplace}}";
                        formulas1[16] = "=Metapnemovirus!K{{toreplace}}";
                        formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}";
                        formulas1[18] = "=L{{toreplace}}+M{{toreplace}}+N{{toreplace}}+O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}";
                        for (int i = 1; i <= 7; i++)
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
                            var excelWorksheet2 = excelWorkBook.Worksheets[i];



                            using (var reader = command.ExecuteReader())
                            {
                                row = 3;
                                column = 1;
                                var tipo_anterior = 1;
                                while (reader.Read())
                                {
                                    if (Convert.ToInt32(reader.GetValue(15)) != tipo_anterior)
                                    {
                                        row++;
                                    }
                                    tipo_anterior = Convert.ToInt32(reader.GetValue(15));
                                    var col = 1;
                                    var readercont = 0;
                                    int stylerow;
                                    if (tipo_anterior == 1)
                                    {
                                        stylerow = 2;
                                    }
                                    else
                                    {
                                        stylerow = row+1;
                                    }
                                    excelWorksheet2.InsertRow(row, 1);
                                    //excelWorksheet.InsertRow(row, 1);
                                    for (int j = 0; j < 17; j++)
                                    {

                                        var cell = excelWorksheet2.Cells[stylerow, col + j];
                                        if (tipo_anterior == 2 && j > 8)
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
                                                    if (j == 10)
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
                                                if (formulas1.ContainsKey(j))
                                                {
                                                    //                                                excelWorksheet2.Cells[row, col + j].Value = 8555;
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
					else{
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
										//excelWorksheet.InsertRow(row, 1);

										//excelWorksheet.Cells[row, 1].Value = reader.GetValue(3).ToString();
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
									var con2 = new SqlConnection(consString);
									con2.Open();

									using (var reader2 = command2.ExecuteReader())
									{
										row = 212;
										column = 6;
										var contador_salto2 = 0;
										while (reader2.Read())
										{
											var col2 = column;
											//excelWorksheet.InsertRow(row, 1);

											//excelWorksheet.Cells[row, 1].Value = reader.GetValue(3).ToString();
											for (int i = 5; i < 58; i++)
											{
												var cell = excelWorksheet.Cells[startRow, col2 + i - 5];
												excelWorksheet.Cells[row, col2 + i - 5].Value = reader2.GetValue(i);
											}
											contador_salto2++;
											if (contador_salto2 % 48 == 0)
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
							else
							{
								using (var reader = command.ExecuteReader())
								{
									while (reader.Read())
									{
										var col = column;
										if (row > startRow && insert_row == true) excelWorksheet.InsertRow(row, 1);

										for (var i = 0; i < reader.FieldCount; i++)
										{
											var cell = excelWorksheet.Cells[startRow, col];
											if (reader.GetValue(i) != null)
											{
												int number;
												bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);

												DateTime dt;
												bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);

												if (isNumber)
												{
													excelWorksheet.Cells[row, col].Value = number;
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

										row++;
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
                                    if (i==1 || i== 3){
                                        colcont+=4;
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

            //inserción de labels y logo en el Excel
            if (ReportCountry != null)
            {
                //inserción de labels
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
                                /**************Inicia inserción de labels automáticos*******************/
                                /*Si en la base de datos, el label se encierra dentro de dobles llaves {{parametro}}, el parámetro se cambiará por el parámetro correspondiente de búsqueda ingresado en el formulario*/
                                /*Llenado parámetros que vienen del formulario*/
                                string labelCountry = "";
                                string labelHospital = "";
                                string labelYear = "";
                                string labelSE = "";
                                string labelStartDate = "";
                                string labelEndDate = "";

                                //Obtención del pais y llenado en la variable
                                if (countryId > 0)
                                {
                                    using (var command2 = new SqlCommand("select * from Country where ID = @CountryID", con))
                                    {
                                        command2.Parameters.Clear();
                                        command2.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryId;
                                        using (var reader2 = command2.ExecuteReader())
                                        {
                                            while (reader2.Read())
                                            {
                                                labelCountry += reader2["Name"];
                                            }
                                        }
                                        command2.Parameters.Clear();
                                    }
                                }
                                if (hospitalId > 0)
                                {
                                    using (var command2 = new SqlCommand("select * from Institution where ID = @InstitutionID", con))
                                    {
                                        command2.Parameters.Clear();
                                        command2.Parameters.Add("@InstitutionID", SqlDbType.Int).Value = hospitalId;
                                        using (var reader2 = command2.ExecuteReader())
                                        {
                                            while (reader2.Read())
                                            {
                                                labelHospital += reader2["FullName"];
                                            }
                                        }
                                        command2.Parameters.Clear();
                                    }
                                }
                                if (year > 0)
                                {
                                    labelYear += year;
                                }
                                if (YearFrom > 0)
                                {
                                    labelYear += "Desde " + YearFrom + " ";
                                }
                                if (YearTo > 0)
                                {
                                    labelYear += "Hasta " + YearTo + " ";
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
                                /*Fin llenado parámetros*/

                                string label = reader["label"].ToString();
                                if (label != "")
                                {
                                    if (label.StartsWith("{{") && label.EndsWith("}}"))
                                    {
                                        switch (label)
                                        {
                                            case "{{country}}":
                                                label = (labelCountry != "" ? ("Pais:" + labelCountry) : "");
                                                break;
                                            case "{{institution}}":
                                                label = (labelHospital != "" ? ("Inst:" + labelHospital) : "");
                                                break;
                                            case "{{year}}":
                                                label = (labelYear != "" ? ("Año:" + labelYear) : "");
                                                break;
                                            case "{{se}}":
                                                label = (labelSE != "" ? ("SE:" + labelSE) : "");
                                                break;
                                            case "{{startDate}}":
                                                label = (labelStartDate != "" ? ("Del:" + labelStartDate) : "");
                                                break;
                                            case "{{endDate}}":
                                                label = (labelEndDate != "" ? ("Al:" + labelEndDate) : "");
                                                break;
                                            case "{{fullinstitution}}":
                                                label = (labelCountry!=""?("Pais:"+labelCountry):"") + " " + (labelHospital != "" ? ("Inst:" + labelHospital) : "");
                                                break;
                                            case "{{fulldate}}":
                                                label = (labelYear != "" ? ("Año:" + labelYear) : "") + " " + (labelSE != "" ? ("SE:" + labelSE) : "") + " " + (labelStartDate != "" ? ("Del:" + labelStartDate) : "") + " " + (labelEndDate != "" ? ("Al:" + labelEndDate) : "");
                                                break;
                                        }

                                    }
                                    excelWorksheet.Cells[insertrow, insertcol].Value = label;
                                }
                            }
                        }
                        command.Parameters.Clear();
                        con.Close();
                    }
                }
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
                }

            }
            /*-----------------------Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/
            /*string datosInstitucion="";
            string datosFechas="";

            using (var con = new SqlConnection(consString))
            {
                if (countryId > 0)
                {
                    using (var command = new SqlCommand("select * from Country where ID = @CountryID", con))
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryId;

                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                datosInstitucion += reader["Name"];
                            }
                        }
                        command.Parameters.Clear();
                        con.Close();
                    }
                }

                if (hospitalId > 0)
                {
                    using (var command = new SqlCommand("select * from Institution where ID = @InstitutionID", con))
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@InstitutionID", SqlDbType.Int).Value = hospitalId;

                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(countryId > 0)
                                {
                                    datosInstitucion += " - ";
                                }
                                datosInstitucion += reader["FullName"];
                            }
                        }
                        command.Parameters.Clear();
                        con.Close();
                    }
                }
            }
            if (year > 0)
            {
                datosFechas += "Año: " + year + " ";
            }
            if (se > 0)
            {
                datosFechas += "SE: " + se + " ";
            }
            if (startDate.HasValue)
            {
                datosFechas += "Fecha inicio: " + startDate.ToString() + " ";
            }
            if (endDate.HasValue)
            {
                datosFechas += "Fecha fin: " + endDate.ToString() + " ";
            }
            excelWorksheet.Cells[3, 1].Value = datosInstitucion;
            excelWorksheet.Cells[4, 1].Value = datosFechas;*/

            /*-----------------------Fin de la Inserción de los parámetros usados para la generación del reporte al Excel--------------------------------------*/

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
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
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
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
                                using (var command = new SqlCommand("EXEC sp_columns "+ i+";", con))
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
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
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

        private static void AppendDataToExcel_IndDes(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo)
        {
            ExcelWorksheet excelWorksheet1 = excelWorkBook.Worksheets["DatosPie"];
            //var excelWorksheet = excelWorkBook.Worksheets[sheet];
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

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 1, nDato1);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 2, nDato2);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 3, nDato1, aDato3);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 4, nDato4);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 5, nDato5);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 6, nDato6);
            //recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 7, nDato1, aDato7);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 8, nDato8);

            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 9, nDato9);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 10, nDato10);
            recuperarDatosIndDes(consString, languaje_, countryId, regionId, year, hospitalId, month, se, startDate, endDate, YearFrom, YearTo, 1, 11, nDato11);
            //****
            var excelWorksheet2 = excelWorkBook.Worksheets[1];

            string titulo = "";
            if (year != 0)
                titulo = year.ToString();

            string nombPais = "";
            if (countryId == 9)
                nombPais = "Costa Rica";
            else if (countryId == 7)
                nombPais = "Chile";
            else if (countryId == 25)
                nombPais = "Surinam";
            else if (countryId == 3)
                nombPais = "Bolivia";
            //**** Titulo
            excelWorksheet2.Cells[row - 4, column - 1].Value = nombPais;
            excelWorksheet2.Cells[row - 3, column - 1].Value = titulo;
            //**** Metas
            ArrayList aMetas = new ArrayList();
            ID_recuperarMetas(aMetas, countryId);

            double[] xx = (double[])aMetas[0];                      // Metas
            String[] yy = (String[])aMetas[1];                      // Unidades de la meta

            for (int nI = 0; nI < xx.Length; ++nI)
                excelWorksheet2.Cells[row + nI, column].Value = ID_formatearMeta(xx[nI], yy[nI]);
            //**** resultados
            double nTemp = 0;
            row = startRow;

            if (nDato1[0] != 0)
            {
                nTemp = (double)(nDato4[0] / nDato1[0] * 100);
                excelWorksheet2.Cells[row, column + 1].Value = ID_formatearMeta(nTemp, yy[0]);
                ID_setResultados(excelWorksheet1, nTemp, xx[0], 2, yy[0]);
            }

            if (nDato8[0] != 0)
            {
                nTemp = (double)(nDato5[0] / nDato8[0] * 100);
                excelWorksheet2.Cells[row + 1, column + 1].Value = ID_formatearMeta(nTemp, yy[1]);
                ID_setResultados(excelWorksheet1, nTemp, xx[1], 3, yy[1]);
            }


            if (nDato1[0] != 0)
            {
                nTemp = (double)(nDato6[0] / nDato1[0] * 100);
                excelWorksheet2.Cells[row + 2, column + 1].Value = ID_formatearMeta(nTemp, yy[2]);
                ID_setResultados(excelWorksheet1, nTemp, xx[2], 4, yy[2]);
            }

            nTemp = (double)nDato9[0];
            excelWorksheet2.Cells[row + 3, column + 1].Value = ID_formatearMeta(nTemp, yy[3]);
            ID_setResultados(excelWorksheet1, nTemp, xx[3], 5, yy[3]);

            nTemp = (double)nDato10[0];
            excelWorksheet2.Cells[row + 4, column + 1].Value = ID_formatearMeta(nTemp, yy[4]);
            ID_setResultados(excelWorksheet1, nTemp, xx[4], 6, yy[4]);

            nTemp = (double)nDato11[0];
            excelWorksheet2.Cells[row + 5, column + 1].Value = ID_formatearMeta(nTemp, yy[5]);
            ID_setResultados(excelWorksheet1, nTemp, xx[5], 7, yy[5]);
            /*
            decimal nTemp = 0;
            int nY = row;
            int nX = column;
            for (int nI = 0; nI <= 5; ++nI)
            {
                excelWorksheet2.Cells[nY + 1, nX + nI].Value = aDato3[nI, 0];

                nTemp = Convert.ToDecimal(aDato3[nI, 1], new CultureInfo("en-US"));
                if (nTemp != 0)
                    excelWorksheet2.Cells[nY + 2, nX + nI].Value = Convert.ToDecimal(aDato7[nI, 1], new CultureInfo("en-US")) / nTemp;
            }
            */
            /*
            decimal nTemp = 0;
            int nY = row;
            int nX = column;
            for (int nI = 0; nI <= 5; ++nI)
            {
                excelWorksheet2.Cells[nY + 1 + nI, nX - 1].Value = aDato3[nI, 0];

                nTemp = Convert.ToDecimal(aDato3[nI, 1], new CultureInfo("en-US"));
                if (nTemp != 0)
                {
                    excelWorksheet2.Cells[nY + 1 + nI, nX].Value = Convert.ToDecimal(aDato7[nI, 1], new CultureInfo("en-US")) / nTemp;
                }
            }
            */
            //****
            InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet2);
        }

        private static void recuperarDatosIndDes(string consString, string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo, int IRAG, int opcion, decimal[] nResuOut, string[,] aResuOut = null)
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
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = IRAG;
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
                                if (reader.GetValue(0).ToString() == "")
                                {
                                    nResuOut[0] = Convert.ToDecimal(0);
                                }
                                else
                                {
                                    nResuOut[0] = Convert.ToDecimal(reader.GetValue(0));
                                }
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
            double[] arr1 = new double[6];
            string[] arr2 = new string[6];

            string cMetas = ConfigurationManager.AppSettings["IndicadoresDesempenioMetas_" + _countryId.ToString()];
            if (cMetas == null)
                cMetas = "0:0:0:0:0:0";

            string[] aMetas = cMetas.Split(':');
            for (int nI = 0; nI < aMetas.Length; ++nI)
            {
                string cMeta = aMetas[nI].Substring(0, aMetas[nI].Length - 1);
                double nMeta = double.Parse(cMeta);
                string cUnid = aMetas[nI].Substring(aMetas[nI].Length - 1, 1);

                arr1[nI] = nMeta;
                arr2[nI] = cUnid;
            }

            _aMetas.Add(arr1);
            _aMetas.Add(arr2);
        }

        private static string ID_formatearMeta(double _Meta, string _Unid)
        {
            string cForma = "";
            string cMeta = _Meta.ToString("###.#", CultureInfo.InvariantCulture);

            if (_Unid == "%")
                cForma = cMeta + _Unid;
            if (_Unid == "D")
                cForma = (_Meta == 1) ? cMeta + " Día" : cMeta + " Días";

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
            else
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

        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            //searchedMsg = myR.getMessage(searchedMsg,countryID,countryLang);
            searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }

    }
}

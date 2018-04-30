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
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
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
                var inst_by_lab = db.InstitutionsConfiguration.OfType<InstitutionConfiguration>().Where(j => j.InstitutionToID == user.InstitutionID).Select(i => i.InstitutionToID).ToList();
                if (user.Institution.AccessLevel == AccessLevel.Country)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.CountryID == user.Institution.CountryID );
                }
                else if (user.Institution.AccessLevel == AccessLevel.Area)
                {
                    institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => i.AreaID == user.Institution.AreaID);
                }
                else
                {
                    if (inst_by_lab.Any())
                    {
                        institutions = db.Institutions.OfType<Hospital>()
                                   .Where(i => inst_by_lab.Contains(i.ID) );
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
        public ActionResult GetExcel(string Report, int CountryID, int? RegionID, int? HospitalID, int? Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? Inusual)        //#### CAFQ
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

                if (user.Institution.AccessLevel == AccessLevel.SelfOnly && HospitalID_ == 0) { HospitalID_ = Convert.ToInt32(user.Institution.ID); }

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
                    case "FLUID":
                        templateToUse = "FluIDTemplate";
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
                            //AppendDataToExcel_IndDes(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo);
                            AppendDataToExcel_IndDes(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual);        //#### CAFQ
                        else if (reportTemplate == "RE1")
                            AppendDataToExcel_REVELAC(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual);        //#### CAFQ
                        else if ((reportTemplate == "R2" || reportTemplate == "R3" || reportTemplate == "R1") && bVariosAnios)
                            AppendDataToExcel_R2_SeveralYears(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual);        //#### CAFQ: 180204
                        else {
                            //AppendDataToExcel(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo);
                            AppendDataToExcel(Languaje_, CountryID_, RegionID_, Year, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, YearFrom, YearTo, Surv, Inusual);        //#### CAFQ
                        }

                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;

                //return new FileStreamResult(ms, "application/xlsx")
                //ExcelPackage.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, outputPath);

                 string nombFile = reportCountry.description == "" ? "Exportable_" : reportCountry.description.ToString().Replace("%", "_").Replace(" ", "_") + "_";            //#### CAFQ
                /*if (reportTemplate == "I1")
                    nombFile = "IndicDesempenio_";
                if (reportTemplate == "RE1")
                    nombFile = "REVELAC-i_";*/

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

        private static void AppendDataToExcel_R2_SeveralYears(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];

            //var xxx = excelWorksheet.Drawings;                // Coleccion de charts
            //var yyy = excelWorksheet.Drawings[0];             // Un chart especifico
            if (storedProcedure == "R2" || storedProcedure == "R3" || storedProcedure == "R1")
                excelWorksheet.Drawings.Remove(0);              // Eliminando grafico por defecto en plantilla

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
                            while (reader.Read())
                            {
                                var col = column;
                                var cell = excelWorksheet.Cells[row, col];

                                if (row > startRow && insert_row == true)
                                    excelWorksheet.InsertRow(row, 1);

                                if (storedProcedure == "R2")
                                    col = column + (nI - YearFrom.Value) + ((nI == YearFrom) ? 0 : 1);

                                for (var i = 0; i < reader.FieldCount; i++)
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
            if (storedProcedure == "R2")
            {
                var myChartCC = excelWorksheet.Drawings.AddChart("ChartColumnClustered", eChartType.ColumnClustered);

                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    int nCol = startColumn + (nI - YearFrom.Value) + 1;
                    var seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(7, nCol, 59, nCol), ExcelRange.GetAddress(7, 2, 59, 2));
                    seriesCC.Header = nI.ToString();
                }

                //myChartCC.Border.Fill.Color = System.Drawing.Color.Red;
                myChartCC.Title.Text = "NÚMERO DE FALLECIDOS POR SEMANA EPIDEMIOLÓGICA";
                myChartCC.Title.Font.Bold = true;
                myChartCC.Legend.Position = eLegendPosition.Bottom;
                myChartCC.SetSize(920, 405);                    // Ancho, Alto in pixel
                myChartCC.SetPosition(startRow - 2, 0, (startColumn + (YearTo.Value - YearFrom.Value) + 1), 40);             // (int row, int rowoffset in pixel, int col, int coloffset in pixel)

                ExcelRange rRang = excelWorksheet.Cells[ExcelRange.GetAddress(startRow - 2, startColumn + 1, startRow - 2, startColumn + 1 + (YearTo.Value - YearFrom.Value))];
                rRang.Merge = true;
                rRang.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                rRang.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                rRang.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                rRang.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }
            else if (storedProcedure == "R3")
            {
                int nFil = startRow;
                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    nFil = startRow + (nI - YearFrom.Value) * 54;
                    int nCol = startColumn;

                    var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStacked" + nI.ToString(), eChartType.ColumnStacked);

                    var seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 1, nFil + 52, nCol + 1), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 1].Value.ToString();

                    seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 2, nFil + 52, nCol + 2), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 2].Value.ToString();

                    seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 3, nFil + 52, nCol + 3), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 3].Value.ToString();

                    seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 4, nFil + 52, nCol + 4), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 4].Value.ToString();

                    seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 5, nFil + 52, nCol + 5), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 5].Value.ToString();

                    seriesCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 6, nFil + 52, nCol + 6), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    seriesCC.Header = excelWorksheet.Cells[startRow - 1, nCol + 6].Value.ToString();

                    myChartCC.Title.Text = "NÚMERO DE CASOS IRAG POR GRUPO DE EDAD Y SE - " + nI.ToString();
                    myChartCC.Title.Font.Bold = true;
                    myChartCC.SetSize(1090, 450);
                    myChartCC.SetPosition(startRow + (23 * (nI - YearFrom.Value)) - 1, 0, 8, 40);
                    myChartCC.Legend.Position = eLegendPosition.Bottom;

                    // Formateo area de datos
                    if (nI > YearFrom.Value)
                    {
                        var rowA = startRow + (nI - YearFrom.Value) * 54;

                        ExcelRange rRang = excelWorksheet.Cells[ExcelRange.GetAddress(rowA, startColumn, rowA + 53, startColumn + 6)];
                        rRang.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        rRang.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        rRang.Style.Border.BorderAround(ExcelBorderStyle.Medium);

                        rRang = excelWorksheet.Cells[ExcelRange.GetAddress(startRow + 53, startColumn, startRow + 53, startColumn + 6)];
                        rRang.Copy(excelWorksheet.Cells[rowA + 53, startColumn]);                // Total
                    }
                }
            }
            else if (storedProcedure == "R1")
            {
                int nFil = startRow;
                for (int nI = YearFrom.Value; nI <= YearTo.Value; ++nI)
                {
                    nFil = startRow + (nI - YearFrom.Value) * 54;
                    int nCol = startColumn;

                    var myChartCC = excelWorksheet.Drawings.AddChart("ColumnStackedLine" + nI.ToString(), eChartType.ColumnClustered);
                    var serieCC = myChartCC.Series.Add(ExcelRange.GetAddress(nFil, nCol + 1, nFil + 52, nCol + 1), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));

                    var myChartLI = myChartCC.PlotArea.ChartTypes.Add(eChartType.Line);
                    var serieLI = myChartLI.Series.Add(ExcelRange.GetAddress(nFil, nCol + 3, nFil + 52, nCol + 3), ExcelRange.GetAddress(nFil, 2, nFil + 52, 2));
                    myChartLI.UseSecondaryAxis = true;

                    myChartCC.Title.Text = "CASOS DE IRAG POR SEMANA EPIDEMIOLOGICA CON % DE HOSPITALIZACIONES - " + nI.ToString();
                    myChartCC.Title.Font.Bold = true;
                    myChartCC.SetSize(900, 420);
                    myChartCC.SetPosition(startRow + (23 * (nI - YearFrom.Value)) - 1, 0, 5, 40);
                    myChartCC.Legend.Remove();

                    // Formateo area de datos
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
                InsertarLabelsExcel(consString, excelWorksheet, countryId, ReportCountry, hospitalId, year, se, startDate, endDate, YearFrom, YearTo);

                //inserción de logo
                InsertarLogoExcel(consString, excelWorksheet, ReportCountry);
            }
        }

        private static void InsertarLabelsExcel(string consString, ExcelWorksheet excelWorksheet, int countryId, int? ReportCountry, int? hospitalId, int? year, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo)
        {//(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)
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
                                            label = (labelCountry != "" ? ("Pais:" + labelCountry) : "") + " " + (labelHospital != "" ? ("Inst:" + labelHospital) : "");
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
        private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)               //#### CAFQ
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string _storedProcedure;
            int excelColTota = 0, nPosiTipo = 0, nInicTip2 = 0, nPoSuViGr = 0;

            _storedProcedure = storedProcedure;
            if (storedProcedure == "R5")
            {
                /*if (countryId == 25 || countryId == 17)
                {
                    _storedProcedure = "R5_2";
                    nPosiTipo = 19;
                    nInicTip2 = 10;                 //Inicio hospitalizados
                    nPoSuViGr = 12;                 // Posic. Ecel Sumatoria
                }
                else
                {
                    nPosiTipo = 15;
                    nInicTip2 = 8;                  //Inicio hospitalizados
                    nPoSuViGr = 10;                 // Posic. Ecel Sumatoria
                }*/
                if (countryId == 17)
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

                        /*if (countryId == 25 || countryId == 17)
                        {
                            formulas1[9] = "=VRS!L{{toreplace}}";
                            formulas1[10] = "=VRS!M{{toreplace}}+Ad!M{{toreplace}}+Parainfluenza!M{{toreplace}}+'Inf A'!M{{toreplace}}+'Inf B'!M{{toreplace}}+Metapnemovirus!M{{toreplace}}";

                            formulas1[11] = "=VRS!M{{toreplace}}";
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
                        }*/
                        if (countryId == 17)
                        {
                            /*formulas1[9] = "=VRS!L{{toreplace}}";
                            formulas1[10] = "=VRS!M{{toreplace}}+Ad!M{{toreplace}}+Parainfluenza!M{{toreplace}}+'Inf A'!M{{toreplace}}+'Inf B'!M{{toreplace}}+Metapnemovirus!M{{toreplace}}";

                            formulas1[11] = "=VRS!M{{toreplace}}";
                            formulas1[12] = "=Ad!M{{toreplace}}";
                            formulas1[13] = "=Parainfluenza!M{{toreplace}}";
                            formulas1[14] = "='Inf A'!M{{toreplace}}";
                            formulas1[15] = "='Inf B'!M{{toreplace}}";
                            formulas1[16] = "=Metapnemovirus!M{{toreplace}}";

                            formulas1[17] = "=D{{toreplace}}+E{{toreplace}}+F{{toreplace}}+G{{toreplace}}+H{{toreplace}}+I{{toreplace}}+J{{toreplace}}+K{{toreplace}}+L{{toreplace}}";
                            formulas1[18] = "=O{{toreplace}}+P{{toreplace}}+Q{{toreplace}}+R{{toreplace}}+S{{toreplace}}+T{{toreplace}}+U{{toreplace}}+V{{toreplace}}+W{{toreplace}}";
                            */
                            formulas1[9] = "=VRS!M{{toreplace}}";
                            formulas1[10] = "=VRS!N{{toreplace}}+Ad!N{{toreplace}}+Parainfluenza!N{{toreplace}}+'Inf A'!N{{toreplace}}+'Inf B'!N{{toreplace}}+Metapnemovirus!N{{toreplace}}";

                            formulas1[11] = "=VRS!N{{toreplace}}";
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
                                formulas1[9] = "=VRS!L{{toreplace}}";
                                formulas1[10] = "=VRS!M{{toreplace}}+Ad!M{{toreplace}}+Parainfluenza!M{{toreplace}}+'Inf A'!M{{toreplace}}+'Inf B'!M{{toreplace}}+Metapnemovirus!M{{toreplace}}";

                                formulas1[11] = "=VRS!M{{toreplace}}";
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
                                command.Parameters.Add("@IRAG", SqlDbType.Int).Value = Surv;                        //#### CAFQ
                                command.Parameters.Add("@SurvInusual", SqlDbType.Bit).Value = SurvInusual;          //#### CAFQ

                                var con2 = new SqlConnection(consString);
                                con2.Open();

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    //row = 212;
                                    int nAnDa = 0;
                                    /*if (countryId == 25 || countryId == 17)
                                    {
                                        row = row - 1 + (9 * 3) + 15;
                                        nAnDa = 8 * 8;              // 8: Nº Age Group
                                    }
                                    else
                                    {
                                        row = 212;
                                        nAnDa = 6 * 8;              // 6: Nº Age Group
                                    }*/
                                    if (countryId == 25)
                                    {
                                        row = row - 1 + (9 * 3) + 15;
                                        nAnDa = 8 * 8;                  // 8: Nº Age Group
                                    }
                                    else
                                    {
                                        if (countryId == 17)
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
                                        //excelWorksheet.InsertRow(row, 1);

                                        //excelWorksheet.Cells[row, 1].Value = reader.GetValue(3).ToString();
                                        for (int i = 5; i < 58; i++)
                                        {
                                            var cell = excelWorksheet.Cells[startRow, col2 + i - 5];
                                            excelWorksheet.Cells[row, col2 + i - 5].Value = reader2.GetValue(i);
                                        }
                                        contador_salto2++;

                                        //if (contador_salto2 % 48 == 0)
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
                                            //int number;
                                            //bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);
                                            double numberD;
                                            bool isNumber = double.TryParse(reader.GetValue(i).ToString(), out numberD);

                                            DateTime dt;
                                            bool isDate = DateTime.TryParse(reader.GetValue(i).ToString(), out dt);

                                            if (isNumber)
                                            {
                                                //excelWorksheet.Cells[row, col].Value = number;
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
                reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet);

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

        private static void AppendDataToExcel_REVELAC(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)         //#### CAFQ
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
        private static void AppendDataToExcel_IndDes(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string reportTemplate, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? YearFrom, int? YearTo, int? Surv, bool? SurvInusual)         //#### CAFQ
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

            /*  : #### ReportCountryConfig ####
            string titulo = "";
            if (year != 0)
                titulo = year.ToString();
            */

            /*string nombPais = "";         : #### ReportCountryConfig ####
            if (countryId == 9)
                nombPais = "Costa Rica";
            else if (countryId == 7)
                nombPais = "Chile";
            else if (countryId == 25)
                nombPais = "Suriname";
            else if (countryId == 17)
                nombPais = "Jamaica";
            else if (countryId == 3)
                nombPais = "Bolivia";*/

            //**** Titulo : #### ReportCountryConfig ####
            /*excelWorksheet2.Cells[row - 4, column - 1].Value = nombPais;
            excelWorksheet2.Cells[row - 3, column - 1].Value = titulo;*/

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
            reportLabels(consString, countryId, languaje_, ReportCountry, hospitalId, year, YearFrom, YearTo, se, startDate, endDate, excelWorkBook, excelWorksheet2);
            InsertarImagenLogo(consString, reportTemplate, ReportCountry, excelWorksheet2);
        }

        private static void reportLabels(string consString, int countryId, string languaje_, int? ReportCountry, int? hospitalId, int? year, int? YearFrom, int? YearTo, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, ExcelWorksheet excelWorksheet)
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
                            int tab = Int32.Parse((string)reader["tab"]);           // Hoja
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
                                            //label = (labelCountry != "" ? ("Pais:" + labelCountry) : "");
                                            label = (labelCountry != "" ? (SgetMsg("msgViewExportarLabelCountry", countryId, languaje_) + ": " + labelCountry) : "");
                                            break;
                                        case "{{institution}}":
                                            //label = (labelHospital != "" ? ("Inst:" + labelHospital) : "");
                                            label = (labelHospital != "" ? (SgetMsg("msgViewExportarLabelInstitutionShort", countryId, languaje_) + ": " + labelHospital) : "");
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
                                        default:                    //#### CAFQ: 180415
                                            label = "";             //#### CAFQ: 180415
                                            break;                  //#### CAFQ: 180415
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
                                if (label == "")
                                    //excelWorksheet.Cells[insertrow, insertcol].Value = cOriginal;
                                    excelWs.Cells[insertrow, insertcol].Value = cOriginal;
                                else
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
                /*if (countryId == 25 || countryId == 17)
                    cForma = (_Meta == 1) ? cMeta + " Day" : cMeta + " Days";
                else
                    cForma = (_Meta == 1) ? cMeta + " Día" : cMeta + " Días";*/
                cForma = (_Meta == 1) ? cMeta + " " + SgetMsg("msgViewExportarLabelDay", countryId, languaje) : cMeta + " " + SgetMsg("msgViewExportarLabelDays", countryId, languaje);
            }

            if (_Unid == "H")
            {
                cMeta = _Meta.ToString("##0", CultureInfo.InvariantCulture);
                /*if (countryId == 25 || countryId == 17)
                    cForma = (_Meta == 1) ? cMeta + " " + "Hour" : cMeta + " " + "Hours";
                else
                    cForma = (_Meta == 1) ? cMeta + " " + "Hora" : cMeta + " " + "Horas";*/
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

        public string getMsg(string msgView)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            string searchedMsg = msgView;
            int? countryID = user.Institution.CountryID;
            string countryLang = user.Institution.Country.Language;

            ResourcesM myR = new ResourcesM();
            searchedMsg = myR.getMessage(searchedMsg,countryID,countryLang);
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

using System;
//using System.Collections.Generic;
using System.Configuration;
using System.Data;
//using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
//using System.Web;
using System.Web.Mvc;
//using System.Xml;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using Paho.Models;
using System.Collections.Generic;

namespace Paho.Controllers {

    [Authorize(Roles = "Admin, Report")]
    public class FluidController : ControllerBase {
        
        public ActionResult Index() {
            var FluidViewModel = new FluidViewModel();
            IQueryable<Region> regions = null;
            IQueryable<Institution> institutions = null;
            var user = UserManager.FindById(User.Identity.GetUserId());
            FluidViewModel.CountryID = user.Institution.CountryID ?? 0;
            if (user.Institution.AccessLevel == AccessLevel.All)
            {
                FluidViewModel.DisplayCountries = true;
                FluidViewModel.DisplayRegionals = true;
                FluidViewModel.DisplayHospitals = true;
            }
            else if (user.Institution is Hospital || user.Institution is AdminInstitution)
            {
                FluidViewModel.DisplayHospitals = true;

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

            FluidViewModel.Countries = db.Countries
                    .Select(c => new CountryView()
                    {
                        Id = c.ID.ToString(),
                        Name = c.Name,
                        Active = c.Active
                    }).ToArray();

            if (regions != null)
            {
                var regionsDisplay = regions.Select(i => new LookupView<Region>()
                {
                    Id = i.orig_country.ToString(),
                    Name = i.Name
                }).ToList();

                if (regionsDisplay.Count() > 1)
                {
                    //var all = new LookupView<Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Region> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    regionsDisplay.Insert(0, all);
                    FluidViewModel.DisplayRegionals = true;
                }

                FluidViewModel.Regions = regionsDisplay;
                //CaseViewModel.Regions = regions;
            }

            if (institutions != null)
            {
                var institutionDisplay = institutions.Select(i => new LookupView<Institution>()
                {
                    Id = i.ID.ToString(),
                    Name = i.Name
                }).ToList();

                if (institutionDisplay.Count() > 1)
                {
                    //var all = new LookupView<Region> { Id = "0", Name = "-- Todo(a)s --" };
                    var all = new LookupView<Institution> { Id = "0", Name = getMsg("msgGeneralMessageAll") };
                    institutionDisplay.Insert(0, all);
                }
                FluidViewModel.Institutions = institutionDisplay;
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

            return View(FluidViewModel);
        }

        [HttpGet]
        public ActionResult Generate(int CountryID, string Languaje_country, int? RegionId, int? HospitalID, int? Year, int? WeekFrom, int? WeekTo, int? surveillance) {
            try {
                var ms = new MemoryStream();
                var Languaje_country_ = db.Countries.Find(CountryID);
                var surv = surveillance ;

                if (surv == null)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());
                    var SARI = user.Institution.SARI;
                    surv = SARI != null ? 1 :  2;
                }

                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["FluIDTemplate"].Replace("{countryId}", CountryID.ToString()))) {
                    using (var excelPackage = new ExcelPackage(fs)) {
                        var excelWorkBook = excelPackage.Workbook;
                        
                        AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 2,false);
                        AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FLUID_IRAG", 8, 4, false);
                        AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FLUID_DEATHS_IRAG", 8, 6, false);
                        AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FLUID_ETI", 8, 7, false);
                        AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FLUID_NATIONAL_VIRUSES", 6, 8, false);
                        //AppendDataToExcel(CountryID, Languaje_country_.Language.ToString(), Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "FluidIragExport", 33, "Fallecidos IRAG");
                        //if (CountryID == 7)
                        //{
                        ConfigToExcel(CountryID, Languaje_country_.Language.ToString(), RegionId, Year, HospitalID, WeekFrom, WeekTo, excelWorkBook, "Leyendas", 1, 9, false);
                        //}

                        excelPackage.SaveAs(ms);
                    }
                }

                ms.Position = 0;

                return new FileStreamResult(ms, "application/xlsx") {
                    FileDownloadName = "FluID_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".xlsx"
                };

                //ViewBag.Message = $"Report generated successfully!";
            } catch (Exception e)             {
                ViewBag.Message = "Reporte no pudo generarse, por favor ponganse en contacto con el administrador";
            }

            return View();
        }

        private static void AppendDataToExcel(int countryId,  string languaje_country, int? regionId, int? year, int? hospitalId, int? weekFrom, int? weekTo, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int sheet, bool? insert_row) 
        {

            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) {CommandType = CommandType.StoredProcedure})
                {
                    //command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = languaje_country;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = weekFrom;
                    command.Parameters.Add("@weekTo", SqlDbType.Int).Value = weekTo;
                    if (sheet == 8)
                    {
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = 2;
                    }

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
        private void ConfigToExcel(int countryId, string languaje_country, int? regionId,  int? year, int? hospitalId, int? weekFrom, int? weekTo, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int sheet, bool? insert_row)
        {
            var excelWorksheet = excelWorkBook.Worksheets[sheet];

            if (year != null)
            {
                excelWorksheet.Cells[2, 1].Value = year.ToString();
            }

            if (hospitalId != null && hospitalId > 0)
            {
                var Institution = db.Institutions.Find(hospitalId);

                excelWorksheet.Cells[2, 5].Value = Institution.Name.ToString();

              } else if (regionId != null)
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

    }
}
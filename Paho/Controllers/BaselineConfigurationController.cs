using OfficeOpenXml;
using Paho.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Paho.Controllers
{

    [Authorize(Roles = "Admin")]
    public class BaselineConfigurationController : ControllerBase
    {
        // GET: BaselineConfiguration
        public ActionResult Index()
        {
            BaselineConfigurationViewModel oBaseLineConfig = new BaselineConfigurationViewModel();

            var countries = db.Countries
                                .Where(i => i.Active == true)
                                .Select(c => new CountryView()
                                {
                                    Id = c.ID.ToString(),
                                    Name = c.Name,
                                    Active = c.Active
                                });

            oBaseLineConfig.Countries = countries;

            return View(oBaseLineConfig);
        }

        public JsonResult GetParameters(int? CountryID)
        {
            string pathLB = ConfigurationManager.AppSettings["GraphicsPath"];
            string filePathLB = pathLB + "\\" + "LinBa_" + CountryID + ".xlsx";
            string jsonData;

            //Opening an existing Excel file
            //FileInfo fi = new FileInfo(@"Path\To\Your\File.xlsx");
            FileInfo fi = new FileInfo(@filePathLB);

            if (fi.Exists == false)                         // Archivo no existe
            {
                string filePathBA;

                var country = db.Countries
                    .Where(i => i.ID == CountryID)
                    .Select(c => new
                    {
                        Id = c.ID,
                        Name = c.Name,
                        Language = c.Language
                    }).ToList();

                if (country[0].Language == "SPA")
                {
                    filePathBA = pathLB + "\\" + "LinBa_Base_SPA.xlsx";
                }
                else
                {
                    filePathBA = pathLB + "\\" + "LinBa_Base_ENG.xlsx";
                }

                FileInfo fiBA = new FileInfo(@filePathBA);
                using (ExcelPackage excelPackageBA = new ExcelPackage(fiBA))
                {
                    ExcelWorksheet wsBA = excelPackageBA.Workbook.Worksheets[1];

                    wsBA.Cells["L7"].Value = country[0].Name;
                    wsBA.Name = country[0].Name;

                    var fileDE = new FileInfo(filePathLB);              // Salvar con otro nombre o destino
                    excelPackageBA.SaveAs(fileDE);
                }
            }

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //Get a WorkSheet by index. Note that EPPlus indexes are base 1, not base 0!
                ExcelWorksheet firstWorksheet = excelPackage.Workbook.Worksheets[1];

                ////Get a WorkSheet by name. If the worksheet doesn't exist, throw an exeption
                //ExcelWorksheet namedWorksheet = excelPackage.Workbook.Worksheets["SomeWorksheet"];

                ////If you don't know if a worksheet exists, you could use LINQ,
                ////So it doesn't throw an exception, but return null in case it doesn't find it
                //ExcelWorksheet anotherWorksheet =
                //    excelPackage.Workbook.Worksheets.FirstOrDefault(x => x.Name == "SomeWorksheet");

                //Get the content from cells A1 and B1 as string, in two different notations
                string title = firstWorksheet.Cells["L3"].Value.ToString();
                string year = firstWorksheet.Cells["L4"].Value.ToString();
                string startWeek = firstWorksheet.Cells["L5"].Value.ToString();
                string totalWeek = firstWorksheet.Cells["L6"].Value.ToString();

                jsonData = "{\"Year\":\"" + year + "\",\"StartWeek\":\"" + startWeek + "\",\"TotalWeek\":\"" + totalWeek + "\",\"Title\":\"" + title + "\"}";
                //string valB1 = firstWorksheet.Cells[1, 2].Value.ToString();

                ////Save your file
                //excelPackage.Save();
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public string SaveParameters(int? CountryID, int? Year, int? StartWeek, int? EndWeek, string Title, HttpPostedFileBase myfile)
        {
            string saveResult = "";

            string pathLB = ConfigurationManager.AppSettings["GraphicsPath"];
            string filePathLB = pathLB + "\\" + "LinBa_" + CountryID + ".xlsx";
            //string jsonData;

            FileInfo fiLB = new FileInfo(@filePathLB);
            using (ExcelPackage excelPackageLB = new ExcelPackage(fiLB))
            {
                ExcelWorksheet wsLB = excelPackageLB.Workbook.Worksheets[1];

                wsLB.Cells["L3"].Value = Title;
                wsLB.Cells["L4"].Value = Year;
                wsLB.Cells["L5"].Value = StartWeek;
                wsLB.Cells["L6"].Value = EndWeek;

                if (Request.Files.Count > 0)            // Attachment
                {
                    string fileName;
                    string path = "";

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file_ = Request.Files[i];
                        fileName = Path.GetFileName(file_.FileName);
                        path = Path.Combine(ConfigurationManager.AppSettings["UploadDir"], fileName);
                        int counterFile = 0;
                        while (System.IO.File.Exists(path))
                        {
                            path = Path.Combine(ConfigurationManager.AppSettings["UploadDir"], counterFile + fileName);
                            counterFile++;
                        }
                        file_.SaveAs(path);
                    }

                    FileInfo fiDa = new FileInfo(@path);
                    using (ExcelPackage excelPackageDa = new ExcelPackage(fiDa))
                    {
                        ExcelWorksheet wsDa = excelPackageDa.Workbook.Worksheets[1];
                        ////Get the content from cells A1 and B1 as string, in two different notations
                        //firstWorksheet.Cells["L3"].Value = Title;
                        //firstWorksheet.Cells["L4"].Value = Year;
                        //firstWorksheet.Cells["L5"].Value = StartWeek;
                        //firstWorksheet.Cells["L6"].Value = EndWeek;

                        wsLB.Cells["B3:H55"].Clear();

                        wsDa.Cells[2, 2, 54, 2].Copy(wsLB.Cells[3, 3, 55, 3]);      // Average Curve
                        wsDa.Cells[2, 6, 54, 6].Copy(wsLB.Cells[3, 5, 55, 5]);      // Epidemic
                        wsDa.Cells[2, 7, 54, 7].Copy(wsLB.Cells[3, 6, 55, 6]);      // Moderate
                        wsDa.Cells[2, 8, 54, 8].Copy(wsLB.Cells[3, 7, 55, 7]);      // High
                        wsDa.Cells[2, 9, 54, 9].Copy(wsLB.Cells[3, 8, 55, 8]);      // Extraordinary

                        /*
                         Copia la columna 5 en la columna 2 Básicamente Source.Copy (Destino)
                            Esto solo copiaría las primeras 100 filas.
	                            workSheet.Cells[1,5,100,5].Copy(workSheet.Cells[1,2,100,2]);
                         * */
                        //excelPackage.Save();
                    }
                }// END If

                excelPackageLB.Save();
            }



            saveResult = "1";

            return saveResult;
        }




        //    public string getMsg(string msgView)
        //    {
        //        var user = UserManager.FindById(User.Identity.GetUserId());
        //        string searchedMsg = msgView;
        //        int? countryID = user.Institution.CountryID;
        //        string countryLang = user.Institution.Country.Language;

        //        ResourcesM myR = new ResourcesM();
        //        searchedMsg = myR.getMessage(searchedMsg, countryID, countryLang);
        //        //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
        //        return searchedMsg;
        //    }

        //    private static string SgetMsg(string msgView, int? countryDisp, string langDisp)
        //    {
        //        string searchedMsg = ResourcesM.SgetMessage(msgView, countryDisp, langDisp);
        //        //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
        //        return searchedMsg;
        //    }
    }
}
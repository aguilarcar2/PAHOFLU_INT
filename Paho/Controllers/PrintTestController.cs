using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.IO;
using OfficeOpenXml;
using Spire.Xls;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    public class PrintTestController : Controller
    {
        // GET: ExportLab
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPrint(string Report, int CountryID, int InstitutionID, int RecordID, int NumSample)
        {
            try
            {
                var ms = new MemoryStream();
                var pdfs = new MemoryStream();
                //var user = UserManager.FindById(User.Identity.GetUserId());
                int CountryID_ = CountryID;
                int? HospitalID_ = InstitutionID;
                int? RecordID_ = RecordID;
                int? NumSample_ = NumSample;
                string Path_Print = ConfigurationManager.AppSettings["PathPrintTest"];
                string Sample_Print_ = "Imp_Mue_" + RecordID_.ToString() + "_" + NumSample_.ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm");

                if (Report == "" )
                {
                    ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
                    return null;
                    ;
                }
                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["PrintTestTemplate"]
                    .Replace("{countryId}", CountryID_.ToString())
                    ))
                {
                    using (var excelPackage = new ExcelPackage(fs))
                    {
                        var excelWorkBook = excelPackage.Workbook;
                        int startColumn = 1;
                        int startRow = 1;
                        bool insertRow = false;

                        AppendDataToExcel(CountryID_, RecordID_, NumSample_, HospitalID_, excelWorkBook, Report, startRow, startColumn, 1, insertRow);

                        var excelPackage_Print = excelPackage;

                        excelPackage.SaveAs(ms);
                        

                    }
                }

                ms.Position = 0;
                //FileStream file = new FileStream(Path_Print + Sample_Print_ + ".xlsx", FileMode.Create, FileAccess.Write);
                //ms.WriteTo(file);
                //file.Close();

                try
                {
                    // Conversion a PDF
                    // Create COM Objects
                    //Microsoft.Office.Interop.Excel.Application excelApplication;
                    //Microsoft.Office.Interop.Excel.Workbook excelWorkbook;

                    // Create new instance of Excel
                    //excelApplication = new Microsoft.Office.Interop.Excel.Application();

                    //excelApplication.Visible = false;
                    //excelApplication.ScreenUpdating = false;
                    //// Make the process silent
                    //excelApplication.DisplayAlerts = false;

                    //// Open the workbook that you wish to export to PDF
                    //excelWorkbook = excelApplication.Workbooks.Open(Path_Print + Sample_Print_ + ".xlsx");

                    ////    excelWorkbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, Path_Print + Sample_Print_ + ".pdf");

                    ////    // Close the workbook, quit the Excel, and clean up regardless of the results...
                    //excelWorkbook.Close();
                    //excelApplication.Quit();

                    //    excelApplication = null;
                    //    excelWorkbook = null;

 
                    Workbook workbook = new Workbook();

                    workbook.LoadFromStream(ms);
                    workbook.SaveToFile(Path_Print + Sample_Print_ + ".pdf", Spire.Xls.FileFormat.PDF);
                    workbook.SaveToStream(pdfs,Spire.Xls.FileFormat.PDF);

                }
                catch (System.Exception ex)
                {
                    string sSource;
                    string sLog;
                    string sEvent;

                    sSource = "PAHOFLU";
                    sLog = "sLog";
                    sEvent = ex.Message.ToString();

                    if (!EventLog.SourceExists(sSource))
                        EventLog.CreateEventSource(sSource, sLog);

                    EventLog.WriteEntry(sSource, sEvent);
                    //Console.Write("PAHOFLU - " + ex.Message);
                }

                // Esto fue desactivado porque ya no necesito grabarlo a disco
                //System.IO.File.Delete(Path_Print + Sample_Print_ + ".xlsx");

                //var pdfContent = new MemoryStream(System.IO.File.ReadAllBytes(Path_Print + Sample_Print_ + ".xlsx"));
                //System.IO.File.Delete(Path_Print + Sample_Print_ + ".pdf");

                //"application/pdf"
                //return new FileStreamResult(ms, "application/xlsx")

                return new FileStreamResult(pdfs, "application/pdf")
                {
                    FileDownloadName = Sample_Print_ + ".pdf"
                };

            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
            }

            return null;
        }

        private static void AppendDataToExcel(int countryId, int? recordID, int? NumSample, int? hospitalId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row)
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
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = "SPA";
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@RecordID", SqlDbType.Int).Value = recordID;
                    command.Parameters.Add("@Number_Sample", SqlDbType.Int).Value = NumSample;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var col = column;
                            excelWorksheet.Cells[7, 15].Value = reader.GetValue(reader.GetOrdinal("Id")).ToString();
                            //Fila 8
                            excelWorksheet.Cells[8, 9].Value =  reader.GetValue(reader.GetOrdinal("NameComplete")).ToString();
                            //Fila 9
                            excelWorksheet.Cells[9, 4].Value = reader.GetValue(reader.GetOrdinal("RUT")).ToString();
                            DateTime DateTransform_;
                            bool isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Fecha_Nacimiento")).ToString(), out DateTransform_);
                            excelWorksheet.Cells[9, 15].Value = DateTransform_;
                            //Fila 10 
                            excelWorksheet.Cells[10, 4].Value = reader.GetValue(reader.GetOrdinal("edad_comp")).ToString();
                            excelWorksheet.Cells[10, 13].Value = reader.GetValue(reader.GetOrdinal("Sexo")).ToString();
                            //Fila 11 
                            excelWorksheet.Cells[11, 5].Value = reader.GetValue(reader.GetOrdinal("Establecimiento")).ToString();
                            excelWorksheet.Cells[11, 13].Value = reader.GetValue(reader.GetOrdinal("region")).ToString();

                            //Fila 14
                            isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Inicio_sintomas")).ToString(), out DateTransform_);
                            excelWorksheet.Cells[14, 6].Value = DateTransform_;
                            excelWorksheet.Cells[14, 15].Value = reader.GetValue(reader.GetOrdinal("Fiebre_Historiafiebre")).ToString();

                            //Fila 16
                            isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Fecha_muestra")).ToString(), out DateTransform_);
                            excelWorksheet.Cells[16, 8].Value = DateTransform_;

                            //Fila 17
                            excelWorksheet.Cells[17, 5].Value = reader.GetValue(reader.GetOrdinal("Tipo_muestra")).ToString();
                            
                            //Fila 19
                            excelWorksheet.Cells[19, 6].Value = reader.GetValue(reader.GetOrdinal("Destino")).ToString();

                            // Revision de los datos de los resultados del laboratorio

                            for (var i = 1; i < 7; i++)
                            {
                                if (reader.GetValue(reader.GetOrdinal("procesado_proceso_"+i.ToString())).ToString() != "")
                                {
                                    if (reader.GetValue(reader.GetOrdinal("tipo_proceso_proceso_"+i.ToString())).ToString() == "IF")
                                    {
                                        //excelWorksheet.Cells[35, 6].Value = reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString();

                                        excelWorksheet.Cells[36, 6].Value += reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper() + ((reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() != "") ? " - " + reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString().ToUpper() + " , " : ", ");

                                        isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("fecha_fin_proceso_" + i.ToString())).ToString(), out DateTransform_);
                                        excelWorksheet.Cells[37, 4].Value = DateTransform_;
                                        excelWorksheet.Cells[38, 6].Value +=   ((excelWorksheet.Cells[38, 6].Value != "" && excelWorksheet.Cells[38, 6].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString();
                                    }

                                    if (reader.GetValue(reader.GetOrdinal("tipo_proceso_proceso_" + i.ToString())).ToString() == "PCR")
                                    {
                                        if (reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza A")
                                        {
                                            excelWorksheet.Cells[23, 6].Value += " "+ reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[23, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() : "";
                                            if (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "")
                                            {
                                                excelWorksheet.Cells[28, 6].Value += " POSITIVO,";
                                                excelWorksheet.Cells[28, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() : "";
                                            }
                                            if (reader.GetValue(reader.GetOrdinal("subtipo_proceso_" + i.ToString())).ToString().Contains("H1"))
                                            {
                                                excelWorksheet.Cells[25, 6].Value += " " + reader.GetValue(reader.GetOrdinal("subtipo_resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                                excelWorksheet.Cells[25, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_subtype_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CT_subtype_proceso_" + i.ToString())).ToString() : "";
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[30, 6].Value += " POSITIVO,";
                                                    excelWorksheet.Cells[30, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }

                                            if (reader.GetValue(reader.GetOrdinal("subtipo_proceso_" + i.ToString())).ToString().Contains("H3"))
                                            {
                                                excelWorksheet.Cells[26, 6].Value += " " + reader.GetValue(reader.GetOrdinal("subtipo_resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                                excelWorksheet.Cells[26, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_subtype_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CT_subtype_proceso_" + i.ToString())).ToString() : "";
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[31, 6].Value += " POSITIVO,";
                                                    excelWorksheet.Cells[31, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_subtype_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                            if (reader.GetValue(reader.GetOrdinal("subtipo_2_proceso_" + i.ToString())).ToString().Contains("H1"))
                                            {
                                                excelWorksheet.Cells[25, 6].Value += " " + reader.GetValue(reader.GetOrdinal("subtipo_resultado_2_proceso_" + i.ToString())).ToString().ToUpper();
                                                excelWorksheet.Cells[25, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_subtype_2_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CT_subtype_2_proceso_" + i.ToString())).ToString() : "";
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[30, 6].Value += " POSITIVO,";
                                                    excelWorksheet.Cells[30, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                             if (reader.GetValue(reader.GetOrdinal("subtipo_2_proceso_" + i.ToString())).ToString().Contains("H3"))
                                            {
                                                excelWorksheet.Cells[26, 6].Value += " " + reader.GetValue(reader.GetOrdinal("subtipo_resultado_2_proceso_" + i.ToString())).ToString().ToUpper();
                                                excelWorksheet.Cells[26, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_subtype_2_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CT_subtype_2_proceso_" + i.ToString())).ToString() : "";
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[31, 6].Value += " POSITIVO,";
                                                    excelWorksheet.Cells[31, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_subtype_2_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }

                                            //excelWorksheet.Cells[27, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[27, 6].Value += " " + "POSITIVO,";
                                            excelWorksheet.Cells[27, 15].Value += (reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[27, 15].Value != "" && excelWorksheet.Cells[27, 15].Value != null) ? ", " : " ") +  reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() : "";
                                            if (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "")
                                            {
                                                excelWorksheet.Cells[32, 6].Value += " POSITIVO,";
                                                excelWorksheet.Cells[32, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[32, 15].Value != "" && excelWorksheet.Cells[32, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() : "";
                                            }
                                            if (reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString() == "Negativo")
                                            {
                                                //excelWorksheet.Cells[23, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[27, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[34, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[32, 6].Value += " POSITIVO";
                                                //excelWorksheet.Cells[32, 15].Value += " " + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString();
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[33, 6].Value += " NEGATIVO,";
                                                    excelWorksheet.Cells[33, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                            //excelWorksheet.Cells[26, 4].Value = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString());
                                        } else if (reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza B")
                                        {
                                            excelWorksheet.Cells[24, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[24, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[24, 15].Value != "" && excelWorksheet.Cells[24, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() : "";
                                            if (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "")
                                            {
                                                excelWorksheet.Cells[29, 6].Value += " POSITIVO,";
                                                excelWorksheet.Cells[29, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "") ? reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() : "";
                                            }
                                            //excelWorksheet.Cells[27, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[27, 6].Value += " " + " POSITIVO," ;
                                            excelWorksheet.Cells[27, 15].Value += (reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[27, 15].Value != "" && excelWorksheet.Cells[27, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() : "";
                                            if (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "")
                                            {
                                                excelWorksheet.Cells[32, 6].Value += " POSITIVO,";
                                                excelWorksheet.Cells[32, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[32, 15].Value != "" && excelWorksheet.Cells[32, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() : "";
                                            }
                                            if (reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString() == "Negativo")
                                            {
                                                //excelWorksheet.Cells[23, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[27, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[34, 6].Value += " NEGATIVO";
                                                //excelWorksheet.Cells[32, 6].Value += " POSITIVO";
                                                //excelWorksheet.Cells[32, 15].Value += " " + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString();
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[33, 6].Value += " NEGATIVO,";
                                                    excelWorksheet.Cells[33, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[33, 15].Value != "" && excelWorksheet.Cells[33, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                        }
                                        else if (!(reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza B" && reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza A"))
                                        {
                                            excelWorksheet.Cells[34, 6].Value += ((excelWorksheet.Cells[34, 6].Value != "" && excelWorksheet.Cells[34, 6].Value != null) ? " , " : " ") + " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper() + " - " + reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[34, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[34, 15].Value != "" && excelWorksheet.Cells[34, 15].Value != null) ? " , " : " ") + reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() : "";
                                            excelWorksheet.Cells[27, 6].Value +=  " POSITIVO,";
                                            //((excelWorksheet.Cells[27, 6].Value != "" && excelWorksheet.Cells[27, 6].Value != null) ? ", " : " ") // Modificacion segun requerimiento Rodrigo Chile
                                            excelWorksheet.Cells[27, 15].Value += (reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[27, 15].Value != "" && excelWorksheet.Cells[27,15].Value != null) ? ", " : " ")  + reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() : "" ;
                                            excelWorksheet.Cells[32, 6].Value += ((excelWorksheet.Cells[32, 6].Value != "" && excelWorksheet.Cells[32, 6].Value != null) ? " , " : " ") + " POSITIVO,";
                                            excelWorksheet.Cells[32, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "") ?  ((excelWorksheet.Cells[32, 15].Value != "" && excelWorksheet.Cells[32, 15].Value != null) ? " , " : " ") + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() : "";

                                            if (reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString() == "Negativo")
                                            {
                                                if (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "")
                                                {
                                                    excelWorksheet.Cells[33, 6].Value += " NEGATIVO,";
                                                    excelWorksheet.Cells[33, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "") ? ", " + reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                        }

                                        //excelWorksheet.Cells[27, 15].Value += " " + reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString();
                                        excelWorksheet.Cells[22, 6].Value += " " + ((excelWorksheet.Cells[22, 6].Value != "" && excelWorksheet.Cells[22, 6].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString() ;
                                        isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("fecha_fin_proceso_" + i.ToString())).ToString(), out DateTransform_);
                                        excelWorksheet.Cells[35, 15].Value = DateTransform_;
                                    }
                                }
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
    }
}
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
using Microsoft.AspNet;
using Microsoft.AspNet.Identity;

namespace Paho.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    public class PrintTestController : ControllerBase
    {
        // GET: ExportLab
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPrint(string Report, int CountryID, int? InstitutionID, int RecordID, int? NumSample)
        {
            try
            {
                var ms = new MemoryStream();
                var pdfs = new MemoryStream();
                //var user = UserManager.FindById(User.Identity.GetUserId());
                int CountryID_ = CountryID;
                int? HospitalID_ = InstitutionID ?? 0;
                int? RecordID_ = RecordID;
                int? NumSample_ = NumSample;
                string AppSetings_form = (Report == "Cases" ) ? "FormRecordTemplate" : "PathPrintTest";
                string Path_Print = ConfigurationManager.AppSettings[AppSetings_form];
                string Sample_Print_ = "Imp_Mue_" + RecordID_.ToString() + "_" + NumSample_.ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm");
                string Record_Print_ = "PAHOFLU_Record_" + RecordID_.ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm");


                if (Report == "" )
                {
                    ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
                    return null;
                }
                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings[AppSetings_form]
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

                try
                {

                    Workbook workbook = new Workbook();

                    workbook.LoadFromStream(ms);
                    //workbook.SaveToFile(Path_Print + Sample_Print_ + ".pdf", Spire.Xls.FileFormat.PDF);
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

        private static bool HasColumn(SqlDataReader Reader, string ColumnName)
        {
            foreach (DataRow row in Reader.GetSchemaTable().Rows)
            {
                if (row["ColumnName"].ToString() == ColumnName)
                    return true;
            } //Still here? Column not found. 
            return false;
        }

        private void AssignValueCell(int row_, int col_,  ExcelWorksheet excelWorksheet, SqlDataReader reader_, string  field_name )
        {
            bool hasMyColumn = (reader_.GetSchemaTable().Select("ColumnName = '"+field_name+"'").Length > 0);
            if (hasMyColumn == true)
            {
                var hasMyColumnType = reader_.GetValue(reader_.GetOrdinal(field_name)).GetType().Name;
                if (hasMyColumnType == "String")
                { excelWorksheet.Cells[row_, col_].Value = reader_.GetValue(reader_.GetOrdinal(field_name)).ToString() == "NULL" ? "" : reader_.GetValue(reader_.GetOrdinal(field_name)).ToString(); }
                else if (hasMyColumnType == "DateTime")
                {
                    DateTime DateTransform_;
                    bool isDate_ = DateTime.TryParse(reader_.GetValue(reader_.GetOrdinal(field_name)).ToString(), out DateTransform_);
                    excelWorksheet.Cells[row_, col_].Value = DateTransform_;
                }
                else if (hasMyColumnType == "Int32")
                {
                    excelWorksheet.Cells[row_, col_].Value = reader_.GetValue(reader_.GetOrdinal(field_name)).ToString();
                }
            }
            
        }

        private void AssignValueCell_value(int row_, int col_, ExcelWorksheet excelWorksheet, string value_cell, string type_value)
        {

                var hasMyColumnType = type_value;
                if (hasMyColumnType == "String")
                { excelWorksheet.Cells[row_, col_].Value = value_cell; }
                else if (hasMyColumnType == "DateTime")
                {
                    DateTime DateTransform_;
                    bool isDate_ = DateTime.TryParse(value_cell, out DateTransform_);
                    excelWorksheet.Cells[row_, col_].Value = DateTransform_;
                }
                else if (hasMyColumnType == "Int32")
                {
                    excelWorksheet.Cells[row_, col_].Value = value_cell;
                }

        }

        private void MarkCell(int row_, int col_, ExcelWorksheet excelWorksheet)
        {

            excelWorksheet.Cells[row_, col_].Style.Border.Diagonal.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            excelWorksheet.Cells[row_, col_].Style.Border.DiagonalUp = true;
            excelWorksheet.Cells[row_, col_].Style.Border.DiagonalDown = true;

        }
        private void Record_information_assign(ExcelWorksheet excelWorksheet, SqlDataReader reader_)
        {
            AssignValueCell(2, 2, excelWorksheet, reader_, "Id");
        }

        private void AppendDataToExcel(int countryId, int? recordID, int? NumSample, int? hospitalId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            var excelWorksheet = excelWorkBook.Worksheets[sheet];
            var row = startRow;
            var column = startColumn;
            string Languaje_ = user.Institution.Country.Language ?? "SPA";

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Languaje", SqlDbType.Text).Value = Languaje_;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    command.Parameters.Add("@RecordID", SqlDbType.Int).Value = recordID;
                    if (storedProcedure == "ExportLab")
                        command.Parameters.Add("@Number_Sample", SqlDbType.Int).Value = NumSample;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (storedProcedure == "ExportLab") { 
                            var col = column;
                            //excelWorksheet.Cells[7, 15].Value = reader.GetValue(reader.GetOrdinal("Id")).ToString();
                            AssignValueCell(7, 15, excelWorksheet, reader, "Id");
                            //Fila 8
                            excelWorksheet.Cells[8, 9].Value = reader.GetValue(reader.GetOrdinal("NameComplete")).ToString();
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
                                if (reader.GetValue(reader.GetOrdinal("procesado_proceso_" + i.ToString())).ToString() != "")
                                {
                                    if (reader.GetValue(reader.GetOrdinal("tipo_proceso_proceso_" + i.ToString())).ToString() == "IF")
                                    {
                                        //excelWorksheet.Cells[35, 6].Value = reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString();

                                        excelWorksheet.Cells[36, 6].Value += reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper() + ((reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() != "") ? " - " + reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString().ToUpper() + " , " : ", ");

                                        isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("fecha_fin_proceso_" + i.ToString())).ToString(), out DateTransform_);
                                        excelWorksheet.Cells[37, 4].Value = DateTransform_;
                                        excelWorksheet.Cells[38, 6].Value += ((excelWorksheet.Cells[38, 6].Value != "" && excelWorksheet.Cells[38, 6].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString();
                                    }

                                    if (reader.GetValue(reader.GetOrdinal("tipo_proceso_proceso_" + i.ToString())).ToString() == "PCR")
                                    {
                                        if (reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza A")
                                        {
                                            excelWorksheet.Cells[23, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
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
                                                    excelWorksheet.Cells[33, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() != "") ? " " + reader.GetValue(reader.GetOrdinal("CTRL_Negative_proceso_" + i.ToString())).ToString() : "";
                                                }
                                            }
                                            //excelWorksheet.Cells[26, 4].Value = Convert.ToDouble(reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString());
                                        }
                                        else if (reader.GetValue(reader.GetOrdinal("virus_proceso_" + i.ToString())).ToString() == "Influenza B")
                                        {
                                            excelWorksheet.Cells[24, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[24, 15].Value += (reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[24, 15].Value != "" && excelWorksheet.Cells[24, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("CT_virus_proceso_" + i.ToString())).ToString() : "";
                                            if (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "")
                                            {
                                                excelWorksheet.Cells[29, 6].Value += " POSITIVO,";
                                                excelWorksheet.Cells[29, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() != "") ? reader.GetValue(reader.GetOrdinal("CTRL_virus_proceso_" + i.ToString())).ToString() : "";
                                            }
                                            //excelWorksheet.Cells[27, 6].Value += " " + reader.GetValue(reader.GetOrdinal("resultado_proceso_" + i.ToString())).ToString().ToUpper();
                                            excelWorksheet.Cells[27, 6].Value += " " + " POSITIVO,";
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
                                            excelWorksheet.Cells[27, 6].Value += " POSITIVO,";
                                            //((excelWorksheet.Cells[27, 6].Value != "" && excelWorksheet.Cells[27, 6].Value != null) ? ", " : " ") // Modificacion segun requerimiento Rodrigo Chile
                                            excelWorksheet.Cells[27, 15].Value += (reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[27, 15].Value != "" && excelWorksheet.Cells[27, 15].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("RNP_proceso_" + i.ToString())).ToString() : "";
                                            excelWorksheet.Cells[32, 6].Value += ((excelWorksheet.Cells[32, 6].Value != "" && excelWorksheet.Cells[32, 6].Value != null) ? " , " : " ") + " POSITIVO,";
                                            excelWorksheet.Cells[32, 15].Value += (reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() != "") ? ((excelWorksheet.Cells[32, 15].Value != "" && excelWorksheet.Cells[32, 15].Value != null) ? " , " : " ") + reader.GetValue(reader.GetOrdinal("CTRL_RNP_proceso_" + i.ToString())).ToString() : "";

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
                                        excelWorksheet.Cells[22, 6].Value += " " + ((excelWorksheet.Cells[22, 6].Value != "" && excelWorksheet.Cells[22, 6].Value != null) ? ", " : " ") + reader.GetValue(reader.GetOrdinal("lab_proceso_" + i.ToString())).ToString();
                                        isDate_ = DateTime.TryParse(reader.GetValue(reader.GetOrdinal("fecha_fin_proceso_" + i.ToString())).ToString(), out DateTransform_);
                                        excelWorksheet.Cells[35, 15].Value = DateTransform_;
                                    }
                                }
                            }

                            //row++;

                        }
                            // Empieza la impresion de la ficha fisica
                            else if (storedProcedure == "Cases"){
                                // Datos de vigilancia
                                AssignValueCell(4, 30, excelWorksheet, reader, "Establecimiento");  // Establecimiento
                                AssignValueCell(5, 9, excelWorksheet, reader, "region_MINSA"); // Region Salud
                                AssignValueCell(5, 30, excelWorksheet, reader, "region_CCSS"); // Region Institucional
                                AssignValueCell(6, 9, excelWorksheet, reader, "Fecha_notificacion"); // Fecha de notificacion
                                AssignValueCell(6, 30, excelWorksheet, reader, "Fecha_registro"); // Fecha registro
                                AssignValueCell(7, 9, excelWorksheet, reader, "tipo_identificacion"); // Tipo de identificacion
                                AssignValueCell(7, 31, excelWorksheet, reader, "Doc_identificacion"); // Numero de identificacion
                                AssignValueCell(8, 31, excelWorksheet, reader, "Registro_hospital"); // Numero de identificacion ENG
                                AssignValueCell(8, 9, excelWorksheet, reader, "Doc_identificacion"); // Numero de identificacion ENG
                                AssignValueCell(9, 6, excelWorksheet, reader, "Apellido1"); // Apellido 1
                                AssignValueCell(9, 31, excelWorksheet, reader, "Nombre1"); // Nombre 1
                                AssignValueCell(10, 6, excelWorksheet, reader, "Apellido2"); // Apellido 2
                                AssignValueCell(10, 31, excelWorksheet, reader, "Nombre2"); // Nombre 2
                                AssignValueCell(11, 9, excelWorksheet, reader, "Fecha_Nacimiento"); // Fecha de nacimiento
                                AssignValueCell(11, 31, excelWorksheet, reader, "Edad"); // Fecha de nacimiento
                                AssignValueCell(11, 41, excelWorksheet, reader, "Tipo_edad"); // Fecha de nacimiento
                                AssignValueCell(12, 9, excelWorksheet, reader, "Grupo_Edad"); // Grupo de edad
                                 if (reader.GetValue(reader.GetOrdinal("Sexo")).ToString()  == "Femenino")
                                {
                                    AssignValueCell_value(12, 41, excelWorksheet, "X", "String");
                                }
                                else if (reader.GetValue(reader.GetOrdinal("Sexo")).ToString() == "Masculino")
                                {
                                    AssignValueCell_value(12, 36, excelWorksheet, "X", "String");
                                }
                                else if (reader.GetValue(reader.GetOrdinal("Sexo")).ToString() == "Desconocido")
                                {
                                    AssignValueCell_value(12, 46, excelWorksheet, "X", "String");
                                }

                                // Referencia geografica
                                AssignValueCell(19, 9, excelWorksheet, reader, "Pais"); // Pais
                                AssignValueCell(19, 31, excelWorksheet, reader, "Provincia"); // Regíón / Departamento / Provincia
                                AssignValueCell(20, 9, excelWorksheet, reader, "Canton"); // Provincia / Municipio / Cantón
                                AssignValueCell(20, 31, excelWorksheet, reader, "Distrito"); // Comuna / (Zona/Comunidad) / Distrito
                                // Falta Pais de procedencia - 21
                                // Falta Urbana y Rural - 21
                                AssignValueCell(22, 9, excelWorksheet, reader, "Direccion"); // Direccion
                                AssignValueCell(22, 31, excelWorksheet, reader, "Telefono"); // Telefono
                                AssignValueCell(23, 9, excelWorksheet, reader, "Latitud"); // Latitud
                                AssignValueCell(23, 31, excelWorksheet, reader, "Longitud"); // Longitud

                                // País dos semanas antes - 25
                                // Departamento dos semanas antes - 25
                                // Municipio - 26
                                // Zona / Comunidad - 26

                                AssignValueCell(23, 9, excelWorksheet, reader, "Latitud"); // Latitud
                                AssignValueCell(23, 31, excelWorksheet, reader, "Longitud"); // Longitud
                                
                                // Embarazada

                                if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "YES")
                                { MarkCell(29, 11,excelWorksheet); }
                                else if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "NO")
                                { MarkCell(29, 15, excelWorksheet); }
                                else if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                { MarkCell(29, 22, excelWorksheet); }

                                AssignValueCell(29, 31, excelWorksheet, reader, "Puerperio"); // Puerperio
                                AssignValueCell(29, 35, excelWorksheet, reader, "Trimestre"); // Trimestre de embarazo

                                // Vacunas
                                AssignValueCell(42, 12, excelWorksheet, reader, "Fuente_vacuna"); // Fuente de la vacuna
                                




                            }
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
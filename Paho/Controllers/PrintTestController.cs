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
using Paho.Models;

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
        public ActionResult GetPrint(string Report, int? CountryID, int? InstitutionID, int? RecordID, int? NumSample)
        {
            try
            {
                var ms = new MemoryStream();
                var pdfs = new MemoryStream();
                //var user = UserManager.FindById(User.Identity.GetUserId());
                int? CountryID_ = CountryID;
                int? HospitalID_ = InstitutionID ?? 0;
                int? RecordID_ = RecordID;
                int? NumSample_ = NumSample;
                string AppSetings_form = (Report == "Cases" ) ? "FormRecordTemplate" : "PrintTestTemplate";
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
                //this code segment write data to file.
                //FileStream fs1 = new FileStream("C:\\PrintTestController.txt", FileMode.OpenOrCreate, FileAccess.Write);
                //StreamWriter writer = new StreamWriter(fs1);
                //writer.Write(e.Message + " \n " + e.Source);
                //writer.Close();
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
                else
                { excelWorksheet.Cells[row_, col_].Value = value_cell; }

        }

        private void MarkCell(int row_, int col_, ExcelWorksheet excelWorksheet)
        {

            excelWorksheet.Cells[row_, col_].Style.Border.Diagonal.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            excelWorksheet.Cells[row_, col_].Style.Border.DiagonalUp = true;
            excelWorksheet.Cells[row_, col_].Style.Border.DiagonalDown = true;

        }

        private void CopyRange(string Range_origin, string Range_destin, ExcelWorksheet excelWorksheet)
        {
            excelWorksheet.Cells[Range_origin].Copy(excelWorksheet.Cells[Range_destin]);
        }

        private void Record_information_assign(ExcelWorksheet excelWorksheet, SqlDataReader reader_)
        {
            AssignValueCell(2, 2, excelWorksheet, reader_, "Id");
        }

        private void AppendDataToExcel(int? countryId, int? recordID, int? NumSample, int? hospitalId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row)
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
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Vigilancia'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Vigilancia")).ToString().ToUpper() == "IRAG" || reader.GetValue(reader.GetOrdinal("Vigilancia")).ToString().ToUpper() == "SARI")
                                    { MarkCell(4, 4, excelWorksheet); } // Tipo de vigilancia IRAG

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Vigilancia'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Vigilancia")).ToString().ToUpper() == "ETI" || reader.GetValue(reader.GetOrdinal("Vigilancia")).ToString().ToUpper() == "ILI")
                                    { MarkCell(4, 9, excelWorksheet); } // Tipo de vigilancia ETI

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Inusitada'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Inusitada")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Inusitada")).ToString().ToUpper() == "YES")
                                    { MarkCell(4, 9, excelWorksheet); } // Si el caso pertenece al a vigilancia inucitada

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Brote'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Brote")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Brote")).ToString().ToUpper() == "YES")
                                    { MarkCell(4, 9, excelWorksheet); } // Si el caso pertenece a un brote

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

                                // Falta País dos semanas antes - 25
                                // Falta Departamento dos semanas antes - 25
                                // Falta Municipio - 26
                                // Falta Zona / Comunidad - 26

                                AssignValueCell(23, 9, excelWorksheet, reader, "Latitud"); // Latitud
                                AssignValueCell(23, 31, excelWorksheet, reader, "Longitud"); // Longitud
                                
                                // Embarazada
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Embarazada'").Length > 0)) {
                                    if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "YES")
                                    { MarkCell(29, 11,excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "NO")
                                    { MarkCell(29, 15, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("Embarazada")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                    { MarkCell(29, 22, excelWorksheet); }
                                }

                                AssignValueCell(29, 31, excelWorksheet, reader, "Puerperio"); // Puerperio
                                AssignValueCell(29, 35, excelWorksheet, reader, "Trimestre"); // Trimestre de embarazo

                                // Vacunas
                                AssignValueCell(42, 12, excelWorksheet, reader, "Fuente_vacuna"); // Fuente de la vacuna

                                //Influenza
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Influenza'").Length > 0))
                                {
                                    if (reader.GetValue(reader.GetOrdinal("Influenza")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Influenza")).ToString().ToUpper() == "YES")
                                    { MarkCell(43, 9, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Influenza")).ToString().ToUpper() == "NO")
                                    { MarkCell(43, 11, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Influenza")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("Influenza")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                    { MarkCell(43, 13, excelWorksheet); }
                                }
                                AssignValueCell(43, 27, excelWorksheet, reader, "Fecha_primera_dosis"); // Fecha de la primera dosis de influenza
                                AssignValueCell(43, 41, excelWorksheet, reader, "Fecha_segunda_dosis"); // Fecha de la segunda dosis de influenza

                                //Neumococo
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Neumococo'").Length > 0))
                                {
                                    if (reader.GetValue(reader.GetOrdinal("Neumococo")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Neumococo")).ToString().ToUpper() == "YES")
                                    { MarkCell(43, 9, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Neumococo")).ToString().ToUpper() == "NO")
                                    { MarkCell(43, 11, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Neumococo")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("Neumococo")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                    { MarkCell(43, 13, excelWorksheet); }
                                }
                                AssignValueCell(43, 27, excelWorksheet, reader, "Neumococo_dosis"); // Fecha de la primera dosis de influenza
                                AssignValueCell(43, 41, excelWorksheet, reader, "Neumococo_fecha"); // Fecha de la segunda dosis de influenza


                                // Antiviral
                                if ((reader.GetSchemaTable().Select("ColumnName = 'AntiViral'").Length > 0))
                                {
                                    if (reader.GetValue(reader.GetOrdinal("AntiViral")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("AntiViral")).ToString().ToUpper() == "YES")
                                    { MarkCell(49, 9, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("AntiViral")).ToString().ToUpper() == "NO")
                                    { MarkCell(49, 11, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("AntiViral")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("AntiViral")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                    { MarkCell(49, 13, excelWorksheet); }
                                }
                                AssignValueCell(49, 27, excelWorksheet, reader, "Antiviral_fecha"); // Fecha de antiviral
                                AssignValueCell(50, 9, excelWorksheet, reader, "Antiviral_tipo"); // Tipo de antiviral
                                                                                                  // Falta dosis de antiviral

                                // Factores de riesgo
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Factores_riesgo'").Length > 0))
                                {
                                    if (reader.GetValue(reader.GetOrdinal("Factores_riesgo")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Factores_riesgo")).ToString().ToUpper() == "YES")
                                    { MarkCell(52, 11, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Factores_riesgo")).ToString().ToUpper() == "NO")
                                    { MarkCell(52, 15, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Factores_riesgo")).ToString().ToUpper() == "NO DATA" || reader.GetValue(reader.GetOrdinal("Factores_riesgo")).ToString().ToUpper() == "SIN INFORMACIÓN")
                                    { MarkCell(52, 22, excelWorksheet); }
                                }

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Tabaquismo'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Tabaquismo")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Tabaquismo")).ToString().ToUpper() == "YES")
                                    { MarkCell(53, 11, excelWorksheet); } // Tabaquismo

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Alcoholismo'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Alcoholismo")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Alcoholismo")).ToString().ToUpper() == "YES")
                                    { MarkCell(53, 11, excelWorksheet); } // Alcoholismo

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Trab_salud'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Trab_salud")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Trab_salud")).ToString().ToUpper() == "YES")
                                    { MarkCell(53, 33, excelWorksheet); } // Trabajador de salud

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Sindrome_down'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Sindrome_down")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Sindrome_down")).ToString().ToUpper() == "YES")
                                    { MarkCell(53, 11, excelWorksheet); } // Sindrome_down

                                // Comorbilidades
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Cardiopatia_cronica'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Cardiopatia_cronica")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Cardiopatia_cronica")).ToString().ToUpper() == "YES")
                                    { MarkCell(57, 11, excelWorksheet); } // Cardiopatia_cronica

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Diabetes'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Diabetes")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Diabetes")).ToString().ToUpper() == "YES")
                                    { MarkCell(57, 22, excelWorksheet); } // Diabetes

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Neurologica_cronica'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Neurologica_cronica")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Neurologica_cronica")).ToString().ToUpper() == "YES")
                                    { MarkCell(57, 33, excelWorksheet); } // Neurologica_cronica

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Asma'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Asma")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Asma")).ToString().ToUpper() == "YES")
                                    { MarkCell(57, 45, excelWorksheet); } // Asma

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Pulmonar_cronica'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Pulmonar_cronica")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Pulmonar_cronica")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 11, excelWorksheet); } // Pulmonar_cronica

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Hepatica_cronica'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Hepatica_cronica")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Hepatica_cronica")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 22, excelWorksheet); } // Hepatica_cronica

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Renal_cronica'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Renal_cronica")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Renal_cronica")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 33, excelWorksheet); } // Renal_cronica

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Inmuno_enf_trat'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Inmuno_enf_trat")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Inmuno_enf_trat")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 45, excelWorksheet); } // Inmuno_enf_trat

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Paralisis_cerebral'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Paralisis_cerebral")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Paralisis_cerebral")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 11, excelWorksheet); } // Paralisis_cerebral

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Indigena'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Indigena")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Indigena")).ToString().ToUpper() == "YES")
                                    { MarkCell(58, 22, excelWorksheet); } // Indigena

                                AssignValueCell(60, 11, excelWorksheet, reader, "Obesidad"); // Obesidad

                                AssignValueCell(60, 33, excelWorksheet, reader, "Otros"); // Otra comorbilidad

                                // Informacion clinica

                                AssignValueCell(64, 9, excelWorksheet, reader, "Inicio_sintomas"); // Fecha de inicio de sintomas
                                AssignValueCell(64, 31, excelWorksheet, reader, "SE_ini_sin"); // Semana de Fecha de inicio de sintomas
                                AssignValueCell(64, 39, excelWorksheet, reader, "Year"); // Anio de Fecha de inicio de sintomas

                                AssignValueCell(64, 9, excelWorksheet, reader, "Fecha_diag"); // Fecha de diagnostico
                                AssignValueCell(64, 31, excelWorksheet, reader, "SE_diag"); // Semana de diagnostico
                                                                                            // Sintomatologia

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Fiebre_Historiafiebre'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Fiebre_Historiafiebre")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Fiebre_Historiafiebre")).ToString().ToUpper() == "YES")
                                    { MarkCell(67, 11, excelWorksheet); } // Fiebre_Historiafiebre

                                // Falta temperatura
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Tos'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Tos")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Tos")).ToString().ToUpper() == "YES")
                                    { MarkCell(67, 33, excelWorksheet); } // Tos

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Dif_resp'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Dif_resp")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Dif_resp")).ToString().ToUpper() == "YES")
                                    { MarkCell(67, 45, excelWorksheet); } // Difucultad respiratoria

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Med_Sat_Oxig'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Med_Sat_Oxig")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Med_Sat_Oxig")).ToString().ToUpper() == "YES")
                                    { MarkCell(68, 11, excelWorksheet); } // Medicion de saturacion de oxigeno

                                AssignValueCell(68, 22, excelWorksheet, reader, "Sat_Oxig_Porc"); // Porcentaje en numero de la saturacion de oxigeno

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Odinofagia'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Odinofagia")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Odinofagia")).ToString().ToUpper() == "YES")
                                    { MarkCell(68, 33, excelWorksheet); } // Odinofagia

                                AssignValueCell(64, 9, excelWorksheet, reader, "Salon"); // Servicio donde atienden al paciente
                                AssignValueCell(64, 33, excelWorksheet, reader, "Diag_ing_CIE10"); // Diagnostico de ingreso
                                                                                                   //Falta otro diagnostico de egreso
                                AssignValueCell(64, 33, excelWorksheet, reader, "Diag_egreso"); //Diagnostico de egreso rel. IRAG

                                //Hospitalizacion
                                AssignValueCell(78, 9, excelWorksheet, reader, "hosp_ing_fecha"); // Fecha de ingreso a hospitalizacion
                                AssignValueCell(78, 30, excelWorksheet, reader, "Hosp_egre_fecha"); // Fecha de egreso de hospitalizacion
                                AssignValueCell(79, 9, excelWorksheet, reader, "cond_egreso"); // Fecha de egreso de hospitalizacion
                                AssignValueCell(79, 30, excelWorksheet, reader, "FalleDate"); // Fecha de egreso de hospitalizacion
                                
                                // UCI
                                AssignValueCell(81, 9, excelWorksheet, reader, "UCI_ing_fecha"); // Fecha de ingreso a hospitalizacion
                                AssignValueCell(81, 30, excelWorksheet, reader, "UCI_egre_fecha"); // Fecha de egreso de hospitalizacion

                                 

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Hallazgo_rad'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Hallazgo_rad")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Hallazgo_rad")).ToString().ToUpper() == "YES")
                                    { MarkCell(83, 11, excelWorksheet); } // Hallazgos radiologicos

                                if ((reader.GetSchemaTable().Select("ColumnName = 'Vent_mec'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("Vent_mec")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Vent_mec")).ToString().ToUpper() == "YES")
                                    { MarkCell(83, 22, excelWorksheet); } // Ventilacion mecanica
                                                                          //Falta Unidad de cuidados intermedios
                                                                          //Falta Unidad critica
                                                                          //Falta ventilacion mecanica no invasiva

                                 
                                if ((reader.GetSchemaTable().Select("ColumnName = 'ECMO'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("ECMO")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("ECMO")).ToString().ToUpper() == "YES")
                                    { MarkCell(84, 22, excelWorksheet); } // ECMO

                                if ((reader.GetSchemaTable().Select("ColumnName = 'VAFO'").Length > 0))
                                    if (reader.GetValue(reader.GetOrdinal("VAFO")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("VAFO")).ToString().ToUpper() == "YES")
                                    { MarkCell(84, 33, excelWorksheet); } // VAFO

                                // Toma de Muestra
                                if ((reader.GetSchemaTable().Select("ColumnName = 'Muestra_tomada'").Length > 0))
                                {
                                    if (reader.GetValue(reader.GetOrdinal("Muestra_tomada")).ToString().ToUpper() == "SI" || reader.GetValue(reader.GetOrdinal("Muestra_tomada")).ToString().ToUpper() == "YES")
                                    { MarkCell(86, 12, excelWorksheet); }
                                    else if (reader.GetValue(reader.GetOrdinal("Muestra_tomada")).ToString().ToUpper() == "NO")
                                    { MarkCell(86, 16, excelWorksheet); }
                                }

                                AssignValueCell(87, 10, excelWorksheet, reader, "Fecha_muestra"); // Fecha toma de muestra
                                AssignValueCell(87, 34, excelWorksheet, reader, "Tipo_muestra"); // Fecha toma de muestra
                                AssignValueCell(88, 10, excelWorksheet, reader, "Fecha_envio"); // Fecha toma de muestra
                                AssignValueCell(88, 34, excelWorksheet, reader, "Lab_envio"); // Fecha toma de muestra
                                // Muestra 2
                                AssignValueCell(90, 10, excelWorksheet, reader, "Fecha_muestra_2"); // Fecha toma de muestra 2
                                AssignValueCell(90, 34, excelWorksheet, reader, "Tipo_muestra_2"); // Fecha toma de muestra 2
                                AssignValueCell(91, 10, excelWorksheet, reader, "Fecha_envio_2"); // Fecha toma de muestra 2
                                AssignValueCell(91, 34, excelWorksheet, reader, "Lab_envio_2"); // Fecha toma de muestra 2

                                // Muestra 3
                                AssignValueCell(93, 10, excelWorksheet, reader, "Fecha_muestra_3"); // Fecha toma de muestra 3
                                AssignValueCell(93, 34, excelWorksheet, reader, "Tipo_muestra_3"); // Fecha toma de muestra 3
                                AssignValueCell(94, 10, excelWorksheet, reader, "Fecha_envio_3"); // Fecha toma de muestra 3
                                AssignValueCell(94, 34, excelWorksheet, reader, "Lab_envio_3"); // Fecha toma de muestra 3

                                //Conclusion del laboratorio
                                AssignValueCell(96, 10, excelWorksheet, reader, "Res_fin_fecha"); // Fecha del resultado final

                                AssignValueCell(97, 10, excelWorksheet, reader, "Resultado_final_1"); // Resultado
                                AssignValueCell(97, 20, excelWorksheet, reader, "Res_fin_Virus_1"); // Tipo de virus
                                AssignValueCell(97, 32, excelWorksheet, reader, "Res_fin_Subtipo_1"); // Subtipo 
                                AssignValueCell(97, 40, excelWorksheet, reader, "Res_fin_Linaje_1"); // Linaje 

                                AssignValueCell(98, 10, excelWorksheet, reader, "Resultado_final_2"); // Resultado 2
                                AssignValueCell(98, 20, excelWorksheet, reader, "Res_fin_Virus_2"); // Tipo de virus 2
                                AssignValueCell(98, 32, excelWorksheet, reader, "Res_fin_Subtipo_2"); // Subtipo 2
                                AssignValueCell(98, 40, excelWorksheet, reader, "Res_fin_Linaje_2"); // Linaje 2

                                AssignValueCell(99, 10, excelWorksheet, reader, "Resultado_final_3"); // Resultado 3
                                AssignValueCell(99, 20, excelWorksheet, reader, "Res_fin_Virus_3"); // Tipo de virus 3
                                AssignValueCell(99, 32, excelWorksheet, reader, "Res_fin_Subtipo_3"); // Subtipo 3
                                AssignValueCell(99, 40, excelWorksheet, reader, "Res_fin_Linaje_3"); // Linaje 3

                                //Estatus del caso
                                
                                AssignValueCell(101, 10, excelWorksheet, reader, "Estatus_caso"); // Estsado del caso
                                AssignValueCell(101, 31, excelWorksheet, reader, "Caso_cerrado_fecha"); // Fecha de cierre de caso
                                AssignValueCell(102, 10, excelWorksheet, reader, "Observaciones"); // Observaciones del caso


                            }
                        }
                    }
                    command.Parameters.Clear();
                    con.Close();
                    if ((storedProcedure == "Cases"))
                    {
                        FluCase flucase = db.FluCases.Find(recordID);
                        var ListTest = flucase.CaseLabTests;
                        var NumTest = ListTest.Count;
                        var fila_original_empieza = 104;
                        if (ListTest.Count > 1) {
                            var fila_original_termina = 113;
                            var fila_copiar_empieza = 114;
                            var fila_copiar_termina = 121;
                            for (var i = 1; i < ListTest.Count; i++)
                            {

                                if (i > 1)
                                {
                                    //fila_original = fila_original + 10;
                                    fila_copiar_empieza = fila_copiar_empieza + 10;
                                    fila_copiar_termina = fila_copiar_empieza + 8;
                                }
                                //CopyRange("B103:AT113", "B113:AT121", excelWorksheet);
                                CopyRange("B"+fila_original_empieza.ToString()+ ":AT" + fila_original_termina.ToString() , "B" + fila_copiar_empieza.ToString() + ":AT" + fila_copiar_termina.ToString(), excelWorksheet);
                            }

                        }

                        var j = 0;
                        foreach (CaseLabTest Test in ListTest)
                        {
                            j = j + 1;
                            AssignValueCell_value(fila_original_empieza, 10, excelWorksheet, flucase.RecDate.ToString(), "DateTime"); // Fecha de recepcion
                            if ((bool)flucase.Processed)
                                { MarkCell(fila_original_empieza, 36, excelWorksheet); }
                            else
                                { MarkCell(fila_original_empieza, 40, excelWorksheet); }
                            AssignValueCell_value(fila_original_empieza + 1, 10, excelWorksheet, flucase.NoProRen.ToString(), "String"); // Laboratorio que procesa
                            AssignValueCell_value(fila_original_empieza + 2, 10, excelWorksheet, Test.LabID.ToString(), "String"); // Laboratorio que procesa
                            
                        } //Still here? Column not found. 




                    }

                }
            }

            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }
    }
}
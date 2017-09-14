using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Paho.Models;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Drawing;
using System.Xml;
using Newtonsoft.Json;
using System.Globalization;
using System.Xml.Xsl;
using System.Collections;
using System.Web.Script.Serialization;
//using System.Xml.XPath;

namespace Paho.Controllers
{
    [Authorize]
    public class GraphController : ControllerBase
    {
        // GET: Graph
        public ActionResult Index()
        {

            return View();
        }

        public JsonResult GetGraphDataSample(string Report, int CountryID, int? RegionID, int? HospitalID, string Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate, int? ReportCountry, string Graph)
        {
            string jsontext = "{\"graph\":{\"graphTitle\":\"Distribución de las proporciones de hospitalizaciones, admisiones en UCI y fallecidos por IRAG según SE\",\"graphXAxisTitle\":\"Semana Epidemiológica\",\"graphYAxisTitle\":\"Porcentaje\",\"graphData\":{\"graphDataItem\":[{\"semana\":\"2017-1\",\"serie1\":\"1.55\",\"serie2\":\"3.53\",\"serie3\":\"2.06\"},{\"semana\":\"2017-2\",\"serie1\":\"1.94\",\"serie2\":\"2.46\",\"serie3\":\"3.19\"},{\"semana\":\"2017-3\",\"serie1\":\"1.61\",\"serie2\":\"3.40\",\"serie3\":\"4.55\"},{\"semana\":\"2017-4\",\"serie1\":\"1.23\",\"serie2\":\"3.00\",\"serie3\":\"0\"},{\"semana\":\"2017-5\",\"serie1\":\"1.33\",\"serie2\":\"2.14\",\"serie3\":\"3.03\"},{\"semana\":\"2017-6\",\"serie1\":\"1.70\",\"serie2\":\"3.98\",\"serie3\":\"0\"},{\"semana\":\"2017-7\",\"serie1\":\"1.38\",\"serie2\":\"1.84\",\"serie3\":\"1.92\"},{\"semana\":\"2017-8\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-9\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-10\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-11\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-12\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-13\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-14\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-15\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-16\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-17\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-18\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-19\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-20\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-21\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-22\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-23\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-24\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-25\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-26\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-27\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-28\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-29\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-30\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-31\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-32\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-33\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-34\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-35\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-36\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-37\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-38\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-39\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-40\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-41\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-42\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-43\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-44\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-45\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-46\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-47\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-48\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-49\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-50\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-51\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-52\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"},{\"semana\":\"2017-53\",\"serie1\":\"0\",\"serie2\":\"0\",\"serie3\":\"0\"}]},\"graphSeries1Label\":\"% de IRAG\",\"graphSeries2Label\":\"% de IRAG en UCI\",\"graphSeries3Label\":\"% de fallecidos por IRAG\"}}";
            return Json(jsontext);
        }
        
        public JsonResult GetYears(int countryId)
        {
            //string resultGetDataYears = "";
            //var resultGetDataYears;
            List<string> resultGetDataYears = new List<string>();
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("GetDataYears", con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();                    
                    con.Open();
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //resultGetDataYears = reader["DataYear"].ToString().Trim();
                            resultGetDataYears.Add(reader["DataYear"].ToString().Trim());

                        }
                    }
                }
            }
            var json = JsonConvert.SerializeObject(resultGetDataYears);
            return Json(json);
        }

        private static void AppendDataToExcelPrevioCambiosAgeGroup(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? irag_, int?eti_)
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
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = irag_;
                    command.Parameters.Add("@ETI", SqlDbType.Int).Value = eti_;

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
                                        stylerow = row + 1;
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
                                command2.Parameters.Add("@IRAG", SqlDbType.Int).Value = irag_;
                                command2.Parameters.Add("@ETI", SqlDbType.Int).Value = eti_;
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
                                    WebClient wc = new WebClient();
                                    byte[] bytes = wc.DownloadData(imagurl);
                                    MemoryStream ms = new MemoryStream(bytes);
                                    Bitmap newImage = new Bitmap(System.Drawing.Image.FromStream(ms));
                                    excelWorksheet.Row(insertrow).Height = newImage.Height;
                                    var picture = excelWorksheet.Drawings.AddPicture("reportlogo", newImage);
                                    picture.SetPosition(insertrow, -(newImage.Height), insertcol, -100);
                                    //picture.SetPosition(insertrow, 0, insertcol, 0);
                                }
                            }
                        }
                        command.Parameters.Clear();
                        con.Close();
                    }
                }

            }

        }
        private static void AppendDataToExcel(string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row, int? ReportCountry, int? irag_, int? eti_)
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
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = irag_;
                    command.Parameters.Add("@ETI", SqlDbType.Int).Value = eti_;

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
                                        stylerow = row + 1;
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
                                command2.Parameters.Add("@IRAG", SqlDbType.Int).Value = irag_;
                                command2.Parameters.Add("@ETI", SqlDbType.Int).Value = eti_;
                                var con2 = new SqlConnection(consString);
                                con2.Open();

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    //row = 212;
                                    int nAnDa = 0;
                                    if (countryId == 25)
                                    {
                                        row = row - 1 + (9 * 3) + 15;
                                        nAnDa = 8 * 8;              // 8: Nº Age Group
                                    }
                                    else
                                    {
                                        row = 212;
                                        nAnDa = 6 * 8;              // 6: Nº Age Group
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

        }

        public JsonResult GetGraphDataTuned(string Report, int CountryID, int? RegionID, int? StateID, int? HospitalID, string Year, int? Month, int? SE, DateTime? StartDate, DateTime? EndDate, int? ReportCountry, string Graph, int? IRAG, int? ETI)
        {
            try
            {
                //var ms = new MemoryStream();
                var user = UserManager.FindById(User.Identity.GetUserId());
                //int CountryID_ = (CountryID >= 0) ? CountryID : (user.Institution.CountryID ?? 0);
                int CountryID_ = CountryID;
                //int? HospitalID_ = (user.Institution.Father_ID > 0 || user.Institution.Father_ID == null) ? HospitalID : Convert.ToInt32(user.Institution.ID);
                int? HospitalID_ = HospitalID;
                //int? RegionID_ = (RegionID >= 0) ? RegionID : (user.Institution.cod_region_institucional ?? 0);
                int? RegionID_ = RegionID;
                //int? StateID_ = (StateID > 0) ? StateID : 0;
                int? StateID_ = StateID;
                string Languaje_ = user.Institution.Country.Language ?? "SPA";
                int? ETI_ = ETI;
                int? IRAG_ = IRAG;
                //################################################################# DESARROLLO
                /*if (Graph == "Graph1" && CountryID == 25)
                {
                    string cGraph1JS = "";
                    return Json(cGraph1JS); ;
                }*/
                //################################################################# END DESARROLLO
                //variable para armar los datos en un XML
                //cada xml corresponde a cada gráfica
                //la idea es utilizar la generación del excel de una vez para generar los datos de las dema's graficas
                XmlDocument myXmlDoc0 = new XmlDocument();
                XmlDocument myXmlDoc1 = new XmlDocument();
                XmlDocument myXmlDoc2 = new XmlDocument();
                XmlDocument myXmlDoc3 = new XmlDocument();
                XmlDocument myXmlDoc4 = new XmlDocument();
                XmlDocument myXmlDoc5 = new XmlDocument();

                //Tratamiento de como pueden venir los valores del año
                if (Year == "")
                {
                    DateTime fechaDefault = DateTime.Now;
                    Year = fechaDefault.Year.ToString();
                }
                string[] years = Year.ToString().Split(',');
                Array.Sort(years, 0, years.Length);
                Year = string.Join(",", years);
                //Fin tratamiento año

                //Inicio de revisión si los datos para la gráfica, año, hospital, etc. existen o no
                string resultGetGraphData = "";
                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand("GetGraphData", con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = CountryID_;
                        command.Parameters.Add("@Graph", SqlDbType.Text).Value = Graph;
                        command.Parameters.Add("@Year", SqlDbType.Text).Value = Year;
                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = RegionID_;
                        command.Parameters.Add("@State_ID", SqlDbType.Int).Value = StateID_;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID_;
                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = IRAG_;
                        command.Parameters.Add("@ETI", SqlDbType.Int).Value = ETI_;
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultGetGraphData = reader["GraphData"].ToString().Trim();

                            }
                        }
                    }
                }
                //Fin de la revisión de si existen los datos para la gráfica

                //Si hay datos de la gráfica solicitada, se regresan esos datos
                if(resultGetGraphData != "")
                {
                    return Json(resultGetGraphData);
                }
                else//si no hay datos, se generará un reporte,y de su Excel resultante se obtendrá la data que se meterá en la base de datos
                {
                    string spSelected = "";
                    switch (Graph)
                    {
                        case "Graph0"://esta servirá para la tabla de antecedentes                          
                        case "Graph1":
                        case "Graph2":
                        case "Graph3":
                        case "Graph4":
                        case "Graph5":
                        case "Graph6":
                            spSelected = "R6";
                            break;
                        case "Graph7":                  //#### CAFQ
                            string jsonTextLB = "";
                            bool yrConversion = false;
                            int yrInt = 0;
                            yrConversion = Int32.TryParse(Year, out yrInt);

                            jsonTextLB = graficoLineasBasales(Languaje_, CountryID_, yrInt, HospitalID_, null, null);
                            return Json(jsonTextLB);
                        case "Graph8":                  // CAFQ: Indicadores Desempeño
                            bool yrConversion1 = false;
                            int yrInt1 = 0;
                            yrConversion1 = Int32.TryParse(Year, out yrInt1);

                            List<String> datosID = new List<string>();

                            graficoIndicadoreDesempenio(Languaje_, CountryID_, yrInt1, HospitalID_, null, null, datosID);

                            var jsonSerialiser = new JavaScriptSerializer();
                            var jsonIndicDesem = jsonSerialiser.Serialize(datosID);

                            return Json(jsonIndicDesem);
                        case "Graph9":                              // CAFQ: ETI1
                            string jsonTextETI = "";

                            jsonTextETI = graficoETINumeroCasos(CountryID_, years, HospitalID_);
                            return Json(jsonTextETI);
                        case "Graph10":                             // CAFQ: ETI2
                            string jsonTextETIPosi = "";

                            jsonTextETIPosi = graficoETINumeroCasosPositivos(CountryID_, years, HospitalID_);
                            return Json(jsonTextETIPosi);
                        case "Graph11":                             // CAFQ: Fallecidos IRAG x GE
                            string jsonTextIRAGGE = "";

                            jsonTextIRAGGE = graficoIRAGxGrupoEdad(CountryID_, years, HospitalID_);
                            return Json(jsonTextIRAGGE);
                        case "Graph12":                             // CAFQ: Fallecidos IRAG x GE
                            string jsonTextFallGE = "";

                            jsonTextFallGE = graficoIRAGFallecidosxGE(CountryID_, years, HospitalID_);
                            return Json(jsonTextFallGE);
                    }

                    //Esta sección sirve para identificar, con base al gráfico, y al stored procedure seleccionado, cual es el reporte(template) que se usará
                    var ReportSelected = db.Reports.FirstOrDefault(i => i.Template == spSelected);
                    var reportCountry = db.ReportsCountries.FirstOrDefault(s => s.ReportID == ReportSelected.ID && s.CountryID == CountryID);
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

                    //como los datos de los gráficos los obtenemos del resultado que se ingresa en el Excel (tipo reportes) es necesario saber  cual es el excel que queremos llenar
                    string templateToUse;
                    templateToUse = spSelected;

                    switch (reportTemplate)
                    {
                        case "D1":
                            templateToUse = "DenominadoresTemplate";
                            break;
                        default:
                            templateToUse = "SariTemplate";
                            break;
                    }

                    var tempy = ConfigurationManager.AppSettings[templateToUse]
                        .Replace("{report}", reportTemplate)
                        .Replace("{countryId}", CountryID_.ToString());

                    var tempy2 = System.IO.File.Exists(tempy);
                    //usar la variable anterior para ver si se procede con la generación o no, pues el archivo puede no existir

                    if (tempy2)
                    {
                        using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings[templateToUse]
                            .Replace("{report}", reportTemplate)
                            .Replace("{countryId}", CountryID_.ToString())
                            ))
                        {
                            using (var excelPackage2 = new ExcelPackage(fs))
                            {
                                var excelWorkBook2 = excelPackage2.Workbook;

                                bool insertRow = true;


                                if (reportTemplate == "R1" || reportTemplate == "R2" || reportTemplate == "R4" || reportTemplate == "D1")
                                {
                                    insertRow = false;
                                }

                                switch (Graph)
                                {
                                    case "Graph0":
                                    case "Graph1":
                                    case "Graph2":
                                    case "Graph3":
                                    case "Graph4":
                                    case "Graph5":
                                        //------------Documento de Excel para los datos-------------
                                        ExcelPackage auxEp = new ExcelPackage();
                                        //----------------Definición de los años en los cuales se hará la generación del reporte
                                        if (Year == "")
                                        {
                                            DateTime fechaDefault = DateTime.Now;
                                            Year = fechaDefault.Year.ToString();
                                        }
                                        string[] years2 = Year.ToString().Split(',');

                                        Array.Sort(years2, 0, years.Length);
                                        //-----------------Fin de la definición--------------------------------------------
                                        //----------------Inicio de armado de estructura de datos en XML que contiene los datos para la gráfica Graph0
                                        
                                        myXmlDoc0.AppendChild(myXmlDoc0.CreateElement("graph"));
                                        XmlNode myXmlNode0;
                                        myXmlNode0 = myXmlDoc0.CreateElement("graphTitle");
                                        myXmlNode0.InnerText = "Número y porcentaje de casos por tipo de comorbilidades, en hospitalizaciones, UCI y fallecidos";
                                        myXmlDoc0.DocumentElement.AppendChild(myXmlNode0);
                                        myXmlNode0 = myXmlDoc0.CreateElement("graphData");                                        
                                        myXmlDoc0.DocumentElement.AppendChild(myXmlNode0);

                                        XmlNode auxXmlNode0;
                                        XmlNode anotherAuxXmlNode0;
                                        
                                        //-----------------Fin del armado de la estructura para Graph0
                                        //----------------Inicio de armado de estructura de datos en XML que contiene los datos para la gráfica Graph1
                                        myXmlDoc1.AppendChild(myXmlDoc1.CreateElement("graph"));
                                        XmlNode myXmlNode;
                                        myXmlNode = myXmlDoc1.CreateElement("graphTitle");
                                        myXmlNode.InnerText = "Distribución de las proporciones de hospitalizaciones, admisiones en UCI y fallecidos por IRAG según SE";
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        myXmlNode = myXmlDoc1.CreateElement("graphXAxisTitle");
                                        myXmlNode.InnerText = "Semana Epidemiológica";
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        myXmlNode = myXmlDoc1.CreateElement("graphYAxisTitle");
                                        myXmlNode.InnerText = "Porcentaje";
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        myXmlNode = myXmlDoc1.CreateElement("graphData");
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        XmlNode auxXmlNode;
                                        XmlNode anotherAuxXmlNode;
                                        //-----------------Fin del armado de la estructura para Graph1
                                        //----------------Inicio del armado de la estructura para Graph2
                                        myXmlDoc2.AppendChild(myXmlDoc2.CreateElement("graph"));
                                        XmlNode myXmlNode2;
                                        myXmlNode2 = myXmlDoc2.CreateElement("graphTitle");
                                        myXmlNode2.InnerText = "Distribución de casos de IRAG según tipos y subtipos de virus de influenza y de las proporciones de positividad de las muestras analizadas según SE de inicio de síntomas";
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphXAxisTitle");
                                        myXmlNode2.InnerText = "Semana Epidemiológica";
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphYAxisTitle");
                                        myXmlNode2.InnerText = "Número de casos positivos";
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphYAxisTitle2");
                                        myXmlNode2.InnerText = "% positivos a influenza";
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphData");
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        XmlNode auxXmlNode2;
                                        XmlNode anotherAuxXmlNode2;
                                        //-------------------Fin del armado de la estructura para Graph2
                                        //----------------Inicio de armado de estructura de datos para la gráfica Graph 3
                                        myXmlDoc3.AppendChild(myXmlDoc3.CreateElement("graph"));
                                        XmlNode myXmlNode3;
                                        myXmlNode3 = myXmlDoc3.CreateElement("graphTitle");
                                        myXmlNode3.InnerText = "Distribución de casos de IRAG según virus respiratorios en vigilancia y  de las proporciones de positividad de las muestras analizadas, según SE de inicio de síntomas";
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphXAxisTitle");
                                        myXmlNode3.InnerText = "Semana Epidemiológica";
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphYAxisTitle");
                                        myXmlNode3.InnerText = "Número de casos positivos";
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphYAxisTitle2");
                                        myXmlNode3.InnerText = "% positivos a influenza";
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphData");
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        XmlNode auxXmlNode3;
                                        XmlNode anotherAuxXmlNode3;
                                        //-------------------Fin del armado para la estructura Graph3
                                        //------------------Inicio del armado para la estructura de datos para Graph4
                                        //----------------Inicio de armado de estructura de datos en XML que contiene los datos para la gráfica
                                        myXmlDoc4.AppendChild(myXmlDoc4.CreateElement("graph"));
                                        XmlNode myXmlNode4;
                                        myXmlNode4 = myXmlDoc4.CreateElement("graphTitle");
                                        myXmlNode4.InnerText = "Distribución de casos de IRAG según tipos y sub tipos virus respiratorios en vigilancia y grupos de edad";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphXAxisTitle");
                                        myXmlNode4.InnerText = "Grupos de edad";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphYAxisTitle");
                                        myXmlNode4.InnerText = "Porcentaje";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);



                                        myXmlNode4 = myXmlDoc4.CreateElement("graphData");
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        XmlNode auxXmlNode4;
                                        XmlNode anotherAuxXmlNode4;
                                        //------------------------Fin del armado de la estructura para Graph 4
                                        //----------------Inicio de armado de estructura de datos para la gráfica Graph5
                                        myXmlDoc5.AppendChild(myXmlDoc5.CreateElement("graph"));
                                        XmlNode myXmlNode5;
                                        myXmlNode5 = myXmlDoc5.CreateElement("graphTitle");
                                        myXmlNode5.InnerText = "Distribución de casos de IRAG según tipos y sub tipos  virus respiratorios en vigilancia y gravedad";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphXAxisTitle");
                                        myXmlNode5.InnerText = "Estadío";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphYAxisTitle");
                                        myXmlNode5.InnerText = "Porcentaje";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphData");
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        XmlNode auxXmlNode5;
                                        XmlNode anotherAuxXmlNode5;
                                        //----------------------Fin del armado de la estructura para Graph5

                                        //for each año  en el array de años, ejecutar esto y concatenar los datos juntos
                                        foreach (string yr in years)
                                        {
                                            var ms = new MemoryStream();
                                            var excelPackage = new ExcelPackage(fs);
                                            var excelWorkBook = excelPackage.Workbook;

                                            bool yrConversion = false;
                                            int yrInt = 0;
                                            yrConversion = Int32.TryParse(yr, out yrInt);

                                            AppendDataToExcel(Languaje_, CountryID_, RegionID_, yrInt, HospitalID_, Month, SE, StartDate, EndDate, excelWorkBook, reportTemplate, reportStartRow, reportStartCol, 1, insertRow, ReportCountry, IRAG_, ETI_);
                                            excelPackage.SaveAs(ms);
                                            ms.Position = 0;
                                            //Cargamos el excel resultante para su manipulacion
                                            ExcelPackage ep = new ExcelPackage(ms);
                                            auxEp = ep;

                                            //-----------Llenado de datos para Graph0
                                            decimal auxVal0;
                                            bool conversionResult0;

                                            XmlNode yearXmlNode0;
                                            XmlNode myAuxXmlNode0;

                                            yearXmlNode0 = myXmlDoc0.CreateElement("year");
                                            XmlAttribute yearNodeAtt0;
                                            yearNodeAtt0 = myXmlDoc0.CreateAttribute("date");
                                            yearNodeAtt0.InnerText = yr;
                                            yearXmlNode0.Attributes.Append(yearNodeAtt0);

                                            myAuxXmlNode0 = myXmlDoc0.CreateElement("graphHeaders");
                                            yearXmlNode0.AppendChild(myAuxXmlNode0);

                                            XmlAttribute headerAtt;

                                            auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[7, 1].Value.ToString();
                                            auxXmlNode0.InnerText = "";
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString();
                                            auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[8, 3].Value.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString();
                                            auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[8, 9].Value.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString();
                                            auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[8, 15].Value.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);
                                            

                                            myAuxXmlNode0 = myXmlDoc0.CreateElement("subGraphHeaders");
                                            yearXmlNode0.AppendChild(myAuxXmlNode0);
                                            
                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            auxXmlNode0.InnerText = "Total de casos";
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 7].Value.ToString();
                                            conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 7].Formula).ToString(), out auxVal0);
                                            auxXmlNode0.InnerText = auxVal0.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 8].Value.ToString();                                            
                                            auxXmlNode0.InnerText = "";
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 13].Value.ToString();
                                            conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 13].Formula).ToString(), out auxVal0);
                                            auxXmlNode0.InnerText = auxVal0.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 14].Value.ToString();
                                            auxXmlNode0.InnerText = "";
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 19].Value.ToString();
                                            conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 19].Formula).ToString(), out auxVal0);
                                            auxXmlNode0.InnerText = auxVal0.ToString();
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);

                                            auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                            headerAtt = myXmlDoc0.CreateAttribute("name");
                                            headerAtt.InnerText = ep.Workbook.Worksheets[2].Cells[10, 20].Value.ToString();
                                            auxXmlNode0.InnerText = "";
                                            auxXmlNode0.Attributes.Append(headerAtt);
                                            myAuxXmlNode0.AppendChild(auxXmlNode0);
                                            //----------------------Prueba de grabado de archivo
                                            
                                            /*Funcional
                                            ExcelPackage ep11 = new ExcelPackage(ms);
                                            string path = @"C:\x\sal1.xlsx";
                                            Stream stream = System.IO.File.Create(path);
                                            ep11.SaveAs(stream);
                                            stream.Close();
                                            */
                                            //------------------------Row 1---------------------------------
                                            int[] filas = {13,17,15,14,19,16,20,27,28,29,30,21,18,31,22,32,39,40,45};
                                            int[] columnas = { 1,7,8,13,14,19,20 };
                                            ep.Workbook.CalcMode = ExcelCalcMode.Automatic;
                                            for (int i = 0; i < filas.Length; i++)
                                            {
                                                myAuxXmlNode0 = myXmlDoc0.CreateElement("row");
                                                XmlAttribute rowNo;
                                                rowNo = myXmlDoc0.CreateAttribute("r_no");
                                                rowNo.InnerText = i.ToString();
                                                myAuxXmlNode0.Attributes.Append(rowNo);
                                                yearXmlNode0.AppendChild(myAuxXmlNode0);

                                                for (int j = 0; j < columnas.Length; j++)
                                                {
                                                    auxXmlNode0 = myXmlDoc0.CreateElement("col");
                                                    XmlAttribute colNo; ;
                                                    colNo = myXmlDoc0.CreateAttribute("c_no");
                                                    colNo.InnerText = j.ToString();
                                                    auxXmlNode0.Attributes.Append(colNo);                                                    
                                                    if (j == 0)
                                                    {
                                                        
                                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]].Value==null?string.Empty: ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]].Value.ToString();
                                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]+1].Value == null ? auxXmlNode0.InnerText : auxXmlNode0.InnerText + ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]+1].Value.ToString();

                                                    }
                                                    else
                                                    {
                                                        conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]].Formula)==null?"0":ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[filas[i], columnas[j]].Formula).ToString(), out auxVal0);
                                                        auxXmlNode0.InnerText = auxVal0.ToString();
                                                        if (columnas[j] % 2 == 0)
                                                        {
                                                            //auxXmlNode0.InnerText = auxVal0*100;
                                                            auxVal0 = auxVal0 * 100;
                                                            auxVal0 = Math.Round(auxVal0, 2);
                                                            auxXmlNode0.InnerText = auxVal0.ToString(new CultureInfo("en-US"));
                                                            auxXmlNode0.InnerText = auxXmlNode0.InnerText + "%";
                                                        }                                                        

                                                    }
                                                    myAuxXmlNode0.AppendChild(auxXmlNode0);
                                                }
                                            }
                                            myXmlNode0.AppendChild(yearXmlNode0);
                                            //------------------------Fin Row 1---------------------------------

                                            /*
                                            conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[176, 6 + i].Formula).ToString(), out auxVal);
                                            auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                            auxVal = Math.Round(auxVal, 2);
                                            anotherAuxXmlNode.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                            */







                                            //-----------Fin de llenado de datos para Graph0

                                            //------------Llenado de datos para Graph1------------------
                                            /*XmlNode yearXmlNode1;
                                            yearXmlNode1 = myXmlDoc1.CreateElement("year");
                                            XmlAttribute yearNodeAtt;
                                            yearNodeAtt = myXmlDoc1.CreateAttribute("date");
                                            yearNodeAtt.InnerText = yr;
                                            yearXmlNode1.Attributes.Append(yearNodeAtt);*/

                                            for (int i = 0; i < 53; i++)
                                            {
                                                auxXmlNode = myXmlDoc1.CreateElement("graphDataItem");

                                                anotherAuxXmlNode = myXmlDoc1.CreateElement("semana");
                                                anotherAuxXmlNode.InnerText = yr + "-" + ep.Workbook.Worksheets[1].Cells[6, 6 + i].Value.ToString();
                                                auxXmlNode.AppendChild(anotherAuxXmlNode);

                                                decimal auxVal;
                                                bool conversionResult;

                                                anotherAuxXmlNode = myXmlDoc1.CreateElement("serie1");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[176, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode.AppendChild(anotherAuxXmlNode);

                                                anotherAuxXmlNode = myXmlDoc1.CreateElement("serie2");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[185, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode.AppendChild(anotherAuxXmlNode);

                                                anotherAuxXmlNode = myXmlDoc1.CreateElement("serie3");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[194, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode.AppendChild(anotherAuxXmlNode);

                                                myXmlNode.AppendChild(auxXmlNode);
                                                /*yearXmlNode1.AppendChild(auxXmlNode);*/
                                            }
                                            /*myXmlNode.AppendChild(yearXmlNode1);*/
                                            //-----Fin de llenado de la parte de datos de Graph1
                                            //------------Llenado de datos para Graph2------------------
                                            /*XmlNode yearXmlNode2;
                                            yearXmlNode2 = myXmlDoc2.CreateElement("year");
                                            XmlAttribute yearNodeAtt2;
                                            yearNodeAtt2 = myXmlDoc2.CreateAttribute("date");
                                            yearNodeAtt2.InnerText = yr;
                                            yearXmlNode2.Attributes.Append(yearNodeAtt2);*/

                                            for (int i = 0; i < 53; i++)
                                            {
                                                auxXmlNode2 = myXmlDoc2.CreateElement("graphDataItem");

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("semana");
                                                anotherAuxXmlNode2.InnerText = yr + "-" + ep.Workbook.Worksheets[1].Cells[6, 6 + i].Value.ToString();
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                decimal auxVal;
                                                bool conversionResult;

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie1");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[757, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie2");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[658, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie3");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[559, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie4");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[460, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie5");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[361, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie6");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[262, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                anotherAuxXmlNode2 = myXmlDoc2.CreateElement("serie7");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[1650, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode2.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal * 100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode2.AppendChild(anotherAuxXmlNode2);

                                                myXmlNode2.AppendChild(auxXmlNode2);
                                                /*yearXmlNode2.AppendChild(auxXmlNode2);*/
                                            }
                                            /*myXmlNode2.AppendChild(yearXmlNode2);*/
                                            //-----Fin de llenado de la parte de datos de Graph2
                                            //------------Llenado de datos para Graph3------------------
                                            /*XmlNode yearXmlNode3;
                                            yearXmlNode3 = myXmlDoc3.CreateElement("year");
                                            XmlAttribute yearNodeAtt3;
                                            yearNodeAtt3 = myXmlDoc3.CreateAttribute("date");
                                            yearNodeAtt3.InnerText = yr;
                                            yearXmlNode3.Attributes.Append(yearNodeAtt3);*/

                                            for (int i = 0; i < 53; i++)
                                            {
                                                auxXmlNode3 = myXmlDoc3.CreateElement("graphDataItem");

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("semana");
                                                anotherAuxXmlNode3.InnerText = yr + "-" + ep.Workbook.Worksheets[1].Cells[6, 6 + i].Value.ToString();
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                decimal auxVal;
                                                bool conversionResult;

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie1");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[1153, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie2");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[1054, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie3");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[955, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie4");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[856, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie5");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[757, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie6");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[658, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie7");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[559, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie8");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[460, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie9");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[361, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie10");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[262, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                anotherAuxXmlNode3 = myXmlDoc3.CreateElement("serie11");
                                                conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells[1651, 6 + i].Formula).ToString(), out auxVal);
                                                auxVal = (conversionResult) ? (auxVal * 100) : 0;
                                                auxVal = Math.Round(auxVal, 2);
                                                anotherAuxXmlNode3.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                //anotherAuxXmlNode.InnerText = (conversionResult) ? (auxVal*100).ToString(new CultureInfo("en-US")) : "0";
                                                auxXmlNode3.AppendChild(anotherAuxXmlNode3);

                                                myXmlNode3.AppendChild(auxXmlNode3);
                                                /*yearXmlNode3.AppendChild(auxXmlNode3);*/
                                            }
                                            /*myXmlNode3.AppendChild(yearXmlNode3);*/
                                            //-----Fin de llenado de la parte de datos de Graph3
                                            //------------Llenado de datos para Graph4------------------
                                            XmlNode yearXmlNode4;
                                            yearXmlNode4 = myXmlDoc4.CreateElement("year");
                                            XmlAttribute yearNodeAtt4;
                                            yearNodeAtt4 = myXmlDoc4.CreateAttribute("date");
                                            yearNodeAtt4.InnerText = yr;
                                            yearXmlNode4.Attributes.Append(yearNodeAtt4);
                                            //yearXmlNode4.AppendChild(yearNodeAtt4);
                                            int indiceInicial = 1156;
                                            for (int i = 11; i < 17; i++)
                                            {
                                                auxXmlNode4 = myXmlDoc4.CreateElement("graphDataItem");

                                                anotherAuxXmlNode4 = myXmlDoc4.CreateElement("edad");

                                                //anotherAuxXmlNode4.InnerText = yr + "-" + ep.Workbook.Worksheets[1].Cells[6, 6 + i].Value.ToString();
                                                anotherAuxXmlNode4.InnerText = ep.Workbook.Worksheets[1].Cells["BJ" + i.ToString()].Value.ToString();
                                                auxXmlNode4.AppendChild(anotherAuxXmlNode4);

                                                decimal auxVal;
                                                bool conversionResult;

                                                for (int j = 0; j < 10; j++)
                                                {
                                                    string labelVirus = "";

                                                    switch (j)
                                                    {
                                                        case 0:
                                                            labelVirus = "serie1";// "Otros";
                                                            break;
                                                        case 1:
                                                            labelVirus = "serie2";//"Adenovirus";
                                                            break;
                                                        case 2:
                                                            labelVirus = "serie3";//"VSR";
                                                            break;
                                                        case 3:
                                                            labelVirus = "serie4";//"Parainfluenza";
                                                            break;
                                                        case 4:
                                                            labelVirus = "serie5";//"Influenza B";
                                                            break;
                                                        case 5:
                                                            labelVirus = "serie6";//"Influenza A/H3";
                                                            break;
                                                        case 6:
                                                            labelVirus = "serie7";//"Influenza A/H1";
                                                            break;
                                                        case 7:
                                                            labelVirus = "serie8";//"Influenza A No Subtipificable";
                                                            break;
                                                        case 8:
                                                            labelVirus = "serie9";//"Influenza A No Suptipificada";
                                                            break;
                                                        case 9:
                                                            labelVirus = "serie10";//"Influenza A(H1N1)pdm09";
                                                            break;
                                                    }
                                                    anotherAuxXmlNode4 = myXmlDoc4.CreateElement(labelVirus);
                                                    conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells["BG" + (indiceInicial + (12 * (i - 11)) - (99 * j)).ToString()].Formula).ToString(), out auxVal);
                                                    auxVal = (conversionResult) ? (auxVal) : 0;
                                                    anotherAuxXmlNode4.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                    auxXmlNode4.AppendChild(anotherAuxXmlNode4);
                                                    yearXmlNode4.AppendChild(auxXmlNode4);

                                                }
                                            }
                                            myXmlNode4.AppendChild(yearXmlNode4);
                                            //-----Fin de llenado de la parte de datos de Graph4
                                            //------------Llenado de datos para Graph5------------------
                                            XmlNode yearXmlNode5;
                                            yearXmlNode5 = myXmlDoc5.CreateElement("year");
                                            XmlAttribute yearNodeAtt5;
                                            yearNodeAtt5 = myXmlDoc5.CreateAttribute("date");
                                            yearNodeAtt5.InnerText = yr;
                                            yearXmlNode5.Attributes.Append(yearNodeAtt5);
                                            //yearXmlNode5.AppendChild(yearNodeAtt5);
                                            int indiceInicial2 = 1157;
                                            for (int i = 18; i < 21; i++)
                                            {
                                                auxXmlNode5 = myXmlDoc5.CreateElement("graphDataItem");

                                                anotherAuxXmlNode5 = myXmlDoc5.CreateElement("estadio");

                                                //anotherAuxXmlNode4.InnerText = yr + "-" + ep4.Workbook.Worksheets[1].Cells[6, 6 + i].Value.ToString();
                                                anotherAuxXmlNode5.InnerText = ep.Workbook.Worksheets[1].Cells["BJ" + i.ToString()].Value.ToString();
                                                auxXmlNode5.AppendChild(anotherAuxXmlNode5);

                                                decimal auxVal;
                                                bool conversionResult;

                                                for (int j = 0; j < 10; j++)
                                                {
                                                    string labelVirus = "";

                                                    switch (j)
                                                    {
                                                        case 0:
                                                            labelVirus = "serie1";// "Otros";
                                                            break;
                                                        case 1:
                                                            labelVirus = "serie2";//"Adenovirus";
                                                            break;
                                                        case 2:
                                                            labelVirus = "serie3";//"VSR";
                                                            break;
                                                        case 3:
                                                            labelVirus = "serie4";//"Parainfluenza";
                                                            break;
                                                        case 4:
                                                            labelVirus = "serie5";//"Influenza B";
                                                            break;
                                                        case 5:
                                                            labelVirus = "serie6";//"Influenza A/H3";
                                                            break;
                                                        case 6:
                                                            labelVirus = "serie7";//"Influenza A/H1";
                                                            break;
                                                        case 7:
                                                            labelVirus = "serie8";//"Influenza A No Subtipificable";
                                                            break;
                                                        case 8:
                                                            labelVirus = "serie9";//"Influenza A No Suptipificada";
                                                            break;
                                                        case 9:
                                                            labelVirus = "serie10";//"Influenza A(H1N1)pdm09";
                                                            break;
                                                    }
                                                    anotherAuxXmlNode5 = myXmlDoc5.CreateElement(labelVirus);
                                                    conversionResult = decimal.TryParse(ep.Workbook.Worksheets[1].Calculate(ep.Workbook.Worksheets[1].Cells["BK" + (indiceInicial2 + (3 * (i - 18)) - (99 * j)).ToString()].Formula).ToString(), out auxVal);
                                                    auxVal = (conversionResult) ? (auxVal) : 0;
                                                    anotherAuxXmlNode5.InnerText = auxVal.ToString(new CultureInfo("en-US"));
                                                    auxXmlNode5.AppendChild(anotherAuxXmlNode5);
                                                    yearXmlNode5.AppendChild(auxXmlNode5);
                                                }
                                            }
                                            myXmlNode5.AppendChild(yearXmlNode5);
                                            //-----Fin de llenado de la parte de datos de Graph5
                                        }
                                        //fin for each año
                                        /*------------------Etiquetas que solo se pueden sacar después de llenado el Excel, y que solo van 1 vez-------------------*/
                                        /*myAuxXmlNode0 = myXmlDoc0.CreateElement("graphHeaders");
                                        yearXmlNode0.AppendChild(myAuxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[7, 1].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString() + " " + ep.Workbook.Worksheets[2].Cells[8, 3].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString() + " " + ep.Workbook.Worksheets[2].Cells[8, 9].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("header");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[7, 3].Value.ToString() + " " + ep.Workbook.Worksheets[2].Cells[8, 15].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);


                                        myAuxXmlNode0 = myXmlDoc0.CreateElement("subGraphHeaders");
                                        yearXmlNode0.AppendChild(myAuxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        auxXmlNode0.InnerText = "Total de casos";
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 7].Formula).ToString(), out auxVal0);
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 7].Value.ToString() + "(" + auxVal0 + ")";
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 8].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 13].Formula).ToString(), out auxVal0);
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 13].Value.ToString() + "(" + auxVal0 + ")";
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 14].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        conversionResult0 = decimal.TryParse(ep.Workbook.Worksheets[2].Calculate(ep.Workbook.Worksheets[2].Cells[11, 19].Formula).ToString(), out auxVal0);
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 19].Value.ToString() + "(" + auxVal0 + ")";
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);

                                        auxXmlNode0 = myXmlDoc0.CreateElement("subheader");
                                        auxXmlNode0.InnerText = ep.Workbook.Worksheets[2].Cells[10, 20].Value.ToString();
                                        myAuxXmlNode0.AppendChild(auxXmlNode0);*/


                                        myXmlNode = myXmlDoc1.CreateElement("graphSeries1Label");
                                        myXmlNode.InnerText = auxEp.Workbook.Worksheets[1].Cells["BJ61"].Value.ToString();
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        myXmlNode = myXmlDoc1.CreateElement("graphSeries2Label");
                                        myXmlNode.InnerText = auxEp.Workbook.Worksheets[1].Cells["BJ62"].Value.ToString();
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);

                                        myXmlNode = myXmlDoc1.CreateElement("graphSeries3Label");
                                        myXmlNode.InnerText = auxEp.Workbook.Worksheets[1].Cells["BJ63"].Value.ToString();
                                        myXmlDoc1.DocumentElement.AppendChild(myXmlNode);
                                        //----------------Fin del armado de la estructura de datos para la gráfica

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries1Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["A757"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries2Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["B658"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries3Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["B559"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries4Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["B460"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries5Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["B361"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries6Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["B262"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);

                                        myXmlNode2 = myXmlDoc2.CreateElement("graphSeries7Label");
                                        myXmlNode2.InnerText = auxEp.Workbook.Worksheets[1].Cells["A1650"].Value.ToString();
                                        myXmlDoc2.DocumentElement.AppendChild(myXmlNode2);
                                        //----------------Fin del armado de la estructura de datos para la gráfica

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries1Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B1153"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries2Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B1054"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries3Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B955"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries4Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B856"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries5Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["A757"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries6Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B658"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries7Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B559"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries8Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B460"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries9Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B361"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries10Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["B262"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);

                                        myXmlNode3 = myXmlDoc3.CreateElement("graphSeries11Label");
                                        myXmlNode3.InnerText = auxEp.Workbook.Worksheets[1].Cells["A1651"].Value.ToString();
                                        myXmlDoc3.DocumentElement.AppendChild(myXmlNode3);
                                        //----------------Fin del armado de la estructura de datos para la gráfica

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries1Label");
                                        myXmlNode4.InnerText = "Otros";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries2Label");
                                        myXmlNode4.InnerText = "Adenovirus";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries3Label");
                                        myXmlNode4.InnerText = "VSR";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries4Label");
                                        myXmlNode4.InnerText = "Parainfluenza";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries5Label");
                                        myXmlNode4.InnerText = "Influenza B";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries6Label");
                                        myXmlNode4.InnerText = "Influenza A/H3";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries7Label");
                                        myXmlNode4.InnerText = "Influenza A/H1";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries8Label");
                                        myXmlNode4.InnerText = "Influenza A No Subtipificable";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries9Label");
                                        myXmlNode4.InnerText = "Influenza A No Suptipificada";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        myXmlNode4 = myXmlDoc4.CreateElement("graphSeries10Label");
                                        myXmlNode4.InnerText = "Influenza A(H1N1)pdm09";
                                        myXmlDoc4.DocumentElement.AppendChild(myXmlNode4);

                                        //----------------Fin del armado de la estructura de datos para la gráfica

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries1Label");
                                        myXmlNode5.InnerText = "Otros";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries2Label");
                                        myXmlNode5.InnerText = "Adenovirus";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries3Label");
                                        myXmlNode5.InnerText = "VSR";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries4Label");
                                        myXmlNode5.InnerText = "Parainfluenza";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries5Label");
                                        myXmlNode5.InnerText = "Influenza B";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries6Label");
                                        myXmlNode5.InnerText = "Influenza A/H3";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries7Label");
                                        myXmlNode5.InnerText = "Influenza A/H1";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries8Label");
                                        myXmlNode5.InnerText = "Influenza A No Subtipificable";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries9Label");
                                        myXmlNode5.InnerText = "Influenza A No Suptipificada";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        myXmlNode5 = myXmlDoc5.CreateElement("graphSeries10Label");
                                        myXmlNode5.InnerText = "Influenza A(H1N1)pdm09";
                                        myXmlDoc5.DocumentElement.AppendChild(myXmlNode5);

                                        //----------------Fin del armado de la estructura de datos para la gráfica
                                        break;
                                }
                            }
                            //Transformar los xml con el xsl
                            //Transformar los xml resultantes en json
                            //Meter los resultados json en la base de datos

                            //---Transformación para la suma de años en los gráficos Graph0, Graph4 y Graph5
                            //---------------------------------------------------------------
                            XslCompiledTransform xslTransform = new XslCompiledTransform();
                            xslTransform.Load(ConfigurationManager.AppSettings["graphG4Transform"]);
                            XmlDocument tempDoc = new XmlDocument();
                            using (XmlWriter writer = tempDoc.CreateNavigator().AppendChild())
                            {
                                xslTransform.Transform(myXmlDoc4, writer);
                            }
                            myXmlDoc4 = tempDoc;
                            //---------------------------------------------------------------
                            xslTransform.Load(ConfigurationManager.AppSettings["graphG5Transform"]);
                            XmlDocument tempDoc2 = new XmlDocument();
                            using (XmlWriter writer = tempDoc2.CreateNavigator().AppendChild())
                            {
                                xslTransform.Transform(myXmlDoc5, writer);
                            }
                            myXmlDoc5 = tempDoc2;
                            //---------------------------------------------------------------                            
                            xslTransform.Load(ConfigurationManager.AppSettings["graphG0Transform"]);
                            XmlDocument tempDoc0 = new XmlDocument();
                            using (XmlWriter writer = tempDoc0.CreateNavigator().AppendChild())
                            {
                                xslTransform.Transform(myXmlDoc0, writer);
                            }
                            myXmlDoc0 = tempDoc0;

                            //-----Conversión del XML a JSON
                            string jsonText0 = JsonConvert.SerializeXmlNode(myXmlDoc0);
                            string jsonText1 = JsonConvert.SerializeXmlNode(myXmlDoc1);
                            string jsonText2 = JsonConvert.SerializeXmlNode(myXmlDoc2);
                            string jsonText3 = JsonConvert.SerializeXmlNode(myXmlDoc3);
                            string jsonText4 = JsonConvert.SerializeXmlNode(myXmlDoc4);
                            string jsonText5 = JsonConvert.SerializeXmlNode(myXmlDoc5);

                            //-------Ingreso de los datos a la base de datos, usando los parámetros de búsqueda
                            //Inicio de revisión si los datos para la gráfica, año, hospital, etc. existen o no

                            string[] graficas = { "Graph0", "Graph1", "Graph2", "Graph3", "Graph4", "Graph5" };

                            foreach (string grafica in graficas)
                            {
                                using (var con = new SqlConnection(consString))
                                {
                                    string jsonGraphData = "";
                                    switch (grafica)
                                    {
                                        case "Graph0":
                                            jsonGraphData = jsonText0;
                                            break;
                                        case "Graph1":
                                            jsonGraphData = jsonText1;
                                            break;
                                        case "Graph2":
                                            jsonGraphData = jsonText2;
                                            break;
                                        case "Graph3":
                                            jsonGraphData = jsonText3;
                                            break;
                                        case "Graph4":
                                            jsonGraphData = jsonText4;
                                            break;
                                        case "Graph5":
                                            jsonGraphData = jsonText5;
                                            break;
                                    }
                                    using (var command = new SqlCommand("SetGraphData", con) { CommandType = CommandType.StoredProcedure })
                                    {
                                        command.Parameters.Clear();
                                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = CountryID_;
                                        command.Parameters.Add("@Graph", SqlDbType.Text).Value = grafica;
                                        command.Parameters.Add("@Year", SqlDbType.Text).Value = Year;
                                        command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = RegionID_;
                                        command.Parameters.Add("@State_ID", SqlDbType.Int).Value = StateID_;
                                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID_;
                                        command.Parameters.Add("@IRAG", SqlDbType.Int).Value = IRAG_;
                                        command.Parameters.Add("@ETI", SqlDbType.Int).Value = ETI_;
                                        command.Parameters.Add("@GraphData", SqlDbType.Text).Value = jsonGraphData;
                                        con.Open();
                                        using (var reader = command.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                resultGetGraphData = reader["GraphData"].ToString().Trim();

                                            }
                                        }
                                    }
                                }
                            }
                            //Fin de la revisión de si existen los datos para la gráfica
                            //--------Fin del ingreso de datos en la base
                            //------------------Regreso del JSON para la gráfica indicada
                            string obtainedJsonData = "";
                            using (var con = new SqlConnection(consString))
                            {
                                using (var command = new SqlCommand("GetGraphData", con) { CommandType = CommandType.StoredProcedure })
                                {
                                    command.Parameters.Clear();
                                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = CountryID_;
                                    command.Parameters.Add("@Graph", SqlDbType.Text).Value = Graph;
                                    command.Parameters.Add("@Year", SqlDbType.Text).Value = Year;
                                    command.Parameters.Add("@Region_ID", SqlDbType.Int).Value = RegionID_;
                                    command.Parameters.Add("@State_ID", SqlDbType.Int).Value = StateID_;
                                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = HospitalID_;
                                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = IRAG_;
                                    command.Parameters.Add("@ETI", SqlDbType.Int).Value = ETI_;
                                    con.Open();
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            obtainedJsonData = reader["GraphData"].ToString().Trim();

                                        }
                                    }
                                }
                            }
                            //------------------Fin Regreso del JSON para la gráfica indicada

                            return Json(obtainedJsonData);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "El reporte no se pudo generar, falta su plantilla asociada";
                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "El reporte no se pudo generar, por favor intente de nuevo";
            }
            return null;
        }

        private static string graficoIndicadoreDesempenio(string languaje_country, int countryId, int? year, int? hospitalId, int? weekFrom, int? weekTo, List<string> datosID)
        {
            /*var user = UserManager.FindById(User.Identity.GetUserId());
            var nombPais = user.Institution.Country.Name;*/
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

            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 1, nDato1);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 2, nDato2);
            //recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 3, nDato1, aDato3);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 4, nDato4);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 5, nDato5);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 6, nDato6);
            //recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 7, nDato1, aDato7);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 8, nDato8);

            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 9, nDato9);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 10, nDato10);
            recuperarDatosIndDes(consString, languaje_country, countryId, 0, year, hospitalId, 0, 0, null, null, 0, 0, 1, 11, nDato11);
            //****
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

            datosID.Add(nombPais + " - " + year.ToString());                  // Titulo

            if (nDato2[0] != 0)
                datosID.Add((nDato1[0] / nDato2[0] * 100).ToString("#,##0.0", CultureInfo.InvariantCulture));
            else
                datosID.Add("0");

            datosID.Add("0");                                               // Grupos Etareos

            if (nDato1[0] != 0)
                datosID.Add((nDato4[0] / nDato1[0] * 100).ToString("#,##0.0", CultureInfo.InvariantCulture));
            else
                datosID.Add("0");

            if (nDato8[0] != 0)
                datosID.Add((nDato5[0] / nDato8[0] * 100).ToString("#,##0.0", CultureInfo.InvariantCulture));
            else
                datosID.Add("0");

            if (nDato1[0] != 0)
                datosID.Add((nDato6[0] / nDato1[0] * 100).ToString("#,##0.0", CultureInfo.InvariantCulture));
            else
                datosID.Add("0");

            datosID.Add((nDato9[0]).ToString("#,##0.0", CultureInfo.InvariantCulture));
            datosID.Add((nDato10[0]).ToString("#,##0.0", CultureInfo.InvariantCulture));
            datosID.Add((nDato11[0]).ToString("#,##0.0", CultureInfo.InvariantCulture));

            /*decimal nTemp = 0;
            for (int nI = 0; nI <= 5; ++nI)
            {
                nTemp = Convert.ToDecimal(aDato3[nI, 1], new CultureInfo("en-US"));
                if (nTemp != 0)
                    datosID.Add((Convert.ToDecimal(aDato7[nI, 1], new CultureInfo("en-US")) / nTemp * 100).ToString("#,##0.0", CultureInfo.InvariantCulture));
                else
                    datosID.Add("0");
            }*/
            //**** metas
            string cMetas = ConfigurationManager.AppSettings["IndicadoresDesempenioMetas_" + countryId.ToString()];
            if (cMetas == null)
                cMetas = "0:0:0:0:0:0";

            string[] aMeta = cMetas.Split(':');
            for (int nI = 0; nI < aMeta.Length; ++nI)
            {
                datosID.Add(aMeta[nI]);
            }

            return "";
        }

        private static void recuperarDatosIndDes(string consString, string languaje_, int countryId, int? regionId, int? year, int? hospitalId, int? month, int? se, DateTime? startDate, DateTime? endDate, int? YearFrom, int? YearTo, int IRAG, int opcion, decimal[] nResuOut, string[,] aResuOut = null)
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand("IndicDesemp", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 1200 })
                //using (var command = new SqlCommand("IndicDesemp", con) { CommandType = CommandType.StoredProcedure})
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
                            if (opcion == 3 || opcion == 7)         // No son pais
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

        private static string graficoLineasBasales(string languaje_country, int countryId, int? year, int? hospitalId, int? weekFrom, int? weekTo)
        {
            string jsonTextLB = "";
            string storedProcedure1 = "FLUID_IRAG_Total_Muestra_Analizadas";
            string storedProcedure2 = "FLUID_IRAG_Total_Muestras_INF_A";
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //****
            ArrayList aCEP1 = new ArrayList();
            ArrayList aCEP2 = new ArrayList();
            ArrayList aUA1 = new ArrayList();
            ArrayList aUA2 = new ArrayList();
            ArrayList aUE1 = new ArrayList();
            ArrayList aUE2 = new ArrayList();

            ArrayList aMuAnTo2 = new ArrayList();
            ArrayList aMuAnTo1 = new ArrayList();
            ArrayList aMuAnIA2 = new ArrayList();
            ArrayList aMuAnIA1 = new ArrayList();

            for (int nI = 0; nI <= 52; ++nI)
            {
                aCEP1.Add(0);
                aCEP2.Add(0);
                aUA1.Add(0);
                aUA2.Add(0);
                aUE1.Add(0);
                aUE2.Add(0);

                aMuAnTo1.Add(0);
                aMuAnTo2.Add(0);
                aMuAnIA1.Add(0);
                aMuAnIA2.Add(0);
            }
            //****
            recuperarDatos(consString, storedProcedure1, countryId, languaje_country, (int)year - 1, aMuAnTo1);
            recuperarDatos(consString, storedProcedure1, countryId, languaje_country, (int)year, aMuAnTo2);
            recuperarDatos(consString, storedProcedure2, countryId, languaje_country, (int)year - 1, aMuAnIA1);
            recuperarDatos(consString, storedProcedure2, countryId, languaje_country, (int)year, aMuAnIA2);
            //****
            for (int nI = 0; nI <= 52; ++nI)                        // Calculando Porcentaje de Positividad
            {
                if ((int)aMuAnTo1[nI] != 0)
                {
                    aMuAnTo1[nI] = Convert.ToDecimal(aMuAnIA1[nI]) / Convert.ToDecimal(aMuAnTo1[nI]);
                }
                if ((int)aMuAnTo2[nI] != 0)
                {
                    aMuAnTo2[nI] = Convert.ToDecimal(aMuAnIA2[nI]) / Convert.ToDecimal(aMuAnTo2[nI]);
                }
            }
            //****
            int nSeIn = 0;                                  // Semana inicio
            int nSeQu = 0;                                  // Semana quiebre
            string cTitu = "";
            string sheet = "";

            if (countryId == 7)
            {
                sheet = "Chile";
                nSeIn = 1;
                cTitu = "Líneas de base: Chile, porcentaje de positividad para influenza durante 2016 en comparación al período 2010 -2015.  Semana epidemiológica 1 a 52";
            }
            else if (countryId == 9)
            {
                sheet = "Costa Rica";
                nSeIn = 15;
                cTitu = "Líneas de base: Costa Rica, porcentaje de positividad para influenza en 2016-2017 en comparación al período 2011-2015/2016. Semana epidemiológica 1 a 52";
            }
            else if (countryId == 3)
            {
                sheet = "BOLIVIA INLASA";
                nSeIn = 1;
                //cTitu = "Líneas de base: Bolivia, porcentaje de positividad para influenza para Bolivia (INLASA) durante 2016, en comparación al período 2010 -2015. Semana epidemiologica 1 a 52.";
                cTitu = "Líneas de base: Bolivia, porcentaje de positividad para influenza durante 2016 en comparación al período 2010-2015. Semana epidemiologica 1 a 52.";
            }
            else if (countryId == 3.1)
            {
                sheet = "BOLIVIA CENETROP";
                nSeIn = 1;
                cTitu = "Líneas de base: porcentaje de positividad para influenza en Bolivia (INLASA) 2016, en comparación al período 2010-2015. Semana epidemiológica 1 a 52";
            }
            else if (countryId == 3.2)
            {
                sheet = "BOLIVIA CENETROP";
                nSeIn = 1;
                cTitu = "Líneas de base: porcentaje de positividad para influenza en Bolivia (CENETROP) 2016, en comparación al período 2010-2015. Semana epidemiológica 1 a 52";
            }
            else if (countryId == 25)
            {
                sheet = "Surinam";
                nSeIn = 1;
                cTitu = "Baselines: Suriname Percentage Positivity for Influenza 2016 and SARI cases (%) (As Compare To 2010 - 2015) - EW 1 to EW52 2016";
            }
            else
            {
                return "";
            }

            nSeQu = 52 - nSeIn + 1;
            //**** 
            recuperarDatosExcel(countryId, aCEP1, aCEP2, aUA1, aUA2, aUE1, aUE2, sheet);
            //**** Crear el JSON
            jsonTextLB = "";
            string cJS = "";
            string cTemp = "";

            if (countryId != 25)
            {
                jsonTextLB = "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Semana Epidemiológica" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Porcentaje" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";
            }
            else
            {
                jsonTextLB = "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Epidemiological week" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Percent" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";
            }
            //** Ultima semana con data
            int nSeFi = 0;
            if (countryId == 9)
            {
                nSeFi = 52;
                for (int nJ = 52; nJ > 0; --nJ)
                {
                    if (nJ > nSeQu)
                    {
                        if (Convert.ToDecimal(aMuAnTo2[nJ - nSeQu]) != 0)
                        {
                            nSeFi = nJ;         // Semana final con datos
                            break;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(aMuAnTo1[nJ + nSeIn - 1]) != 0)
                        {
                            nSeFi = nJ;         // Semana final con datos
                            break;
                        }
                    }
                }
            }
            else
            {
                nSeFi = 52;
                for (int nJ = 52; nJ > 0; --nJ)
                {
                    if (Convert.ToDecimal(aMuAnTo1[nJ]) != 0)
                    {
                        nSeFi = nJ;         // Semana final con datos
                        break;
                    }
                }
            }
            //**
            int nK = 0;
            decimal nTemp = 0;
            //for (int nI = 1; nI <= 52; ++nI)
            for (int nI = 1; nI <= nSeFi; ++nI)
            {
                if (nI > nSeQu)
                {
                    nK = nI - nSeQu;
                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + nK.ToString() + "\",";
                    nTemp = Convert.ToDecimal(aCEP2[nK]) * 100;
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";           // Curva Epidemica Promedio
                    nTemp = Convert.ToDecimal(aUA2[nK]) * 100;
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral de Alerta
                    nTemp = Convert.ToDecimal(aUE2[nK]) * 100;
                    cTemp = cTemp + "\"" + "serie3" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral Estacional
                    nTemp = Convert.ToDecimal(aMuAnTo2[nK]) * 100;
                    cTemp = cTemp + "\"" + "serie4" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\"";          // Porcentaje de Positividad
                    cTemp = cTemp + "}";
                }
                else
                {
                    nK = nI + nSeIn - 1;
                    /*
                    if (countryId == 3)                  //#### DESARROLLO: SOLO PARA PRUEBAS : ELIMINAR ####//
                    {   //#### DESARROLLO: SOLO PARA PRUEBAS : ELIMINAR ####//
                        cTemp = "{";
                        cTemp = cTemp + "\"" + "semana" + "\":\"" + nK.ToString() + "\",";
                        nTemp = Convert.ToDecimal(aCEP2[nK]) * 100;
                        cTemp = cTemp + "\"" + "serie1" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";           // Curva Epidemica Promedio
                        nTemp = Convert.ToDecimal(aUA2[nK]) * 100;
                        cTemp = cTemp + "\"" + "serie2" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral de Alerta
                        nTemp = Convert.ToDecimal(aUE2[nK]) * 100;
                        cTemp = cTemp + "\"" + "serie3" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral Estacional
                        nTemp = Convert.ToDecimal(aMuAnTo2[nK]) * 100;
                        cTemp = cTemp + "\"" + "serie4" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\"";          // Porcentaje de Positividad
                        cTemp = cTemp + "}";
                    }
                    else
                    {*/
                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + nK.ToString() + "\",";

                    nTemp = Convert.ToDecimal(aCEP1[nK]) * 100;
                    //if (nI > nSeFi) { nTemp = 0; }
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";           // Curva Epidemica Promedio

                    nTemp = Convert.ToDecimal(aUA1[nK]) * 100;
                    //if (nI > nSeFi) { nTemp = 0; }
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral de Alerta

                    nTemp = Convert.ToDecimal(aUE1[nK]) * 100;
                    //if (nI > nSeFi) { nTemp = 0; }
                    cTemp = cTemp + "\"" + "serie3" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\",";             // Umbral Estacional

                    nTemp = Convert.ToDecimal(aMuAnTo1[nK]) * 100;
                    //if (nI > nSeFi) { nTemp = 0; }
                    cTemp = cTemp + "\"" + "serie4" + "\":\"" + nTemp.ToString("##0.0", new CultureInfo("en-US")) + "\"";          // Porcentaje de Positividad

                    cTemp = cTemp + "}";
                    //}
                }

                cJS = (nI == 1) ? cTemp : cJS + "," + cTemp;
            }
            cJS = "[" + cJS + "]";

            if (countryId != 25)
            {
                jsonTextLB = jsonTextLB + cJS + "},";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Curva epidémica promedio para porcentaje de positividad de influenza" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "Umbral de alerta" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "Umbral Estacional" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "Porcentaje de positividad para influenza 2016" + "\"";
                jsonTextLB = jsonTextLB + "}}";
            }
            else
            {
                jsonTextLB = jsonTextLB + cJS + "},";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Average epidemic curve for influenza positivity percentage" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "Alert threshold" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "Seasonal threshold" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "Positivity percentage for influenza year 2016" + "\"";
                jsonTextLB = jsonTextLB + "}}";
            }

            return jsonTextLB;
        }

        private static void recuperarDatos(string consString, string storedProcedure, int countryId, string languaje_country, int year, ArrayList aData)
        {
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = languaje_country;
                    command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@Mes_", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@SE", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@Fecha_inicio", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@Fecha_fin", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@label_beg", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@table_temp_name", SqlDbType.Int).Value = null;
                    command.Parameters.Add("@IRAG", SqlDbType.Int).Value = 1;

                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader.GetValue(0).ToString() == "")
                            {
                            }
                            else
                            {
                                if ((int)reader.GetValue(0) <= 52)
                                    aData[(int)reader.GetValue(0)] = reader.GetValue(1);
                            }
                        }
                    }

                    command.Parameters.Clear();
                    con.Close();
                }
            }
        }

        private static void recuperarDatosExcel(int CountryID, ArrayList aCEP1, ArrayList aCEP2, ArrayList aUA1, ArrayList aUA2, ArrayList aUE1, ArrayList aUE2, string sheet)
        {
            string cPathPlan = "";
            cPathPlan = ConfigurationManager.AppSettings["GraphicsPath"];
            cPathPlan = cPathPlan + "LinBa_X.xlsx";

            using (var fs = System.IO.File.OpenRead(cPathPlan))
            {
                using (var excelPackage = new ExcelPackage(fs))
                {
                    var excelWorkBook = excelPackage.Workbook;
                    int nSema = 0;
                    decimal nCEP = 0;
                    decimal nUA = 0;
                    decimal nUE = 0;
                    int row = 3;
                    int col = 1;

                    var excelWorksheet = excelWorkBook.Worksheets[sheet];

                    for (int nI = 1; nI <= 52; ++nI)
                    {
                        nSema = Convert.ToInt32(excelWorksheet.Cells[row, col].Value);
                        nCEP = Convert.ToDecimal(excelWorksheet.Cells[row, col + 1].Value);
                        nUA = Convert.ToDecimal(excelWorksheet.Cells[row, col + 2].Value);
                        nUE = Convert.ToDecimal(excelWorksheet.Cells[row, col + 3].Value);

                        if (nI > 38 && CountryID == 9)
                        {
                            aCEP2[nSema] = nCEP;
                            aUA2[nSema] = nUA;
                            aUE2[nSema] = nUE;
                        }
                        else
                        {
                            aCEP1[nSema] = nCEP;
                            aUA1[nSema] = nUA;
                            aUE1[nSema] = nUE;
                        }

                        ++row;
                    }
                }
            }
        }

        public static string graficoETINumeroCasos(int countryId, string[] years, int? hospitalId)
        {
            //System.Diagnostics.Debug.WriteLine("graficoETINumeroCasos->START");
            ArrayList aData = new ArrayList();
            string jsonTextLB = "";
            string storedProcedure = "FLUID_ETI";
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //****
            try
            {
                for (int nI = 0; nI < years.Length; ++nI)
                {
                    recuperarDatosETI(consString, storedProcedure, countryId, hospitalId, Int32.Parse(years[nI]), aData);
                }
                //**** Crear el JSON
                string cTitu = "Número de casos ETI por semana epidemiológica - " + string.Join(",", years);
                jsonTextLB = "";
                string cJS = "";
                string cTemp = "";

                jsonTextLB = jsonTextLB + "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Semana Epidemiológica" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Número de Casos" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle2" + "\":\"" + "Porcentaje de Casos ETI del Total de Consultas" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";

                for (int nI = 0; nI < aData.Count; ++nI)
                {
                    string[] aDaSe;
                    aDaSe = (string[])aData[nI];

                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + aDaSe[0] + "-" + aDaSe[1] + "\",";
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + aDaSe[2] + "\",";               // Casos
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + aDaSe[3] + "\"";               // Porcentaje
                    cTemp = cTemp + "}";

                    cJS = (nI == 0) ? cTemp : cJS + "," + cTemp;
                }

                cJS = "[" + cJS + "]";
                jsonTextLB = jsonTextLB + cJS + "},";

                jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Número de Casos ETI" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "% ETI del total de consultas" + "\"";
                jsonTextLB = jsonTextLB + "}}";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("graficoETINumeroCasos->Error:" + e.Message + "<-");
            }
            //**** 
            //System.Diagnostics.Debug.WriteLine("graficoETINumeroCasos->END");
            return jsonTextLB;
        }//END graficoETINumeroCasos

        private static void recuperarDatosETI(string consString, string storProc, int countryId, int? hospitalId, int? year, ArrayList aData)
        {
            //System.Diagnostics.Debug.WriteLine("recuperarDatosETI->START");
            try
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storProc, con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = null;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@weekTo", SqlDbType.Int).Value = 0;

                        //System.Threading.Thread.Sleep(1000);         // Stop de 1 segundos
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            int nCasos, nPoIn;              //, nCasosI, nPoInI;
                            double nPorc;                   //, nPorcI;
                            CultureInfo oCI = new CultureInfo("en-US");
                            //Random rnd = new Random();          //#### DESARROLLO

                            while (reader.Read())
                            {
                                nCasos = (int)reader.GetValue(4);
                                nPoIn = (int)reader.GetValue(3);                            // Poblacion inscrita
                                nPorc = (nPoIn == 0) ? 0 : (Convert.ToDouble(nCasos) / Convert.ToDouble(nPoIn)) * 100;
                                /*if (nPorc == 0)         //#### DESARROLLO
                                    nPorc = rnd.Next(1, 99);    //#### DESARROLLO*/
                                aData.Add(new string[] { reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), nCasos.ToString(), nPorc.ToString("###0.0", oCI) });
                                /*//**** Datos para grafico 10
                                nCasosI = (int)reader.GetValue(6);                           // Casos + a influenza
                                nPoInI = (int)reader.GetValue(5);                            // Casos ETI (con muestra)
                                nPorcI = (nPoInI == 0) ? 0 : (Convert.ToDouble(nCasosI) / Convert.ToDouble(nPoInI)) * 100;

                                aDataI.Add(new string[]{ reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), nCasosI.ToString(), nPorcI.ToString("###0.0", oCI)});
                                */
                            }
                        }
                        command.Parameters.Clear();
                        con.Close();
                    }
                }
                //System.Diagnostics.Debug.WriteLine("recuperarDatosETI->END");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("recuperarDatosETI->Error:" + e.Message + "<-");
            }
        }//END recuperarDatosETI

        public static string graficoETINumeroCasosPositivos(int countryId, string[] years, int? hospitalId)
        {
            //System.Diagnostics.Debug.WriteLine("graficoETINumeroCasosPositivos->START");
            ArrayList aData = new ArrayList();
            string jsonTextLB = "";
            string storedProcedure = "FLUID_ETI";
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //****
            try
            {
                for (int nI = 0; nI < years.Length; ++nI)
                {
                    recuperarDatosETIPositivos(consString, storedProcedure, countryId, hospitalId, Int32.Parse(years[nI]), aData);
                }
                //**** Crear el JSON
                string cTitu = "Número de casos ETI positivos a influenza por semana epidemiológica - " + string.Join(",", years) + " (porcentaje de casos positivos a influenza de todos casos de ETI)";
                jsonTextLB = "";
                string cJS = "";
                string cTemp = "";

                jsonTextLB = jsonTextLB + "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Semana Epidemiológica" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Número de Casos Positivos" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle2" + "\":\"" + "Porcentaje de Casos Positivos" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";

                for (int nI = 0; nI < aData.Count; ++nI)
                {
                    string[] aDaSe;
                    aDaSe = (string[])aData[nI];

                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + aDaSe[0] + "-" + aDaSe[1] + "\",";
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + aDaSe[2] + "\",";              // Números de casos
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + aDaSe[3] + "\"";               // Porcentaje de casos
                    cTemp = cTemp + "}";

                    cJS = (nI == 0) ? cTemp : cJS + "," + cTemp;
                }

                cJS = "[" + cJS + "]";
                jsonTextLB = jsonTextLB + cJS + "},";

                jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Casos ETI Positivos a Influenza" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "% de Casos ETI Positivos a Influenza" + "\"";
                jsonTextLB = jsonTextLB + "}}";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("graficoETINumeroCasosPositivos->Error:" + e.Message + "<-");
            }
            //**** 
            //System.Diagnostics.Debug.WriteLine("graficoETINumeroCasosPositivos->END");
            return jsonTextLB;
        }//END-graficoETINumeroCasos

        private static void recuperarDatosETIPositivos(string consString, string storProc, int countryId, int? hospitalId, int? year, ArrayList aData)
        {
            //System.Diagnostics.Debug.WriteLine("recuperarDatosETIPositivos->START");
            try
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storProc, con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = null;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@weekTo", SqlDbType.Int).Value = 0;

                        //System.Threading.Thread.Sleep(1000);         // Stop de 2 segundos
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            int nCasos, nPoIn;
                            double nPorc;
                            CultureInfo oCI = new CultureInfo("en-US");

                            while (reader.Read())
                            {
                                nCasos = (int)reader.GetValue(6);                           // Casos + a influenza
                                nPoIn = (int)reader.GetValue(5);                            // Casos ETI (con muestra)
                                nPorc = (nPoIn == 0) ? 0 : (Convert.ToDouble(nCasos) / Convert.ToDouble(nPoIn)) * 100;

                                aData.Add(new string[] { reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), nCasos.ToString(), nPorc.ToString("###0.0", oCI) });
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
                //System.Diagnostics.Debug.WriteLine("recuperarDatosETIPositivos->END");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("recuperarDatosETIPositivos->Error:" + e.Message + "<-");
            }
        }//END recuperarDatosETIPositivos

        public static string graficoIRAGFallecidosxGE(int countryId, string[] years, int? hospitalId)
        {
            //System.Diagnostics.Debug.WriteLine("graficoIRAGFallecidosxGE->START");
            ArrayList aData = new ArrayList();
            string jsonTextLB = "";
            string storedProcedure = "FLUID_DEATHS_IRAG";
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //****
            try
            {
                for (int nI = 0; nI < years.Length; ++nI)
                {
                    recuperarDatosIRAGFallecidosxGE(consString, storedProcedure, countryId, hospitalId, Int32.Parse(years[nI]), aData);
                }
                //**** Crear el JSON  
                string cTitu = "Distribución de fallecidos de IRAG por grupos de edad por semana epidemiológica - " + string.Join(",", years);
                jsonTextLB = "";
                string cJS = "";
                string cTemp = "";
                if (countryId == 25)
                    cTitu = "Distribution of SARI deaths by age groups per epidemiological week - " + string.Join(",", years);
                //****
                jsonTextLB = jsonTextLB + "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Semana Epidemiológica" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Número de Casos" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";

                for (int nI = 0; nI < aData.Count; ++nI)
                {
                    string[] aDaSe;
                    aDaSe = (string[])aData[nI];

                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + aDaSe[0] + "-" + aDaSe[1] + "\",";
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + aDaSe[2] + "\",";              //
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + aDaSe[3] + "\",";               //
                    cTemp = cTemp + "\"" + "serie3" + "\":\"" + aDaSe[4] + "\",";               //
                    cTemp = cTemp + "\"" + "serie4" + "\":\"" + aDaSe[5] + "\",";               //
                    cTemp = cTemp + "\"" + "serie5" + "\":\"" + aDaSe[6] + "\",";               //
                    cTemp = cTemp + "\"" + "serie6" + "\":\"" + aDaSe[7] + "\",";               //
                    cTemp = cTemp + "\"" + "serie7" + "\":\"" + aDaSe[8] + "\"";               //
                    if (countryId == 25)
                    {
                        cTemp = cTemp + ",";
                        cTemp = cTemp + "\"" + "serie8" + "\":\"" + aDaSe[9] + "\",";               //
                        cTemp = cTemp + "\"" + "serie9" + "\":\"" + aDaSe[10] + "\"";               //
                    }

                    cTemp = cTemp + "}";

                    cJS = (nI == 0) ? cTemp : cJS + "," + cTemp;
                }

                cJS = "[" + cJS + "]";
                jsonTextLB = jsonTextLB + cJS + "},";

                if (countryId == 25)
                {
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Under 6 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "6 to 11 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "12 to 23 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "2 to 4 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries5Label" + "\":\"" + "5 to 14 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries6Label" + "\":\"" + "15 to 49 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries7Label" + "\":\"" + "50 to 64 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries8Label" + "\":\"" + "65 years +" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries9Label" + "\":\"" + "Age unknown" + "\"";
                    jsonTextLB = jsonTextLB + "}}";
                }
                else
                {
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "0 a <2 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "2 a <5 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "5 a 19 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "20 a 39 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries5Label" + "\":\"" + "40 a 59 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries6Label" + "\":\"" + "60 años y +" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries7Label" + "\":\"" + "Edad desconocida" + "\"";
                    jsonTextLB = jsonTextLB + "}}";
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("graficoIRAGFallecidosxGE->Error:" + e.Message + "<-");
            }
            //**** 
            //System.Diagnostics.Debug.WriteLine("graficoIRAGFallecidosxGE->END");
            return jsonTextLB;
        }//END graficoETINumeroCasos

        private static void recuperarDatosIRAGFallecidosxGE(string consString, string storProc, int countryId, int? hospitalId, int? year, ArrayList aData)
        {
            //System.Diagnostics.Debug.WriteLine("recuperarDatosIRAGFallecidosxGE->START");
            try
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storProc, con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = null;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@weekTo", SqlDbType.Int).Value = 0;

                        //System.Threading.Thread.Sleep(1000);         // Stop de 2 segundos
                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            string cAnio, cSema;
                            string cGE1, cGE2, cGE3, cGE4, cGE5, cGE6, cGE7, cGE8, cGE9;

                            while (reader.Read())
                            {
                                cAnio = reader.GetValue(1).ToString();
                                cSema = reader.GetValue(2).ToString();
                                cGE1 = ((int)reader.GetValue(3)).ToString();
                                cGE2 = ((int)reader.GetValue(4)).ToString();
                                cGE3 = ((int)reader.GetValue(5)).ToString();
                                cGE4 = ((int)reader.GetValue(6)).ToString();
                                cGE5 = ((int)reader.GetValue(7)).ToString();
                                cGE6 = ((int)reader.GetValue(8)).ToString();
                                cGE7 = ((int)reader.GetValue(9)).ToString();
                                if (countryId == 25)
                                {
                                    cGE8 = ((int)reader.GetValue(10)).ToString();
                                    cGE9 = ((int)reader.GetValue(11)).ToString();
                                    aData.Add(new string[] { cAnio, cSema, cGE1, cGE2, cGE3, cGE4, cGE5, cGE6, cGE7, cGE8, cGE9 });
                                }
                                else
                                {
                                    aData.Add(new string[] { cAnio, cSema, cGE1, cGE2, cGE3, cGE4, cGE5, cGE6, cGE7 });
                                }
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
                //System.Diagnostics.Debug.WriteLine("recuperarDatosIRAGFallecidosxGE->END");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("recuperarDatosIRAGFallecidosxGE->Error:" + e.Message + "<-");
            }
        }//END-recuperarDatosIRAGFallecidosxGE

        public static string graficoIRAGxGrupoEdad(int countryId, string[] years, int? hospitalId)
        {
            //System.Diagnostics.Debug.WriteLine("graficoIRAGxGrupoEdad->START");
            ArrayList aData = new ArrayList();
            string jsonTextLB = "";
            string storedProcedure = "FLUID_IRAG";
            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //****
            try
            {
                for (int nI = 0; nI < years.Length; ++nI)
                {
                    recuperarDatosIRAGxGrupoEdad(consString, storedProcedure, countryId, hospitalId, Int32.Parse(years[nI]), aData);
                }
                //**** Crear el JSON
                string cTitu = "Distribución de total de casos de IRAG por  grupos de edad y semana epidemiológica - " + string.Join(",", years);
                jsonTextLB = "";
                string cJS = "";
                string cTemp = "";
                if (countryId == 25)
                    cTitu = "Distribution of total SARI cases by age group and epidemiological week - " + string.Join(",", years);

                jsonTextLB = jsonTextLB + "{\"" + "graph" + "\":";
                jsonTextLB = jsonTextLB + "{\"" + "graphTitle" + "\":\"" + cTitu + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphXAxisTitle" + "\":\"" + "Semana Epidemiológica" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphYAxisTitle" + "\":\"" + "Número de Casos" + "\",";
                jsonTextLB = jsonTextLB + "\"" + "graphData" + "\":{\"" + "graphDataItem" + "\":";

                for (int nI = 0; nI < aData.Count; ++nI)
                {
                    string[] aDaSe;
                    aDaSe = (string[])aData[nI];

                    cTemp = "{";
                    cTemp = cTemp + "\"" + "semana" + "\":\"" + aDaSe[0] + "-" + aDaSe[1] + "\",";
                    cTemp = cTemp + "\"" + "serie1" + "\":\"" + aDaSe[2] + "\",";              //
                    cTemp = cTemp + "\"" + "serie2" + "\":\"" + aDaSe[3] + "\",";               //
                    cTemp = cTemp + "\"" + "serie3" + "\":\"" + aDaSe[4] + "\",";               //
                    cTemp = cTemp + "\"" + "serie4" + "\":\"" + aDaSe[5] + "\",";               //
                    cTemp = cTemp + "\"" + "serie5" + "\":\"" + aDaSe[6] + "\",";               //
                    cTemp = cTemp + "\"" + "serie6" + "\":\"" + aDaSe[7] + "\"";               //
                    if (countryId == 25)
                    {
                        cTemp = cTemp + ",";
                        cTemp = cTemp + "\"" + "serie7" + "\":\"" + aDaSe[8] + "\",";               //
                        cTemp = cTemp + "\"" + "serie8" + "\":\"" + aDaSe[9] + "\"";               //
                    }

                    cTemp = cTemp + "}";

                    cJS = (nI == 0) ? cTemp : cJS + "," + cTemp;
                }

                cJS = "[" + cJS + "]";
                jsonTextLB = jsonTextLB + cJS + "},";

                if (countryId == 25)
                {
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "Under 6 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "6 to 11 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "12 to 23 months" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "2 to 4 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries5Label" + "\":\"" + "5 to 14 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries6Label" + "\":\"" + "15 to 49 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries7Label" + "\":\"" + "50 to 64 years" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries8Label" + "\":\"" + "65 years +" + "\"";
                }
                else
                {
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries1Label" + "\":\"" + "0 a <2 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries2Label" + "\":\"" + "2 a <5 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries3Label" + "\":\"" + "5 a 19 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries4Label" + "\":\"" + "20 a 39 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries5Label" + "\":\"" + "40 a 59 años" + "\",";
                    jsonTextLB = jsonTextLB + "\"" + "graphSeries6Label" + "\":\"" + "60 años y +" + "\"";
                }

                jsonTextLB = jsonTextLB + "}}";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("graficoIRAGxGrupoEdad->Error:" + e.Message + "<-");
            }
            //**** 
            //System.Diagnostics.Debug.WriteLine("graficoIRAGxGrupoEdad->END");
            return jsonTextLB;
        }//END

        private string getMsg(string msgView)
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
            string searchedMsg = ResourcesM.SgetMessage(msgView,countryDisp,langDisp);
            //searchedMsg = myR.getMessage(searchedMsg, 0, "ENG");
            return searchedMsg;
        }

        private static void recuperarDatosIRAGxGrupoEdad(string consString, string storProc, int countryId, int? hospitalId, int? year, ArrayList aData)
        {
            //System.Diagnostics.Debug.WriteLine("recuperarDatosIRAGxGrupoEdad->START");
            try
            {
                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand(storProc, con) { CommandType = CommandType.StoredProcedure })
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                        command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = null;
                        command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                        command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                        command.Parameters.Add("@Mes_", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@SE", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@Fecha_inicio", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@Fecha_fin", SqlDbType.Int).Value = null;
                        command.Parameters.Add("@weekFrom", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@weekTo", SqlDbType.Int).Value = 0;

                        con.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            string cAnio, cSema;
                            string cGE1, cGE2, cGE3, cGE4, cGE5, cGE6, cGE7, cGE8;

                            while (reader.Read())
                            {
                                cAnio = reader.GetValue(1).ToString();
                                cSema = reader.GetValue(2).ToString();
                                cGE1 = ((int)reader.GetValue(11)).ToString();
                                cGE2 = ((int)reader.GetValue(12)).ToString();
                                cGE3 = ((int)reader.GetValue(13)).ToString();
                                cGE4 = ((int)reader.GetValue(14)).ToString();
                                cGE5 = ((int)reader.GetValue(15)).ToString();
                                cGE6 = ((int)reader.GetValue(16)).ToString();
                                if (countryId == 25)
                                {
                                    cGE7 = ((int)reader.GetValue(17)).ToString();
                                    cGE8 = ((int)reader.GetValue(18)).ToString();
                                    aData.Add(new string[] { cAnio, cSema, cGE1, cGE2, cGE3, cGE4, cGE5, cGE6, cGE7, cGE8 });
                                }
                                else
                                {
                                    aData.Add(new string[] { cAnio, cSema, cGE1, cGE2, cGE3, cGE4, cGE5, cGE6 });
                                }
                            }
                        }

                        command.Parameters.Clear();
                        con.Close();
                    }
                }
                System.Diagnostics.Debug.WriteLine("recuperarDatosIRAGxGrupoEdad->END");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("graficoIRAGxGrupoEdad->Error:" + e.Message + "<-");
            }
        }//END-

    }
}
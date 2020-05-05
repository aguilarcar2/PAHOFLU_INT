using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace WSConsume
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Invocando WS PAHO - CR Inciensa->STAR");

            //CargarDesdeWS();
            Console.WriteLine("Cantidad de argumentos: {0}", args.Length);
            foreach (string argumento in args)
            {
                Console.WriteLine("Argumentos: {0}", argumento);
            }
            //Console.ReadKey();

            if (args.Length == 0)
            {
                CargarDesdeWS();

                System.Console.WriteLine("Invocando WS PAHO - CR Inciensa->END");
            }
            else if(args[0].ToUpper() == "IMPORT")
            {
                ImportDataToINCIENSAWS();
            }


            
        }

        public static bool CargarDesdeWS()
        {
            //System.Console.WriteLine("CargarDesdeWS->STAR");
            int server = 2;
            string cKeyValidate = "W~(4n-Xp@fcJRVV3gepmYeU3=8Rrg3C,{RUvmXH6shWT48;P";
            WSPaCr.WSBDSoapClient WSD = new WSPaCr.WSBDSoapClient();
            //System.Console.WriteLine("CargarDesdeWS->Invocando el WS");
            var miJson = WSD.GetDaPI(server, cKeyValidate);
            //System.Console.WriteLine("CargarDesdeWS->Data en miJson");
            System.Console.WriteLine("CargarDesdeWS->Los primeros 30 Chars recuperados->" + miJson.Substring(1, 30));

            DataTable dtExcelData = (DataTable)JsonConvert.DeserializeObject(miJson, typeof(DataTable));
            System.Console.WriteLine("CargarDesdeWS->Data a DataTable");
            try
            {
                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                //System.Console.WriteLine("CargarDesdeWS->Data a dbo.ImportLab START");
                using (var con = new SqlConnection(consString))
                {
                    using (var sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.ImportLab";
                        sqlBulkCopy.BulkCopyTimeout = 1200;
                        con.Open();
                        sqlBulkCopy.WriteToServer(dtExcelData);
                        con.Close();
                    }
                }
                //System.Console.WriteLine("CargarDesdeWS->Data a dbo.ImportLab END");

                //Archivo de exportacion de resultados y ejecucion de StoreProcedure
                var ms = new MemoryStream();
                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["ImportTemplate"]
                .Replace("{countryId}", "9")
                ))

                using (var excelPackage = new ExcelPackage(fs))
                {
                    //System.Console.WriteLine("->111");
                    var excelWorkBook = excelPackage.Workbook;
                    int startColumn = 1;
                    int startRow = 3;
                    bool insertRow = true;
                    string Report = "ImportLab_CR"; 
                    string user = "PAHOINCIENSA";

                    AppendDataToExcel(9, excelWorkBook, Report, startRow, startColumn, 1, insertRow);
                    excelPackage.SaveAs(ms);

                    //FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.Now.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName + ".XLSX");
                    FileInfo notImportedFile = new FileInfo(ConfigurationManager.AppSettings["ImportFailedFolder"] + user + "_" + DateTime.Now.ToString("yyyyMMddhhmmsst") + ".XLSX");
                    FileStream aFile = new FileStream(notImportedFile.FullName, FileMode.Create);
                    aFile.Seek(0, SeekOrigin.Begin);
                    ExcelPackage excelNotImported = new ExcelPackage();

                    excelNotImported.Load(ms);
                    excelNotImported.SaveAs(aFile);
                    aFile.Close();

                    //hacer inserción en la base de datos, bitácora de subidas
                    var consStringLogImport = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                    using (var conImportLog = new SqlConnection(consStringLogImport))
                    {
                        using (var commandImportLog = new SqlCommand("importLogSP", conImportLog) { CommandType = CommandType.StoredProcedure })
                        {
                            //var user = UserManager.FindById(User.Identity.GetUserId());
                            commandImportLog.Parameters.Clear();
                            commandImportLog.Parameters.Add("@Fecha_Import", SqlDbType.DateTime).Value = DateTime.Now.ToString();
                            //commandImportLog.Parameters.Add("@User_Import", SqlDbType.NVarChar).Value = User.Identity.Name;
                            commandImportLog.Parameters.Add("@User_Import", SqlDbType.NVarChar).Value = user;
                            //commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = user.Institution.CountryID;
                            commandImportLog.Parameters.Add("@Country_ID", SqlDbType.Int).Value = 9;
                            //commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = ConfigurationManager.AppSettings["ImportFailedFolder"] + User.Identity.Name + "_" + DateTime.UtcNow.ToString("yyyyMMddhhmmsst") + "_" + Request.Files["file"]?.FileName;
                            commandImportLog.Parameters.Add("@Filename", SqlDbType.NVarChar).Value = notImportedFile.FullName;
                            var returnParameter = commandImportLog.Parameters.Add("@ReturnVal", SqlDbType.Int);
                            returnParameter.Direction = ParameterDirection.ReturnValue;

                            conImportLog.Open();

                            using (var reader2 = commandImportLog.ExecuteReader())
                            {
                                if (returnParameter.Value.ToString() == "1")
                                {
                                    //ViewBag.Message = $"Archivo registrado en al bitácora.";
                                    System.Console.WriteLine("Archivo registrado en la bitácora.");
                                }
                            }
                        }
                    }
                    //fin de inserción
                }
                ms.Position = 0;

                System.Console.WriteLine("CargarDesdeWS->END");
            }
            catch (Exception e)
            {
                //ViewBag.Message = "El archivo no se pudo cargar, por favor, compruebe el archivo -  " + e.Message;
                System.Console.WriteLine("El archivo no se pudo cargar, por favor verifique -  " + e.Message);
            }

            return true;
        }

        private static void AppendDataToExcel(int countryId, ExcelWorkbook excelWorkBook, string storedProcedure, int startRow, int startColumn, int sheet, bool? insert_row)
        {
            var excelWorksheet = excelWorkBook.Worksheets[1];
            var row = startRow;
            var column = startColumn;

            var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (var con = new SqlConnection(consString))
            {
                using (var command = new SqlCommand(storedProcedure, con) { CommandType = CommandType.StoredProcedure })
                {
                    command.Parameters.Clear();
                    command.CommandTimeout = 1200;

                    con.Open();
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
                                    int number = 0;
                                    //bool isNumber = int.TryParse(reader.GetValue(i).ToString(), out number);
                                    bool isNumber = false;

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
                    command.Parameters.Clear();
                    con.Close();
                }
            }
            // Apply only if it has a Total row at the end and hast SUM in range, i.e. SUM(A1:A4)
            //excelWorksheet.DeleteRow(row, 2);
        }

        public static async Task<bool> ImportDataToINCIENSAWS()
        {
            try
            {

                var url = ConfigurationManager.AppSettings["ExportDataURLBase"] + "?DpvmuszJE_=9&Mbmhvbkf_=SPA&Qspdfcvsf_=1&dLfz=" + ConfigurationManager.AppSettings["KeyHostWSINCIENSA"];
                var url_1 = ConfigurationManager.AppSettings["ExportDataURLBase"] + "?DpvmuszJE_=9&Mbmhvbkf_=SPA&Qspdfcvsf_=2&dLfz=" + ConfigurationManager.AppSettings["KeyHostWSINCIENSA"];
                var url_2 = ConfigurationManager.AppSettings["ExportDataURLBase"] + "?DpvmuszJE_=9&Mbmhvbkf_=SPA&Qspdfcvsf_=3&dLfz=" + ConfigurationManager.AppSettings["KeyHostWSINCIENSA"];

                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


                /** Empieza la exportación de los datos a INCIENSA **/
                ///** Tabla Export_Info_Case_CR_temporal *** //
                System.Console.WriteLine("Invocando WS PAHO to INCIENSA Export_Info_Case_CR_temporal ->BEGIN");

                System.Console.WriteLine("CargarDesdeWS-> URL -> " + url);
                var httpWebRequestQR = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequestQR.ContentType = "application/json";
                httpWebRequestQR.Method = "GET";

                System.Console.WriteLine("CargarDesdeWS-> GET -> " + httpWebRequestQR.Address.PathAndQuery ); 
                var httpResponseQR = (HttpWebResponse)httpWebRequestQR.GetResponse();
                using (var streamReader = new StreamReader(httpResponseQR.GetResponseStream()))
                {
                    var resultQR = streamReader.ReadToEnd();
                    string jsonStringsign = resultQR;

                    File.WriteAllText(@"C:\Temp\csc.json", jsonStringsign);
                    //dynamic jsonWS = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStringsign);
                    //System.Console.WriteLine("CargarDesdeWS-> jsonWS -> " + jsonWS.GetType());
                    //var i = 1;
                    //foreach (var obj in jsonWS)
                    //{
                    //    //Console.WriteLine(obj);
                    //    i = i + 1;
                    //    if (i == 5) break;
                    //}
                    //System.Console.WriteLine("CargarDesdeWS-> jsonStringsign -> +++++ ");

                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonStringsign, (typeof(DataTable)));

                    //System.Console.WriteLine("CargarDesdeWS-> dt -> +++++ ");


                    //for (int i = 0; i < dt.Columns.Count; i++)
                    //{
                    //    if (dt.Columns[i].ColumnName == "Inicio_sintomas"
                    //        || dt.Columns[i].ColumnName == "Fecha_muestra"
                    //        || dt.Columns[i].ColumnName == "Fecha_notificacion"
                    //        || dt.Columns[i].ColumnName == "Fecha_registro"
                    //        || dt.Columns[i].ColumnName == "Fecha_Nacimiento"
                    //        || dt.Columns[i].ColumnName == "DestFechaLlegada1"
                    //        //|| dt.Columns[i].ColumnName == "DestFechaSalida1"
                    //        //|| dt.Columns[i].ColumnName == "DestFechaLlegada2"
                    //        //|| dt.Columns[i].ColumnName == "DestFechaSalida2"
                    //        //|| dt.Columns[i].ColumnName == "DestPrevSintoma3"
                    //        //|| dt.Columns[i].ColumnName == "DestFechaLlegada3"
                    //        //|| dt.Columns[i].ColumnName == "DestFechaSalida3"
                    //        //|| dt.Columns[i].ColumnName == "Fecha_primera_dosis"
                    //        //|| dt.Columns[i].ColumnName == "Fecha_segunda_dosis"
                    //        //|| dt.Columns[i].ColumnName == "Neumococo_fecha"
                    //        //|| dt.Columns[i].ColumnName == "Antiviral_fecha"
                    //        //|| dt.Columns[i].ColumnName == "Fecha_diag"
                    //        //|| dt.Columns[i].ColumnName == "hosp_ing_fecha"
                    //        //|| dt.Columns[i].ColumnName == "Hosp_egre_fecha"
                    //        //|| dt.Columns[i].ColumnName == "InfeccHospitFecha"
                    //        //|| dt.Columns[i].ColumnName == "UCI_ing_fecha"
                    //        //|| dt.Columns[i].ColumnName == "UCI_egre_fecha"
                    //        //|| dt.Columns[i].ColumnName == "Fecha_envio"
                    //        //|| dt.Columns[i].ColumnName == "Caso_cerrado_fecha"
                    //        //|| dt.Columns[i].ColumnName == "Recepcion_fecha"
                    //        //|| dt.Columns[i].ColumnName == "RecDate_National"
                    //        //|| dt.Columns[i].ColumnName == "Res_fin_fecha"
                    //        //|| dt.Columns[i].ColumnName == "FeverDate"
                    //        )
                    //    {
                    //        dt.Columns[i].DataType = typeof(DateTime?);
                    //    }
                    //}

                    using (var con = new SqlConnection(consString))
                    {

                        SqlCommand command_DELETE = new SqlCommand("DELETE FROM Export_Info_Case_CR_temporal", con);
                        command_DELETE.Connection.Open();
                        command_DELETE.ExecuteNonQuery();
                        command_DELETE.Connection.Close();

                        using (var sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            sqlBulkCopy.DestinationTableName = "dbo.Export_Info_Case_CR_temporal";
                            sqlBulkCopy.BulkCopyTimeout = 1200;
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }

                }

                System.Console.WriteLine("Invocando WS PAHO to INCIENSA  Export_Info_Case_CR_temporal ->END");


                ///** Tabla Export_Info_Case_CR_temporal *** //
                System.Console.WriteLine("Invocando WS PAHO to INCIENSA Export_Info_Test_Case_CR_temporal ->BEGIN");

                System.Console.WriteLine("CargarDesdeWS-> URL -> " + url_1);
                var httpWebRequestQR_2 = (HttpWebRequest)WebRequest.Create(url_1);
                httpWebRequestQR_2.ContentType = "application/json";
                httpWebRequestQR_2.Method = "GET";

                System.Console.WriteLine("CargarDesdeWS-> GET -> " + httpWebRequestQR_2.Address.PathAndQuery);
                var httpResponseQR_2 = (HttpWebResponse)httpWebRequestQR_2.GetResponse();
                using (var streamReader = new StreamReader(httpResponseQR_2.GetResponseStream()))
                {
                    var resultQR = streamReader.ReadToEnd();
                    string jsonStringsign = resultQR;
                    //dynamic jsonWS = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStringsign);
                    //System.Console.WriteLine("CargarDesdeWS-> jsonTables -> " + jsonStringsign);

                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonStringsign, (typeof(DataTable)));

                    using (var con = new SqlConnection(consString))
                    {

                        SqlCommand command_DELETE = new SqlCommand("DELETE FROM Export_Info_Test_Case_CR_temporal", con);
                        command_DELETE.Connection.Open();
                        command_DELETE.ExecuteNonQuery();
                        command_DELETE.Connection.Close();

                        using (var sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            sqlBulkCopy.DestinationTableName = "dbo.Export_Info_Test_Case_CR_temporal";
                            sqlBulkCopy.BulkCopyTimeout = 1200;
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }

                }

                System.Console.WriteLine("Invocando WS PAHO to INCIENSA  Export_Info_Test_Case_CR_temporal ->END");


                ///** Tabla Export_INCIENSA_CR_temporal *** //
                System.Console.WriteLine("Invocando WS PAHO to INCIENSA Export_INCIENSA_CR_temporal ->BEGIN");

                System.Console.WriteLine("CargarDesdeWS-> URL -> " + url_2);
                var httpWebRequestQR_3 = (HttpWebRequest)WebRequest.Create(url_2);
                httpWebRequestQR_3.ContentType = "application/json";
                httpWebRequestQR_3.Method = "GET";

                System.Console.WriteLine("CargarDesdeWS-> GET -> " + httpWebRequestQR_3.Address.PathAndQuery);
                var httpResponseQR_3 = (HttpWebResponse)httpWebRequestQR_3.GetResponse();
                using (var streamReader = new StreamReader(httpResponseQR_3.GetResponseStream()))
                {
                    var resultQR = streamReader.ReadToEnd();
                    string jsonStringsign = resultQR;
                    //dynamic jsonWS = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStringsign);
                    //System.Console.WriteLine("CargarDesdeWS-> jsonTables -> " + jsonStringsign);

                    DataTable dt = (DataTable)JsonConvert.DeserializeObject(jsonStringsign, (typeof(DataTable)));

                    using (var con = new SqlConnection(consString))
                    {

                        SqlCommand command_DELETE = new SqlCommand("DELETE FROM Export_INCIENSA_CR_temporal", con);
                        command_DELETE.Connection.Open();
                        command_DELETE.ExecuteNonQuery();
                        command_DELETE.Connection.Close();

                        using (var sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            sqlBulkCopy.DestinationTableName = "dbo.Export_INCIENSA_CR_temporal";
                            sqlBulkCopy.BulkCopyTimeout = 1200;
                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                }

                System.Console.WriteLine("Invocando WS PAHO to INCIENSA  Export_INCIENSA_CR_temporal ->END");

                using (var con = new SqlConnection(consString))
                {
                    using (var command = new SqlCommand("Export_Temporal_to_Historic_Case_CR", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                }


            }
            catch (Exception e)
            {
                //ViewBag.Message = "El archivo no se pudo cargar, por favor, compruebe el archivo -  " + e.Message;
                System.Console.WriteLine("El archivo no se pudo cargar, por favor verifique -  " + e.Message);
            }

            return true;

        }

        public object Deserialize(string jsonText, Type valueType)
        {
            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();

            json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            StringReader sr = new StringReader(jsonText);
            Newtonsoft.Json.JsonTextReader reader = new JsonTextReader(sr);
            object result = json.Deserialize(reader, valueType);
            reader.Close();

            return result;
        }

        static async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
            }
            return response;
        }

        public static DataTable JsonToDataTable(string json, string tableName)
        {
            bool columnsCreated = false;
            DataTable dt = new DataTable(tableName);

            Newtonsoft.Json.Linq.JObject root = Newtonsoft.Json.Linq.JObject.Parse(json);
            Newtonsoft.Json.Linq.JArray items = (Newtonsoft.Json.Linq.JArray)root[tableName];

            Newtonsoft.Json.Linq.JObject item = default(Newtonsoft.Json.Linq.JObject);
            Newtonsoft.Json.Linq.JToken jtoken = default(Newtonsoft.Json.Linq.JToken);

            for (int i = 0; i <= items.Count - 1; i++)
            {
                // Create the columns once
                if (columnsCreated == false)
                {
                    item = (Newtonsoft.Json.Linq.JObject)items[i];
                    jtoken = item.First;

                    while (jtoken != null)
                    {
                        dt.Columns.Add(new DataColumn(((Newtonsoft.Json.Linq.JProperty)jtoken).Name.ToString()));
                        jtoken = jtoken.Next;
                    }

                    columnsCreated = true;
                }

                // Add each of the columns into a new row then put that new row into the DataTable
                item = (Newtonsoft.Json.Linq.JObject)items[i];
                jtoken = item.First;

                // Create the new row, put the values into the columns then add the row to the DataTable
                DataRow dr = dt.NewRow();

                while (jtoken != null)
                {
                    dr[((Newtonsoft.Json.Linq.JProperty)jtoken).Name.ToString()] = ((Newtonsoft.Json.Linq.JProperty)jtoken).Value.ToString();
                    jtoken = jtoken.Next;
                }

                dt.Rows.Add(dr);
            }

            return dt;

        }

    }


}
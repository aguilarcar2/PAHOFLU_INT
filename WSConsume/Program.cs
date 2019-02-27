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

namespace WSConsume
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Invocando WS PAHO - CR Inciensa->STAR");

            CargarDesdeWS();

            System.Console.WriteLine("Invocando WS PAHO - CR Inciensa->END");
        }

        public static bool CargarDesdeWS()
        {
            int server = 2;
            string cKeyValidate = "W~(4n-Xp@fcJRVV3gepmYeU3=8Rrg3C,{RUvmXH6shWT48;P";
            WSPaCr.WSBDSoapClient WSD = new WSPaCr.WSBDSoapClient();
            var miJson = WSD.GetDaPI(server, cKeyValidate);

            DataTable dtExcelData = (DataTable)JsonConvert.DeserializeObject(miJson, typeof(DataTable));

            try
            {
                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (var con = new SqlConnection(consString))
                {
                    using (var sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        sqlBulkCopy.DestinationTableName = "dbo.ImportLab";
                        con.Open();
                        sqlBulkCopy.WriteToServer(dtExcelData);
                        con.Close();
                    }
                }

                //Archivo de exportacion de resultados y ejecucion de StoreProcedure
                var ms = new MemoryStream();
                using (var fs = System.IO.File.OpenRead(ConfigurationManager.AppSettings["ImportTemplate"]
                .Replace("{countryId}", "9")
                ))

                using (var excelPackage = new ExcelPackage(fs))
                {
                    var excelWorkBook = excelPackage.Workbook;
                    int startColumn = 1;
                    int startRow = 3;
                    bool insertRow = true;
                    string Report = "ImportLab_CR";
                    string user = "PAHOAdminAutomatic";

                    AppendDataToExcel(9, excelWorkBook, Report, startRow, startColumn, 1, insertRow);

                    excelPackage.SaveAs(ms);

                    //implementación para el guardado del archivo

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
                //ViewBag.Message = $"Archivo trabajado correctamente!";
                //ViewBag.Message = "Archivo procesado. En la lista inferior podrá ver la lista de los registros que no fueron importados. Lo podrá localizar por la hora de subida y por su usuario. Puede hacer clic en la flecha azul para descargarlo.";
                //return View();
                //return new FileStreamResult(ms, "application/xlsx")
                //{
                //    FileDownloadName = "NoInsertados.xlsx"
                //};

                //{ storeProcedureMessage}
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
                    command.CommandTimeout = 180;
                    //command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = countryId;
                    //command.Parameters.Add("@Languaje", SqlDbType.Text).Value = "SPA";
                    //command.Parameters.Add("@Year_case", SqlDbType.Int).Value = year;
                    //command.Parameters.Add("@Hospital_ID", SqlDbType.Int).Value = hospitalId;
                    //command.Parameters.Add("@Mes_", SqlDbType.Int).Value = month;
                    //command.Parameters.Add("@SE", SqlDbType.Int).Value = se;
                    //command.Parameters.Add("@Fecha_inicio", SqlDbType.Date).Value = startDate;
                    //command.Parameters.Add("@Fecha_fin", SqlDbType.Date).Value = endDate;

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
    }
}

﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace Paho.Controllers
{
    //[Authorize]
    public class ExportDataController : Controller
    {
        // GET: ExportData
        public ActionResult Index()
        {
            return View();
        }

        [WebMethod]
        public string GetData(int DpvmuszJE_, string Mbmhvbkf_, int Qspdfcvsf_, string dLfz)
        //  Parameters  
        //     DpvmuszJE_ ->  CountryID_        //
        //     Mbmhvbkf_ ->  Languaje_        //
        //     Qspdfcvsf_ ->  Procedure_        //
        {
            try
            {
                var RequestType_ = Request.RequestType;
                var UserHostAddress_ = Request.UserHostAddress;
                var UserHostName_ = Request.UserHostName;
                var HttpMethod = Request.HttpMethod;
                var REMOTE_ADDR_ = Request.ServerVariables["REMOTE_ADDR"];
                var consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                var IP_Addr_Host = (UserHostAddress_.Contains(".")) ? UserHostAddress_.ToString() : REMOTE_ADDR_.ToString();
                DataTable dt_InfoCase = new DataTable();
                DataTable dt_InfoTest = new DataTable();
                DataTable dt_InfoINCIENSA = new DataTable();

                string cKeyValidate = ConfigurationManager.AppSettings["KeyHostWSINCIENSA"];

                if (dLfz != cKeyValidate)
                {
                    return "No válido";
                }

                //if (IP_Addr_Host == ConfigurationManager.AppSettings["IPHostWSINCIENSA"])
                //{
                using (var con = new SqlConnection(consString))
                    {
                        var jsondata_app = new List<Object>();
                        var item_data_InfoCase = new Dictionary<string, Object>();
                        var item_data_InfoTest = new Dictionary<string, Object>();
                        var item_data_InfoINCIENSA = new Dictionary<string, Object>();
                        JavaScriptSerializer jsInfoINCIENSA = new JavaScriptSerializer();
                        var item_data_InfoCaseString = "";
                        var item_data_InfoTestString = "";
                        var item_data_InfoINCIENSAString = "";
                        //var item_data_InfoINCIENSA = new jsInfoINCIENSA.Deserialize<dynamic>(""); ;
                        dynamic item_data_InfoCase_Dynamic = new System.Dynamic.ExpandoObject();
                        dynamic item_data_InfoTest_Dynamic = new System.Dynamic.ExpandoObject();
                        dynamic item_data_InfoINCIENSA_Dynamic = new System.Dynamic.ExpandoObject();

                        if (Qspdfcvsf_ == 1)
                        {
                            using (var command = new SqlCommand("Export_Info_Case_CR_SP", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 2000 })
                            {
                                DateTime AnioReport = DateTime.Now;
                                command.Parameters.Clear();
                                command.Parameters.Add("@Country_ID", SqlDbType.Int).Value = DpvmuszJE_;
                                command.Parameters.Add("@Languaje", SqlDbType.NVarChar).Value = Mbmhvbkf_;

                                con.Open();
                                dt_InfoCase.Load(command.ExecuteReader());
                                command.Parameters.Clear();
                                con.Close();

                                Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
                                json.NullValueHandling = NullValueHandling.Ignore;
                                json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                                json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                                json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                json.Converters.Add(new DataTableConverter());
                                json.NullValueHandling = NullValueHandling.Include;
                                StringWriter sw = new StringWriter();
                                Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
                                writer.Formatting = Formatting.Indented;
                                writer.QuoteChar = '"';
                                json.Serialize(writer, dt_InfoCase);

                                item_data_InfoCaseString = sw.ToString();
                                writer.Close();
                                sw.Close();

                                //item_data_InfoCaseString = JsonConvert.SerializeObject(dt_InfoCase, Formatting.Indented);

                                return item_data_InfoCaseString.Replace("\\\"", "'").Replace("\"\"", "\"null\"");
                             }
                        }

                        if (Qspdfcvsf_ == 2)
                        {
                            using (var command_1 = new SqlCommand("Export_Info_Test_Case_CR_SP", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 2000 })
                            {
                                command_1.Parameters.Clear();

                                con.Open();
                                dt_InfoTest.Load(command_1.ExecuteReader());
                                command_1.Parameters.Clear();
                                con.Close();

                                Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();
                                json.NullValueHandling = NullValueHandling.Ignore;
                                json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                                json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                                json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                json.NullValueHandling = NullValueHandling.Include;
                                json.Converters.Add(new DataTableConverter());
                                StringWriter sw = new StringWriter();
                                Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
                                writer.Formatting = Formatting.Indented;
                                writer.QuoteChar = '"';
                                json.Serialize(writer, dt_InfoTest);

                                item_data_InfoTestString = sw.ToString();
                                writer.Close();
                                sw.Close();

                                //item_data_InfoTestString = JsonConvert.SerializeObject(dt_InfoTest, Formatting.Indented);
                                ////item_data_InfoTest_Dynamic = jsInfoINCIENSA.Deserialize<dynamic>(item_data_InfoTestString);

                                return item_data_InfoTestString.Replace("\\\"", "'").Replace("\"\"", "\"null\"");
                            }
                        }

                        if (Qspdfcvsf_ == 3)
                        {
                            using (var command_2 = new SqlCommand("Export_INCIENSA_CR_SP", con) { CommandType = CommandType.StoredProcedure, CommandTimeout = 2000 })
                            {
                                command_2.Parameters.Clear();

                                con.Open();
                                dt_InfoINCIENSA.Load(command_2.ExecuteReader());
                                command_2.Parameters.Clear();
                                con.Close();


                                Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer() ;
                                json.NullValueHandling = NullValueHandling.Ignore;
                                json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                                json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
                                json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                json.NullValueHandling = NullValueHandling.Include;
                                json.Converters.Add(new DataTableConverter());
                                StringWriter sw = new StringWriter();
                                Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
                                writer.Formatting = Formatting.Indented;
                                writer.QuoteChar = '"';
                                json.Serialize(writer, dt_InfoINCIENSA);

                                item_data_InfoINCIENSAString = sw.ToString();
                                writer.Close();
                                sw.Close();
                                //item_data_InfoINCIENSAString = JsonConvert.SerializeObject(dt_InfoINCIENSA, Formatting.Indented);
                                ////item_data_InfoINCIENSA_Dynamic = jsInfoINCIENSA.Deserialize<dynamic>(item_data_InfoINCIENSAString);

                                return item_data_InfoINCIENSAString.Replace("\\\"", "'").Replace("\"\"", "\"null\"");

                            }
                        }

                        //jsondata_app.Add(new
                        //{
                        //    Info_PAHOFLUCase = item_data_InfoCase_Dynamic,
                        //    Info_PAHOFLUTest =  item_data_InfoTest_Dynamic,
                        //    Info_PAHOFLUINCIENSA = item_data_InfoINCIENSA_Dynamic
                        //});

                        //var JSONStr = JsonConvert.SerializeObject(jsondata_app, Formatting.Indented);

                        //return Json(jsondata_app, JsonRequestBehavior.AllowGet);
                        //string resultat_Json = JsonConvert.SerializeObject(jsondata_app, Formatting.Indented);

                        return null;
                    }
            //}
            }
            catch (Exception e)
            {
                return  "+++  El reporte no se pudo generar, por favor intente de nuevo: " + e.Message + " ---";
            }

            return null;

        }

    }
}
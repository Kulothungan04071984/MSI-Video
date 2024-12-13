

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MSI.Models;
using NuGet.Protocol.Plugins;
using System.Configuration;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace MSI.Controllers
{
    public class SystemController : Controller
    {
        private readonly string ConnectionString;
        public SystemController(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("conn");

        }


        public IActionResult Index()
        {
            var datalist = GetData() ?? new List<Systemid>();
            return View(datalist);
        }


        [HttpPost]
        public JsonResult Delete(string SystemId)
        {
            int resultdel = 0;
            try
            {
                resultdel = deleteSystemid(SystemId);
            }

            catch (Exception ex)
            {
                resultdel = 0;
            }
            return Json(resultdel);
        }
        [HttpPost]  
        public JsonResult Savedata(string SystemId,string Usertype)
        {
            try
            {

                var result = SaveDataToDatabase(SystemId,Usertype);
                
                

                if (result[0].Insert_status == 1) 
                {
                    Index();
                    return Json(new { success = true, Message = "System Add successflly" });
                   
                }
                else if (result[0].Insert_status == 0)
                {
                    Index();
                    return Json(new { success = false, Message = "The System is Already Added " });
                }
                else

                {
                    Index();
                    return Json(new { success = false, Message = "System is not Added" });
                }

              
            }
            catch (Exception ex)
            {
                return Json(new { success =false, Message = "Invalid Data"});
            }

            
        }
        public List<insert_stat> SaveDataToDatabase(string SystemId ,string Usertype)
        {
           // var dt = DateTime.Today;
            var insertlist= new List<insert_stat>();
           
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_Insert_SystemId", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SystemId", SystemId);
                    cmd.Parameters.AddWithValue ("@Usertype", Usertype);
                    connection.Open();
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                insertlist.Add(new insert_stat
                                {
                                    Insert_status = reader.GetInt32(0)
                                });
                            }
                        }

                    }
                    connection.Close();
                }
              

            }
            return insertlist;
        }
        public List<Systemid> GetData()
        {
            var dataList = new List<Systemid>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_GetSystem_Id", connection))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dataList.Add(new Systemid
                                {
                                    Id = reader.GetString(0),
                                    SystemId = reader.GetString(1),
                                });
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return dataList;
        }
        
        public int deleteSystemid( string deletesystemId) 
        {
            int result = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("pro_delete_ipaddress", connection))
                {
                    cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.AddWithValue("@SystemId", deletesystemId);
                    result= cmd.ExecuteNonQuery();
                    connection.Close(); 
                }
            } 
            return result;
        }
    }
} 
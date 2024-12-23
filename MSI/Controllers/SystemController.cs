

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
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace MSI.Controllers
{
    public class SystemController : Controller
    {
        private readonly DataManagementcs _ipAddress;
        public SystemController(DataManagementcs ipAddress)
        {
            _ipAddress = ipAddress;
        }

        public IActionResult Index()
        {
           var datalist = _ipAddress.GetData() ?? new List<Systemid>();
            return View(datalist);
        }
        [HttpPost]
        public JsonResult Delete(string SystemId)
        {
            int resultdel = 0;
            try
            {
                resultdel = _ipAddress.deleteSystemid(SystemId);
            }

            catch (Exception ex)
            {
                resultdel = 0;
            }
            return Json(resultdel);
        }
        [HttpPost]
        public JsonResult Savedata(string SystemId, string Usertype,string StageName)
        {
            try
            {
                if (Usertype == "Admin")        
                    Usertype = "1";
                else                           
                    Usertype = "2";
                
                    var insertlist = _ipAddress.SaveDataToDatabase(SystemId,Usertype,StageName);

                if (insertlist == 1)
                {
                    Index();
                    return Json(new { success = true, Message = "System Add successfully" });

                }
                else if (insertlist == 5)
                {
                    Index();
                    return Json(new { success = false, Message = "The System is Already Added " });
                }
                else

                {
                    Index();
                    return Json(new { success = false, Message = "Error Inserting The Value" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = "Invalid Data" });
            }
        }
            [HttpPost]
            public JsonResult Updatedata(string SystemId, string Usertype,string StageName, string Updatesystemid,string Updateusertype,string UpdateStageName)
            {
            
                try
                {
                   if(Usertype=="Admin")
                     Usertype = "1";
                  else                  
                     Usertype = "2";                                
                   if (Updateusertype == "Admin")                  
                     Updateusertype = "1";                                
                   else           
                    Updateusertype = "2";
                
                    var updateresult = _ipAddress.UpdateDataToDatabase(SystemId, Usertype, StageName, Updatesystemid, Updateusertype, UpdateStageName);


                    if (updateresult == 1)
                    {
                        Index();
                        return Json(new { success = true, Message = "System Updated successfully" });

                    }
                    else if (updateresult == 5)
                    {
                        Index();
                        return Json(new { success = false, Message = "The System is Already Added " });
                    }
                    else

                    {
                        Index();
                        return Json(new { success = false, Message = "Error Updating The Value" });
                    }


                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Message = "Invalid Data" });
                }


            }

    }
}

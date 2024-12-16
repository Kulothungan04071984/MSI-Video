

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
                resultdel =_ipAddress.deleteSystemid(SystemId);
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

                var result = _ipAddress.SaveDataToDatabase(SystemId,Usertype);
                
                

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
        
    }
} 
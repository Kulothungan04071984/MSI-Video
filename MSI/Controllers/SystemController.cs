

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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


            var objSystem = new Systemid
            {
                lstcustomname = _ipAddress.getcustomernames(),
                lstaddsystem = datalist,
                lstfgno = new List<SelectListItem>()
            };

            return View(objSystem);
            // var datalist = _ipAddress.GetData() ?? new List<Systemid>();
            // var model = new Systemid
            // {
            //     lstaddsystem = datalist
            // };
            // return View(datalist);
        }
        [HttpGet]
        public JsonResult GetFgNamesByCustomer(int customerId)
        {
            try
            {
                // Get the list of FG Names for the selected customer
                var fgNames = _ipAddress.getfgnames(customerId);

                // Return the list of FG Names as JSON
                return Json(fgNames);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
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
        public JsonResult Savedata(string SystemId, string Usertype, string StageName, string customername, string fgno)
        {
            try
            {
                if (Usertype == "Admin")
                    Usertype = "1";
                else
                    Usertype = "2";

                var insertlist = _ipAddress.SaveDataToDatabase(SystemId, Usertype, StageName, customername, fgno);

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
        public JsonResult Updatedata(string SystemDetailsid, string SystemId, string Usertype, string StageName, string Updatesystemid, string Updateusertype, string UpdateStageName)
        {

            try
            {
                if (Usertype == "Admin")
                    Usertype = "1";
                else
                    Usertype = "2";
                if (Updateusertype == "Admin")
                    Updateusertype = "1";
                else
                    Updateusertype = "2";

                //var updateresult = _ipAddress.UpdateDataToDatabase(SystemId, Usertype, StageName, Updatesystemid, Updateusertype, UpdateStageName);
                var updateresult = _ipAddress.UpdateDataToDatabase(SystemDetailsid, Updatesystemid, Updateusertype, UpdateStageName);


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

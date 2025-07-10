using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using MSI.Models;
using Microsoft.AspNetCore.StaticFiles;
using MSI.Models;


namespace MSI.Controllers
{
    public class ApprovedController : Controller
    {
        private readonly DataManagementcs _approvedData;
        public ApprovedController(DataManagementcs approvedData)
        {
            _approvedData = approvedData;
        }

        public IActionResult ApprovedDetails()
        {
          
            FileApprovedData objapprove = new FileApprovedData();
            objapprove.lstapprovecustomers = _approvedData.approvedGetCustomer();
            objapprove.lstapprovefgnames = new List<SelectListItem>();
            objapprove.lstapprovedata = _approvedData.GetApprovedData();
            return View(objapprove);
        }

        [HttpGet]
        public JsonResult GetFgNamesByCustomer(int customerId)
        {
            try
            {
                // Get the list of FG Names for the selected customer
                var approvefgNames = _approvedData.approveGetFgnames(customerId);

                // Return the list of FG Names as JSON
                return Json(approvefgNames);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }
        [HttpGet]
        public JsonResult Getfiledetails(string customerId,string fgid)
        {
            FileApprovedData fileapproveddetails = new FileApprovedData();
            try 
            {
                fileapproveddetails.lstapprovedata = _approvedData.GetApprovedDatabyfg(customerId, fgid);
                return Json(fileapproveddetails.lstapprovedata);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return Json(fileapproveddetails.lstapprovedata);
            }

        }

  




    }
}      

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using MSI.Models;
using Microsoft.AspNetCore.StaticFiles;


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
        public ActionResult Download(string fileName)
        {
            string decodedFileName = Uri.UnescapeDataString(fileName);
            // Assuming the files are stored in a folder on the server
            string fileDirectory = @"\\192.168.1.188\MSI_Videos\uploads"; // Change this to the actual folder where your files are stored
            string filePath = Path.Combine(fileDirectory, decodedFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Get the file data
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileExtension = Path.GetExtension(filePath);

            // Return the file as a download
            return File(fileBytes, "application/octet-stream", decodedFileName);
        }
    }
}

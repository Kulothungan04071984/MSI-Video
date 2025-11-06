using Microsoft.AspNetCore.Mvc;
using MSI.Models;

namespace MSI.Controllers
{
    public class DocVerifiedController : Controller
    {
        public readonly DataManagementcs _dataManagementcs;
        public DocVerifiedController(DataManagementcs dataManagementcs)
        {
            _dataManagementcs = dataManagementcs;
        }
        public IActionResult ShowDocDetails()
        {
            var resultDocDetails = _dataManagementcs.getDocVerifiedList();
            return View(resultDocDetails);
        }

        public JsonResult docStatus(string Docid,string filePath)
        {
            var resultUpdateStatus = _dataManagementcs.updateDocStatus(Convert.ToInt32(Docid),filePath);
            return Json(resultUpdateStatus);
        }

        public JsonResult docRejectStatusUpdate(string Docid, string RejectReason,string filePath)
        {
            var result = _dataManagementcs.updateDocRejectDetails(Convert.ToInt32(Docid), RejectReason, filePath);
            return Json(result);
        }

        public JsonResult getPdfFileName(string filepath)
        {
            var resultfilename=_dataManagementcs.pdfFileCopyfromServer(filepath);
            if( string.IsNullOrEmpty(resultfilename))
            {
                ViewBag.ErrorMessage = "File Not Found";
                return Json("File Not Found");
            }
            return Json(resultfilename);
        }

        public ActionResult Download(string fileName)
        {

            string decodedFileName = Uri.UnescapeDataString(fileName);
            // Assuming the files are stored in a folder on the server
            //string fileDirectory = @"\\192.168.1.121\MSI_Applications\uploads"; // Change this to the actual folder where your files are stored
            string filePath = decodedFileName;
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

        [HttpPost]
        public JsonResult deleteFileMapping(int documentId)
        {
            int resultdel = 0;
            try
            {
                resultdel = _dataManagementcs.deleteFileMapping1(documentId);
            }

            catch (Exception ex)
            {
                resultdel = 0;
            }
            return Json(resultdel);
        }


    }
}

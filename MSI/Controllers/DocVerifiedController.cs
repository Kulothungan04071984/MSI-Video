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
    }
}

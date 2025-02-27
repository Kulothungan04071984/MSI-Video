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

        public JsonResult docStatus(string Docid)
        {
            var resultUpdateStatus = _dataManagementcs.updateDocStatus(Convert.ToInt32(Docid));
            return Json(resultUpdateStatus);
        }

        public JsonResult docRejectStatusUpdate(string Docid, string RejectReason)
        {
            var result = _dataManagementcs.updateDocRejectDetails(Convert.ToInt32(Docid), RejectReason);
            return Json(result);
        }
    }
}

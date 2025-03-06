using Microsoft.AspNetCore.Mvc;
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
            var approvedlist = _approvedData.GetApprovedData() ?? new List<FileApprovedData>();
            return View(approvedlist);
        }
        
    }
}

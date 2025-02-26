using Microsoft.AspNetCore.Mvc;
using MSI.Models;

namespace MSI.Controllers
{
    public class DocVerifiedController : Controller
    {
        public readonly DataManagementcs _dataManagementcs;
        public DocVerifiedController(DataManagementcs dataManagementcs)
        {
                _dataManagementcs= dataManagementcs;
        }
        public IActionResult ShowDocDetails()
        {
            var resultDocDetails = _dataManagementcs.getDocVerifiedList();
            return View(resultDocDetails);
        }
    }
}

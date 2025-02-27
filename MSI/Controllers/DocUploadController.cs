using Microsoft.AspNetCore.Mvc;

namespace MSI.Controllers
{
    public class DocUploadController : Controller
    {
        public IActionResult ShowuploadDetails()
        {
            return View();
        }
    }
}

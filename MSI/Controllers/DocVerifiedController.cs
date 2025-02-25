using Microsoft.AspNetCore.Mvc;

namespace MSI.Controllers
{
    public class DocVerifiedController : Controller
    {
        public IActionResult ShowDocDetails()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using MSI.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;


namespace MSI.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataManagementcs _dataAccess;
        public LoginController(DataManagementcs dataAccess)
        {
            _dataAccess = dataAccess;

        }
        public IActionResult Login()
        {
            return View();
        }

        public JsonResult loginValidation(string userid, string password)
        {
            string res = string.Empty;
            var result = _dataAccess.getLoginDetails(userid, password);
            if (result != null)
            {
                if (result.Rows.Count > 0)
                {
                    if (result.Rows[0][0].ToString() == userid && result.Rows[0][1].ToString() == password)
                    {
                        if (result.Rows[0][2].ToString() == "2")
                            res = "valid-user";
                        else
                            res = "valid";
                    }
                    else
                        res = "invalid";
                }
                else
                    res = "invalid";

            }
            else if (result == null)
                res = "invalid";
            // res = "valid"; //Testing
            return Json(res);
        }
        [HttpPost]
        public JsonResult UpdatePassword(string userid, string password)
        {
            try
            {
                var result = _dataAccess.UpdatePassword(userid, password);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
    }
}



using Microsoft.AspNetCore.Mvc;
using MSI.Models;
using System;

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

        // This is the login validation method
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
                        string userType = result.Rows[0][2].ToString(); // 1, 2, or 3
                        if (userType == "1")
                        {
                            res = "Prod_Admin";
                            ViewData["LayoutType"] = "1"; // Admin layout
                        }
                        else if (userType == "2")
                        {
                            res = "Prod_User";
                            ViewData["LayoutType"] = "2"; // User layout
                        }
                        else if (userType == "3")
                        {
                            res = "Doc_Dept";
                            ViewData["LayoutType"] = "3"; // Document Department layout
                        }
                        else
                        {
                            res = "QA_Dept";
                            ViewData["LayoutType"] = "4"; // QA Department layout
                        }
                        // Store the user type in session for later use in the layout
                        HttpContext.Session.SetString("UserType", userType);

                        // Store the layout type in session for persistence across views
                        HttpContext.Session.SetString("LayoutType", ViewData["LayoutType"].ToString());

                        return Json(res); // successful login
                    }
                    else
                    {
                        res = "invalid";
                    }
                }
                else
                {
                    res = "invalid"; // No rows found
                }
            }
            else
            {
                res = "invalid"; // No result from database
            }

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

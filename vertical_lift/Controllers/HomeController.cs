using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View();

        }

       
        [HttpPost]
        public ActionResult Login1(string UserId, string PW)
        {
            using (S1542Entities db = new S1542Entities())
            {
              /*  string dec = HomeController.Decrypt(PW, "mskl-1sv4-xklq23");*/
                // Retrieve user by UserId and PW
                var user = db.UserMasters.FirstOrDefault(u => u.UserId == UserId && u.PW == PW);

                if (user != null && user.DeleteStatus == "Y")
                {
                    user.LastLogin = DateTime.Now;
                    db.SaveChanges();
                    // Store user information in the session
                    Session["UserId"] = user.UserId;
                    Session["UserType"] = user.UserType;
                    Session["UserName"] = user.UserName;
                    Session["LineNos"] = user.LineNos;
                    Session["FloorNo"] = user.FloorNo;
                    Session["LastLogin"] = user.LastLogin?.ToString("dd-MMM-yyyy hh:mm tt");

                    return Json(new
                    {
                        success = true,
                        message = "Login successful!",
                        redirectUrl = Url.Action("Index", "Home")
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (user != null && user.DeleteStatus == "N")
                {
                    return Json(new
                    {
                        block = true,
                        message = "Please Contact Admin.",
                        redirectUrlblock = Url.Action("Login", "Home")
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        invalid = true,
                        message = "Invalid username or password.",
                        redirectUrlinvalid = Url.Action("Login", "Home")
                    }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        public ActionResult Index()
        {

            if (Session["UserId"] != null)
            {
                ViewBag.UserName = Session["UserName"];
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }

        public ActionResult Logout()
        {
            Session.Clear();  
            Session.Abandon();

            return RedirectToAction("Login", "Home");
        }

        /*public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }*/
    }
}
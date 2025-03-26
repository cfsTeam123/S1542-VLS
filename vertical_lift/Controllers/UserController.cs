using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            using (S1542Entities db = new S1542Entities())
            {

                var users = db.UserMasters
                              .Where(s => s.UserName != null && s.DeleteStatus == "Y")
                              .Select(u => new { u.UserId }) 
                              .ToList();
            
                    ViewBag.UserList = new SelectList(users, "UserId", "UserId");
               
            }

            return View();
        }

        // Fetch Username Based on UserID
        public JsonResult GetUserName(string userId) 
        {
            using (S1542Entities db = new S1542Entities())
            {
                var user = db.UserMasters.FirstOrDefault(u => u.UserId == userId);

                if (user != null)
                {
                    return Json(new { success = true, userName = user.UserName }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "User not found." }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult AddEdit(int MTransNo = 0)
        {
            S1542Entities db = new S1542Entities();
            /*UserMasterValidation b = new UserMasterValidation();*/

            ViewBag.UserType = new List<SelectListItem>(){
                new SelectListItem {Text="ADMIN", Value="ADMIN", Selected=false},
                new SelectListItem {Text="SUPERVISOR", Value="SUPERVISOR", Selected=false},
                new SelectListItem {Text="OPERATOR", Value="OPERATOR", Selected=false},
                };

            ViewBag.FloorNo = new List<SelectListItem>(){
                new SelectListItem {Text="Ground Floor", Value="Ground Floor", Selected=false},
                new SelectListItem {Text="Floor 1", Value="Floor 1", Selected=false},
                new SelectListItem {Text="Floor 2", Value="Floor 2", Selected=false},
                };

            ViewBag.LineNos = new List<SelectListItem>(){
                new SelectListItem {Text="1", Value="1", Selected=false},
                new SelectListItem {Text="2", Value="2", Selected=false},
                new SelectListItem {Text="3", Value="3", Selected=false},
                new SelectListItem {Text="4", Value="4", Selected=false},
                };
            ViewBag.DeleteStatus = new List<SelectListItem>(){
                new SelectListItem {Text="Active", Value="Y", Selected=false},
                new SelectListItem {Text="Inactive", Value="N", Selected=false},
                };

            var data = db.UserMasters.Where(s => s.MTransNo == MTransNo).FirstOrDefault();
            if (MTransNo > 0)
            {
                UserMasterValidation user = new UserMasterValidation();
                user.MTransNo = MTransNo;
                user.UserType = data.UserType;
                user.UserName = data.UserName;
                user.EmailId = data.EmailId;
                user.LineNos = data.LineNos;
                user.FloorNo = data.FloorNo;
                user.ContactNo = data.ContactNo;
                user.DeleteStatus = data.DeleteStatus;
                user.UserId = data.UserId;
                user.PW = data.PW;
                return View(user);
            }
            else
            {
                UserMaster user = new UserMaster();
                ViewBag.UserType = new List<SelectListItem>(){
                new SelectListItem {Text="ADMIN", Value="ADMIN", Selected=false},
                new SelectListItem {Text="SUPERVISOR", Value="SUPERVISOR", Selected=false},
                new SelectListItem {Text="OPERATOR", Value="OPERATOR", Selected=false},
                };

                ViewBag.FloorNo = new List<SelectListItem>(){
                new SelectListItem {Text="Ground Floor", Value="Ground Floor", Selected=false},
                new SelectListItem {Text="Floor 1", Value="Floor 1", Selected=false},
                new SelectListItem {Text="Floor 2", Value="Floor 2", Selected=false},
                };

                ViewBag.LineNos = new List<SelectListItem>(){
                new SelectListItem {Text="1", Value="1", Selected=false},
                new SelectListItem {Text="2", Value="2", Selected=false},
                new SelectListItem {Text="3", Value="3", Selected=false},
                new SelectListItem {Text="4", Value="4", Selected=false},
                };

                ViewBag.DeleteStatus = new List<SelectListItem>(){
                new SelectListItem {Text="Active", Value="Y", Selected=false},
                new SelectListItem {Text="Inactive", Value="N", Selected=false},
                };
            }
            return View();
        }




        public ActionResult AddEdit1(UserMasterValidation data)
        {
            S1542Entities db = new S1542Entities();
            UserMasterValidation b = new UserMasterValidation();

            try
            {
                if (data.UserName != null)
                {
                    if (data.MTransNo > 0)
                    {
                        // Edit existing BinMaster
                        UserMaster user = db.UserMasters.Where(s => s.MTransNo == data.MTransNo).FirstOrDefault();
                        int? ModifiedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        if (user != null)
                        {
                            bool barcodeExists = db.UserMasters.Any(x => x.UserId == data.UserId && x.MTransNo != data.MTransNo);

                            if (barcodeExists)
                            {
                                return Json("userExist1");
                            }
                            user.LineNos = data.LineNos;
                            user.UserName = data.UserName;
                            user.ContactNo = data.ContactNo;
                            user.EmailId = data.EmailId;
                            user.FloorNo = data.FloorNo;
                            user.DeleteStatus = data.DeleteStatus;
                            user.PW = data.PW;
                            user.ModifiedBy = ModifiedBy;
                            user.ModifiedOn = DateTime.Now;
                          

                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            // Handle the case where the MTransNo doesn't exist in the database
                            ModelState.AddModelError("", "The specified MTransNo does not exist.");
                            return View(b);
                        }
                    }
                    else
                    {
                        bool barcodeExists = db.UserMasters.Any(x => x.UserId == data.UserId);
                        if (barcodeExists)
                        {
                            return Json("userExist");
                        }
                        //string encVa = HomeController.Encrypt(data.PW, "mskl-1sv4-xklq23");
                        int? createdBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        UserMaster UserMaster = new UserMaster
                        {
                            UserType = data.UserType,
                            LineNos = data.LineNos,
                            UserName = data.UserName,
                            ContactNo = data.ContactNo,
                            EmailId = data.EmailId,
                            FloorNo = data.FloorNo,
                            DeleteStatus = data.DeleteStatus,
                            UserId = data.UserId,
                            PW = data.PW,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.Now,
                            LockStatus="N"
                    };
                        db.UserMasters.Add(UserMaster);
                        db.SaveChanges();
                    }
                }

                // Optionally, redirect after the operation, or you can stay on the same view
                return RedirectToAction("AddEdit", "User");
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary) and return an error message
                ModelState.AddModelError("", "An error occurred while saving the data: " + ex.Message);
                return View(b);
            }

        }

        public ActionResult Delete(int MTransNo)
        {
            S1542Entities db = new S1542Entities();
            UserMasterValidation q = new UserMasterValidation();
            try
            {
                var item = db.UserMasters.FirstOrDefault(a => a.MTransNo == MTransNo);
                if (item != null && item.DeleteStatus == "N")
                {
                    item.LockStatus = "Y";

                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}
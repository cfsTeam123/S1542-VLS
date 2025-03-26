using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class MaterialMasterController : Controller
    {
        // GET: MaterialMaster
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddEdit(int MTransNo = 0)
        {
            S1542Entities db = new S1542Entities();
           //add material status
            ViewBag.Status = new List<SelectListItem>(){
                    new SelectListItem {Text="Inactive", Value="N", Selected=false},
                    new SelectListItem {Text="Active", Value="Y", Selected=false},
                    };
            var data = db.MaterialMasters.Where(s => s.MTransNo == MTransNo).FirstOrDefault();

            //click on edit button auto deta fetch 
            if (MTransNo > 0)
            {
                MaterialMasterValidation materials = new MaterialMasterValidation();
                materials.MTransNo = MTransNo;
                materials.MaterialName = data.MaterialName;
                materials.Description = data.Description;
                materials.Status = data.Status;
                return View(materials);
            }
            else
            {
                //MaterialMasterValidation materials = new MaterialMasterValidation();
                //ViewBag.Status = new List<SelectListItem>(){
                //    new SelectListItem {Text="Inactive", Value="N", Selected=false},
                //    new SelectListItem {Text="Active", Value="Y", Selected=false},
                //    };
            }
            return View();
        }

        public ActionResult AddEdit1(MaterialMasterValidation data)
        {
            S1542Entities db = new S1542Entities();
            MaterialMasterValidation b = new MaterialMasterValidation();
            try
            {
                if (data.MaterialName != null)
                {
                    //click on update button (update code)
                    if (data.MTransNo > 0)
                    {
                        // Edit existing BinMaster
                        MaterialMaster materialMaster = db.MaterialMasters.FirstOrDefault(x => x.MTransNo == data.MTransNo);
                        int? modifiedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        if (materialMaster != null)
                        {
                            materialMaster.Status = data.Status;
                            materialMaster.ModifiedBy = modifiedBy;
                            materialMaster.ModifiedOn = DateTime.Now;
                            db.Entry(materialMaster).State = System.Data.Entity.EntityState.Modified;
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
                        // Add new Material 
                        int? createdBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        MaterialMaster materialmaster = new MaterialMaster
                        {
                            MaterialName = data.MaterialName,
                            Description = data.Description,
                            Status = data.Status,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.Now,
                            DeleteStatus = "N"
                        };
                        db.MaterialMasters.Add(materialmaster);
                        db.SaveChanges();
                    }
                }

                // Optionally, redirect after the operation, or you can stay on the same view
                return RedirectToAction("AddEdit", "BinMaster");
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary) and return an error message
                ModelState.AddModelError("", "An error occurred while saving the data: " + ex.Message);
                return View(b);
            }
        }

        //Delete material
        public ActionResult Delete(int MTransNo)
        {
            S1542Entities db = new S1542Entities();
            MaterialMasterValidation q = new MaterialMasterValidation();
            try
            {
                var item = db.MaterialMasters.FirstOrDefault(a => a.MTransNo == MTransNo);
                if (item != null && item.Status == "N")
                {
                    item.DeleteStatus = "Y";
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
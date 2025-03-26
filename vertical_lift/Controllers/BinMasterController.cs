using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class BinMasterController : Controller
    {
       
        // GET: BinMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEdit(int MTransNo = 0)
        {
          
            S1542Entities db = new S1542Entities();
            //BinMasterValidation b = new BinMasterValidation();
            ViewBag.BinType = new List<SelectListItem>(){
                    new SelectListItem {Text="Type A", Value="Type A", Selected=false},
                    new SelectListItem {Text="Type B", Value="Type B", Selected=false},
                    new SelectListItem {Text="Type C", Value="Type C", Selected=false},
                    };
           /* ViewBag.dimension = new List<SelectListItem>(){
                    new SelectListItem {Text="300 × 400 × 150", Value="300 × 400 × 150", Selected=false},
                    new SelectListItem {Text="500 × 325 × 200", Value="500 × 325 × 200", Selected=false},
                    new SelectListItem {Text="650 × 450 × 325", Value="650 × 450 × 325", Selected=false},
                    };*/
            ViewBag.status = new List<SelectListItem>(){
                    new SelectListItem {Text="Inactive", Value="N", Selected=false},
                    new SelectListItem {Text="Active", Value="Y", Selected=false},
                    };
            var data = db.BinMasters.Where(s => s.MTransNo == MTransNo).FirstOrDefault();
            if (MTransNo > 0)
            {
               
                BinMasterValidation binMaster = new BinMasterValidation();
                binMaster.MTransNo = MTransNo;
                binMaster.BinType = data.BinType;
                binMaster.barcode = data.barcode;
                binMaster.dimension = data.dimension;
                binMaster.status = data.status;
                return View(binMaster);
            }
            else
            {
                BinMaster binMaster = new BinMaster();
                ViewBag.BinType = new List<SelectListItem>(){
                    new SelectListItem {Text="Type A", Value="Type A", Selected=false},
                    new SelectListItem {Text="Type B", Value="Type B", Selected=false},
                    new SelectListItem {Text="Type C", Value="Type C", Selected=false}, 
                    };
              /*  ViewBag.dimension = new List<SelectListItem>(){
                    new SelectListItem {Text="Select Dimension", Value="", Selected=true},
                    new SelectListItem {Text="300 × 400 × 150", Value="300 × 400 × 150", Selected=false},
                    new SelectListItem {Text="500 × 325 × 200", Value="500 × 325 × 200", Selected=false},
                    new SelectListItem {Text="650 × 450 × 325", Value="650 × 450 × 325", Selected=false},
                    };*/
                ViewBag.status = new List<SelectListItem>(){
                    new SelectListItem {Text="Inactive", Value="N", Selected=false},
                    new SelectListItem {Text="Active", Value="Y", Selected=false},
                    };
            }
            return View();
        }





        public ActionResult AddEdit1(BinMasterValidation data)
        {
            S1542Entities db = new S1542Entities();
            BinMasterValidation b = new BinMasterValidation();

            try
            {
                if (data.barcode != null)
                {
                    if (data.MTransNo > 0)
                    {
                        // Edit existing BinMaster
                        BinMaster binMaster = db.BinMasters.FirstOrDefault(x => x.MTransNo == data.MTransNo);
                        int? modifiedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        if (binMaster != null)
                        {
                            bool barcodeExists = db.BinMasters.Any(x => x.barcode == data.barcode && x.MTransNo != data.MTransNo);

                            if (barcodeExists)
                            {
                                return Json("BCExist1");
                            }
                            binMaster.status = data.status;
                            binMaster.barcode = data.barcode;
                            binMaster.ModifiedBy = modifiedBy;
                            binMaster.ModifiedOn = DateTime.Now;
                            db.Entry(binMaster).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            // Handle the case where the MTransNo doesn't exist in the database
                           // ModelState.AddModelError("", "The specified MTransNo does not exist.");

                            return Json("Not Found");
                        }
                    }
                    else
                    {
                        // Check if barcode already exists
                        bool barcodeExists = db.BinMasters.Any(x => x.barcode == data.barcode);
                        if (barcodeExists)
                        {
                            return Json("BCExist");
                        }

                        // Add new BinMaster
                        int? createdBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        BinMaster binMaster = new BinMaster
                        {
                            BinType = data.BinType,
                            dimension = data.dimension,
                            status = data.status,
                            barcode = data.barcode,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.Now,
                            DeleteStatus = "N"
                        };
                        db.BinMasters.Add(binMaster);
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

        public ActionResult Delete(int MTransNo)
        {
            using (S1542Entities db = new S1542Entities())
            {
                try
                {
                    var item = db.BinMasters.FirstOrDefault(a => a.MTransNo == MTransNo);

                    if (item != null && item.status == "N") 
                    {
                        item.DeleteStatus = "Y"; 

                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        return Json(new { success = true });
                    }
                    return Json(new { success = false, message = "Record not found or status is not 'N'." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

    }

}

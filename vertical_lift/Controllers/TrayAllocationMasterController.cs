
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class TrayAllocationMasterController : Controller
    {
        // GET: TrayAllocationMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEdit()
        {
            using (S1542Entities db = new S1542Entities())
            {

                var BinType = db.Goods_Existing
                                .Where(s => s.QualityInspection == "NA" && s.Type =="Material")
                                .Select(u => new { u.BinType })
                                .Distinct()
                                .OrderBy(u => u.BinType) 
                                .ToList();

                ViewBag.BinType = new SelectList(BinType, "BinType", "BinType");

            }


            ViewBag.QualityInspection = new List<SelectListItem>(){
                    new SelectListItem {Text="Yes", Value="Yes", Selected=false},
                    new SelectListItem {Text="No", Value="No", Selected=false},
                    };


            return View();
        }
        public JsonResult GetDimension(string BinType)
        {
            try
            {
                using (S1542Entities db = new S1542Entities())
                {
                    if (string.IsNullOrEmpty(BinType))
                    {
                        return Json(new { success = false, message = "Bin Type is required." }, JsonRequestBehavior.AllowGet);
                    }

                    var dimensionData = db.Goods_Existing
                                          .Where(u => u.BinType == BinType)
                                          .Select(u => u.Dimension)
                                          .FirstOrDefault(); // Fetch Dimension

                    var sides = db.Goods_Existing
                                 .Where(g => g.BinType == BinType && g.QualityInspection == "NA" && g.Type == "Material")
                                 .Select(g => g.Side)
                                 .Distinct()
                                 .ToList();

                   

                   
                    if (dimensionData != null || sides.Any())
                    {
                        return Json(new
                        {
                            success = true,
                            Dimension = dimensionData,
                            sideNo = sides,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "No matching records found." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // FetchDimension and Tray no Based on Bin type
      
        public JsonResult GetTrays(string BinType, int Side)
        {
            try
            {
                using (S1542Entities db = new S1542Entities())
                {
                    if (string.IsNullOrEmpty(BinType) || Side == 0)
                    {
                        return Json(new { success = false, message = "Bin Type and Side are required." }, JsonRequestBehavior.AllowGet);
                    }

                    var trayNos = db.Goods_Existing
                                    .Where(t => t.BinType == BinType && t.Side == Side && t.QualityInspection == "NA" && t.Type == "Material")
                                    .Select(t => t.TrayNo)
                                    .Distinct()
                                    .OrderBy(t => t)
                                    .ToList();

                  

                    if (trayNos.Any())
                    {
                        return Json(new { success = true, TrayNos = trayNos }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "No Tray Numbers found for the selected Side." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult UpdateQualityInspection(TrayAllocation model)
        {
            using (S1542Entities db = new S1542Entities())
            {
                var trayRecords = db.Goods_Existing
                    .Where(t => t.Side == model.Side && t.TrayNo == model.TrayNo)
                    .ToList(); // Fetch all matching records

                if (trayRecords.Any()) // Check if any matching records exist
                {
                    foreach (var record in trayRecords)
                    {
                        record.QualityInspection = model.QualityInspection;
                    }

                    db.SaveChanges(); // Save all changes in one transaction

                    return Json(new { success = true, message = "Quality Inspection updated for all matching trays." });
                }
                else
                {
                    return Json(new { success = false, message = "Tray No not found." });
                }
            }
        }




    }
}
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;
using vertical_lift.Profinet;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace vertical_lift.Controllers
{
    public class LoadOperationController : Controller
    {
        //private PLC plc = null;
        //private ExceptionCode errCode;
        //private int close_flag = 0;
        //string plc_ip = "192.168.0.1";

        S1542Entities db = new S1542Entities();
        // GET: LoadOperation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadOperation()
        {
            return View();
        }
       

        public ActionResult LoadOperationWithGRN()
        {
                /*retrive Load type data*/
                var Loadtype = db.Goods_Existing
                   .Where(s => s.QualityInspection == "Yes"
                            && s.Status == "AVAILABLE"
                            && s.Type.Trim() != "Kitting")
                   .Select(u => new { u.Type })
                   .Distinct()
                   .ToList();

                ViewBag.Type = new SelectList(Loadtype, "Type", "Type");

            /*Retrive Bintype data*/
                var BinType = db.Goods_Existing
                                .Where(s => s.QualityInspection == "Yes"
                    && s.Status == "AVAILABLE"
                    && s.Type.Trim() != "Kitting")
                                .Select(u => new { u.BinType })
                                .Distinct()
                                .ToList();

                ViewBag.BinType = new SelectList(BinType, "BinType", "BinType");
          
                var Material = db.MaterialMasters
                                .Where(s => s.Status == "Y")
                                .Select(u => new { u.MaterialName })
                                /* .Distinct()*/
                                .ToList();

                ViewBag.Material = new SelectList(Material, "MaterialName", "MaterialName");

            
            return View();
        }
        //fetch bintype based on type
        public ActionResult GetBinTypes(string type)
        {
          
                var binTypes = db.Goods_Existing
                        .Where(g => g.Type == type && g.QualityInspection == "Yes"
                    && g.Status == "AVAILABLE")
                        .Select(g => new { Value = g.BinType, Text = g.BinType })
                        .Distinct()
                        .ToList();

                return Json(binTypes, JsonRequestBehavior.AllowGet);
            
        }
        // FetchDimension and Tray no Based on Bin type
        public JsonResult GetDimension(string BinType)
        {
            try
            {
                
                    if (string.IsNullOrEmpty(BinType))
                    {
                        return Json(new { success = false, message = "Bin Type is required." }, JsonRequestBehavior.AllowGet);
                    }

                    var dimensionData = db.Goods_Existing
                                          .Where(u => u.BinType == BinType)
                                          .Select(u => u.Dimension)
                                          .FirstOrDefault(); // Fetch only Dimension


                    if (dimensionData != null)
                    {
                        return Json(new
                        {
                            success = true,
                            Dimension = dimensionData,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "No matching records found." }, JsonRequestBehavior.AllowGet);
                    }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /* Get Nearest Available Tray  on call button*/
        public JsonResult GetNearestAvailableTray(string type, string binType, string dimension)
        {
           
                // Fetch the nearest available tray for Side 1 (LHS)
                var nearestSide1Tray = db.Goods_Existing
                    .Where(t => t.Status == "Available"
                        && t.QualityInspection == "Yes"
                        && t.Type.Trim() != "Kitting"
                        && t.Side == 1 // LHS
                        && (string.IsNullOrEmpty(type) || t.Type == type)
                        && (string.IsNullOrEmpty(binType) || t.BinType == binType)
                        && (string.IsNullOrEmpty(dimension) || t.Dimension == dimension))
                    .OrderBy(t => t.TrayNo)
                    .FirstOrDefault();

                // Fetch the nearest available tray for Side 2 (RHS)
                var nearestSide2Tray = db.Goods_Existing
                    .Where(t => t.Status == "Available"
                        && t.QualityInspection == "Yes"
                        && t.Type.Trim() != "Kitting"
                        && t.Side == 2 // RHS
                        && (string.IsNullOrEmpty(type) || t.Type == type)
                        && (string.IsNullOrEmpty(binType) || t.BinType == binType)
                        && (string.IsNullOrEmpty(dimension) || t.Dimension == dimension))
                    .OrderBy(t => t.TrayNo)
                    .FirstOrDefault();

                // Determine which tray is closer (lower TrayNo)
                var nearestTray = (nearestSide1Tray != null && nearestSide2Tray != null)
                    ? (nearestSide1Tray.TrayNo < nearestSide2Tray.TrayNo ? nearestSide1Tray : nearestSide2Tray)
                    : nearestSide1Tray ?? nearestSide2Tray;

                if (nearestTray == null)
                {
                    return Json(new { success = false, message = "No available trays found with the given criteria." }, JsonRequestBehavior.AllowGet);
                }

            //Communication with PLC  Click on try send command 
            //Communication with PLC  Click on try send command 
            //PLC plc = new PLC(CPU_Type.S71200, plc_ip, (short)0, (short)1);
            //errCode = plc.Open();

            //plc.Write("MW142", nearestTray.TrayNo);
            //plc.Write("MW144", nearestTray.Side);
            //bool data = plc.ReadBits(102, 1);//checkking %M102.1 this
            //plc.Close();
            bool data1 = true;

            return Json(new { success = true, tray = new { nearestTray.TrayNo, nearestTray.Status, nearestTray.Location, nearestTray.Side, data = data1 } }, JsonRequestBehavior.AllowGet);
           
        }

        //on enter barcode  check if bar code exist or not in DB
        [HttpGet]
        public JsonResult ValidateBarcode(int barcode, int TrayNo)
        {
            string status="";
            bool binExists = db.BinMasters.Any(b => b.barcode == barcode);
            if (binExists)
            {
                // bool goods = db.Goods_Existing.FirstOrDefault(g => g.BinBarcode == barcode);
                bool abc = db.Goods_Existing.Any(g => g.BinBarcode == barcode && g.Status == "LOADED");


                if (abc)
                {
                    status = "true";
                }
                else
                {
                    status = "false";
                }
            }
            
                return Json(new
                {
                    binExists,
                    status,
                    message = "Validation completed."
                }, JsonRequestBehavior.AllowGet);
            
        }






        /*Click on Add button all values insert into database load temp table*/
        [HttpPost]
        public JsonResult InsertTempData(Load_Temp_Table model)
        {
            
                try
                {
                    if (model != null)
                    {
                        // Get CreatedBy from session
                        int? CreatedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;
                        // Check if barcode exists in BinMaster table
                        bool binExists = db.BinMasters.Any(b => b.barcode == model.BinBarcode);

                        if (!binExists)
                            return Json("Barcode Not Match");

                        // Check if barcode already exists in Goods_Existing
                        var existingGoods = db.Goods_Existing
                                              .FirstOrDefault(g => g.BinBarcode == model.BinBarcode);

                        if (existingGoods != null)
                        {
                            // Barcode already assigned, don't modify again
                            model.BinNo = existingGoods.BinLocation ?? 0;
                        }
                        else
                        {
                            // Barcode not assigned yet, find empty BinBarcode
                            var emptyGoods = db.Goods_Existing
                                               .FirstOrDefault(g => g.BinBarcode == 0 && g.TrayNo == model.TrayNo  && g.Side ==model.Side);

                            if (emptyGoods != null)
                            {
                                // Assign barcode and update
                                emptyGoods.BinBarcode = model.BinBarcode;
                                emptyGoods.ModifiedOn = DateTime.Now;
                                emptyGoods.ModifiedBy = CreatedBy;

                                model.BinNo = emptyGoods.BinLocation ?? 0;
                            }
                            else
                            {
                                return Json("No empty BinBarcode found to assign.");
                            }
                        }

                        // Insert into Load_Temp_Table
                        db.Load_Temp_Table.Add(model);
                        db.SaveChanges();

                        return Json("BarcodeExist");
                    }
                    else
                    {
                        return Json(new { success = false, message = "Invalid data" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error: " + ex.Message });
                }
            
        }





        /*fetch data into table from DB Loadtemptable*/
        [HttpGet]
        public JsonResult FetchDataToTable()
        {
            

            var data = db.Load_Temp_Table
                         .Select(x => new
                         {
                             x.MTransNo,
                             x.BinNo,
                             x.BinBarcode,
                             x.GRNNO,
                             x.MaterialBarcode,
                             x.MaterialDesc,
                             x.BatchNo,
                             x.Style,
                             x.Qty,
                             x.MaxQty,
                         }).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /*Delete material from temp table*/
        public ActionResult DeleteMaterial(int MTransNo)
        {
           
                try
                {
                    var item = db.Load_Temp_Table.FirstOrDefault(a => a.MTransNo == MTransNo);
                    if (item != null)
                    {
                        db.Load_Temp_Table.Remove(item); 
                        db.SaveChanges();

                        return Json(new { success = true });
                    }
                    return Json(new { success = false, message = "Record not found" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
           
        }


        //save all data to database
        [HttpPost]
        public ActionResult SaveAllData(List<LoadTempTableValidation> dataList)
        {
            try
            {
                int? CreatedBy = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : (int?)null;

                // 1. Add each item to ExistingDetails
                foreach (var item in dataList)
                {
                    ExistingDetail detail = new ExistingDetail
                    {
                        BinLocation = item.BinNo,
                        BinBarcode = item.binbarcode,
                        GRNNO = item.GRNNO,
                        MaterialBarcode = item.materialbarcode,
                        MaterialDescription = item.MaterialDesc,
                        BatchNo = item.batchno,
                        Style = item.style,
                        Qty = item.qty,
                        BinType = item.BinType,
                        TrayNo = item.TrayNo,
                        Side = item.Side,
                        CreatedBy = CreatedBy,
                        CreatedOn = DateTime.Now,
                        InspectionType = "Yes",
                        Action = "LOAD MATERIAL",
                        MaxQty = item.MaxQty
                    };
                    db.ExistingDetails.Add(detail);

                    TransactionDetail detail1 = new TransactionDetail
                    {
                        BinLocation = item.BinNo,
                        BinBarcode = item.binbarcode,
                        MaterialDescription = item.MaterialDesc,
                        MaterialBarcode = item.materialbarcode,
                        BatchNo = item.batchno,
                        Style = item.style,
                        Qty = item.qty,
                        GRNNO = item.GRNNO,
                        BinType = item.BinType,
                        TrayNo = item.TrayNo,
                        Side = item.Side,
                        CreatedBy = CreatedBy,
                        CreatedOn = DateTime.Now,
                        InspectionType = "Yes",
                        Action = "LOAD MATERIAL",
                        MaxQty = item.MaxQty

                    };
                    db.TransactionDetails.Add(detail1);
                }

                // 2. Group by BinBarcode, TrayNo, and Side and sum qty
                var groupedData = dataList
     .GroupBy(d => new { d.binbarcode, d.TrayNo, d.Side, d.MaxQty })
     .Select(g => new
     {
         BinBarcode = g.Key.binbarcode,
         MaxQty = g.Key.MaxQty,
         TrayNo = g.Key.TrayNo,
         Side = g.Key.Side,
         TotalQty = g.Sum(x => x.qty)
     }).ToList();

                // 3. Update Goods_Existing for each BinBarcode + TrayNo + Side
                foreach (var group in groupedData)
                {
                    var existingBin = db.Goods_Existing.FirstOrDefault(b =>
                        b.BinBarcode == group.BinBarcode &&
                        b.TrayNo == group.TrayNo &&
                        b.Side == group.Side);

                    if (existingBin != null)
                    {
                        existingBin.Qty = (existingBin.Qty ?? 0) + group.TotalQty;
                        existingBin.Status = "LOADED";
                        existingBin.ModifiedBy = CreatedBy;
                        existingBin.ModifiedOn = DateTime.Now;

                        // Update MaxQty only if it's not set or needs updating
                        if (existingBin.MaxQty == null || existingBin.MaxQty < group.MaxQty)
                        {
                            existingBin.MaxQty = group.MaxQty;
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = $"Bin not found for BinBarcode: {group.BinBarcode}, TrayNo: {group.TrayNo}, Side: {group.Side}" });
                    }
                }


                // Save all changes once after processing
                db.SaveChanges();

                // Truncate Load_Temp_Table after successful save
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Load_Temp_Table");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error: " + ex.Message);
            }
        }


    }
}
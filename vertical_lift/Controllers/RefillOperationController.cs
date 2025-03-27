using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class RefillOperationController : Controller
    {
        // GET: RefillOperation
        S1542Entities conn = new S1542Entities();
        public ActionResult Refill()
        {
            //for GRN dropdown
            //ViewBag.GrnType = new List<SelectListItem>
            //    {
            //        new SelectListItem { Text = "With GRN", Value = "Yes",Selected=false },
            //        new SelectListItem { Text = "Without GRN", Value = "No",Selected=false },                   
            //    };

            var MaterialsAndBinTypes = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.InspectionType == "Yes")
                               .Select(u => new { u.MaterialDescription, u.BinType })
                               .Distinct()
                               .ToList();

            ViewBag.Material = new SelectList(MaterialsAndBinTypes, "MaterialDescription", "MaterialDescription");
            ViewBag.BinType = new SelectList("", "", "");
            ViewBag.BinNo = new SelectList("", "", "");
            return View();

        }

        ////still not ins used might use in future
        //public JsonResult GetGrnNo(string GrnNo)
        //{
        //    //fwtch grn details in dropown 
        //    var MaterialsAndBinTypes = conn.ExistingDetails
        //         .Where(x => x.Action == "LOAD MATERIAL" && x.InspectionType == GrnNo)
        //         .Select(u => new { u.MaterialDescription, u.BinType })
        //         .Distinct()
        //         .ToList();
        //    return Json(MaterialsAndBinTypes, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult tabledtl()
        {
            //gets bintype for dropdown
            var tabdtl = conn.Refill_Temp_Table.ToList();
            return Json(tabdtl, JsonRequestBehavior.AllowGet);
        }   
        public JsonResult DeleteItem(int mTransNo)
        {
         var itemToDelete = conn.Refill_Temp_Table
                                           .FirstOrDefault(x => x.MTransNo == mTransNo);
            if (itemToDelete != null)
            {
                conn.Refill_Temp_Table.Remove(itemToDelete);
                conn.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else{
                return Json("Record not found", JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetBinType(string Material)
        {
            //gets bintype for dropdown
            var BinType = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.MaterialDescription == Material && x.InspectionType == "Yes")
                               .Select(u => new { u.BinType })
                               .Distinct()
                               .ToList();
            return Json(BinType, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBinno(string BinType, string Material)
        {
            //fwtch grn details in dropown 
            var Binloc = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.MaterialDescription == Material && x.InspectionType == "Yes" && x.BinType == BinType)
                               .Select(u => new { u.BinLocation })
                               .Distinct()
                               .ToList();

            var getdiemension = conn.Goods_Existing.Where(x => x.BinType == BinType).Select(a => a.Dimension).FirstOrDefault();

            return Json(new { Binloc, getdiemension }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetDtl(string BinType, string material, int binlocation)
        {
            //get other datils based on materials
            var dtls = (from a in conn.ExistingDetails
                        join b in conn.Goods_Existing on a.BinBarcode equals b.BinBarcode
                        where a.MaterialDescription == material && a.BinLocation == binlocation
                        select new
                        {
                            b.Dimension,
                            a.Qty,
                            b.TrayNo,
                            b.Side,
                            b.MaxQty
                        }).FirstOrDefault();  // Executes the query and converts it into a list


            return Json(dtls, JsonRequestBehavior.AllowGet);  // Return as JSON
        }


        public JsonResult SaveDtl(string BinType, string material, int binlocation, string Dimension, int Avlqty, int Refilqty, int TrayNo, int side)
        {
            //all data take and update it to database
            //  var fetchdtls = conn.Database.ExecuteSqlCommand("select * FROM [S1542].[dbo].[Goods_Existing] where BinLocation= '"+binlocation+"' AND  Dimension='"+Dimension+"' AND  BinType='"+BinType+"' and TrayNo='"+TrayNo+"' And side='"+side+"'");
            var fetchdl = (from a in conn.ExistingDetails
                           join b in conn.Goods_Existing on a.TrayNo equals b.TrayNo
                           where a.Side == side & a.TrayNo == TrayNo && a.MaterialDescription == material && a.BinLocation == binlocation
                           select new
                           {
                               a,
                               b
                           }).FirstOrDefault();

            //check if same data is again comes for inserting
            var tempdtl = conn.Refill_Temp_Table.Where(x => x.TrayNo == TrayNo && x.Side == side && x.MaterialDesc == material && x.RefilQty == Refilqty && x.BinType == BinType).FirstOrDefault();
            if (tempdtl != null)
            {
                return Json("DataExistAlready", JsonRequestBehavior.AllowGet);
            }
            else if (fetchdl != null)
            {
                //data saved to temp table 
                Refill_Temp_Table Rtable = new Refill_Temp_Table();
                Rtable.MaterialDesc = material;
                Rtable.Side = side;
                Rtable.TrayNo = TrayNo;
                Rtable.BinType = BinType;
                Rtable.GRNNO = fetchdl.a.GRNNO;
                Rtable.BinBarcode = fetchdl.a.BinBarcode;
                Rtable.BinNo = binlocation;
                Rtable.MaterialBarcode = (int)fetchdl.a.MaterialBarcode;
                Rtable.BatchNo = fetchdl.a.BatchNo;
                Rtable.Style = fetchdl.a.Style;
                Rtable.AvlQty = Avlqty;
                Rtable.RefilQty = Refilqty;
                Rtable.Status = "Submit";
                conn.Refill_Temp_Table.Add(Rtable);
                conn.SaveChanges();

            }
            
            return Json("Saved", JsonRequestBehavior.AllowGet);  // Return as JSON
        }
    }
    
}
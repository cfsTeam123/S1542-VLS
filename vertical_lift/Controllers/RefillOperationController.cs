using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;

namespace vertical_lift.Controllers
{
    public class RefillOperationController : Controller
    {
        // GET: RefillOperation
        S1542Entities conn=new S1542Entities();
        public ActionResult Refill()
        {
            //for GRN dropdown
            ViewBag.GrnType = new List<SelectListItem>
                {
                    new SelectListItem { Text = "With GRN", Value = "Yes",Selected=false },
                    new SelectListItem { Text = "Without GRN", Value = "No",Selected=false },                   
                };

            //var MaterialsAndBinTypes = conn.ExistingDetails
            //                   .Where(x => x.Action == "LOAD MATERIAL")
            //                   .Select(u => new { u.MaterialDescription, u.BinType })
            //                   .Distinct()
            //                   .ToList();

            //ViewBag.Material = new SelectList(MaterialsAndBinTypes, "MaterialDescription", "MaterialDescription");
            ViewBag.BinType = new SelectList("", "", "");
            ViewBag.Material = new SelectList("", "", "");
            ViewBag.BinNo = new SelectList("", "", "");
            return View();
           
        }
        public JsonResult GetGrnNo(string GrnNo)
        {
            //fwtch grn details in dropown 
            var MaterialsAndBinTypes = conn.ExistingDetails
                 .Where(x => x.Action == "LOAD MATERIAL" && x.InspectionType == GrnNo)
                 .Select(u => new { u.MaterialDescription, u.BinType })
                 .Distinct()
                 .ToList();
            return Json(MaterialsAndBinTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBinType(string Material, string grnno)
        {
            //gets bintype for dropdown
            var BinType = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.MaterialDescription == Material && x.InspectionType == grnno)
                               .Select(u => new { u.BinType })  
                               .Distinct()  
                               .ToList();
            return Json(BinType, JsonRequestBehavior.AllowGet);  
        }   
     

        public JsonResult GetBinno(string BinType, string Material, string grnno)
        {
            //fwtch grn details in dropown 
            var Binloc = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.MaterialDescription == Material && x.InspectionType == grnno && x.BinType==BinType)
                               .Select(u => new { u.BinLocation })
                               .Distinct()
                               .ToList();

            var getdiemension=conn.Goods_Existing.Where(x=>x.BinType==BinType).Select(a=>a.Dimension).FirstOrDefault();

            return Json(new { Binloc, getdiemension }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetDtl(string BinType,string material,int binlocation)
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
                            b.Side
                        }).FirstOrDefault();  // Executes the query and converts it into a list


            return Json(dtls, JsonRequestBehavior.AllowGet);  // Return as JSON
        }


        public JsonResult SaveDtl(string BinType,string material, int binlocation,string Dimension,int Avlqty,int Refilqty,int TrayNo,int side)
        {
            //all data take and update it to database

            //  var fetchdtls = conn.Database.ExecuteSqlCommand("select * FROM [S1542].[dbo].[Goods_Existing] where BinLocation= '"+binlocation+"' AND  Dimension='"+Dimension+"' AND  BinType='"+BinType+"' and TrayNo='"+TrayNo+"' And side='"+side+"'");
            var fetchdl = (from a in conn.ExistingDetails
                           join b in conn.Goods_Existing on a.TrayNo equals b.TrayNo
                           where a.Side == side & a.TrayNo == TrayNo && a.MaterialDescription == material && a.BinLocation == binlocation 
                           select new
                           {
                               a,b
                           }).FirstOrDefault();

            //data saved to temp table 
            Refill_Temp_Table Rtable = new Refill_Temp_Table();
            Rtable.MaterialDesc = material;
            Rtable.BinNo = binlocation;
            Rtable.GRNNO = fetchdl.a.GRNNO;
            Rtable.MaterialBarcode = (int)fetchdl.a.MaterialBarcode;
            Rtable.Style=fetchdl.a.Style;
            Rtable.BatchNo= fetchdl.a.BatchNo;
            Rtable.Qty = Refilqty;
            conn.Refill_Temp_Table.Add(Rtable);
            conn.SaveChanges();         
            return Json("", JsonRequestBehavior.AllowGet);  // Return as JSON
        }


    }
}
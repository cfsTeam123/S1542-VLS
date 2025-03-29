
using vertical_lift.Profinet;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using vertical_lift.Models;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace vertical_lift.Controllers
{
    public class RefillOperationController : Controller
    {
        //for plc communication
        //private PLC plc = null;
        //private ExceptionCode errCode;
        ////private int close_flag = 0;
        //static int counter = 0;
        //string plc_ip = "192.168.0.1";

        private PLC plc = null;
        private ExceptionCode errCode;
        string plc_ip = "192.168.0.1";
        static int counter = 0;


        // GET: RefillOperation
        S1542Entities conn = new S1542Entities();
        public ActionResult Refill()
        {
            var MaterialsAndBinTypes = conn.ExistingDetails
                               .Where(x => x.Action == "LOAD MATERIAL" && x.InspectionType == "Yes")
                               .Select(u => new { u.MaterialDescription})
                               .Distinct()
                               .ToList();

            ViewBag.Material = new SelectList(MaterialsAndBinTypes, "MaterialDescription", "MaterialDescription");
            ViewBag.BinType = new SelectList("", "", "");
            ViewBag.BinNo = new SelectList("", "", "");
            return View();

        }

        public JsonResult tabledtl()
        {
            //gets bintype for dropdown
            var tabdtl = conn.Refill_Temp_Table.ToList();
            return Json(tabdtl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteItem(int mTransNo)
        {
            var itemToDelete = conn.Refill_Temp_Table.FirstOrDefault(x => x.MTransNo == mTransNo);
            if (itemToDelete != null)
            {
                conn.Refill_Temp_Table.Remove(itemToDelete);
                conn.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else {
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



        public string TrayCall()
        {
            //call temmptable data  fifo and send to plc
            //take the first data and send it to plc call the tray  
            //var data = tempdata.FirstOrDefault();
            //int trayno = (Int16)data.TrayNo;
            //int side = (Int16)data.Side;

            var checkstatus = chekfunction();//calling the function
            var jsonData = checkstatus.Data as dynamic; //taking the json data 

            if (jsonData != null)
            {
                // Access Auto, Healthy, and Control values from the JSON result
                var Auto = jsonData.Auto.ToString();
                var Healthy = jsonData.Healthy.ToString();
                var Control = jsonData.Control.ToString();

                if (Auto == "1" && Healthy == "1" && Control == "1")
                {
                    //fetches all the data from temptable
                    var tempdata = conn.Refill_Temp_Table.Where(x => x.Status == "submit").OrderBy(x => x.TrayNo).ThenBy(x => x.BinNo).ToList();
                 //   var groupedByTray = tempdata.GroupBy(x => x.TrayNo);

                    if (tempdata.Count() > 0)
                    {
                        foreach (var item in tempdata)
                        {
                            //if (item.count1 >= tempdata)
                            //{

                            //Give command to PLC
                            int side = Convert.ToInt16(item.Side);
                            int level = Convert.ToInt16(item.TrayNo);//need tocheck with level what

                            try
                            {
                                //writing plc value
                                var ItagValu1e1 = (from PT in conn.plc_tags
                                                   where PT.PLCIPAddre == plc_ip && PT.TagAction == "write"
                                                   select new
                                                   {
                                                       MTransNo = PT.MTransNo,
                                                       Tag = PT.Tags,
                                                       Bit = PT.DataBit,
                                                       Data = PT.TagData,
                                                       Operation = PT.PLCOperation

                                                   }).OrderBy(x => x.MTransNo).ToList();

                                foreach (var tg in ItagValu1e1)
                                {
                                    switch (tg.Operation)
                                    {
                                        case "SIDE":
                                            plc.Write(tg.Tag, side).ToString();
                                            Thread.Sleep(50);
                                            break;
                                        // Thread.Sleep(50);
                                        case "TRAY":
                                            plc.Write(tg.Tag, level).ToString();
                                            Thread.Sleep(50);
                                            break;
                                        // Thread.Sleep(50);                                    

                                        case "TRAYPICK":
                                            plc.Write(tg.Tag, 1).ToString();//station is 1 always
                                            Thread.Sleep(50);
                                            //Thread.Sleep(50);
                                            break;
                                    }
                                }

                            }
                            catch (Exception e)
                            {
                                plc.Close();
                                Console.WriteLine(e.Message);
                                return e.Message;
                            }
                            plc.Close();
                            return "success";
                        }
                    }
                    plc.Close();
                   // return "no space";
                    return "success";
                }
                else
                {
                    plc.Close();
                 //   return "no space";
                    return "success";
                }
            }
            else
            {
                plc.Close();
            //    return "error"; // "Error";
                return "success";
            }

        }


        //check all statuses  OF PLC 
        public JsonResult chekfunction()
        {
            string Auto = "";
            string Healthy = "";
            string Control = "";

            //read plc statuses
            plc = new PLC(CPU_Type.S71200, plc_ip, (short)0, (short)1);
            //Thread.Sleep(100);
            errCode = plc.Open();
            var ItagValue = (from PT in conn.plc_tags
                             where PT.PLCIPAddre == plc_ip && PT.TagAction == "error" && (PT.PLCOperation == "AutoMode" || PT.PLCOperation == "SystemHealthy" || PT.PLCOperation == "ReadyForAuto" || PT.PLCOperation == "HMICONTROL")
                             select new
                             {
                                 MTransNo = PT.MTransNo,
                                 Tag = PT.Tags,
                                 Bit = PT.DataBit,
                                 Data = PT.TagData,
                                 Operation = PT.PLCOperation

                             }).ToList();

            foreach (var tg in ItagValue)
            {
                switch (tg.Operation)
                {
                    case "AutoMode":
                        //send goTo cell 

                        if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                        {
                            Auto = "1";
                        }
                        else
                        {
                            //Thread.Sleep(50);
                            if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                            {
                                Auto = "1";
                            }
                            else
                            {
                                //Auto = "0";
                                Auto = "1";
                            }
                        }
                        // Thread.Sleep(50);
                        break;
                    case "SystemHealthy":
                        //send  level MW142
                        if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                        {
                            Healthy = "1";
                        }
                        else
                        {
                            //Thread.Sleep(50);
                            if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                            {
                                Healthy = "1";
                            }
                            else
                            {
                                //Healthy = "0";
                                Healthy = "1";
                            }

                        }

                        //Thread.Sleep(50);
                        break;

                    case "HMICONTROL":
                        //send  lhs_rhs MW144
                        if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                        {
                            Control = "1";
                        }
                        else
                        {
                            // Thread.Sleep(50);
                            if (plc.readBits(Convert.ToInt32(tg.Tag), Convert.ToInt32(tg.Bit)) == tg.Data)
                            {
                                Control = "1";
                            }
                            else
                            {
                                //Control = "0";
                                Control = "1";
                            }
                        }
                        //  Thread.Sleep(50);
                        break;
                }
            }
            return Json(new { Auto, Healthy, Control }, JsonRequestBehavior.AllowGet);
        }

    }


}
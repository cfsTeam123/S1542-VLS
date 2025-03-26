using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class ExistingDetailsValidation
    {
        public int MTransNo { get; set; }
        public Nullable<int> Side { get; set; }
        public Nullable<int> TrayNo { get; set; }
        public string BinType { get; set; }
        public Nullable<int> BinNo { get; set; }
        public Nullable<int> BinBarcode { get; set; }
        public string MaterialDesc { get; set; }
        public Nullable<int> MaterialBarcode { get; set; }
        public string BatchNo { get; set; }
        public string Style { get; set; }
        public Nullable<int> Qty { get; set; }
        public string GRNNO { get; set; }
        public string InspectionType { get; set; }
        public string UloadKittingCategory { get; set; }
        public string Action { get; set; }
        public Nullable<int> ConfirmQty { get; set; }
        public Nullable<int> LineNos { get; set; }
        public string KittingCat { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> MaxQty { get; set; }
    }
}
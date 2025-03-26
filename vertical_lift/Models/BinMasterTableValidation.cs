using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class BinMasterTableValidation
    {
        public int MtransNo { get; set; }
        
        public Nullable<int> TrayNo { get; set; }

        public Nullable<int> BinNo { get; set; }
        public int BinBarcode { get; set; }
        public Nullable<int> MaxQty { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
}
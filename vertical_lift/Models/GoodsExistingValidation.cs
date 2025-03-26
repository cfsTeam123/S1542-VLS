using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class GoodsExistingValidation
    {
        public int MTransNo { get; set; }
        public int Side { get; set; }
        public int TrayNo { get; set; }
        public string BinType { get; set; }
        public string Dimension { get; set; }
        public int Location { get; set; }
        public Nullable<int> BinLocation { get; set; }
        public Nullable<int> BinBarcode { get; set; }
        public string QualityInspection { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string Material { get; set; }
    }
}
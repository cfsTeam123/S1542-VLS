using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class TrayAllocation
    {
        public int MTransNo { get; set; }
        public int Side { get; set; }
        public int TrayNo { get; set; }
        public string BinType { get; set; }
       
        public string Dimension { get; set; }
        public int Location { get; set; }
        public Nullable<int> MaxqtyBin { get; set; }
        public Nullable<int> MinqtyBin { get; set; }
        public string QualityInspection { get; set; }
        public string Status { get; set; }
        public Nullable<int> MaxWeight { get; set; }
        public Nullable<int> MinWeight { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string DeleteStatus { get; set; }
        public string Material { get; set; }
        public string Type { get; set; }

    }
}
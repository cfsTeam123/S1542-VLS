using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class BinMasterValidation
    {
        public int MTransNo { get; set; }
        public string status { get; set; }
        public string dimension { get; set; }
        public Nullable<int> barcode { get; set; }
        public string BinType { get; set; }
        public Nullable<short> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<short> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string DeleteStatus { get; set; }

    }
}
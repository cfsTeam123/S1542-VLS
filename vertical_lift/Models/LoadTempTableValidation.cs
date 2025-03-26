using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class LoadTempTableValidation
    {
        public int MTransNo { get; set; }
        public int BinNo { get; set; }
        public int binbarcode { get; set; }
        public string GRNNO { get; set; }
        public string MaterialDesc { get; set; }
        public int materialbarcode { get; set; }
        public string batchno { get; set; }
        public string style { get; set; }
        public int qty { get; set; }
        public string Material { get; set; }
        public int Side { get; set; }
        public int TrayNo { get; set; }
        public string BinType { get; set; }
        public string Dimension { get; set; }
        public int Location { get; set; }
        public int Type { get; set; }


        public string BinTypeWGRN { get; set; }
        public int TypeWGRN { get; set; }
        public Nullable<int> MaxQty { get; set; }

        public List<ExistingDetailsValidation> Details { get; set; }
        public List<TransactionDetailsValidation> Details1 { get; set; }
    }
}
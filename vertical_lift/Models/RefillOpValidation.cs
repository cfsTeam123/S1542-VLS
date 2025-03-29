using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class RefillOpValidation
    {
        public int MTransNo { get; set; }
        public int BinNo { get; set; }
        public int binbarcode { get; set; }
        public string GRNNO { get; set; }
        public string GrnType { get; set; }
        public string MaterialDesc { get; set; }
        public string DispMaterialDesc { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "MaterialBarcode must be exactly 4 digits.")]
        public string MaterialBarcode { get; set; }
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
        public int Avlqty { get; set; }
        public int RefilQty { get; set; }
        public int DispRefilQty { get; set; }
        public int MaxQty { get; set; }

        public string BinTypeWGRN { get; set; }
        public int BinBarcode { get; set; }
        public int matchbarcode { get; set; }
        public int TypeWGRN { get; set; }


        public List<ExistingDetailsValidation> Details { get; set; }
    }
}
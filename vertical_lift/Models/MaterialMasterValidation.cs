using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class MaterialMasterValidation
    {
        public int MTransNo { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string DeleteStatus { get; set; }

    }
}
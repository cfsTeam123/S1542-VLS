//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace vertical_lift.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaterialMaster
    {
        public int MTransNo { get; set; }
        public string MaterialName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string DeleteStatus { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vertical_lift.Models
{
    public class UserMasterValidation
    {
        public int MTransNo { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string PW { get; set; }
        public string UserType { get; set; }
        public string ContactNo { get; set; }
        public string LineNos { get; set; }
        public string EmailId { get; set; }
        public string FloorNo { get; set; }
        public string BioId1 { get; set; }
        public string BioId2 { get; set; }
        public string BioData1 { get; set; }
        public string BioData2 { get; set; }
        public string ImagePath { get; set; }
        public string LockStatus { get; set; }
        public Nullable<short> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<short> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string DeleteStatus { get; set; }
        //public Nullable<short> SubscID { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
    }
}
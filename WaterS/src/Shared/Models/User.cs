using System;

namespace WaterS.Shared.Models
{
    public class User
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public bool IsActive { get; set; }
        public string KindType { get; set; }//Company/Station/Driver/Admin/Owner
        public string KindTypeAr { get; set; }//

    }
}
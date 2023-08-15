using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using WaterS.Domain.Contracts;

namespace WaterS.Domain.Entities.Catalog
{
    public class Customer : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Userid { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public int AccountId { get; set; }
        public int No { get; set; }

        public int BottleNo { get; set; }
        public string BottleNoStatue { get; set; } = "مستخدم";

        //[Column(TypeName = "Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime LastFillDate { get; set; }
        public string statue { get; set; } = "saved";

        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int RegionId { get; set; }
        public int BottleTypeId { get; set; }
                    public int LastFillDateDays { get; set; }

        public virtual Company Company { get; set; }
        public virtual Station Station { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual Region Region { get; set; }
        public virtual BottleType BottleType { get; set; }

        public virtual ICollection<CustomerPhone> CustomerPhones { get; set; }


    }
}
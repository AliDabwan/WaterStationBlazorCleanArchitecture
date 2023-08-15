using System.Collections;
using System.Collections.Generic;
using WaterS.Domain.Contracts;

namespace WaterS.Domain.Entities.Catalog
{
    public class DriverRegion : AuditableEntity<int>
    {




        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        //public virtual ICollection<Customer> CustomerList { get; set; }

    }
}
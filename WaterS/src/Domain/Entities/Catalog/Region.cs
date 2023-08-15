using System.Collections;
using System.Collections.Generic;
using WaterS.Domain.Contracts;

namespace WaterS.Domain.Entities.Catalog
{
    public class Region : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        //public  virtual ICollection<Company> Companies { get; set; }

        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public virtual ICollection<Customer> CustomerList { get; set; }


    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WaterS.Domain.Contracts;


namespace WaterS.Domain.Entities.Catalog
{
    public class Accounts : AuditableEntity<int>
    {

        public int No { get; set; }

        public string Name { get; set; }
        public int AccountType { get; set; }//debit=0
        public string CategoryType { get; set; }//العملاء - المبيعات =0





        public string UserId { get; set; }



        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int CustomerId { get; set; }
        public virtual ICollection<AccTransMovment> AccTransMovments { get; set; }

        //public int CompanyId { get; set; }
        //public virtual Company Company { get; set; }

        //public int StationId { get; set; }
        //public virtual Station Station { get; set; }
        //public int DriverId { get; set; }
        //public virtual Driver Driver { get; set; }
        //public int CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }

    }
}

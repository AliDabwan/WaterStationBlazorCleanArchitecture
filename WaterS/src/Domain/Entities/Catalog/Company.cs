using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WaterS.Domain.Contracts;


namespace WaterS.Domain.Entities.Catalog
{
    public class Company : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string ResName { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


        public string Userid { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public int AccountId { get; set; }
        public int No { get; set; }
        public string KindType { get; set; }
        public string KindTypeAr { get; set; }
        public int MyCompanyID { get; set; }
        public int MystationID { get; set; }

        public bool IsActive { get; set; }
        public DateTime ActivateDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}

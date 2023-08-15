using System.ComponentModel.DataAnnotations;
using WaterS.Domain.Contracts;


namespace WaterS.Domain.Entities.Catalog
{
    public class Talap : AuditableEntity<int>
    {

        public int No { get; set; }

        public string ServiceRate { get; set; }//Good//poor
        public string TalapStatue { get; set; }//Running-Complete-Declined
        public string TalapStatueAr { get; set; }//جاري-مكتمل-ملغي



        public string Comment { get; set; }//التعليق


        public int BottleNo { get; set; }

        public string TalapDate { get; set; }
        public string TalapArrivalDate { get; set; }
        public string TalapArrivalTime { get; set; }



        public string DoneByName{ get; set; }

        public int DoneByAccountId { get; set; }

        public decimal Price { get; set; }
        public decimal Paid { get; set; }//debit=0


        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}

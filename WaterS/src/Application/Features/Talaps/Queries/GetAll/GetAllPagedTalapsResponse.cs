
using System;
using System.Collections.Generic;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.Talaps.Queries.GetAllTalaps
{
    public class GetAllPagedTalapsResponse
    {
        public int serial { get; set; }

        public int    Id { get; set; }
        public int No { get; set; }

        public string ServiceRate { get; set; }//Good//poor
        public string TalapStatue { get; set; }//Running-Complete-Declined
        public string TalapStatueAr { get; set; }//جاري-مكتمل-ملغي
        public string DoneByName { get; set; }

        public int DoneByAccountId { get; set; }
        //public DateTime LastFillDate { get; set; }


        public string TalapDate { get; set; }
        //التعليق
        public string Comment { get; set; }//التعليق


        public int BottleNo { get; set; }
        public string TalapArrivalDate { get; set; }
        public string TalapArrivalTime { get; set; }


        public decimal Price { get; set; }
        public decimal Paid { get; set; }//debit=0


        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string DriverName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerBotlType { get; set; }

        public string StationName { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedOn { get; set; }//

        public virtual Region Region { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<CustomerPhone> CustomerPhones { get; set; }

    }
}
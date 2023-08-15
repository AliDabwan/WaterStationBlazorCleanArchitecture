using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WaterS.Domain.Contracts;


namespace WaterS.Domain.Entities.Catalog
{
    public class AccTrans : AuditableEntity<int>
    {

        /// <summary>
        /// 1- open balance ,2- talap , 3-voucher
        /// </summary>
        public int TransType { get; set; }//,voucher name
        public int TransTypeId { get; set; }//رقم تسلسي للسند


        public int RefId { get; set; }//  المرجع رقم الطلب  


        public string Note { get; set; }//debit=0
        public decimal Ammount { get; set; }//debit=0

       


        public string Transby { get; set; }//تمت العملية بواسطة اسم السائق او الذي قبض او استلم

        public string UserId { get; set; }


        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public virtual ICollection<AccTransMovment> TransMovments { get; set; }

    }
}

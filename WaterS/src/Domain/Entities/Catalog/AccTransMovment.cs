using System.ComponentModel.DataAnnotations;
using WaterS.Domain.Contracts;


namespace WaterS.Domain.Entities.Catalog
{
    public class AccTransMovment : AuditableEntity<int>
    {

        /// <summary>
        /// 1- debit ,2- Credit 
        /// </summary>
        public int EntryType { get; set; }//,
       
        
       


        public string Note { get; set; }
        public string NoteDebit { get; set; }
        public string NoteCredit { get; set; }
        public decimal DebitAmmount { get; set; }//debit=0
        public decimal CreditAmmount { get; set; }//credit=0






        public int AccTransId { get; set; }//
        public AccTrans AccTrans { get; set; }//

        public int AccountsId { get; set; }//
        public Accounts Accounts { get; set; }//
        public int DriverId { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        //public virtual Station Station { get; set; }
    }
}

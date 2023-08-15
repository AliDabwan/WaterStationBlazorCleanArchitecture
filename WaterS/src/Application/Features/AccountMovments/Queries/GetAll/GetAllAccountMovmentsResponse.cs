
using System;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.AccountMovments.Queries.GetAll
{
    public class GetAllAccountMovmentsResponse
    {
        public int Id { get; set; }
        public int EntryType { get; set; }//,



        public string Note { get; set; }


        public string NoteDebit { get; set; }
        public string NoteCredit { get; set; }
        public decimal DebitAmmount { get; set; }//debit=0
        public decimal CreditAmmount { get; set; }//credit=0



        public DateTime CreatedOn { get; set; }//



        public int AccTransId { get; set; }//

        public int AccountsId { get; set; }//


        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public Accounts Accounts { get; set; }//

    }
}
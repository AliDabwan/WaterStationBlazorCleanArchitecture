
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.AccountMovments.Queries.GetById
{
    public class GetAccountMovmentByIdResponse
    {
        public int Id { get; set; }
        public int EntryType { get; set; }//,





        public string Note { get; set; }
        public decimal DebitAmmount { get; set; }//debit=0
        public decimal CreditAmmount { get; set; }//credit=0






        public int AccTransId { get; set; }//

        public int AccountId { get; set; }//


        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public Accounts Accounts { get; set; }//

    }
}
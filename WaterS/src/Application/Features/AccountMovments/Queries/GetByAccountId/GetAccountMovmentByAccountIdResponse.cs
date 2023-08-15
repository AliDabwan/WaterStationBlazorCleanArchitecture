
namespace WaterS.Application.Features.AccountMovments.Queries.GetByAccountId
{
    public class GetAccountMovmentByAccountIdResponse
    {
        public int Id { get; set; }
        public int EntryType { get; set; }//,




        public decimal Balance { get; set; }//credit=0



        public int AccountsId { get; set; }//


        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }

    }
}
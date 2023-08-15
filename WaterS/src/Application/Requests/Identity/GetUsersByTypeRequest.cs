
namespace WaterS.Application.Requests.Identity
{
    public class GetUsersByTypeRequest
    {
        public int KindId { get; set; }//
        public int StationId { get; set; }//
        public int DriverId { get; set; }//
        public int CustomerId { get; set; }//        public string UserId { get; set; }
    }
}
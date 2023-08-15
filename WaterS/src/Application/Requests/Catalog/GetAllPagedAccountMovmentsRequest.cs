
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedAccountMovmentsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int AccountId { get; set; }


    }
}
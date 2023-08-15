
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedStationsRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}
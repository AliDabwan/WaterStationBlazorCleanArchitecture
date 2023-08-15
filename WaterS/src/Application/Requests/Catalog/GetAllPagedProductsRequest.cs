
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedProductsRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}
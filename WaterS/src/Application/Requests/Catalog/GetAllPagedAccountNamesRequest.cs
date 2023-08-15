
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedAccountNamesRequest : PagedRequest
    {
        public string SearchString { get; set; }
    }
}

namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedDriversRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int withOutInclud { get; set; } = 0;

    }
}
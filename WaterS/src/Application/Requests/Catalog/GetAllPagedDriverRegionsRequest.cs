
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedDriverRegionsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }


    }
}
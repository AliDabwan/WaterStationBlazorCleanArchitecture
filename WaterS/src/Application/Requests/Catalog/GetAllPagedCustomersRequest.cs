
namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedCustomersRequest : PagedRequest
    {
        public string SearchString { get; set; }
        
                    public string SearchBy { get; set; } 
        public string Statue { get; set; }

        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int RegionId { get; set; }
        public int withOutInclud { get; set; } = 0;

    }
}
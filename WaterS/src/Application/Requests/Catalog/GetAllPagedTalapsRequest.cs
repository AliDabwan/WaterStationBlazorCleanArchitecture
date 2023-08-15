
using System;

namespace WaterS.Application.Requests.Catalog
{
    public class GetAllPagedTalapsRequest : PagedRequest
    {
        public string SearchString { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int RegionId { get; set; }

        public string Statue { get; set; }
        public string? DateFrom { get; set; }//
        public string? DateTo { get; set; }//

    }
}
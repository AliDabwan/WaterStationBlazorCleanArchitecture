
using System.Collections.Generic;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.DriverRegions.Queries.GetAllPaged
{
    public class GetAllPagedDriverRegionsResponse
    {
        public int serial { get; set; }

        public int Id { get; set; }
        //public int MyCompanyId { get; set; }
        //public int MyStationId { get; set; }
        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public int DriverId { get; set; }
        public bool ShowDetails { get; set; }

        public virtual Driver Driver { get; set; }
        public int CustomerCounts { get; set; }

        public virtual ICollection<GetAllPagedCustomersResponse> CustomerList { get; set; }

    }
}
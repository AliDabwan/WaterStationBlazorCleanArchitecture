
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion
{
    public class GetDriverRegioneResponse
    {
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
        public virtual Driver Driver { get; set; }
    }
}
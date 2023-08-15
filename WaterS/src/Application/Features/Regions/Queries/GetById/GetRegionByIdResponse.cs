
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.Regions.Queries.GetById
{
    public class GetRegionByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
    }
}
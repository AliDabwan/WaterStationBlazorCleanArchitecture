
using System.Collections.Generic;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.Regions.Queries.GetAll
{
    public class GetAllRegionsResponse
    {
        public int serial { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public virtual ICollection<Customer> CustomerList { get; set; }

    }
}
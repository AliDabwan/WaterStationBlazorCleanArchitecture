
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.CustomerPhones.Queries.GetAll
{
    public class GetAllCustomerPhonesResponse
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public int CompanyId { get; set; }

        public int AccountId { get; set; }

        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
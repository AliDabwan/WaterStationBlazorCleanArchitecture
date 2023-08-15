using WaterS.Domain.Contracts;

namespace WaterS.Domain.Entities.Catalog
{
    public class CustomerPhone : AuditableEntity<int>
    {
        public string PhoneNumber { get; set; }
        public string Description { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public int StationId { get; set; }
        public virtual Station Station { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
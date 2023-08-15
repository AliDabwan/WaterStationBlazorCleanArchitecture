using WaterS.Domain.Contracts;

namespace WaterS.Domain.Entities.Catalog
{
    public class BottleType : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int FillDays { get; set; }
        public decimal Price { get; set; }
    }
}
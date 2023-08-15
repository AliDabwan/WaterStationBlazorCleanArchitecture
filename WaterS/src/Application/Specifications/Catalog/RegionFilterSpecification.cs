using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class RegionFilterSpecification : HeroSpecification<Region>
    {
        public RegionFilterSpecification(string searchString)
        {
            Includes.Add(x => x.Station);

            Includes.Add(x=>x.CustomerList);
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name != null && (p.Name.Contains(searchString));
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}
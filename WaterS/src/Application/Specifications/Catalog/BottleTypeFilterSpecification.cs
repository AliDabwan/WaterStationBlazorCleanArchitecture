using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class BottleTypeFilterSpecification : HeroSpecification<BottleType>
    {
        public BottleTypeFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
        public BottleTypeFilterSpecification(string searchString,int x)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Equals(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}

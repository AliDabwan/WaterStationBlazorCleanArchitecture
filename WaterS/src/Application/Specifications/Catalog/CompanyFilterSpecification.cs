using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class CompanyFilterSpecification : HeroSpecification<Company>
    {
        public CompanyFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Adress.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
       
    }
}

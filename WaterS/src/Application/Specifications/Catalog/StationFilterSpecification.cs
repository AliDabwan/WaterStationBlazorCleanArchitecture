using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class StationFilterSpecification : HeroSpecification<Station>
    {
        public StationFilterSpecification(string searchString)
        {
            Includes.Add(a => a.myCompany);

            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Adress.Contains(searchString) || p.myCompany.Name.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
       
    }
}

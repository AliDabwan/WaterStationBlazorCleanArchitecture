using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class DriverFilterSpecification : HeroSpecification<Driver>
    {
        public DriverFilterSpecification(string searchString)
        {
            Includes.Add(a => a.myCompany);
            Includes.Add(a => a.myStation);
            Includes.Add(a => a.CustomerList);
            Includes.Add(a => a.DriverRegionList);

            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) 
                                || p.myStation.Name.Contains(searchString)
                || p.Phone.Contains(searchString)

                ;
            }
            else
            {
                Criteria = p => true;
            }
        }
       
    }
}

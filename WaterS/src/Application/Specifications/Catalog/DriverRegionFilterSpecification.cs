using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class DriverRegionFilterSpecification : HeroSpecification<DriverRegion>
    {
        public DriverRegionFilterSpecification(string searchString)
        {
            Includes.Add(a => a.Driver);
            Includes.Add(a => a.Region);
            Includes.Add(a => a.Station);
            Includes.Add(a => a.Company);
            //Includes.Add(a => a.CustomerList);


            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p =>   p.Station.Name.Contains(searchString) ||
                                  p.Region.Name.Contains(searchString) ||
                                  p.Driver.Name.Contains(searchString)
                  
                ;
            }
            else
            {
                Criteria = p => p.Region.Name != null;
            }
        }
    }
}
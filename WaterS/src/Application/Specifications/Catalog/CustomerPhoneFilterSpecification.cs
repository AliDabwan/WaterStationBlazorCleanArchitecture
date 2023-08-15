using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class CustomerPhoneFilterSpecification : HeroSpecification<CustomerPhone>
    {

        public CustomerPhoneFilterSpecification(string searchString)
        {
            //Includes.Add(a => a.Company);
            //Includes.Add(a => a.Station);
            //Includes.Add(a => a.Driver);

            Includes.Add(a => a.Customer);
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.PhoneNumber.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
        
    }
}

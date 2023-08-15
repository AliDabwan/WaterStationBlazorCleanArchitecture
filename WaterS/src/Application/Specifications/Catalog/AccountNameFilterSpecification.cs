using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Statue;

namespace WaterS.Application.Specifications.Catalog
{
    public class AccountNameFilterSpecification : HeroSpecification<Accounts>
    {
        public AccountNameFilterSpecification(string searchString)
        {
            //Includes.Add(a => a.AccTransMovments);
           
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.CategoryType.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }

    }
}

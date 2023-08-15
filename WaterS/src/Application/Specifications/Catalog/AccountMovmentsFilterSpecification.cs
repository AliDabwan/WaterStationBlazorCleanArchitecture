using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Statue;

namespace WaterS.Application.Specifications.Catalog
{
    public class AccountMovmentsFilterSpecification : HeroSpecification<AccTransMovment>
    {
        public AccountMovmentsFilterSpecification()
        {
            Includes.Add(a => a.Accounts);
            Includes.Add(a => a.AccTrans);

            //Includes.Add(a => a.Driver);
            //Includes.Add(a => a.Customer);
            //Includes.Add(a => a.Region);



            Criteria = p => true;

        }

    }
}

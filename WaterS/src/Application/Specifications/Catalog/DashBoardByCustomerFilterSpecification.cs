using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;
using WaterS.Shared.Constants.Statue;

namespace WaterS.Application.Specifications.Catalog
{
    public class DashBoardByCustomerFilterSpecification : HeroSpecification<Talap>
    {
        public DashBoardByCustomerFilterSpecification()
        {
            //Includes.Add(a => a.Company);
            //Includes.Add(a => a.Station);
            //Includes.Add(a => a.Driver);
            //Includes.Add(a => a.Customer);
            //Includes.Add(a => a.Region);


            
                Criteria = p =>  p.TalapStatue == StatueConstants.Completed;
           
        }
       
    }
}

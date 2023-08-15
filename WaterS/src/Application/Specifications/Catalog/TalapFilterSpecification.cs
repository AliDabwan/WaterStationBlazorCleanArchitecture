using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class TalapFilterSpecification : HeroSpecification<Talap>
    {
        public TalapFilterSpecification(string searchString)
        {
            Includes.Add(a => a.Company);
            Includes.Add(a => a.Station);
            Includes.Add(a => a.Driver);
            Includes.Add(a => a.Customer);
            Includes.Add(a => a.Region);
            Includes.Add(a => a.Customer.CustomerPhones);
            Includes.Add(a => a.Customer.BottleType);


            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Comment.Contains(searchString)|| p.TalapStatueAr.Contains(searchString)
                || p.Customer.Name.Contains(searchString)
                                || p.Station.Name.Contains(searchString)
                                                                || p.Station.Name.Contains(searchString)
                    || p.Driver.Name.Contains(searchString)

                || p.Driver.Name.Contains(searchString)
                                || p.Region.Name.Contains(searchString)

                 || p.Customer.Phone.Contains(searchString)
                                  || p.Customer.Adress.Contains(searchString)
                                                                    || p.Customer.BottleType.Name.Contains(searchString)

                                  || p.BottleNo.ToString().Contains(searchString)

                || p.TalapDate.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
       
    }
}

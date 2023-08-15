using WaterS.Application.Specifications.Base;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Specifications.Catalog
{
    public class CustomerFilterSpecification : HeroSpecification<Customer>
    {
        public CustomerFilterSpecification(string searchString,string searchBy)
        {
            Includes.Add(a => a.Company);
            Includes.Add(a => a.Station);
            Includes.Add(a => a.Driver);
            Includes.Add(a => a.Region);
            Includes.Add(a => a.BottleType);
            Includes.Add(a => a.CustomerPhones);

            if (!string.IsNullOrEmpty(searchString))
            {

                if (!string.IsNullOrEmpty(searchBy))
                {
                    if (searchBy == "Name")
                    {
                        Criteria = p => p.Name.Contains(searchString);


                    }
                    else if (searchBy == "BottleNo")
                    {
                        Criteria = p => p.BottleNo.ToString().Contains(searchString);


                    }
                    else if (searchBy == "Phone")
                    {
                        Criteria = p => p.Phone.Contains(searchString);


                    }
                    else if (searchBy == "TBottleNo")
                    {
                        Criteria = p => p.BottleNo.ToString()==searchString;


                    }
                    else if (searchBy == "TPhone")
                    {
                        Criteria = p => p.Phone==searchString;


                    }
                    //else if (searchBy == "LastFillDays")
                    //{
                    //    Criteria = p => p.LastFillDate.ToString("yyyy/MM/dd") == searchString;
                    //    //Criteria = p => p.LastFillDateDays.ToString() == searchString;

                    //    //Criteria = p => true;

                    //}

                    else
                    {
                        Criteria = p => p.Name.Contains(searchString)
            || p.Adress.Contains(searchString)
                            || p.BottleNo.ToString().Contains(searchString)

            || p.Phone.Contains(searchString)
             || p.Station.Name.Contains(searchString)
              || p.Region.Name.Contains(searchString);
                    }


                }
                else
                {
                    Criteria = p => p.Name.Contains(searchString)
               || p.Adress.Contains(searchString)
                               || p.BottleNo.ToString().Contains(searchString)

               || p.Phone.Contains(searchString)
                || p.Station.Name.Contains(searchString)
                 || p.Region.Name.Contains(searchString);
                }
               
            }
            else
            {
                Criteria = p => true;
            }
        }
       
    }
}

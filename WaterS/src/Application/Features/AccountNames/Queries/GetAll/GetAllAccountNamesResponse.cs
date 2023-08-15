using System.Collections;
using System.Collections.Generic;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.AccountNames.Queries.GetAll
{
    public class GetAllAccountNamesResponse
    {
        public int Id { get; set; }

        public int No { get; set; }

        public string Name { get; set; }
        public int AccountType { get; set; }//debit=0
        public string CategoryType { get; set; }//العملاء - المبيعات =0





        public string UserId { get; set; }


        public int CompanyId { get; set; }
        public int StationId { get; set; }
        public int DriverId { get; set; }
        public int CustomerId { get; set; }
        public virtual ICollection<AccTransMovment> AccTransMovments { get; set; }

    }
}
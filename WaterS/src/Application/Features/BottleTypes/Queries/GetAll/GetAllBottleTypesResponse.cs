
using System.Collections.Generic;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;

namespace WaterS.Application.Features.BottleTypes.Queries.GetAll
{
    public class GetAllBottleTypesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FillDays { get; set; }
        public decimal Price { get; set; }

        public int BottlCount { get; set; }
        public bool ShowDetails { get; set; }
        public virtual ICollection<GetAllPagedCustomersResponse> CustomerList { get; set; }

    }
}
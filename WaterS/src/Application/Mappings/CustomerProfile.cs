using AutoMapper;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<AddEditCustomerCommand, Customer>().ReverseMap();
            CreateMap<GetAllPagedCustomersResponse, Customer>().ReverseMap();
            CreateMap<GetCustomerByIdResponse, Customer>().ReverseMap();
        }
    }
}
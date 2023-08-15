using AutoMapper;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Application.Features.CustomerPhones.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class CustomerPhoneProfile : Profile
    {
        public CustomerPhoneProfile()
        {
            CreateMap<AddEditCustomerPhoneCommand, CustomerPhone>().ReverseMap();
            CreateMap<GetCustomerPhoneByIdResponse, CustomerPhone>().ReverseMap();
            CreateMap<GetAllCustomerPhonesResponse, CustomerPhone>().ReverseMap();
        }
    }
}
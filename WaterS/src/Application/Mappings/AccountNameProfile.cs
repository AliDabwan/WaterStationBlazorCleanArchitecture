using AutoMapper;
using WaterS.Application.Features.AccountNames.Commands.AddEdit;
using WaterS.Application.Features.AccountNames.Queries.GetAll;
using WaterS.Application.Features.AccountNames.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class AccountNameProfile : Profile
    {
        public AccountNameProfile()
        {
            CreateMap<AddEditAccountNameCommand, Accounts>().ReverseMap();
            CreateMap<GetAccountNameByIdResponse, Accounts>().ReverseMap();
            CreateMap<GetAllAccountNamesResponse, Accounts>().ReverseMap();
        }
    }
}
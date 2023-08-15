using AutoMapper;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.AccountMovments.Queries.GetById;
using WaterS.Application.Features.AccountMovments.Queries.GetByAccountId;

using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class AccountMovmentProfile : Profile
    {
        public AccountMovmentProfile()
        {
            CreateMap<AddEditAccountMovmentCommand, AccTransMovment>().ReverseMap();
            CreateMap<GetAccountMovmentByIdResponse, AccTransMovment>().ReverseMap();
            CreateMap<GetAllAccountMovmentsResponse, AccTransMovment>().ReverseMap();
            CreateMap<GetAccountMovmentByAccountIdResponse, AccTransMovment>().ReverseMap();

        }
    }
}
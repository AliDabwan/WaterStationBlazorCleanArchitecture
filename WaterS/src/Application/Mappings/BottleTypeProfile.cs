using AutoMapper;
using WaterS.Application.Features.BottleTypes.Commands.AddEdit;
using WaterS.Application.Features.BottleTypes.Queries.GetAll;
using WaterS.Application.Features.BottleTypes.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class BottleTypeProfile : Profile
    {
        public BottleTypeProfile()
        {
            CreateMap<AddEditBottleTypeCommand, BottleType>().ReverseMap();
            CreateMap<GetBottleTypeByIdResponse, BottleType>().ReverseMap();
            CreateMap<GetAllBottleTypesResponse, BottleType>().ReverseMap();
        }
    }
}
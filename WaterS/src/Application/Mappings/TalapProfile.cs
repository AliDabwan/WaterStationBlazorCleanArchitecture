using AutoMapper;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Features.Talaps.Queries.GetTalapById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class TalapProfile : Profile
    {
        public TalapProfile()
        {
            CreateMap<AddEditTalapCommand, Talap>().ReverseMap();
            CreateMap<GetAllPagedTalapsResponse, Talap>().ReverseMap();
            CreateMap<GetTalapByIdResponse, Talap>().ReverseMap();
        }
    }
}
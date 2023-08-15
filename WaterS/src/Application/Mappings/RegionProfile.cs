using AutoMapper;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.Regions.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<AddEditRegionCommand, Region>().ReverseMap();
            CreateMap<GetRegionByIdResponse, Region>().ReverseMap();
            CreateMap<GetAllRegionsResponse, Region>().ReverseMap();
            CreateMap<GetAllRegionsResponse, Region>().ReverseMap();

        }
    }
}
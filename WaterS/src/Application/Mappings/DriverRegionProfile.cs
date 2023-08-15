using AutoMapper;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class DriverRegionProfile : Profile
    {
        public DriverRegionProfile()
        {
            CreateMap<AddEditDriverRegionCommand, DriverRegion>().ReverseMap();
            CreateMap<GetDriverRegioneResponse, DriverRegion>().ReverseMap();

        }
    }
}
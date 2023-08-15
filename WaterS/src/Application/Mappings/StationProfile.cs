using AutoMapper;
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Application.Features.Stations.Queries.GetStationById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class StationProfile : Profile
    {
        public StationProfile()
        {
            CreateMap<AddEditStationCommand, Station>().ReverseMap();
            CreateMap<GetAllPagedStationsResponse, Station>().ReverseMap();
            CreateMap<GetStationByIdResponse, Station>().ReverseMap();
        }
    }
}
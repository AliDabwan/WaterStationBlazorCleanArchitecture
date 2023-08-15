using AutoMapper;
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Drivers.Queries.GetDriverById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<AddEditDriverCommand, Driver>().ReverseMap();
            CreateMap<GetAllPagedDriversResponse, Driver>().ReverseMap();
            CreateMap<GetDriverByIdResponse, Driver>().ReverseMap();
        }
    }
}
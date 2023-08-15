using AutoMapper;
using WaterS.Application.Responses.Identity;
using WaterS.Infrastructure.Models.Identity;

namespace WaterS.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
        }
    }
}
using AutoMapper;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;
using WaterS.Infrastructure.Models.Identity;

namespace WaterS.Infrastructure.Mappings
{
    public class RoleClaimProfile : Profile
    {
        public RoleClaimProfile()
        {
            CreateMap<RoleClaimResponse, BlazorHeroRoleClaim>()
                .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            CreateMap<RoleClaimRequest, BlazorHeroRoleClaim>()
                .ForMember(nameof(BlazorHeroRoleClaim.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(BlazorHeroRoleClaim.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();
        }
    }
}
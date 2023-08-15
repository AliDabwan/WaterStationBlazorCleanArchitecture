using AutoMapper;
using WaterS.Application.Requests.Identity;
using WaterS.Application.Responses.Identity;

namespace WaterS.Client.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<PermissionResponse, PermissionRequest>().ReverseMap();
            CreateMap<RoleClaimResponse, RoleClaimRequest>().ReverseMap();
        }
    }
}
using AutoMapper;
using WaterS.Application.Responses.Audit;
using WaterS.Infrastructure.Models.Audit;

namespace WaterS.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}
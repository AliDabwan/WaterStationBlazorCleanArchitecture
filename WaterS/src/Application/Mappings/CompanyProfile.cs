using AutoMapper;
using WaterS.Application.Features.Companies.Commands.AddEdit;
using WaterS.Application.Features.Companies.Queries.GetAll;
using WaterS.Application.Features.Companies.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<AddEditCompanyCommand, Company>().ReverseMap();
            CreateMap<GetCompanyByIdResponse, Company>().ReverseMap();
            CreateMap<GetAllCompaniesResponse, Company>().ReverseMap();
        }
    }
}
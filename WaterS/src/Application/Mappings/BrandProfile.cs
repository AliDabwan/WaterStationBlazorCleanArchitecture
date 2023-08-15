using AutoMapper;
using WaterS.Application.Features.Brands.Commands.AddEdit;
using WaterS.Application.Features.Brands.Queries.GetAll;
using WaterS.Application.Features.Brands.Queries.GetById;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<AddEditBrandCommand, Brand>().ReverseMap();
            CreateMap<GetBrandByIdResponse, Brand>().ReverseMap();
            CreateMap<GetAllBrandsResponse, Brand>().ReverseMap();
        }
    }
}
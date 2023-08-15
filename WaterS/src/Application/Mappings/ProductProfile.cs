using AutoMapper;
using WaterS.Application.Features.Products.Commands.AddEdit;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddEditProductCommand, Product>().ReverseMap();
        }
    }
}
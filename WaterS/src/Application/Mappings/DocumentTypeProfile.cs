using AutoMapper;
using WaterS.Application.Features.DocumentTypes.Commands.AddEdit;
using WaterS.Application.Features.DocumentTypes.Queries.GetAll;
using WaterS.Application.Features.DocumentTypes.Queries.GetById;
using WaterS.Domain.Entities.Misc;

namespace WaterS.Application.Mappings
{
    public class DocumentTypeProfile : Profile
    {
        public DocumentTypeProfile()
        {
            CreateMap<AddEditDocumentTypeCommand, DocumentType>().ReverseMap();
            CreateMap<GetDocumentTypeByIdResponse, DocumentType>().ReverseMap();
            CreateMap<GetAllDocumentTypesResponse, DocumentType>().ReverseMap();
        }
    }
}
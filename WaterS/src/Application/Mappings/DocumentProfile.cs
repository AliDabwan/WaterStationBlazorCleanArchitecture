using AutoMapper;
using WaterS.Application.Features.Documents.Commands.AddEdit;
using WaterS.Application.Features.Documents.Queries.GetById;
using WaterS.Domain.Entities.Misc;

namespace WaterS.Application.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<AddEditDocumentCommand, Document>().ReverseMap();
            CreateMap<GetDocumentByIdResponse, Document>().ReverseMap();
        }
    }
}
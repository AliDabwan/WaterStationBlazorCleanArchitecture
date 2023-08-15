using System.Threading.Tasks;
using WaterS.Application.Features.Documents.Commands.AddEdit;
using WaterS.Application.Features.Documents.Queries.GetAll;
using WaterS.Application.Features.Documents.Queries.GetById;
using WaterS.Application.Requests.Documents;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Misc.Document
{
    public interface IDocumentManager : IManager
    {
        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}
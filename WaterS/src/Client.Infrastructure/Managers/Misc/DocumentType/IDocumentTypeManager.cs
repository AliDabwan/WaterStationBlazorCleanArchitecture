using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.DocumentTypes.Commands.AddEdit;
using WaterS.Application.Features.DocumentTypes.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Misc.DocumentType
{
    public interface IDocumentTypeManager : IManager
    {
        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
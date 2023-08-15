using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.Talaps.Commands.AddEdit;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Talap
{
    public interface ITalapManager : IManager
    {
        Task<PaginatedResult<GetAllPagedTalapsResponse>> GetTalapsAsync(GetAllPagedTalapsRequest request);
        Task<PaginatedResult<GetAllPagedTalapsResponse>> GetTalapsAsync();


        Task<IResult<List<GetAllPagedTalapsResponse>>> GetListAsync();

        Task<IResult<string>> GetTalapAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditTalapCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
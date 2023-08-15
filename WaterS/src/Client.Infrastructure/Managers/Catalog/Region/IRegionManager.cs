using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.Regions.Commands.AddEdit;
using WaterS.Application.Features.Regions.Queries.GetAll;
using WaterS.Application.Features.Regions.Queries.GetById;

using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Region
{
    public interface IRegionManager : IManager
    {
        Task<IResult<List<GetAllRegionsResponse>>> GetAllAsync();
        Task<IResult<GetRegionByIdResponse>> GetRegionAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditRegionCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
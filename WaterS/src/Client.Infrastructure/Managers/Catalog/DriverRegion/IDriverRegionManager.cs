using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.DriverRegions.Commands.AddEdit;
using WaterS.Application.Features.DriverRegions.Queries.GetAllPaged;
using WaterS.Application.Features.DriverRegions.Queries.GetDriverRegion;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.DriverRegion
{
    public interface IDriverRegionManager : IManager
    {
        Task<PaginatedResult<GetAllPagedDriverRegionsResponse>> GetDriverRegionsAsync(GetAllPagedDriverRegionsRequest request);
        Task<PaginatedResult<GetAllPagedDriverRegionsResponse>> GetDriverRegionsAsync();

        //Task<IResult<string>> GetDriverRegionAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditDriverRegionCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
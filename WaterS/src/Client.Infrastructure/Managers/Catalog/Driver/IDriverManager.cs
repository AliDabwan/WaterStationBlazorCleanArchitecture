using System.Threading.Tasks;
using WaterS.Application.Features.Drivers.Commands.AddEdit;
using WaterS.Application.Features.Drivers.Queries.GetAllDrivers;
using WaterS.Application.Features.Drivers.Queries.GetDriverById;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Driver
{
    public interface IDriverManager : IManager
    {
        Task<PaginatedResult<GetAllPagedDriversResponse>> GetDriversAsync(GetAllPagedDriversRequest request);
        Task<PaginatedResult<GetAllPagedDriversResponse>> GetDriversAsync();

        Task<IResult<GetDriverByIdResponse>> GetDriverAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditDriverCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
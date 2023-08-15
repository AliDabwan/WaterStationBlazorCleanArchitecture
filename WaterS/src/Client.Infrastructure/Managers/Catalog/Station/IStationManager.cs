using System.Threading.Tasks;
using WaterS.Application.Features.Stations.Commands.AddEdit;
using WaterS.Application.Features.Stations.Queries.GetAllStations;
using WaterS.Application.Features.Stations.Queries.GetStationById;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Station
{
    public interface IStationManager : IManager
    {
        Task<PaginatedResult<GetAllPagedStationsResponse>> GetStationsAsync(GetAllPagedStationsRequest request);
        Task<PaginatedResult<GetAllPagedStationsResponse>> GetStationsAsync();

        Task<IResult<GetStationByIdResponse>> GetStationAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditStationCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
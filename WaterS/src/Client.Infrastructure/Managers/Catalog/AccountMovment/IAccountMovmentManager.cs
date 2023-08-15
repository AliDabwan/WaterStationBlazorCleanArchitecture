using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountMovments.Commands.AddEdit;
using WaterS.Application.Features.AccountMovments.Queries.GetAll;
using WaterS.Application.Features.AccountMovments.Queries.GetById;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.AccountMovment
{
    public interface IAccountMovmentManager : IManager
    {
        Task<IResult<List<GetAllAccountMovmentsResponse>>> GetAllAsync();
        Task<PaginatedResult<GetAllAccountMovmentsResponse>> GetAccountMovmentsAsync(GetAllPagedAccountMovmentsRequest request);

        Task<IResult<string>> GetAccountBalanceAsync(int accountid);
        
                    Task<IResult<GetAccountMovmentByIdResponse>> GetAccountMovmentsAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditAccountMovmentCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
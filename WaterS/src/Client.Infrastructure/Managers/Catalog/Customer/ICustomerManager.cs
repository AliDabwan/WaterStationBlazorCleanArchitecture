using System.Threading.Tasks;
using WaterS.Application.Features.Customers.Commands.AddEdit;
using WaterS.Application.Features.Customers.Queries.GetAllCustomers;
using WaterS.Application.Features.Customers.Queries.GetCustomerById;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Customer
{
    public interface ICustomerManager : IManager
    {
        Task<PaginatedResult<GetAllPagedCustomersResponse>> GetCustomersAsync(GetAllPagedCustomersRequest request);
        Task<PaginatedResult<GetAllPagedCustomersResponse>> GetCustomersAsync();

        Task<IResult<GetCustomerByIdResponse>> GetCustomerAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditCustomerCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<int>> UnDeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
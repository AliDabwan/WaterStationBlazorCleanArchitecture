using System.Threading.Tasks;
using WaterS.Application.Features.Products.Commands.AddEdit;
using WaterS.Application.Features.Products.Queries.GetAllPaged;
using WaterS.Application.Requests.Catalog;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.Product
{
    public interface IProductManager : IManager
    {
        Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

        Task<IResult<string>> GetProductImageAsync(int id);

        Task<IResult<int>> SaveAsync(AddEditProductCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
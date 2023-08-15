using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.AccountNames.Commands.AddEdit;
using WaterS.Application.Features.AccountNames.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.AccountName
{
    public interface IAccountNameManager : IManager
    {
        Task<IResult<List<GetAllAccountNamesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditAccountNameCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
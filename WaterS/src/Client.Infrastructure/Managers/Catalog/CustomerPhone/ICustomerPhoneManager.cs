using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Features.CustomerPhones.Commands.AddEdit;
using WaterS.Application.Features.CustomerPhones.Queries.GetAll;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Catalog.CustomerPhone
{
    public interface ICustomerPhoneManager : IManager
    {
        Task<IResult<List<GetAllCustomerPhonesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditCustomerPhoneCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}
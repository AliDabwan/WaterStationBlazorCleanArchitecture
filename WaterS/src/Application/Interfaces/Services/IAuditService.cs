using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Responses.Audit;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Interfaces.Services
{
    public interface IAuditService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

        Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}
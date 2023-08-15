using System.Collections.Generic;
using System.Threading.Tasks;
using WaterS.Application.Responses.Audit;
using WaterS.Shared.Wrapper;

namespace WaterS.Client.Infrastructure.Managers.Audit
{
    public interface IAuditManager : IManager
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync();

        Task<IResult<string>> DownloadFileAsync(string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}
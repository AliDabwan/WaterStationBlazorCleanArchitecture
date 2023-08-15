using System.Threading.Tasks;
using WaterS.Shared.Settings;
using WaterS.Shared.Wrapper;

namespace WaterS.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}
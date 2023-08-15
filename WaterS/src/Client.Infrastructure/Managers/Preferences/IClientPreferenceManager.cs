using MudBlazor;
using System.Threading.Tasks;
using WaterS.Shared.Managers;

namespace WaterS.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}
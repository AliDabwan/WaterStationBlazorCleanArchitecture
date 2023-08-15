using System.Linq;
using WaterS.Shared.Constants.Localization;
using WaterS.Shared.Settings;

namespace WaterS.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";

        //TODO - add server preferences
    }
}
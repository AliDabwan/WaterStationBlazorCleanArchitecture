using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WaterS.Client.Extensions;
using WaterS.Client.Infrastructure.Managers.Preferences;
using WaterS.Client.Infrastructure.Settings;
using WaterS.Shared.Constants.Localization;

namespace WaterS.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder
                          .CreateDefault(args)
                          .AddRootComponents()
                          .AddClientServices();
            var host = builder.Build();
            var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
            if (storageService != null)
            {
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ClientPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    //culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
                culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
               
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }
            await builder.Build().RunAsync();
        }
    }
}
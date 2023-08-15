
using Newtonsoft.Json;
using WaterS.Application.Interfaces.Serialization.Settings;

namespace WaterS.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}
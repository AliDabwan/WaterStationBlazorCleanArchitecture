using System.Text.Json;
using WaterS.Application.Interfaces.Serialization.Options;

namespace WaterS.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}
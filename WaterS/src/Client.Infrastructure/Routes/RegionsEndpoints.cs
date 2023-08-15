
namespace WaterS.Client.Infrastructure.Routes
{
    public static class RegionsEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
        public static string GetRegion(int regionId)
        {
            return $"api/v1/regions/id/{regionId}";
        }

        public static string Export = "api/v1/regions/export";

        public static string GetAll = "api/v1/regions";
        public static string Delete = "api/v1/regions"; 
        public static string Save = "api/v1/regions";
        public static string GetCount = "api/v1/regions/count";
    }
}
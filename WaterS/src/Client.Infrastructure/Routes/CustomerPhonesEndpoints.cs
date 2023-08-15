
namespace WaterS.Client.Infrastructure.Routes
{
    public static class CustomerPhonesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = "api/v1/customerphones/export";

        public static string GetAll = "api/v1/customerphones";
        public static string Delete = "api/v1/customerphones"; 
        public static string Save = "api/v1/customerphones";
        public static string GetCount = "api/v1/customerphones/count";
    }
}
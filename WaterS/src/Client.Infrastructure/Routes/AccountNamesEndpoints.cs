
namespace WaterS.Client.Infrastructure.Routes
{
    public static class AccountNamesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string Export = "api/v1/accountnames/export";

        public static string GetAll = "api/v1/accountnames";
        public static string Delete = "api/v1/accountnames"; 
        public static string Save = "api/v1/accountnames";
        public static string GetCount = "api/v1/accountnames/count";
    }
}
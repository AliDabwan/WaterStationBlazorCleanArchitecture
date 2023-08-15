
namespace WaterS.Client.Infrastructure.Routes
{
    public static class CompaniesEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
      
        public static string Export = "api/v1/companies/export";

        public static string GetAll = "api/v1/companies";
        public static string Delete = "api/v1/companies"; 
        public static string Save = "api/v1/companies";
        public static string GetCount = "api/v1/companies/count";
    }
}
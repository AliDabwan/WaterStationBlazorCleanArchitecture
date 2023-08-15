
using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class CustomersEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
        public static string GetAllPaged(int pageNumber, int pageSize, int customerId, int companyId, int stationId, int driverId, int regionId , int without, string searchString,string searchBy, string statue, string[] orderBy)
        {
            var url = $"api/v1/customers?pageNumber={pageNumber}&pageSize={pageSize}&customerId={customerId}&companyId={companyId}&stationId={stationId}&driverId={driverId}&regionId={regionId}&without={without}&searchString={searchString}&searchBy={searchBy}&statue={statue}&orderBy=";
            if (orderBy?.Any() == true)
            {
                foreach (var orderByPart in orderBy)
                {
                    url += $"{orderByPart},";
                }
                url = url[..^1]; // loose training ,
            }
            return url;
        }
        public static string GetAlls()
        {
           
            return GetAll;
        }
        public static string UnDelete(int CustomerId)
        {
            return $"api/v1/customers/bymac/id/{CustomerId}";

        }

        public static string GetCustomer(int CustomerId)
        {
            return $"api/v1/customers/id/{CustomerId}";
        }
        public static string Export = "api/v1/customers/export";

        public static string GetAll = "api/v1/customers";
        public static string Delete = "api/v1/customers";

        public static string Save = "api/v1/customers";
        public static string GetCount = "api/v1/customers/count";
    }
}
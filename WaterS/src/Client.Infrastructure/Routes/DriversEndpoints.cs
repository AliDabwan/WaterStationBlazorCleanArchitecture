
using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class DriversEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
        public static string GetAllPaged(int pageNumber, int pageSize, int companyId, int stationId, int driverId,int without, string searchString, string[] orderBy)
        {
            var url = $"api/v1/drivers?pageNumber={pageNumber}&pageSize={pageSize}&companyId={companyId}&stationId={stationId}&driverId={driverId}&without={without}&searchString={searchString}&orderBy=";
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

        public static string GetDriver(int DriverId)
        {
            return $"api/v1/drivers/id/{DriverId}";
        }
        public static string Export = "api/v1/drivers/export";

        public static string GetAll = "api/v1/drivers";
        public static string Delete = "api/v1/drivers"; 
        public static string Save = "api/v1/drivers";
        public static string GetCount = "api/v1/drivers/count";
    }
}
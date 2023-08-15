using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class DriverRegionsEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize,int companyId, int stationId, int driverId,int regionId, string searchString, string[] orderBy)
        {
            var url = $"api/v1/driverregions?pageNumber={pageNumber}&pageSize={pageSize}&companyId={companyId}&stationId={stationId}&driverId={driverId}&regionId={regionId}&searchString={searchString}&orderBy=";
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

        public static string GetCount = "api/v1/driverregions/count";

        public static string GetDriverRegion(int DriverRegionId)
        {
            return $"api/v1/driverregions/id/{DriverRegionId}";
        }


        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string GetAlls()
        {
            return GetAll;
        }
        public static string GetAll = "api/v1/driverregions";

        public static string Save = "api/v1/driverregions";
        public static string Delete = "api/v1/driverregions";
        public static string Export = "api/v1/driverregions/export";
        public static string ChangePassword = "api/identity/account/changepassword";
        public static string UpdateProfile = "api/identity/account/updateprofile";
    }
}
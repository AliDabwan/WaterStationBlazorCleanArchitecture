
using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class StationsEndpoints
    {
        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"api/v1/stations?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

        public static string GetStation(int stationId)
        {
            return $"api/v1/stations/id/{stationId}";
        }

        public static string Export = "api/v1/stations/export";

        public static string GetAll = "api/v1/stations";
        public static string Delete = "api/v1/stations"; 
        public static string Save = "api/v1/stations";
        public static string GetCount = "api/v1/stations/count";
    }
}
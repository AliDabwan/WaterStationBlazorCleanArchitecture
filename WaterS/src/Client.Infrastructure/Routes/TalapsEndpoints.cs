
using System;
using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class TalapsEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
        public static string GetAllPaged(int pageNumber, int pageSize,int customerId, int companyId, int stationId, int driverId,int regionId,
            string statue, string? dateFrom, string? dateTo,
            string searchString, string[] orderBy)
        {
            var url = $"api/v1/talaps?pageNumber={pageNumber}&pageSize={pageSize}&customerId={customerId}&companyId={companyId}&stationId={stationId}&driverId={driverId}&regionId={regionId}&statue={statue}&dateFrom={dateFrom}&dateTo={dateTo}&searchString={searchString}&orderBy=";
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

        public static string GetTalap(int TalapId)
        {
            return $"api/v1/talaps/id/{TalapId}";
        }
        public static string Export = "api/v1/talaps/export";

        public static string GetAll = "api/v1/talaps";
        public static string Delete = "api/v1/talaps"; 
        public static string Save = "api/v1/talaps";
        public static string GetCount = "api/v1/talaps/count";
    }
}
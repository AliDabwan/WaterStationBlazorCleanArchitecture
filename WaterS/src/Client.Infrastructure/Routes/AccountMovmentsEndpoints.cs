
using System.Linq;

namespace WaterS.Client.Infrastructure.Routes
{
    public static class AccountMovmentsEndpoints
    {
        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }
        public static string GetAccountMovments(int accountsId)
        {
            return $"api/v1/accountmovments/accountsId/{accountsId}";
        }
        public static string GetAllPaged(int pageNumber, int pageSize, int accountId, string searchString, string[] orderBy)
        {
            var url = $"api/v1/accountmovments?pageNumber={pageNumber}&pageSize={pageSize}&accountId={accountId}&searchString={searchString}&orderBy=";
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
        public static string GetAccountBalance(int AccountId)
        {
            return $"api/v1/accountmovments/{AccountId}";
        }
        public static string Export = "api/v1/accountmovments/export";

        public static string GetAll = "api/v1/accountmovments";
        public static string Delete = "api/v1/accountmovments"; 
        public static string Save = "api/v1/accountmovments";
        public static string GetCount = "api/v1/accountmovments/count";
    }
}
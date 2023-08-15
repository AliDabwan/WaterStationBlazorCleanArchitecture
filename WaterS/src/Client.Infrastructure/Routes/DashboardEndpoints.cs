
namespace WaterS.Client.Infrastructure.Routes
{
    public class DashboardEndpoints
    {
        public static string GetData = "api/v1/dashboard";
        public static string GetTalapsByDriver(string userId)
        {
            return $"api/chats/{userId}";
        }
     
    }
}
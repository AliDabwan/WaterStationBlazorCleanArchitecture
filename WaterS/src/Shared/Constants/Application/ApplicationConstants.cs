
namespace WaterS.Shared.Constants.Application
{
    public static class ApplicationConstants
    {
        public static class SignalR
        {
            public const string HubUrl = "/signalRHub";
            public const string SendUpdateDashboard = "UpdateDashboardAsync";
            public const string SendUpdateHome = "UpdateHomeAsync";
            public const string ReceiveUpdateDashboard = "UpdateDashboard";
            public const string ReceiveUpdateHome = "UpdateHome";

            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";
            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";
            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";
        }
        public static class Cache
        {
            public const string GetAllBrandsCacheKey = "all-brands";
            public const string GetAllBottletypesCacheKey = "all-bottletypes";
            public const string GetAllCustomerPhonesCacheKey = "all-customerphones";

            public const string GetAllCompaniesCacheKey = "all-companies";
            public const string GetAllStationsCacheKey = "all-stations";
            public const string GetAllDriversCacheKey = "all-drivers";
            public const string GetAllTalapsCacheKey = "all-talaps";
            public const string GetAllCustomersCacheKey = "all-customers";
            public const string GetAllRegionsCacheKey = "all-regions";
            public const string GetAllAccountsCacheKey = "all-accounts";
            public const string GetAllAccountMovmentsCacheKey = "all-accountmovments";
            public const string GetAllAccountNamesCacheKey = "all-accountnames";

            public const string GetAllDocumentTypesCacheKey = "all-document-types";

            public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
            {
                return $"all-{entityFullName}-extended-attributes";
            }

            public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
            {
                return $"all-{entityFullName}-extended-attributes-{entityId}";
            }
        }

        public static class MimeTypes
        {
            public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WaterS.Shared.Constants.Permission
{
    public static class Permissions
    {
        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
            public const string Export = "Permissions.Products.Export";
            public const string Search = "Permissions.Products.Search";
        }

        public static class Brands
        {
            public const string View = "Permissions.Brands.View";
            public const string Create = "Permissions.Brands.Create";
            public const string Edit = "Permissions.Brands.Edit";
            public const string Delete = "Permissions.Brands.Delete";
            public const string Export = "Permissions.Brands.Export";
            public const string Search = "Permissions.Brands.Search";
        }
        public static class BottleTypes
        {
            public const string View = "Permissions.BottleTypes.View";
            public const string Create = "Permissions.BottleTypes.Create";
            public const string Edit = "Permissions.BottleTypes.Edit";
            public const string Delete = "Permissions.BottleTypes.Delete";
            public const string Export = "Permissions.BottleTypes.Export";
            public const string Search = "Permissions.BottleTypes.Search";
        }
        public static class CustomerPhones
        {
            public const string View = "Permissions.CustomerPhones.View";
            public const string Create = "Permissions.CustomerPhones.Create";
            public const string Edit = "Permissions.CustomerPhones.Edit";
            public const string Delete = "Permissions.CustomerPhones.Delete";
            public const string Export = "Permissions.CustomerPhones.Export";
            public const string Search = "Permissions.CustomerPhones.Search";
        }
        public static class AccountMovments
        {
            public const string View = "Permissions.AccountMovments.View";
            public const string Create = "Permissions.AccountMovments.Create";
            public const string Edit = "Permissions.AccountMovments.Edit";
            public const string Delete = "Permissions.AccountMovments.Delete";
            public const string Export = "Permissions.AccountMovments.Export";
            public const string Search = "Permissions.AccountMovments.Search";
        }
        public static class Regions
        {
            public const string View = "Permissions.Regions.View";
            public const string Create = "Permissions.Regions.Create";
            public const string Edit = "Permissions.Regions.Edit";
            public const string Delete = "Permissions.Regions.Delete";
            public const string Export = "Permissions.Regions.Export";
            public const string Search = "Permissions.Regions.Search";
        }
        public static class Companies
        {
            public const string View = "Permissions.Companies.View";
            public const string Create = "Permissions.Companies.Create";
            public const string Edit = "Permissions.Companies.Edit";
            public const string Delete = "Permissions.Companies.Delete";
            public const string Export = "Permissions.Companies.Export";
            public const string Search = "Permissions.Companies.Search";
        }
        public static class Stations
        {
            public const string View = "Permissions.Stations.View";
            public const string Create = "Permissions.Stations.Create";
            public const string Edit = "Permissions.Stations.Edit";
            public const string Delete = "Permissions.Stations.Delete";
            public const string Export = "Permissions.Stations.Export";
            public const string Search = "Permissions.Stations.Search";
        }
        public static class Drivers
        {
            public const string View = "Permissions.Drivers.View";
            public const string Create = "Permissions.Drivers.Create";
            public const string Edit = "Permissions.Drivers.Edit";
            public const string Delete = "Permissions.Drivers.Delete";
            public const string Export = "Permissions.Drivers.Export";
            public const string Search = "Permissions.Drivers.Search";
        }
        public static class Talaps
        {
            public const string View = "Permissions.Talaps.View";
            public const string Create = "Permissions.Talaps.Create";
            public const string Edit = "Permissions.Talaps.Edit";
            public const string Delete = "Permissions.Talaps.Delete";
            public const string Export = "Permissions.Talaps.Export";
            public const string Search = "Permissions.Talaps.Search";
        }
        public static class Customers
        {
            public const string View = "Permissions.Customers.View";
            public const string Create = "Permissions.Customers.Create";
            public const string Edit = "Permissions.Customers.Edit";
            public const string Proccess = "Permissions.Customers.Proccess";
            public const string Delete = "Permissions.Customers.Delete";
            public const string Export = "Permissions.Customers.Export";
            public const string Search = "Permissions.Customers.Search";
        }
        public static class AccountName
        {
            public const string View = "Permissions.AccountNames.View";
            public const string Create = "Permissions.AccountNames.Create";
            public const string Edit = "Permissions.AccountNames.Edit";
            public const string Delete = "Permissions.AccountNames.Delete";
            public const string Export = "Permissions.AccountNames.Export";
            public const string Search = "Permissions.AccountNames.Search";
        }
        public static class DriverRegions
        {
            public const string View = "Permissions.DriverRegions.View";
            public const string Create = "Permissions.DriverRegions.Create";
            public const string Edit = "Permissions.DriverRegions.Edit";
            public const string Delete = "Permissions.DriverRegions.Delete";
            public const string Export = "Permissions.DriverRegions.Export";
            public const string Search = "Permissions.DriverRegions.Search";
        }
        public static class Documents
        {
            public const string View = "Permissions.Documents.View";
            public const string Create = "Permissions.Documents.Create";
            public const string Edit = "Permissions.Documents.Edit";
            public const string Delete = "Permissions.Documents.Delete";
            public const string Search = "Permissions.Documents.Search";
        }

        public static class DocumentTypes
        {
            public const string View = "Permissions.DocumentTypes.View";
            public const string Create = "Permissions.DocumentTypes.Create";
            public const string Edit = "Permissions.DocumentTypes.Edit";
            public const string Delete = "Permissions.DocumentTypes.Delete";
            public const string Export = "Permissions.DocumentTypes.Export";
            public const string Search = "Permissions.DocumentTypes.Search";
        }

        public static class DocumentExtendedAttributes
        {
            public const string View = "Permissions.DocumentExtendedAttributes.View";
            public const string Create = "Permissions.DocumentExtendedAttributes.Create";
            public const string Edit = "Permissions.DocumentExtendedAttributes.Edit";
            public const string Delete = "Permissions.DocumentExtendedAttributes.Delete";
            public const string Export = "Permissions.DocumentExtendedAttributes.Export";
            public const string Search = "Permissions.DocumentExtendedAttributes.Search";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Export = "Permissions.Users.Export";
            public const string Search = "Permissions.Users.Search";
        }

        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Create = "Permissions.Roles.Create";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Delete = "Permissions.Roles.Delete";
            public const string Search = "Permissions.Roles.Search";

           
        }

        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Delete = "Permissions.RoleClaims.Delete";
            public const string Search = "Permissions.RoleClaims.Search";
        }

        public static class Communication
        {
            public const string Chat = "Permissions.Communication.Chat";
        }

        public static class Preferences
        {
            public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";

            //TODO - add permissions
        }

        public static class Dashboards
        {
            public const string View = "Permissions.Dashboards.View";
        }

        public static class Hangfire
        {
            public const string View = "Permissions.Hangfire.View";
        }

        public static class AuditTrails
        {
            public const string View = "Permissions.AuditTrails.View";
            public const string Export = "Permissions.AuditTrails.Export";
            public const string Search = "Permissions.AuditTrails.Search";
        }
        /// <summary>
        /// Returns a list of Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRegisteredPermissions()
        {
            var permssions = new List<string>();
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                    permssions.Add(propertyValue.ToString());
            }
            return permssions;
        }
    }
}
using System.Collections.Generic;
using WaterS.Application.Features.Talaps.Queries.GetAllTalaps;
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.Dashboards.Queries.GetData
{
    public class DashboardDataResponse
    {
        public int ProductCount { get; set; }
        public int BottleTypeCount { get; set; }
        public int CompanyCount { get; set; }
        public int CompanyAdminCount { get; set; }
        public int StationAdminCount { get; set; }
        public int DriverAdminCount { get; set; }
        public int CustomerAdminCount { get; set; }
        public int StationManagerCount { get; set; }
        public int DriverManagerCount { get; set; }
        public int DriverStationsCount { get; set; }
        public int StationCount { get; set; }
        public int DriverCount { get; set; }
        public int BrandCount { get; set; }

        public ICollection<GetAllPagedTalapsResponse> AllTalaps { get; set; }
        public ICollection<AccTrans> AllTrans { get; set; }


        public int TalapsCompleted { get; set; }
        public int TalapsInProcess { get; set; }

        public int DocumentCount { get; set; }
        public int DocumentTypeCount { get; set; }
        public int DocumentExtendedAttributeCount { get; set; }
        public int UserCount { get; set; }
        public int RoleCount { get; set; }
        public List<ChartSeries> DataEnterBarChart { get; set; } = new();
        public Dictionary<string, double> ProductByBrandTypePieChart { get; set; }
    }

    public class ChartSeries
    {
        public ChartSeries() { }

        public string Name { get; set; }
        public double[] Data { get; set; }
    }

}
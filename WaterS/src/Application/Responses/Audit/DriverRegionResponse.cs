using System;

namespace WaterS.Application.Responses.Audit
{
    public class DriverRegionResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public string CompanyName { get; set; }
       
        public int myCompany { get; set; }
        public int myStation { get; set; }
       
    }
}
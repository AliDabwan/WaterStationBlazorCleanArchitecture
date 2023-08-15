
using WaterS.Domain.Entities.Catalog;

namespace WaterS.Application.Features.Drivers.Queries.GetDriverById
{
    public class GetDriverByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ResName { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }


        public string Userid { get; set; }
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public int AccountId { get; set; }
        public int No { get; set; }
        public string KindType { get; set; }
        public string KindTypeAr { get; set; }

        public int StationId { get; set; }
        public virtual Station myStation { get; set; }
        public int CompanyId { get; set; }
        public virtual Company myCompany { get; set; }

    }
}
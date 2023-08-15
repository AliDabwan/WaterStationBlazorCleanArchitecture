
namespace WaterS.Application.Features.BottleTypes.Queries.GetById
{
    public class GetBottleTypeByIdResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FillDays { get; set; }
        public decimal Price { get; set; }
    }
}
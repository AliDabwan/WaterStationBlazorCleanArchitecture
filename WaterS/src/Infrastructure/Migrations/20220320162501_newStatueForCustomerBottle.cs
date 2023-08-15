using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class newStatueForCustomerBottle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BottleNoStatue",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottleNoStatue",
                table: "Customers");
        }
    }
}

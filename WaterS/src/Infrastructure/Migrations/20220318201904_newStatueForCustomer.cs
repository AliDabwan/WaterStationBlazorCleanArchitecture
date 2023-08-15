using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class newStatueForCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "statue",
                table: "Customers",
                type: "nvarchar(max)",
                defaultValue:"saved",
                nullable: true );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statue",
                table: "Customers");
        }
    }
}

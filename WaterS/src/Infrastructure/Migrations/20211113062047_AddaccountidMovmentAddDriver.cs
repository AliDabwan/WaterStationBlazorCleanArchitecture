using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class AddaccountidMovmentAddDriver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverId",
                table: "AccTransMovment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverId",
                table: "AccTransMovment");
        }
    }
}

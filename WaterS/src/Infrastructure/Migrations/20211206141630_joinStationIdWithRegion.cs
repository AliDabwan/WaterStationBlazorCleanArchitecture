using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class joinStationIdWithRegion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StationId",
                table: "Regions",
                type: "int",
                nullable: true,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_StationId",
                table: "Regions",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Regions_Stations_StationId",
                table: "Regions",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Regions_Stations_StationId",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Regions_StationId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Regions");
        }
    }
}

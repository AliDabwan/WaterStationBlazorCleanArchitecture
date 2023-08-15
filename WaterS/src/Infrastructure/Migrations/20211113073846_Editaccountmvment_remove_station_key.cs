using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class Editaccountmvment_remove_station_key : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTransMovment_Stations_StationId",
                table: "AccTransMovment");

            migrationBuilder.DropIndex(
                name: "IX_AccTransMovment_StationId",
                table: "AccTransMovment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AccTransMovment_StationId",
                table: "AccTransMovment",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccTransMovment_Stations_StationId",
                table: "AccTransMovment",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

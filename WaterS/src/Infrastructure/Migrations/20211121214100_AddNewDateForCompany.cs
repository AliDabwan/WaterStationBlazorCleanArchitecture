using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class AddNewDateForCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivateDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivateDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");
        }
    }
}

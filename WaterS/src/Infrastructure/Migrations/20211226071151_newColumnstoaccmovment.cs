using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class newColumnstoaccmovment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteCredit",
                table: "AccTransMovment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NoteDebit",
                table: "AccTransMovment",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteCredit",
                table: "AccTransMovment");

            migrationBuilder.DropColumn(
                name: "NoteDebit",
                table: "AccTransMovment");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace WaterS.Infrastructure.Migrations
{
    public partial class AddaccountidMovment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTransMovment_Accounts_AccountsId",
                table: "AccTransMovment");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "AccTransMovment");

            migrationBuilder.AlterColumn<int>(
                name: "AccountsId",
                table: "AccTransMovment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccTransMovment_Accounts_AccountsId",
                table: "AccTransMovment",
                column: "AccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccTransMovment_Accounts_AccountsId",
                table: "AccTransMovment");

            migrationBuilder.AlterColumn<int>(
                name: "AccountsId",
                table: "AccTransMovment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "AccTransMovment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AccTransMovment_Accounts_AccountsId",
                table: "AccTransMovment",
                column: "AccountsId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

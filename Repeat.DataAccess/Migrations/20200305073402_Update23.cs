using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetUsers_AspNetUsers_UserID",
                table: "SetUsers");

            migrationBuilder.DropIndex(
                name: "IX_SetUsers_UserID",
                table: "SetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAlive",
                table: "Questions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAlive",
                table: "Questions");

            migrationBuilder.CreateIndex(
                name: "IX_SetUsers_UserID",
                table: "SetUsers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_SetUsers_AspNetUsers_UserID",
                table: "SetUsers",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

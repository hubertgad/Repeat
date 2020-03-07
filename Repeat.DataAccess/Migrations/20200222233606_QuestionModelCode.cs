using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class QuestionModelCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Questions",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_SetID",
                table: "Tests",
                column: "SetID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Sets_SetID",
                table: "Tests",
                column: "SetID",
                principalTable: "Sets",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Sets_SetID",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_SetID",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Questions");
        }
    }
}

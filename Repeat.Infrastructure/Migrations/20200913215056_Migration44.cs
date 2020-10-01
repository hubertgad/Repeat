using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Migration44 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestQuestions_Tests_TestID1",
                table: "TestQuestions");

            migrationBuilder.DropIndex(
                name: "IX_TestQuestions_TestID1",
                table: "TestQuestions");

            migrationBuilder.DropColumn(
                name: "TestID1",
                table: "TestQuestions");

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionID",
                table: "Tests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentQuestionID",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "TestID1",
                table: "TestQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestQuestions_TestID1",
                table: "TestQuestions",
                column: "TestID1",
                unique: true,
                filter: "[TestID1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_TestQuestions_Tests_TestID1",
                table: "TestQuestions",
                column: "TestID1",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

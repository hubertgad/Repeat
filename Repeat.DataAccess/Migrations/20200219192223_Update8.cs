using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerPoints_Tests_TestID",
                table: "AnswerPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerPoints",
                table: "AnswerPoints");

            migrationBuilder.RenameTable(
                name: "AnswerPoints",
                newName: "QuestionPoints");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerPoints_TestID",
                table: "QuestionPoints",
                newName: "IX_QuestionPoints_TestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionPoints",
                table: "QuestionPoints",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionPoints_Tests_TestID",
                table: "QuestionPoints",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionPoints_Tests_TestID",
                table: "QuestionPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionPoints",
                table: "QuestionPoints");

            migrationBuilder.RenameTable(
                name: "QuestionPoints",
                newName: "AnswerPoints");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionPoints_TestID",
                table: "AnswerPoints",
                newName: "IX_AnswerPoints_TestID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerPoints",
                table: "AnswerPoints",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerPoints_Tests_TestID",
                table: "AnswerPoints",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

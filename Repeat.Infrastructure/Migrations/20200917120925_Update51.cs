using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update51 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_TestQuestions_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "TestQuestionTestID",
                table: "ChoosenAnswers");

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_TestID_QuestionID",
                table: "ChoosenAnswers",
                columns: new[] { "TestID", "QuestionID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_TestQuestions_TestID_QuestionID",
                table: "ChoosenAnswers",
                columns: new[] { "TestID", "QuestionID" },
                principalTable: "TestQuestions",
                principalColumns: new[] { "TestID", "QuestionID" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_TestQuestions_TestID_QuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_TestID_QuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.AddColumn<int>(
                name: "TestQuestionQuestionID",
                table: "ChoosenAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestQuestionTestID",
                table: "ChoosenAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers",
                columns: new[] { "TestQuestionTestID", "TestQuestionQuestionID" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_TestQuestions_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers",
                columns: new[] { "TestQuestionTestID", "TestQuestionQuestionID" },
                principalTable: "TestQuestions",
                principalColumns: new[] { "TestID", "QuestionID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}

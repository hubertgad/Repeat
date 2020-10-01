using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update42 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.DropTable(
                name: "QuestionResponses");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.AddColumn<int>(
                name: "TestID",
                table: "ChoosenAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestQuestionQuestionID",
                table: "ChoosenAnswers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestQuestionTestID",
                table: "ChoosenAnswers",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_TestQuestions_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_TestQuestionTestID_TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "TestID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "TestQuestionQuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "TestQuestionTestID",
                table: "ChoosenAnswers");

            migrationBuilder.AddColumn<int>(
                name: "QuestionResponseID",
                table: "ChoosenAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionID = table.Column<int>(type: "int", nullable: false),
                    TestID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionResponses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionResponses_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_QuestionResponseID",
                table: "ChoosenAnswers",
                column: "QuestionResponseID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionResponses_TestID",
                table: "QuestionResponses",
                column: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers",
                column: "QuestionResponseID",
                principalTable: "QuestionResponses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

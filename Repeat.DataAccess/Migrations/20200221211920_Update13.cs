using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_Tests_TestID",
                table: "ChoosenAnswers");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_TestID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "AnswerIndex",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionIndex",
                table: "ChoosenAnswers");

            migrationBuilder.AddColumn<int>(
                name: "AnswerID",
                table: "ChoosenAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionID",
                table: "ChoosenAnswers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionResponseID",
                table: "ChoosenAnswers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionResponses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false)
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
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "AnswerID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.AddColumn<int>(
                name: "AnswerIndex",
                table: "ChoosenAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "CorrectAnswer",
                table: "ChoosenAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "QuestionIndex",
                table: "ChoosenAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_TestID",
                table: "ChoosenAnswers",
                column: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_Tests_TestID",
                table: "ChoosenAnswers",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

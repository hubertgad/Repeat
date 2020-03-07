using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerPoints");

            migrationBuilder.DropTable(
                name: "QuestionPoints");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Tests",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ChoosenAnswers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestID = table.Column<int>(nullable: false),
                    QuestionIndex = table.Column<int>(nullable: false),
                    AnswerIndex = table.Column<int>(nullable: false),
                    GivenAnswer = table.Column<bool>(nullable: false),
                    CorrectAnswer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoosenAnswers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChoosenAnswers_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_TestID",
                table: "ChoosenAnswers",
                column: "TestID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoosenAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionPoints",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionIndex = table.Column<int>(type: "int", nullable: false),
                    TestID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionPoints_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerPoints",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerIndex = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<bool>(type: "bit", nullable: false),
                    GivenAnswer = table.Column<bool>(type: "bit", nullable: false),
                    QuestionPointID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                        column: x => x.QuestionPointID,
                        principalTable: "QuestionPoints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerPoints_QuestionPointID",
                table: "AnswerPoints",
                column: "QuestionPointID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPoints_TestID",
                table: "QuestionPoints",
                column: "TestID");
        }
    }
}

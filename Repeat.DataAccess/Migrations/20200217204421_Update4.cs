using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestID",
                table: "Questions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    ActualQuestionIndex = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AnswerPoints",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestID = table.Column<int>(nullable: false),
                    QuestionIndex = table.Column<int>(nullable: false),
                    AchievedPoints = table.Column<int>(nullable: false),
                    MaxPoints = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AnswerPoints_Tests_TestID",
                        column: x => x.TestID,
                        principalTable: "Tests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TestID",
                table: "Questions",
                column: "TestID");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerPoints_TestID",
                table: "AnswerPoints",
                column: "TestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tests_TestID",
                table: "Questions",
                column: "TestID",
                principalTable: "Tests",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tests_TestID",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "AnswerPoints");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TestID",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TestID",
                table: "Questions");
        }
    }
}

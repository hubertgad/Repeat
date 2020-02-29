using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnswerPoints",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerIndex = table.Column<int>(nullable: false),
                    GivenAnswer = table.Column<bool>(nullable: false),
                    CorrectAnswer = table.Column<bool>(nullable: false),
                    QuestionPointID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                        column: x => x.QuestionPointID,
                        principalTable: "QuestionPoints",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerPoints_QuestionPointID",
                table: "AnswerPoints",
                column: "QuestionPointID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerPoints");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionResponseID",
                table: "ChoosenAnswers",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers",
                column: "QuestionResponseID",
                principalTable: "QuestionResponses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionResponseID",
                table: "ChoosenAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_QuestionResponses_QuestionResponseID",
                table: "ChoosenAnswers",
                column: "QuestionResponseID",
                principalTable: "QuestionResponses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

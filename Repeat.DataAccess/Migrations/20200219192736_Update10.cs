using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                table: "AnswerPoints");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionPointID",
                table: "AnswerPoints",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                table: "AnswerPoints",
                column: "QuestionPointID",
                principalTable: "QuestionPoints",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                table: "AnswerPoints");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionPointID",
                table: "AnswerPoints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerPoints_QuestionPoints_QuestionPointID",
                table: "AnswerPoints",
                column: "QuestionPointID",
                principalTable: "QuestionPoints",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

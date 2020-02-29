using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class update22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID",
                table: "DeletedAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DeletedAnswers_DeletedQuestionID",
                table: "DeletedAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedQuestionID",
                table: "DeletedAnswers");

            migrationBuilder.AddColumn<int>(
                name: "DeletedQuestionID1",
                table: "DeletedAnswers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAnswers_DeletedQuestionID1",
                table: "DeletedAnswers",
                column: "DeletedQuestionID1");

            migrationBuilder.AddForeignKey(
                name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID1",
                table: "DeletedAnswers",
                column: "DeletedQuestionID1",
                principalTable: "DeletedQuestions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID1",
                table: "DeletedAnswers");

            migrationBuilder.DropIndex(
                name: "IX_DeletedAnswers_DeletedQuestionID1",
                table: "DeletedAnswers");

            migrationBuilder.DropColumn(
                name: "DeletedQuestionID1",
                table: "DeletedAnswers");

            migrationBuilder.AddColumn<int>(
                name: "DeletedQuestionID",
                table: "DeletedAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAnswers_DeletedQuestionID",
                table: "DeletedAnswers",
                column: "DeletedQuestionID");

            migrationBuilder.AddForeignKey(
                name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID",
                table: "DeletedAnswers",
                column: "DeletedQuestionID",
                principalTable: "DeletedQuestions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

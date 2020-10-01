using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update52 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Sets",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Questions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Shares_UserID",
                table: "Shares",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_OwnerID",
                table: "Sets",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_OwnerID",
                table: "Questions",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_ChoosenAnswers_QuestionID",
                table: "ChoosenAnswers",
                column: "QuestionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChoosenAnswers_Questions_QuestionID",
                table: "ChoosenAnswers",
                column: "QuestionID",
                principalTable: "Questions",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_OwnerID",
                table: "Questions",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Sets_AspNetUsers_OwnerID",
                table: "Sets",
                column: "OwnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Shares_AspNetUsers_UserID",
                table: "Shares",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChoosenAnswers_Questions_QuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_OwnerID",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sets_AspNetUsers_OwnerID",
                table: "Sets");

            migrationBuilder.DropForeignKey(
                name: "FK_Shares_AspNetUsers_UserID",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Shares_UserID",
                table: "Shares");

            migrationBuilder.DropIndex(
                name: "IX_Sets_OwnerID",
                table: "Sets");

            migrationBuilder.DropIndex(
                name: "IX_Questions_OwnerID",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_ChoosenAnswers_QuestionID",
                table: "ChoosenAnswers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Sets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "OwnerID",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievedPoints",
                table: "AnswerPoints");

            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "AnswerPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AchievedPoints",
                table: "AnswerPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "AnswerPoints",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

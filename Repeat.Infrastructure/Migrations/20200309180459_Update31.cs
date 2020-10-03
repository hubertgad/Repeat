using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class Update31 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedAnswers");

            migrationBuilder.DropTable(
                name: "DeletedPictures");

            migrationBuilder.DropTable(
                name: "DeletedQuestions");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Answers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Answers");

            migrationBuilder.CreateTable(
                name: "DeletedQuestions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    CategoryID1 = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OwnerID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PictureID = table.Column<int>(type: "int", nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedQuestions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeletedQuestions_Categories_CategoryID1",
                        column: x => x.CategoryID1,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeletedQuestions_Pictures_PictureID",
                        column: x => x.PictureID,
                        principalTable: "Pictures",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeletedAnswers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DeletedQuestionID1 = table.Column<int>(type: "int", nullable: true),
                    IsTrue = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedAnswers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID1",
                        column: x => x.DeletedQuestionID1,
                        principalTable: "DeletedQuestions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeletedPictures",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedPictures", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeletedPictures_DeletedQuestions_ID",
                        column: x => x.ID,
                        principalTable: "DeletedQuestions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeletedAnswers_DeletedQuestionID1",
                table: "DeletedAnswers",
                column: "DeletedQuestionID1");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedQuestions_CategoryID1",
                table: "DeletedQuestions",
                column: "CategoryID1");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedQuestions_PictureID",
                table: "DeletedQuestions",
                column: "PictureID");
        }
    }
}

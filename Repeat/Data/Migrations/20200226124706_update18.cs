using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repeat.Data.Migrations
{
    public partial class update18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeletedQuestions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    QuestionText = table.Column<string>(maxLength: 1000, nullable: false),
                    Code = table.Column<string>(maxLength: 1000, nullable: true),
                    OwnerID = table.Column<string>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    PictureID = table.Column<int>(nullable: true),
                    CategoryID1 = table.Column<int>(nullable: true)
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
                    ID = table.Column<int>(nullable: false),
                    AnswerText = table.Column<string>(maxLength: 1000, nullable: false),
                    IsTrue = table.Column<bool>(nullable: false),
                    DeletedQuestionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedAnswers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeletedAnswers_DeletedQuestions_DeletedQuestionID",
                        column: x => x.DeletedQuestionID,
                        principalTable: "DeletedQuestions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeletedPictures",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true)
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
                name: "IX_DeletedAnswers_DeletedQuestionID",
                table: "DeletedAnswers",
                column: "DeletedQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedQuestions_CategoryID1",
                table: "DeletedQuestions",
                column: "CategoryID1");

            migrationBuilder.CreateIndex(
                name: "IX_DeletedQuestions_PictureID",
                table: "DeletedQuestions",
                column: "PictureID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeletedAnswers");

            migrationBuilder.DropTable(
                name: "DeletedPictures");

            migrationBuilder.DropTable(
                name: "DeletedQuestions");
        }
    }
}

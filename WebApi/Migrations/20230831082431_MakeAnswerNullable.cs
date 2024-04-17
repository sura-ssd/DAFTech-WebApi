using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class MakeAnswerNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminAnswers_QuestionAnswers_QuestionAnswerId",
                table: "AdminAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Client_ClientId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_ClientId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_AdminAnswers_QuestionAnswerId",
                table: "AdminAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "QuestionAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_ClientId",
                table: "QuestionAnswers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAnswers_QuestionAnswerId",
                table: "AdminAnswers",
                column: "QuestionAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminAnswers_QuestionAnswers_QuestionAnswerId",
                table: "AdminAnswers",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Client_ClientId",
                table: "QuestionAnswers",
                column: "ClientId",
                principalTable: "Client",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

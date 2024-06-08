using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headhunter.Migrations
{
    /// <inheritdoc />
    public partial class AddedResumeAndVacancyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResumeId",
                table: "Chats",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VacancyId",
                table: "Chats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ResumeId",
                table: "Chats",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_VacancyId",
                table: "Chats",
                column: "VacancyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Resumes_ResumeId",
                table: "Chats",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Vacancies_VacancyId",
                table: "Chats",
                column: "VacancyId",
                principalTable: "Vacancies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Resumes_ResumeId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Vacancies_VacancyId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ResumeId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_VacancyId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "VacancyId",
                table: "Chats");
        }
    }
}

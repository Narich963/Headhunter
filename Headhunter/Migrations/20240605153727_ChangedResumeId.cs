using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Headhunter.Migrations
{
    /// <inheritdoc />
    public partial class ChangedResumeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Resumes_ResumeId",
                table: "Modules");

            migrationBuilder.AlterColumn<int>(
                name: "ResumeId",
                table: "Modules",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Resumes_ResumeId",
                table: "Modules",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Resumes_ResumeId",
                table: "Modules");

            migrationBuilder.AlterColumn<int>(
                name: "ResumeId",
                table: "Modules",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Resumes_ResumeId",
                table: "Modules",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

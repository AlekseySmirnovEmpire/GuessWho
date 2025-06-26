using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Task13_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Files_FileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Files_FileId",
                table: "Users",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Files_FileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Files_FileId",
                table: "Users",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640.Migrations
{
    /// <inheritdoc />
    public partial class InitFileDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__FileDetai__contr__6754599E",
                table: "FileDetails");

            migrationBuilder.DropIndex(
                name: "IX_FileDetails_contributionId",
                table: "FileDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_contributionId",
                table: "FileDetails",
                column: "contributionId");

            migrationBuilder.AddForeignKey(
                name: "FK__FileDetai__contr__6754599E",
                table: "FileDetails",
                column: "contributionId",
                principalTable: "Contributions",
                principalColumn: "contributionId");
        }
    }
}

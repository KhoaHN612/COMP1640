using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP1640.Migrations
{
    /// <inheritdoc />
    public partial class FileDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "contributionId",
                table: "FileDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileDetails_contributionId",
                table: "FileDetails",
                column: "contributionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileDetails_Contributions_contributionId",
                table: "FileDetails",
                column: "contributionId",
                principalTable: "Contributions",
                principalColumn: "contributionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileDetails_Contributions_contributionId",
                table: "FileDetails");

            migrationBuilder.DropIndex(
                name: "IX_FileDetails_contributionId",
                table: "FileDetails");

            migrationBuilder.AlterColumn<int>(
                name: "contributionId",
                table: "FileDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}

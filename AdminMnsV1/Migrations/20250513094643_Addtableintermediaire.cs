using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class Addtableintermediaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentId",
                table: "Classs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classs_StudentId",
                table: "Classs",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classs_AspNetUsers_StudentId",
                table: "Classs",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classs_AspNetUsers_StudentId",
                table: "Classs");

            migrationBuilder.DropIndex(
                name: "IX_Classs_StudentId",
                table: "Classs");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Classs");
        }
    }
}

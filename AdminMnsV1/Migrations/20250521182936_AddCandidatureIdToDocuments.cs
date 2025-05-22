using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidatureIdToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidatureId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CandidatureId",
                table: "Documents",
                column: "CandidatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents",
                column: "CandidatureId",
                principalTable: "Candidatures",
                principalColumn: "CandidatureId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CandidatureId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CandidatureId",
                table: "Documents");
        }
    }
}

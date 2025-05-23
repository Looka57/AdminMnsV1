using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Attends_AspNetUsers_StudentId",
                table: "Attends");

            migrationBuilder.DropForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_AspNetUsers_UserId",
                table: "Candidatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_CandidatureStatus_candidatureStatutId",
                table: "Candidatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_AdminId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_StudentId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Candidatures_candidatureStatutId",
                table: "Candidatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatureStatus",
                table: "CandidatureStatus");

            migrationBuilder.DropColumn(
                name: "documentDateStatutValidate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "documentStatut",
                table: "Documents");

            migrationBuilder.RenameTable(
                name: "CandidatureStatus",
                newName: "CandidatureStatuses");

            migrationBuilder.RenameColumn(
                name: "documentName",
                table: "Documents",
                newName: "DocumentName");

            migrationBuilder.RenameColumn(
                name: "documentDepositDate",
                table: "Documents",
                newName: "DocumentDepositDate");

            migrationBuilder.RenameColumn(
                name: "candidatureStatutId",
                table: "Candidatures",
                newName: "Progress");

            migrationBuilder.AddColumn<int>(
                name: "DocumentStatusId",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CandidatureStatusId",
                table: "Candidatures",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatureStatuses",
                table: "CandidatureStatuses",
                column: "CandidatureStatusId");

            migrationBuilder.CreateTable(
                name: "DocumentStatuses",
                columns: table => new
                {
                    DocumentStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentStatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatuses", x => x.DocumentStatusId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentStatusId",
                table: "Documents",
                column: "DocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_CandidatureStatusId",
                table: "Candidatures",
                column: "CandidatureStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_AspNetUsers_StudentId",
                table: "Attends",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends",
                column: "ClasseId",
                principalTable: "SchoolClass",
                principalColumn: "ClasseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_AspNetUsers_UserId",
                table: "Candidatures",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_CandidatureStatuses_CandidatureStatusId",
                table: "Candidatures",
                column: "CandidatureStatusId",
                principalTable: "CandidatureStatuses",
                principalColumn: "CandidatureStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_AdminId",
                table: "Documents",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_StudentId",
                table: "Documents",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents",
                column: "CandidatureId",
                principalTable: "Candidatures",
                principalColumn: "CandidatureId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentStatuses_DocumentStatusId",
                table: "Documents",
                column: "DocumentStatusId",
                principalTable: "DocumentStatuses",
                principalColumn: "DocumentStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Attends_AspNetUsers_StudentId",
                table: "Attends");

            migrationBuilder.DropForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_AspNetUsers_UserId",
                table: "Candidatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidatures_CandidatureStatuses_CandidatureStatusId",
                table: "Candidatures");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_AdminId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_AspNetUsers_StudentId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentStatuses_DocumentStatusId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentStatusId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Candidatures_CandidatureStatusId",
                table: "Candidatures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CandidatureStatuses",
                table: "CandidatureStatuses");

            migrationBuilder.DropColumn(
                name: "DocumentStatusId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CandidatureStatusId",
                table: "Candidatures");

            migrationBuilder.RenameTable(
                name: "CandidatureStatuses",
                newName: "CandidatureStatus");

            migrationBuilder.RenameColumn(
                name: "DocumentName",
                table: "Documents",
                newName: "documentName");

            migrationBuilder.RenameColumn(
                name: "DocumentDepositDate",
                table: "Documents",
                newName: "documentDepositDate");

            migrationBuilder.RenameColumn(
                name: "Progress",
                table: "Candidatures",
                newName: "candidatureStatutId");

            migrationBuilder.AddColumn<string>(
                name: "documentDateStatutValidate",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "documentStatut",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CandidatureStatus",
                table: "CandidatureStatus",
                column: "CandidatureStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_candidatureStatutId",
                table: "Candidatures",
                column: "candidatureStatutId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_AspNetUsers_StudentId",
                table: "Attends",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends",
                column: "ClasseId",
                principalTable: "SchoolClass",
                principalColumn: "ClasseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_AspNetUsers_UserId",
                table: "Candidatures",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Candidatures_CandidatureStatus_candidatureStatutId",
                table: "Candidatures",
                column: "candidatureStatutId",
                principalTable: "CandidatureStatus",
                principalColumn: "CandidatureStatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_AdminId",
                table: "Documents",
                column: "AdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_AspNetUsers_StudentId",
                table: "Documents",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Candidatures_CandidatureId",
                table: "Documents",
                column: "CandidatureId",
                principalTable: "Candidatures",
                principalColumn: "CandidatureId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

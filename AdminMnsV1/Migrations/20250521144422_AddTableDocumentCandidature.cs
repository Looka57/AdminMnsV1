using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class AddTableDocumentCandidature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attends_Classs_ClasseId",
                table: "Attends");

            migrationBuilder.DropTable(
                name: "Classs");

            migrationBuilder.CreateTable(
                name: "CandidatureStatus",
                columns: table => new
                {
                    CandidatureStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidatureStatus", x => x.CandidatureStatusId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameDocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocumentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SchoolClass",
                columns: table => new
                {
                    ClasseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClass", x => x.ClasseId);
                    table.ForeignKey(
                        name: "FK_SchoolClass_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    documentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    documentDepositDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    documentStatut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    documentDateStatutValidate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Documents_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "DocumentTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    CandidatureId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CandidatureCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CandidatureValidationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    candidatureStatutId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.CandidatureId);
                    table.ForeignKey(
                        name: "FK_Candidatures_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidatures_CandidatureStatus_candidatureStatutId",
                        column: x => x.candidatureStatutId,
                        principalTable: "CandidatureStatus",
                        principalColumn: "CandidatureStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Candidatures_SchoolClass_ClassId",
                        column: x => x.ClassId,
                        principalTable: "SchoolClass",
                        principalColumn: "ClasseId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_candidatureStatutId",
                table: "Candidatures",
                column: "candidatureStatutId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_ClassId",
                table: "Candidatures",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_UserId",
                table: "Candidatures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AdminId",
                table: "Documents",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId",
                table: "Documents",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StudentId",
                table: "Documents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClass_StudentId",
                table: "SchoolClass",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends",
                column: "ClasseId",
                principalTable: "SchoolClass",
                principalColumn: "ClasseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attends_SchoolClass_ClasseId",
                table: "Attends");

            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "CandidatureStatus");

            migrationBuilder.DropTable(
                name: "SchoolClass");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.CreateTable(
                name: "Classs",
                columns: table => new
                {
                    ClasseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYear = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    NameClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classs", x => x.ClasseId);
                    table.ForeignKey(
                        name: "FK_Classs_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classs_StudentId",
                table: "Classs",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_Classs_ClasseId",
                table: "Attends",
                column: "ClasseId",
                principalTable: "Classs",
                principalColumn: "ClasseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableDelayAbsenceStatusReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdministratorId",
                table: "Attends",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.CreateTable(
                name: "DelayAbsStatuses",
                columns: table => new
                {
                    DelayAbsStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelayAbsStatuses", x => x.DelayAbsStatusId);
                });

            migrationBuilder.CreateTable(
                name: "ReasonAbsences",
                columns: table => new
                {
                    ReasonAbsenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReasonWording = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonAbsences", x => x.ReasonAbsenceId);
                });

            migrationBuilder.CreateTable(
                name: "ReasonDelays",
                columns: table => new
                {
                    ReasonDelayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonDelays", x => x.ReasonDelayId);
                });

            migrationBuilder.CreateTable(
                name: "Absences",
                columns: table => new
                {
                    AbsenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JustificationStatus = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateValidated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministratorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReasonAbsenceId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absences", x => x.AbsenceId);
                    table.ForeignKey(
                        name: "FK_Absences_AspNetUsers_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Absences_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Absences_DelayAbsStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "DelayAbsStatuses",
                        principalColumn: "DelayAbsStatusId");
                    table.ForeignKey(
                        name: "FK_Absences_ReasonAbsences_ReasonAbsenceId",
                        column: x => x.ReasonAbsenceId,
                        principalTable: "ReasonAbsences",
                        principalColumn: "ReasonAbsenceId");
                });

            migrationBuilder.CreateTable(
                name: "Delays",
                columns: table => new
                {
                    DelayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JustificationStatus = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateValidated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministratorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReasonDelayId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delays", x => x.DelayId);
                    table.ForeignKey(
                        name: "FK_Delays_AspNetUsers_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Delays_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Delays_DelayAbsStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "DelayAbsStatuses",
                        principalColumn: "DelayAbsStatusId");
                    table.ForeignKey(
                        name: "FK_Delays_ReasonDelays_ReasonDelayId",
                        column: x => x.ReasonDelayId,
                        principalTable: "ReasonDelays",
                        principalColumn: "ReasonDelayId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attends_AdministratorId",
                table: "Attends",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_AdministratorId",
                table: "Absences",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_ReasonAbsenceId",
                table: "Absences",
                column: "ReasonAbsenceId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_StatusId",
                table: "Absences",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Absences_StudentId",
                table: "Absences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Delays_AdministratorId",
                table: "Delays",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Delays_ReasonDelayId",
                table: "Delays",
                column: "ReasonDelayId");

            migrationBuilder.CreateIndex(
                name: "IX_Delays_StatusId",
                table: "Delays",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Delays_StudentId",
                table: "Delays",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attends_AspNetUsers_AdministratorId",
                table: "Attends",
                column: "AdministratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attends_AspNetUsers_AdministratorId",
                table: "Attends");

            migrationBuilder.DropTable(
                name: "Absences");

            migrationBuilder.DropTable(
                name: "Delays");

            migrationBuilder.DropTable(
                name: "ReasonAbsences");

            migrationBuilder.DropTable(
                name: "DelayAbsStatuses");

            migrationBuilder.DropTable(
                name: "ReasonDelays");

            migrationBuilder.DropIndex(
                name: "IX_Attends_AdministratorId",
                table: "Attends");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "Attends");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);
        }
    }
}

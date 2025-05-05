using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedPropertyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

        }
    }
}

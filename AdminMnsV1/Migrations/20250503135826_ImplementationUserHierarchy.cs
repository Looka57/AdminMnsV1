using System;
using AdminMnsV1.Models;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminMnsV1.Migrations
{
    /// <inheritdoc />
    public partial class ImplementationUserHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sexe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),

                    //Le Discriminator est une colonne spéciale qu'Entity Framework Core utilise dans le cadre de la stratégie d'héritage Table Per Hierarchy (TPH).
                    //Discriminator est une colonne dans votre table de base(dans notre cas, la table Users) qui indique le type.NET(la classe C#) auquel correspond chaque enregistrement de cette table.
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),

                    //Table Admin
                    Service = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Administrator_Role = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    //Table Expert
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: true),

                    //Table Student
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SocialSecurityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FranceTravailNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

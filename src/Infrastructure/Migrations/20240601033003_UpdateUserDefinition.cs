using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackHub.Security.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "passwordreset",
                schema: "security",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "email",
                schema: "security",
                table: "users",
                newName: "emailaddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "emailaddress",
                schema: "security",
                table: "users",
                newName: "email");

            migrationBuilder.AddColumn<DateTime>(
                name: "passwordreset",
                schema: "security",
                table: "users",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}

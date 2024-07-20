using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginAttemptsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "loginattempts",
                schema: "security",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loginattempts",
                schema: "security",
                table: "users");
        }
    }
}

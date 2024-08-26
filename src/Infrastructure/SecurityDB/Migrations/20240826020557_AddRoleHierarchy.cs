using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "parentroleid",
                schema: "security",
                table: "roles",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_parentroleid",
                schema: "security",
                table: "roles",
                column: "parentroleid");

            migrationBuilder.AddForeignKey(
                name: "FK_roles_roles_parentroleid",
                schema: "security",
                table: "roles",
                column: "parentroleid",
                principalSchema: "security",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_roles_roles_parentroleid",
                schema: "security",
                table: "roles");

            migrationBuilder.DropIndex(
                name: "IX_roles_parentroleid",
                schema: "security",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "parentroleid",
                schema: "security",
                table: "roles");
        }
    }
}

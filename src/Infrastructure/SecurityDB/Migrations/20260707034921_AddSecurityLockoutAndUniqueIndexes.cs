using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityLockoutAndUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_service_client_permissions_clientid_accountid_resource_acti~",
                schema: "security",
                table: "service_client_permissions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockeduntil",
                schema: "security",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_service_client_permissions_clientid_accountid_resource_acti~",
                schema: "security",
                table: "service_client_permissions",
                columns: new[] { "clientid", "accountid", "resource", "action", "scope", "audience" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clients_name",
                schema: "security",
                table: "clients",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_service_client_permissions_clientid_accountid_resource_acti~",
                schema: "security",
                table: "service_client_permissions");

            migrationBuilder.DropIndex(
                name: "IX_clients_name",
                schema: "security",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "lockeduntil",
                schema: "security",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "IX_service_client_permissions_clientid_accountid_resource_acti~",
                schema: "security",
                table: "service_client_permissions",
                columns: new[] { "clientid", "accountid", "resource", "action" });
        }
    }
}

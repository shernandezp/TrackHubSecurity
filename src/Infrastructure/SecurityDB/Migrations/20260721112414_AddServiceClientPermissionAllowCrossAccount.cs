using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceClientPermissionAllowCrossAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "allowcrossaccount",
                schema: "security",
                table: "service_client_permissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Behaviour-preserving backfill. Until now a NULL accountid was an implicit wildcard:
            // the enforcement predicate was (!permission.AccountId.HasValue || permission.AccountId
            // == accountId), so such a row matched a token for ANY tenant. Those rows ARE
            // cross-account grants today, so upgrading a live deployment must declare them as such
            // rather than silently revoking them. Grants already bound to a specific account are
            // untouched and become genuinely tenant-bound.
            // Operators should review security.service_client_permissions after upgrading and clear
            // allowcrossaccount on any client that does not need platform-wide reach.
            migrationBuilder.Sql(
                """
                UPDATE security.service_client_permissions
                SET allowcrossaccount = TRUE
                WHERE accountid IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allowcrossaccount",
                schema: "security",
                table: "service_client_permissions");
        }
    }
}

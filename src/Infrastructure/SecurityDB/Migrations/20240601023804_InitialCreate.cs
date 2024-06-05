using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.CreateTable(
                name: "policies",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    secondname = table.Column<string>(type: "text", nullable: true),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    secondsurname = table.Column<string>(type: "text", nullable: true),
                    dob = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    passwordreset = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    verified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "actions",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ResourceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_actions_resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "security",
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_policy",
                schema: "security",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    policyid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_policy", x => new { x.policyid, x.userid });
                    table.ForeignKey(
                        name: "FK_user_policy_policies_policyid",
                        column: x => x.policyid,
                        principalSchema: "security",
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_policy_users_userid",
                        column: x => x.userid,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "security",
                columns: table => new
                {
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    roleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.roleid, x.userid });
                    table.ForeignKey(
                        name: "FK_user_role_roles_roleid",
                        column: x => x.roleid,
                        principalSchema: "security",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_role_users_userid",
                        column: x => x.userid,
                        principalSchema: "security",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_action_policy",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resourceid = table.Column<int>(type: "integer", nullable: false),
                    actionid = table.Column<int>(type: "integer", nullable: false),
                    policyid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_action_policy", x => x.id);
                    table.ForeignKey(
                        name: "FK_resource_action_policy_actions_actionid",
                        column: x => x.actionid,
                        principalSchema: "security",
                        principalTable: "actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_action_policy_policies_policyid",
                        column: x => x.policyid,
                        principalSchema: "security",
                        principalTable: "policies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_action_policy_resources_resourceid",
                        column: x => x.resourceid,
                        principalSchema: "security",
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_action_role",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resourceid = table.Column<int>(type: "integer", nullable: false),
                    actionid = table.Column<int>(type: "integer", nullable: false),
                    roleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_action_role", x => x.id);
                    table.ForeignKey(
                        name: "FK_resource_action_role_actions_actionid",
                        column: x => x.actionid,
                        principalSchema: "security",
                        principalTable: "actions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_action_role_resources_resourceid",
                        column: x => x.resourceid,
                        principalSchema: "security",
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_action_role_roles_roleid",
                        column: x => x.roleid,
                        principalSchema: "security",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actions_ResourceId",
                schema: "security",
                table: "actions",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_policy_actionid",
                schema: "security",
                table: "resource_action_policy",
                column: "actionid");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_policy_policyid",
                schema: "security",
                table: "resource_action_policy",
                column: "policyid");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_policy_resourceid",
                schema: "security",
                table: "resource_action_policy",
                column: "resourceid");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_role_actionid",
                schema: "security",
                table: "resource_action_role",
                column: "actionid");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_role_resourceid",
                schema: "security",
                table: "resource_action_role",
                column: "resourceid");

            migrationBuilder.CreateIndex(
                name: "IX_resource_action_role_roleid",
                schema: "security",
                table: "resource_action_role",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_user_policy_userid",
                schema: "security",
                table: "user_policy",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_userid",
                schema: "security",
                table: "user_role",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "resource_action_policy",
                schema: "security");

            migrationBuilder.DropTable(
                name: "resource_action_role",
                schema: "security");

            migrationBuilder.DropTable(
                name: "user_policy",
                schema: "security");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "security");

            migrationBuilder.DropTable(
                name: "actions",
                schema: "security");

            migrationBuilder.DropTable(
                name: "policies",
                schema: "security");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "security");

            migrationBuilder.DropTable(
                name: "users",
                schema: "security");

            migrationBuilder.DropTable(
                name: "resources",
                schema: "security");
        }
    }
}

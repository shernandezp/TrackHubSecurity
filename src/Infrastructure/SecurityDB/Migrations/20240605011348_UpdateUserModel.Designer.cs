﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TrackHub.Security.Infrastructure.SecurityDB;

#nullable disable

namespace TrackHub.Security.Infrastructure.SecurityDB.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240605011348_UpdateUserModel")]
    partial class UpdateUserModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", b =>
                {
                    b.Property<int>("ActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ActionId"));

                    b.Property<string>("ActionName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer");

                    b.HasKey("ActionId");

                    b.HasIndex("ResourceId");

                    b.ToTable("actions", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Policy", b =>
                {
                    b.Property<int>("PolicyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PolicyId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<string>("PolicyName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.HasKey("PolicyId");

                    b.ToTable("policies", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", b =>
                {
                    b.Property<int>("ResourceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ResourceId"));

                    b.Property<string>("ResourceName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.HasKey("ResourceId");

                    b.ToTable("resources", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionPolicy", b =>
                {
                    b.Property<int>("ResourceActionPolicyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ResourceActionPolicyId"));

                    b.Property<int>("ActionId")
                        .HasColumnType("integer")
                        .HasColumnName("actionid");

                    b.Property<int>("PolicyId")
                        .HasColumnType("integer")
                        .HasColumnName("policyid");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer")
                        .HasColumnName("resourceid");

                    b.HasKey("ResourceActionPolicyId");

                    b.HasIndex("ActionId");

                    b.HasIndex("PolicyId");

                    b.HasIndex("ResourceId");

                    b.ToTable("resource_action_policy", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionRole", b =>
                {
                    b.Property<int>("ResourceActionRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ResourceActionRoleId"));

                    b.Property<int>("ActionId")
                        .HasColumnType("integer")
                        .HasColumnName("actionid");

                    b.Property<int>("ResourceId")
                        .HasColumnType("integer")
                        .HasColumnName("resourceid");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("roleid");

                    b.HasKey("ResourceActionRoleId");

                    b.HasIndex("ActionId");

                    b.HasIndex("ResourceId");

                    b.HasIndex("RoleId");

                    b.ToTable("resource_action_role", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RoleId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.HasKey("RoleId");

                    b.ToTable("roles", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid")
                        .HasColumnName("accountid");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DOB")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("dob");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("emailaddress");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("firstname");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("lastname");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("password");

                    b.Property<string>("SecondName")
                        .HasColumnType("text")
                        .HasColumnName("secondname");

                    b.Property<string>("SecondSurname")
                        .HasColumnType("text")
                        .HasColumnName("secondsurname");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("username");

                    b.Property<DateTime?>("Verified")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("verified");

                    b.HasKey("UserId");

                    b.ToTable("users", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.UserPolicy", b =>
                {
                    b.Property<int>("PolicyId")
                        .HasColumnType("integer")
                        .HasColumnName("policyid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.HasKey("PolicyId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("user_policy", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.UserRole", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("integer")
                        .HasColumnName("roleid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("userid");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("user_role", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", "Resource")
                        .WithMany("Actions")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionPolicy", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", "Action")
                        .WithMany()
                        .HasForeignKey("ActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Policy", "Policy")
                        .WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Action");

                    b.Navigation("Policy");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionRole", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", "Action")
                        .WithMany()
                        .HasForeignKey("ActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Action");

                    b.Navigation("Resource");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.UserPolicy", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Policy", "Policy")
                        .WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Policy");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.UserRole", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", b =>
                {
                    b.Navigation("Actions");
                });
#pragma warning restore 612, 618
        }
    }
}

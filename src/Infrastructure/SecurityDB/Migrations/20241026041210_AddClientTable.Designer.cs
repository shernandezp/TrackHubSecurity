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
    [Migration("20241026041210_AddClientTable")]
    partial class AddClientTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
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

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.HasKey("ActionId");

                    b.ToTable("actions", "security");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Client", b =>
                {
                    b.Property<Guid>("ClientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("salt");

                    b.Property<string>("Secret")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("secret");

                    b.HasKey("ClientId");

                    b.ToTable("clients", "security");
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

                    b.Property<string>("Name")
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

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceAction", b =>
                {
                    b.Property<int>("ResourceId")
                        .HasColumnType("integer")
                        .HasColumnName("resourceid");

                    b.Property<int>("ActionId")
                        .HasColumnType("integer")
                        .HasColumnName("actionid");

                    b.HasKey("ResourceId", "ActionId");

                    b.HasIndex("ActionId");

                    b.ToTable("resource_action", "security");
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

                    b.HasIndex("PolicyId");

                    b.HasIndex("ResourceId", "ActionId");

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

                    b.HasIndex("RoleId");

                    b.HasIndex("ResourceId", "ActionId");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<int?>("ParentRoleId")
                        .HasColumnType("integer")
                        .HasColumnName("parentroleid");

                    b.HasKey("RoleId");

                    b.HasIndex("ParentRoleId");

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

                    b.Property<DateOnly?>("DOB")
                        .HasColumnType("date")
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

                    b.Property<int>("LoginAttempts")
                        .HasColumnType("integer")
                        .HasColumnName("loginattempts");

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

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceAction", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", "Action")
                        .WithMany("ResourceActions")
                        .HasForeignKey("ActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", "Resource")
                        .WithMany("ResourceActions")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Action");

                    b.Navigation("Resource");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionPolicy", b =>
                {
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

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceAction", "ResourceAction")
                        .WithMany()
                        .HasForeignKey("ResourceId", "ActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Policy");

                    b.Navigation("Resource");

                    b.Navigation("ResourceAction");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceActionRole", b =>
                {
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

                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.ResourceAction", "ResourceAction")
                        .WithMany()
                        .HasForeignKey("ResourceId", "ActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Resource");

                    b.Navigation("ResourceAction");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", b =>
                {
                    b.HasOne("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", "ParentRole")
                        .WithMany("ChildRoles")
                        .HasForeignKey("ParentRoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentRole");
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

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Action", b =>
                {
                    b.Navigation("ResourceActions");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Resource", b =>
                {
                    b.Navigation("ResourceActions");
                });

            modelBuilder.Entity("TrackHub.Security.Infrastructure.SecurityDB.Entities.Role", b =>
                {
                    b.Navigation("ChildRoles");
                });
#pragma warning restore 612, 618
        }
    }
}

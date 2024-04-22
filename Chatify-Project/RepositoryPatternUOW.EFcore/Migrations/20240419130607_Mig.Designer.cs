﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RepositoryPatternUOW.EFcore;

#nullable disable

namespace RepositoryPattern.EFcore.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240419130607_Mig")]
    partial class Mig
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RepositoryPattern.Core.Models.VerificationCode", b =>
                {
                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Code", "UserId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("VerifcationCodes");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.Group", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.RefreshToken", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .HasMaxLength(44)
                        .HasColumnType("nvarchar(44)");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.HasKey("UserId", "Token");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email");

                    b.HasIndex("UserName");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.UserConnection", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("ConnectionId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("UserId", "ConnectionId");

                    b.ToTable("UserConnections");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.UserGroup", b =>
                {
                    b.Property<string>("GroupId")
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserGroup");
                });

            modelBuilder.Entity("RepositoryPattern.Core.Models.VerificationCode", b =>
                {
                    b.HasOne("RepositoryPatternUOW.Core.Models.User", "User")
                        .WithOne("VerificationCode")
                        .HasForeignKey("RepositoryPattern.Core.Models.VerificationCode", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.RefreshToken", b =>
                {
                    b.HasOne("RepositoryPatternUOW.Core.Models.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.UserConnection", b =>
                {
                    b.HasOne("RepositoryPatternUOW.Core.Models.User", "User")
                        .WithMany("UserConnections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.UserGroup", b =>
                {
                    b.HasOne("RepositoryPatternUOW.Core.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RepositoryPatternUOW.Core.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RepositoryPatternUOW.Core.Models.User", b =>
                {
                    b.Navigation("RefreshTokens");

                    b.Navigation("UserConnections");

                    b.Navigation("VerificationCode");
                });
#pragma warning restore 612, 618
        }
    }
}
﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProyectoFinal_JorgeSayegh.Contexts;

#nullable disable

namespace ProyectoFinal_JorgeSayegh.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241120192204_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.UserFavorite", b =>
                {
                    b.Property<Guid>("FavoriteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WidgetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FavoriteId");

                    b.HasIndex("UserId");

                    b.HasIndex("WidgetId");

                    b.ToTable("UserFavorites");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.Widget", b =>
                {
                    b.Property<Guid>("WidgetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DataUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsFavorite")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RefreshInterval")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("WidgetId");

                    b.HasIndex("CreatedBy");

                    b.ToTable("Widgets");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.WidgetSetting", b =>
                {
                    b.Property<Guid>("SettingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SettingKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("WidgetId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SettingId");

                    b.HasIndex("UserId");

                    b.HasIndex("WidgetId");

                    b.ToTable("WidgetSettings");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.UserFavorite", b =>
                {
                    b.HasOne("ProyectoFinal_JorgeSayegh.Models.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("ProyectoFinal_JorgeSayegh.Models.Widget", "Widget")
                        .WithMany("UserFavorites")
                        .HasForeignKey("WidgetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Widget");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.Widget", b =>
                {
                    b.HasOne("ProyectoFinal_JorgeSayegh.Models.User", "User")
                        .WithMany("Widgets")
                        .HasForeignKey("CreatedBy")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.WidgetSetting", b =>
                {
                    b.HasOne("ProyectoFinal_JorgeSayegh.Models.User", "User")
                        .WithMany("WidgetSettings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("ProyectoFinal_JorgeSayegh.Models.Widget", "Widget")
                        .WithMany("Settings")
                        .HasForeignKey("WidgetId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Widget");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("WidgetSettings");

                    b.Navigation("Widgets");
                });

            modelBuilder.Entity("ProyectoFinal_JorgeSayegh.Models.Widget", b =>
                {
                    b.Navigation("Settings");

                    b.Navigation("UserFavorites");
                });
#pragma warning restore 612, 618
        }
    }
}

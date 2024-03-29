﻿// <auto-generated />
using System;
using HIOF.GamingSocial.GameTimePlan.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HIOF.GamingSocial.GameTimePlan.Data.Migrations
{
    [DbContext(typeof(GameTimePlanDbContext))]
    partial class GameTimePlanDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HIOF.GamingSocial.GameTimePlan.Data.GameTimePlanData", b =>
                {
                    b.Property<int>("GameTimePlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GameTimePlanId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("GameTimePlanId");

                    b.ToTable("GameTimePlan");

                    b.HasData(
                        new
                        {
                            GameTimePlanId = 1,
                            Description = "Be there or be square!",
                            EndTime = new DateTime(2023, 6, 15, 22, 0, 0, 0, DateTimeKind.Unspecified),
                            GameId = 23,
                            GroupId = 1,
                            Name = "Dota 2 Tournament",
                            StartTime = new DateTime(2023, 6, 15, 18, 0, 0, 0, DateTimeKind.Unspecified)
                        },
                        new
                        {
                            GameTimePlanId = 2,
                            Description = "Bring your best game!",
                            EndTime = new DateTime(2023, 6, 18, 23, 0, 0, 0, DateTimeKind.Unspecified),
                            GameId = 26,
                            GroupId = 2,
                            Name = "CS:GO Showdown",
                            StartTime = new DateTime(2023, 6, 18, 19, 0, 0, 0, DateTimeKind.Unspecified)
                        });
                });
#pragma warning restore 612, 618
        }
    }
}

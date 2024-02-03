using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HIOF.GamingSocial.GameTimePlan.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameTimePlan",
                columns: table => new
                {
                    GameTimePlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTimePlan", x => x.GameTimePlanId);
                });

            migrationBuilder.InsertData(
                table: "GameTimePlan",
                columns: new[] { "GameTimePlanId", "Description", "EndTime", "GameId", "GroupId", "Name", "StartTime" },
                values: new object[,]
                {
                    { 1, "Be there or be square!", new DateTime(2023, 6, 15, 22, 0, 0, 0, DateTimeKind.Unspecified), 23, 1, "Dota 2 Tournament", new DateTime(2023, 6, 15, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Bring your best game!", new DateTime(2023, 6, 18, 23, 0, 0, 0, DateTimeKind.Unspecified), 26, 2, "CS:GO Showdown", new DateTime(2023, 6, 18, 19, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTimePlan");
        }
    }
}

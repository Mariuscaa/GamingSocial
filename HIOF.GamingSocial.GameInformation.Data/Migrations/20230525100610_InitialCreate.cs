using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HIOF.GamingSocial.GameInformation.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProfileGameCollection",
                columns: table => new
                {
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    GameRating = table.Column<int>(type: "int", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGameCollection", x => new { x.ProfileId, x.GameId });
                });

            migrationBuilder.CreateTable(
                name: "VideoGameInformation",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SteamAppId = table.Column<int>(type: "int", nullable: true),
                    GameTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GiantbombGuid = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    GameDescription = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGameInformation", x => x.GameId);
                });

            migrationBuilder.InsertData(
                table: "ProfileGameCollection",
                columns: new[] { "GameId", "ProfileId", "GameRating" },
                values: new object[,]
                {
                    { 23, new Guid("58ed69d7-031d-4c8d-a636-3459afa19b46"), null },
                    { 135, new Guid("58ed69d7-031d-4c8d-a636-3459afa19b46"), null },
                    { 23, new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c"), null },
                    { 26, new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c"), null },
                    { 26, new Guid("5f3a7799-5550-4151-a478-d45fbea29a8a"), null },
                    { 135, new Guid("5f3a7799-5550-4151-a478-d45fbea29a8a"), null },
                    { 26, new Guid("cdd891fc-5bcc-491e-9aab-0c3d40bee349"), null },
                    { 135, new Guid("d455ce99-45a4-4ee2-932f-5696a5c3b169"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileGameCollection");

            migrationBuilder.DropTable(
                name: "VideoGameInformation");
        }
    }
}

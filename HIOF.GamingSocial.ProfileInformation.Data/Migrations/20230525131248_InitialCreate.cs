using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HIOF.GamingSocial.ProfileInformation.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Friend",
                columns: table => new
                {
                    ProfileGuid1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileGuid2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friend", x => new { x.ProfileGuid1, x.ProfileGuid2 });
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembership",
                columns: table => new
                {
                    ProfileGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    MemberType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembership", x => new { x.GroupId, x.ProfileGuid });
                });

            migrationBuilder.CreateTable(
                name: "Invite",
                columns: table => new
                {
                    InviteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RelatedId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invite", x => x.InviteId);
                });

            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    ProfileGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(56)", maxLength: 56, nullable: false),
                    Age = table.Column<int>(type: "int", maxLength: 3, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.ProfileGuid);
                });

            migrationBuilder.InsertData(
                table: "Friend",
                columns: new[] { "ProfileGuid1", "ProfileGuid2" },
                values: new object[,]
                {
                    { new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c"), new Guid("cdd891fc-5bcc-491e-9aab-0c3d40bee349") },
                    { new Guid("5f3a7799-5550-4151-a478-d45fbea29a8a"), new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c") },
                    { new Guid("cdd891fc-5bcc-491e-9aab-0c3d40bee349"), new Guid("d455ce99-45a4-4ee2-932f-5696a5c3b169") },
                    { new Guid("d455ce99-45a4-4ee2-932f-5696a5c3b169"), new Guid("58ed69d7-031d-4c8d-a636-3459afa19b46") }
                });

            migrationBuilder.InsertData(
                table: "Group",
                columns: new[] { "GroupId", "Description", "GroupName", "IsHidden", "IsPrivate", "PhotoUrl" },
                values: new object[,]
                {
                    { 1, "A group for DotA 2 enthusiasts to discuss strategies and plan matches.", "DotA Heroes", false, false, null },
                    { 2, "Join us in mastering the latest FPS games like Valorant, CS:GO, and more.", "FPS Masters", false, false, null },
                    { 3, "A place for Starcraft players to share insights and game tactics.", "Strategic Minds", false, false, null }
                });

            migrationBuilder.InsertData(
                table: "GroupMembership",
                columns: new[] { "GroupId", "ProfileGuid", "MemberType" },
                values: new object[,]
                {
                    { 1, new Guid("58ed69d7-031d-4c8d-a636-3459afa19b46"), "Member" },
                    { 1, new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c"), "Owner" },
                    { 2, new Guid("5f3a7799-5550-4151-a478-d45fbea29a8a"), "Member" },
                    { 2, new Guid("cdd891fc-5bcc-491e-9aab-0c3d40bee349"), "Owner" },
                    { 3, new Guid("d455ce99-45a4-4ee2-932f-5696a5c3b169"), "Owner" }
                });

            migrationBuilder.InsertData(
                table: "Profile",
                columns: new[] { "ProfileGuid", "Age", "Bio", "Country", "Name", "PhotoUrl", "UserName" },
                values: new object[,]
                {
                    { new Guid("58ed69d7-031d-4c8d-a636-3459afa19b46"), 26, "Retro gaming nerd. From DOOM to Plants vs zombies, I play them all. Let's go back to the classics!", "Japan", "Mia", null, "RetroGamer" },
                    { new Guid("5b98dd2e-33e9-49d0-8e6a-3c5a286a761c"), 27, "DotA 2 enthusiast. Always ready for the next match. Let's climb the ranks together!", "United Kingdom", "Daniel", null, "GGDragon" },
                    { new Guid("5f3a7799-5550-4151-a478-d45fbea29a8a"), 29, "RPG lover. Exploring the vast world of Elder Scrolls Online. Join my guild and let's quest together!", "Australia", "Dylan", null, "EpicQuester" },
                    { new Guid("cdd891fc-5bcc-491e-9aab-0c3d40bee349"), 30, "FPS addict. Currently mastering CSGO. Join me if you can keep up!", "Germany", "Laura", null, "RogueHunter" },
                    { new Guid("d455ce99-45a4-4ee2-932f-5696a5c3b169"), 32, "Chess player turned Warhammer expert. Looking for strategic minds.", "United States", "Samuel", null, "StrategistKing" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friend");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "GroupMembership");

            migrationBuilder.DropTable(
                name: "Invite");

            migrationBuilder.DropTable(
                name: "Profile");
        }
    }
}

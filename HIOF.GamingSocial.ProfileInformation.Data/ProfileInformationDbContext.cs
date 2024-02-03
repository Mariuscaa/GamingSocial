using Microsoft.EntityFrameworkCore;

namespace HIOF.GamingSocial.ProfileInformation.Data
{
    public class ProfileInformationDbContext : DbContext
    {
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupMembership> GroupMembership { get; set; }
        public DbSet<Friend> Friend { get; set; }
        public DbSet<Invite> Invite { get; set; }

        public ProfileInformationDbContext()
        {

        }

        public ProfileInformationDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>(mb =>
            {
                mb.Property(profile => profile.ProfileGuid);
                mb.Property(profile => profile.UserName).HasMaxLength(20);
                mb.Property(profile => profile.Name).HasMaxLength(50);
                mb.Property(profile => profile.Bio).HasMaxLength(500);
                mb.Property(profile => profile.Country).HasMaxLength(56);
                mb.Property(profile => profile.Age).HasMaxLength(3);
                mb.Property(profile => profile.PhotoUrl).HasMaxLength(2048).IsRequired(false);

                mb.HasKey(profile => profile.ProfileGuid);

                mb.HasData(
                    new Profile
                    {
                        ProfileGuid = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                        UserName = "GGDragon",
                        Name = "Daniel",
                        Bio = "DotA 2 enthusiast. Always ready for the next match. Let's climb the ranks together!",
                        Country = "United Kingdom",
                        Age = 27,
                        PhotoUrl = null
                    },
                    new Profile
                    {
                        ProfileGuid = new Guid("CDD891FC-5BCC-491E-9AAB-0C3D40BEE349"),
                        UserName = "RogueHunter",
                        Name = "Laura",
                        Bio = "FPS addict. Currently mastering CSGO. Join me if you can keep up!",
                        Country = "Germany",
                        Age = 30,
                        PhotoUrl = null
                    },
                    new Profile
                    {
                        ProfileGuid = new Guid("D455CE99-45A4-4EE2-932F-5696A5C3B169"),
                        UserName = "StrategistKing",
                        Name = "Samuel",
                        Bio = "Chess player turned Warhammer expert. Looking for strategic minds.",
                        Country = "United States",
                        Age = 32,
                        PhotoUrl = null
                    },
                    new Profile
                    {
                        ProfileGuid = new Guid("58ED69D7-031D-4C8D-A636-3459AFA19B46"),
                        UserName = "RetroGamer",
                        Name = "Mia",
                        Bio = "Retro gaming nerd. From DOOM to Plants vs zombies, I play them all. Let's go back to the classics!",
                        Country = "Japan",
                        Age = 26,
                        PhotoUrl = null
                    },
                    new Profile
                    {
                        ProfileGuid = new Guid("5F3A7799-5550-4151-A478-D45FBEA29A8A"),
                        UserName = "EpicQuester",
                        Name = "Dylan",
                        Bio = "RPG lover. Exploring the vast world of Elder Scrolls Online. Join my guild and let's quest together!",
                        Country = "Australia",
                        Age = 29,
                        PhotoUrl = null
                    });

            });

            modelBuilder.Entity<Group>(mb =>
            {
                mb.Property(group => group.GroupId);
                mb.Property(group => group.GroupName).HasMaxLength(50);
                mb.Property(group => group.Description).HasMaxLength(500);
                mb.Property(group => group.IsHidden);
                mb.Property(group => group.IsPrivate);
                mb.Property(group => group.PhotoUrl).HasMaxLength(2048).IsRequired(false);

                mb.HasKey(group => group.GroupId);

                mb.HasData(
                    new Group
                    {
                        GroupId = 1,
                        GroupName = "DotA Heroes",
                        Description = "A group for DotA 2 enthusiasts to discuss strategies and plan matches.",
                        IsHidden = false,
                        IsPrivate = false,
                        PhotoUrl = null
                    },
                    new Group
                    {
                        GroupId = 2,
                        GroupName = "FPS Masters",
                        Description = "Join us in mastering the latest FPS games like Valorant, CS:GO, and more.",
                        IsHidden = false,
                        IsPrivate = false,
                        PhotoUrl = null
                    },
                    new Group
                    {
                        GroupId = 3,
                        GroupName = "Strategic Minds",
                        Description = "A place for Starcraft players to share insights and game tactics.",
                        IsHidden = false,
                        IsPrivate = false,
                        PhotoUrl = null
                    });
            });

            modelBuilder.Entity<GroupMembership>(mb =>
            {
                mb.Property(groupMembership => groupMembership.GroupId);
                mb.Property(groupMembership => groupMembership.ProfileGuid);
                mb.Property(groupMembership => groupMembership.MemberType);

                mb.HasKey(mb => new { mb.GroupId, mb.ProfileGuid });

                mb.HasData(
                    new GroupMembership
                    {
                        GroupId = 1,
                        ProfileGuid = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                        MemberType = "Owner"
                    },
                    new GroupMembership
                    {
                        GroupId = 2,
                        ProfileGuid = new Guid("CDD891FC-5BCC-491E-9AAB-0C3D40BEE349"),
                        MemberType = "Owner"
                    },
                    new GroupMembership
                    {
                        GroupId = 3,
                        ProfileGuid = new Guid("D455CE99-45A4-4EE2-932F-5696A5C3B169"),
                        MemberType = "Owner"
                    },
                    new GroupMembership
                    {
                        GroupId = 1,
                        ProfileGuid = new Guid("58ED69D7-031D-4C8D-A636-3459AFA19B46"),
                        MemberType = "Member"
                    },
                    new GroupMembership
                    {
                        GroupId = 2,
                        ProfileGuid = new Guid("5F3A7799-5550-4151-A478-D45FBEA29A8A"),
                        MemberType = "Member"
                    });
            });

            modelBuilder.Entity<GroupMembership>()
                .HasKey(mb => new { mb.GroupId, mb.ProfileGuid });

            modelBuilder.Entity<Friend>(mb =>
            {
                mb.Property(friend => friend.ProfileGuid1);
                mb.Property(friend => friend.ProfileGuid2);

                mb.HasKey(friend => new { friend.ProfileGuid1, friend.ProfileGuid2 });

                mb.HasData(
                    new Friend
                    {
                        ProfileGuid1 = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                        ProfileGuid2 = new Guid("CDD891FC-5BCC-491E-9AAB-0C3D40BEE349"),
                    },
                    new Friend
                    {
                        ProfileGuid1 = new Guid("D455CE99-45A4-4EE2-932F-5696A5C3B169"),
                        ProfileGuid2 = new Guid("58ED69D7-031D-4C8D-A636-3459AFA19B46"),
                    },
                    new Friend
                    {
                        ProfileGuid1 = new Guid("5F3A7799-5550-4151-A478-D45FBEA29A8A"),
                        ProfileGuid2 = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                    },
                    new Friend
                    {
                        ProfileGuid1 = new Guid("CDD891FC-5BCC-491E-9AAB-0C3D40BEE349"),
                        ProfileGuid2 = new Guid("D455CE99-45A4-4EE2-932F-5696A5C3B169"),
                    });
            });


            modelBuilder.Entity<Invite>(mb =>
            {
                mb.Property(inv => inv.InviteId);
                mb.Property(inv => inv.SenderGuid);
                mb.Property(inv => inv.ReceiverGuid);
                mb.Property(inv => inv.InviteType);
                mb.Property(inv => inv.Message).HasMaxLength(200).IsRequired(false);
                mb.Property(inv => inv.RelatedId).IsRequired(false);

                mb.HasKey(inv => new { inv.InviteId });
            });
        }
    }
}
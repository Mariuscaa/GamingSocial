using Microsoft.EntityFrameworkCore;

namespace HIOF.GamingSocial.GameInformation.Data;

public class VideoGameDbContext : DbContext
{
    public DbSet<VideoGameInformation> VideoGameInformation { get; set; }
    public DbSet<ProfileGameCollection> ProfileGameCollection { get; set; }

    public VideoGameDbContext()
    {

    }

    public VideoGameDbContext(DbContextOptions<VideoGameDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VideoGameInformation>(mb =>
        {
            mb.Property(game => game.GameId);
            mb.Property(game => game.SteamAppId).IsRequired(false);
            mb.Property(game => game.GameTitle).HasMaxLength(200);
            mb.Property(game => game.GiantbombGuid).HasMaxLength(11).IsRequired(false);
            mb.Property(game => game.GameDescription).HasMaxLength(450).IsRequired(false);

            mb.HasKey(game => game.GameId);
        });

        modelBuilder.Entity<ProfileGameCollection>(mb =>
        {
            mb.Property(p_g_collection => p_g_collection.ProfileId);
            mb.Property(p_g_collection => p_g_collection.GameId);
            mb.Property(p_g_collection => p_g_collection.GameRating).HasMaxLength(3).IsRequired(false);

            mb.HasKey(p_g_collection => new { p_g_collection.ProfileId, p_g_collection.GameId });

            mb.HasData(
                new ProfileGameCollection
                {
                    ProfileId = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                    GameId = 23,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("58ED69D7-031D-4C8D-A636-3459AFA19B46"),
                    GameId = 23,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("5B98DD2E-33E9-49D0-8E6A-3C5A286A761C"),
                    GameId = 26,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("CDD891FC-5BCC-491E-9AAB-0C3D40BEE349"),
                    GameId = 26,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("D455CE99-45A4-4EE2-932F-5696A5C3B169"),
                    GameId = 135,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("58ED69D7-031D-4C8D-A636-3459AFA19B46"),
                    GameId = 135,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("5F3A7799-5550-4151-A478-D45FBEA29A8A"),
                    GameId = 26,
                    GameRating = null
                },
                new ProfileGameCollection
                {
                    ProfileId = new Guid("5F3A7799-5550-4151-A478-D45FBEA29A8A"),
                    GameId = 135,
                    GameRating = null
                });
        });

    }
}

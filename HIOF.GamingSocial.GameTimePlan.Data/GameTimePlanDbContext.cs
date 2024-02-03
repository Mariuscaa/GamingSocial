using Microsoft.EntityFrameworkCore;

namespace HIOF.GamingSocial.GameTimePlan.Data;
public class GameTimePlanDbContext : DbContext
{
    public DbSet<GameTimePlanData> GameTimePlan { get; set; }
    public GameTimePlanDbContext()
    {

    }

    public GameTimePlanDbContext(DbContextOptions options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameTimePlanData>(mb =>
        {
            mb.Property(gtp => gtp.GameTimePlanId);
            mb.Property(gtp => gtp.Name).HasMaxLength(50);
            mb.Property(gtp => gtp.Description).HasMaxLength(200);
            mb.Property(gtp => gtp.GameId);
            mb.Property(gtp => gtp.GroupId);
            mb.Property(gtp => gtp.StartTime);
            mb.Property(gtp => gtp.EndTime);

            mb.HasKey(gtp => gtp.GameTimePlanId);

            mb.HasData(
                new GameTimePlanData
                {
                    GameTimePlanId = 1,
                    Name = "Dota 2 Tournament",
                    Description = "Be there or be square!",
                    GameId = 23,
                    GroupId = 1,
                    StartTime = new DateTime(2023, 6, 15, 18, 0, 0),
                    EndTime = new DateTime(2023, 6, 15, 22, 0, 0)
                },
                new GameTimePlanData
                {
                    GameTimePlanId = 2,
                    Name = "CS:GO Showdown",
                    Description = "Bring your best game!",
                    GameId = 26,
                    GroupId = 2,
                    StartTime = new DateTime(2023, 6, 18, 19, 0, 0),
                    EndTime = new DateTime(2023, 6, 18, 23, 0, 0)
                });
        });

    }
}

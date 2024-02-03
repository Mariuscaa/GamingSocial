using Microsoft.EntityFrameworkCore;

namespace HIOF.GamingSocial.Chat.Data;

public class ChatDbContext : DbContext
{
    public DbSet<ChatMessage> ChatMessage { get; set; }

    public ChatDbContext()
    {

    }

    public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMessage>(mb =>
        {
            mb.Property(chat => chat.ChatId);
            mb.Property(chat => chat.Sender);
            mb.Property(chat => chat.Reciever).IsRequired(false);
            mb.Property(chat => chat.groupid);
            mb.Property(chat => chat.Message);
            mb.Property(chat => chat.MessageSent);

            mb.HasKey(chat => chat.ChatId);
        });


    }
}

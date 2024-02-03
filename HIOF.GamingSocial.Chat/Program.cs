using HIOF.GamingSocial.Chat.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HIOF.GamingSocial.Chat;

public class Program
{
    
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddDbContext<ChatDbContext>(options =>
        {

            options.UseSqlServer(builder.Configuration.GetConnectionString("ChatDbContext"));
        });

        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/ChatSource";
            settings.LogName = "GS/ChatLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/ChatSource"))
        {
            EventLog.CreateEventSource("GS/ChatSource", "GS/ChatLog");
        }

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetService<ChatDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        
        app.Run();
    }
}
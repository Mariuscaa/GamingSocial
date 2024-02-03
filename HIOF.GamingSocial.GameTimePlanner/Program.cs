using HIOF.GamingSocial.GameTimePlan.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace HIOF.GamingSocial.GameTimePlanner;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/GameTimePlannerSource";
            settings.LogName = "GS/GameTimePlannerLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/GameTimePlannerSource")) 
        {
            EventLog.CreateEventSource("GS/GameTimePlannerSource", "GS/GameTimePlannerLog");
        }

        builder.Services.AddDbContext<GameTimePlanDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("GameTimePlanDbContext"));
        });
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
            var dbContext = scope.ServiceProvider.GetService<GameTimePlanDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        app.Run();
    }
}
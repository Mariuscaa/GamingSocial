using HIOF.GamingSocial.GameInformation.Data;
using HIOF.GamingSocial.GameInformation.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace GameInformation;

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
     
        builder.Services.AddDbContext<VideoGameDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("VideoGameInformationDb"));
        });

        builder.Services.AddGrpc();
        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/GameInformationSource";
            settings.LogName = "GS/GameInformationLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/GameInformationSource"))
        {
            EventLog.CreateEventSource("GS/GameInformationSource", "GS/GameInformationLog");
        }

        var app = builder.Build();
            
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGrpcService<VideoGameService>();

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetService<VideoGameDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        app.Run(); 
    }
}
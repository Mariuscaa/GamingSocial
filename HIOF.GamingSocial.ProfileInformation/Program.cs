using Microsoft.EntityFrameworkCore;
using HIOF.GamingSocial.ProfileInformation.Data;
using System.Diagnostics;
using System.Reflection;

namespace ProfileInformation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });


        builder.Services.AddDbContext<ProfileInformationDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();
            options.UseSqlServer(builder.Configuration.GetConnectionString("ProfileInformationDb"));
        });
        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/ProfileInformationSource";
            settings.LogName = "GS/ProfileInformationLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/ProfileInformationSource"))
        {
            EventLog.CreateEventSource("GS/ProfileInformationSource", "GS/ProfileInformationLog");
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
            var dbContext = scope.ServiceProvider.GetService<ProfileInformationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        app.Run();
    }
}
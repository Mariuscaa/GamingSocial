using HIOF.GamingSocial.PublicProfileInformation.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace HIOF.GamingSocial.PublicProfileInformation;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.Configure<KeyServiceSettings>(builder.Configuration.GetSection("KeyService"));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/PublicProfileInformationSource";
            settings.LogName = "GS/PublicProfileInformationLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/PublicProfileInformationSource"))
        {
            EventLog.CreateEventSource("GS/PublicProfileInformationSource", "GS/PublicProfileInformationLog");
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
        app.Run();
    }
}

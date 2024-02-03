using HIOF.GamingSocial.PublicGameInformation.Configuration;
using HIOF.GamingSocial.PublicGameInformation.Services;
using System.Diagnostics;
using System.Reflection;

namespace HIOF.GamingSocial.RetrieveGameInfo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<KeyServiceSettings>(builder.Configuration.GetSection("KeyService"));
        builder.Services.Configure<UrlServiceSettings>(builder.Configuration.GetSection("UrlService"));
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });
        builder.Services.AddGrpc();

        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/PublicGameInformationSource";
            settings.LogName = "GS/PublicGameInformationLog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/PublicGameInformationSource"))
        {
            EventLog.CreateEventSource("GS/PublicGameInformationSource", "GS/PublicGameInformationLog");
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
        app.MapGrpcService<PublicSteamGamesService>();

        app.Run();
    }
}
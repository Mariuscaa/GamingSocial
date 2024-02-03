using HIOF.GamingSocial.GUI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.Diagnostics;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;

namespace HIOF.GamingSocial.GUI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/octet-stream" });
        });
        builder.Services.AddBlazoredToast();
        builder.Services.AddBlazoredSessionStorage();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddBlazoredToast();
        builder.Services.AddScoped<ChatService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<FriendService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<GameService>();
        builder.Services.AddScoped<ChatAPIService>();
        builder.Services.AddScoped<ChatHub>();
        builder.Services.AddScoped<GameTimePlannerService>();
        builder.Services.AddScoped<PublicGameService>();
        builder.Services.AddScoped<GroupService>();
        builder.Services.AddScoped<InviteService>();


        builder.Services.AddSingleton<CooldownService>();

        builder.Services.AddHttpClient("ProfileInformationAPI", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7087");
        });

        builder.Services.AddHttpClient("GameCollectionAPI", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7296");
        });
        builder.Services.AddHttpClient("ChatAPI", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7192");
        });

        builder.Services.AddHttpClient("GameTimePlannerAPI", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7287");
        });
        builder.Logging.AddEventLog(settings =>
        {
            settings.SourceName = "GS/GUISource";
            settings.LogName = "GS/GUILog";
        });

        // Note: This requires administrative permissions.
        if (!EventLog.SourceExists("GS/GUISource"))
        {
            EventLog.CreateEventSource("GS/GUISource", "GS/GUILog");
        }
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapHub<ChatHub>("/chat");
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}
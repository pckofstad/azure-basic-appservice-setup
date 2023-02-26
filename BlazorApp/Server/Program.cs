using BlazorApp.Server.Configuration;
using BlazorApp.Server.DbContext;
using BlazorApp.Server.Infrastructure;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace BlazorApp.Server
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Logging and configuration
            var logConfig = builder.Configuration.GetSection("Logging");
            builder.Services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConfiguration(logConfig);
                config.AddConsole();
                config.AddAzureWebAppDiagnostics();
            });

            builder.Services.AddSingleton(new ConfigurationInfo(
                    config: builder.Configuration,
                    logger: builder.Services.BuildServiceProvider().GetService<ILogger<ConfigurationInfo>>()));
            ConfigurationInfo.IsConfigurationHealthOk();

            // Telemetry
            // Add telemetry if application insight key is set.
            if (!string.IsNullOrWhiteSpace(ConfigurationInfo.GetAPPINSIGHTS_INSTRUMENTATIONKEY()))
            {
                // Application insight will read from the default configuration key that we already has defined as the same key in ConfigurationInfo
                builder.Services.AddApplicationInsightsTelemetry();
                TelemetryDebugWriter.IsTracingDisabled = true;
            }

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // DbContextSetup DbContext
            DbContextSetup.ConfigureDatabase(builder.Services);


            // Storage Account Setup
            (new AzureStorageService(builder.Configuration)).EnsureStorageContainers();
            builder.Services.AddTransient<AzureStorageService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
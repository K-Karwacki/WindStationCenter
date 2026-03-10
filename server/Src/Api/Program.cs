using Api.Config;
using DotNetEnv;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using MQTTnet;

namespace Api;

public class Program
{
    private const string AppSettingsPath = "Config/Json";

    private static WebApplication BuildApp()
    {
        // Load .env file BEFORE building configuration
        // TraversePath searches upward from current directory to find .env
        Env.TraversePath().Load();
        
        var builder = WebApplication.CreateBuilder();
        
        // Load appsettings from Config/Json since they're not in the default location
        builder.Configuration
            .AddJsonFile($"{AppSettingsPath}/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{AppSettingsPath}/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        var appSettings = AppSettingsFactory.Create(builder.Configuration);
        var serviceManager = new ServiceManager(builder.Services, appSettings, builder.Environment);
        serviceManager.ConfigureAndInitializeServices();
        
        Console.WriteLine("Build complete.");
        return builder.Build();
    }
    public static async Task Main(string[] args)
    {
        var app = BuildApp();
        
        // Development-only middleware
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("✓ Running in Development mode");
            app.UseOpenApi();
            app.UseSwaggerUi();
        }
        else if(app.Environment.IsProduction())
        {
            Console.WriteLine("✓ Running in Production mode");
        }
        else if(app.Environment.IsStaging())
        {
            Console.WriteLine("✓ Running in Staging mode");
        }
        
        // Configure middleware pipeline
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        // Configure Mqtt client: bind settings and call ConnectAsync.

        // Bind Mqtt settings from configuration (may come from appsettings or environment)
        var mqttSettings = app.Configuration.GetSection("Mqtt").Get<Domain.Settings.MqttSettings>();
        
        // Use the hosted MQTT client service registered by AddMqttControllers()
        // so controllers and hosted services share the same connected client.
        var mqttClientService = app.Services.GetRequiredService<IMqttClientService>();
        await mqttClientService.ConnectAsync(mqttSettings!.Broker, mqttSettings.Port);
        
        // Apply pending migrations automatically
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            var migrations = pendingMigrations as string[] ?? pendingMigrations.ToArray();
            if (migrations.Length != 0)
            {
                Console.WriteLine($"[DB] Applying {migrations.Length} pending migration(s)...");
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("[DB] ✓ Migrations applied successfully");
            }
            else
            {
                Console.WriteLine("[DB] ✓ Database schema is up to date");
            }
        }
        app.GenerateApiClientsFromOpenApi("../../../client/src/generated-ts-client.ts", "./openapi.json").GetAwaiter().GetResult();
        
        await app.RunAsync();
    }
    
}
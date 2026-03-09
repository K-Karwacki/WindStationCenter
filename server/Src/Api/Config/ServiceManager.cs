using System.Text;
using Api.Mqtt;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Features;
using Application.Features.Auth;
using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Domain.Exceptions;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Utility;
using Domain.Settings;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mqtt.Controllers;
using MQTTnet;
using NSwag;
using NSwag.Generation.Processors.Security;
using StackExchange.Redis;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.GroupRealtime;

namespace Api.Config;

public sealed class ServiceManager(IServiceCollection services, AppSettings appSettings, IWebHostEnvironment env) 
{
    
    public void ConfigureAndInitializeServices()
    {
        ConfigureLogger();
        
        ConfigureAppSettings();

        ConfigureDbContext();

        ConfigureRepositories();
        
        ConfigureAuth();

        ConfigureControllersAndFeatures();
        
        ConfigureCors();
        
        ConfigureSse();
        
        ConfigureMqtt();
        
        if (env.IsDevelopment())
        {
            ConfigureSwagger();
        }
    }

    private void ConfigureAppSettings()
    {
        Console.WriteLine("Loading AppSettings...");
        
        services.AddSingleton(appSettings);
        services.AddSingleton(appSettings.DbSettings);
        services.AddSingleton(appSettings.JwtSettings);
        services.AddSingleton(appSettings.CorsSettings);
        services.AddSingleton(appSettings.MqttSettings);
        
        Console.WriteLine("AppSettings configuration loaded.");
    }

    private void ConfigureDbContext()
    {
        Console.WriteLine("Loading DbContext...");
        
        services.AddDbContext<MyDbContext>((sp, conf) =>
        {
            conf.UseNpgsql(appSettings.DbSettings.PSqlConnectionString);
            conf.AddEfRealtimeInterceptor(sp);
        });
        
        Console.WriteLine("DbContext configuration loaded.");
    }

    private void ConfigureAuth()
    {
        Console.WriteLine("Loading Auth...");
        
        try
        {
            appSettings.JwtSettings.Validate();
        }
        catch (ConfigurationFailureException e)
        {
            Console.Error.WriteLine(e.Message);
            throw;
        }
        
        services.AddScoped<IJwt, Infrastructure.Auth.Jwt>();

        // Configure JWT Authentication
        services
            .AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings.JwtSettings.Issuer,
                    ValidAudience = appSettings.JwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtSettings.Secret))
                };

                // Read JWT from cookies, or Authorization header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["accessToken"];
                        if (token != null)
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddAuthorization();
        
        Console.WriteLine("Auth configuration loaded.");
    }

    private void ConfigureLogger()
    {
        Console.WriteLine("Loading Logger...");
        
        // Add logging
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Error);
            // logging.AddProvider(new FileLoggerProvider("logs/app.log"));
        });
        
        Console.WriteLine("Logger configuration loaded.");
    }

    private void ConfigureCors()
    {
        Console.WriteLine("Loading Cors...");
        
        // Configure CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins(appSettings.CorsSettings.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        Console.WriteLine("Cors configuration loaded.");
    }

    private void ConfigureRepositories()
    {
        Console.WriteLine("Loading Repositories...");
        
        // Should scan for every repository that inherits IBaseRepository<>
        services.Scan(scan => scan
            .FromAssemblyOf<UserRepository>()
                .AddClasses(classes => classes.AssignableTo(typeof(IBaseRepository<>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        
        Console.WriteLine("Repositories configuration loaded.");
    }
    
    private void ConfigureControllersAndFeatures()
    {
        Console.WriteLine("Loading Controllers and Features...");
        
        // Explicitly add controllers from this assembly
        services.AddControllers()
            .AddApplicationPart(typeof(ServiceManager).Assembly);
        
        services.AddMqttControllers();

        
        services.AddScoped<LoginHandler>();
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<IAuthFeature, AuthFeature>();
        
        services.AddSingleton<IEnvHelper, Infrastructure.Utils.EnvHelper>();
        services.AddSingleton<IHashingUtils, Infrastructure.Utils.HashingUtils>();
        Console.WriteLine("Controllers and Features configuration loaded.");
    }

    private void ConfigureSse()
    {
        Console.WriteLine("Loading SSE...");
        
        services.AddInMemorySseBackplane();
        services.AddEfRealtime();
        services.AddGroupRealtime();

        Console.WriteLine("SSE configuration loaded.");
    }

    private void ConfigureMqtt()
    {
        Console.WriteLine("Loading Mqtt...");

        services.AddSingleton<IMqttClient>(sp =>
        {
            var factory = new MqttClientFactory();
            return factory.CreateMqttClient();
        });

        services.AddSingleton<MqttPublisher>();
        
        Console.WriteLine("Mqtt configuration loaded.");
    }
    private void ConfigureSwagger()
    {
        Console.WriteLine("Loading Swagger...");
        
        services.AddOpenApiDocument(configure =>
            {
                configure.Title = "Swagger UI";

                configure.AddSecurity(name: "JWT", swaggerSecurityScheme: new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey, 
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                configure.OperationProcessors.Add(item: new AspNetCoreOperationSecurityScopeProcessor(name: "JWT"));
                
            });
        
        Console.WriteLine("Swagger UI configuration loaded.");
    }
    
}
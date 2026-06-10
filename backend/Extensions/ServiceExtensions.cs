using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Audit.EntityFramework;
using backend.Data;
using backend.Data.SeederData;
using backend.Domain.User;
using backend.Services.Auth;
using backend.Services.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace backend.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                  options.UseNpgsql(configuration.GetConnectionString("Default"))
                    // This automatically intercepts SaveChanges / SaveChangesAsync and triggers Audit.NET
                    .AddInterceptors(new AuditSaveChangesInterceptor()));
        }
        public static void ConfigureSerilog(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSerilog(options =>
                {
                    var columnOptions = new Dictionary<string, ColumnWriterBase>
                                {
                                { "message", new RenderedMessageColumnWriter() },
                                { "message_template", new MessageTemplateColumnWriter() },
                                { "level", new LevelColumnWriter() },
                                { "timestamp", new TimestampColumnWriter() },
                                { "exception", new ExceptionColumnWriter() },
                                { "properties", new LogEventSerializedColumnWriter() }
                                };

                    options.MinimumLevel.Information()
                        .WriteTo.Console(
                            restrictedToMinimumLevel: LogEventLevel.Debug,
                            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                        )
                        .WriteTo.File("Logs/log.txt",
                            rollOnFileSizeLimit: true,
                            rollingInterval: RollingInterval.Day,
                            fileSizeLimitBytes: 1000000,
                            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                            restrictedToMinimumLevel: LogEventLevel.Warning
                        )
                        .WriteTo.PostgreSQL(
                            connectionString: configuration["ConnectionStrings:Default"],
                            tableName: "logs",
                            columnOptions: columnOptions,
                            needAutoCreateTable: true,
                            restrictedToMinimumLevel: LogEventLevel.Warning
                        );
                });
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<Account, Privilege>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
                };

                // Setup for 0Auth 2.0 Setup refresh and access-token
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (!string.IsNullOrEmpty(ctx.Request.Cookies["access-token"]))
                        {
                            ctx.Token = ctx.Request.Cookies["access-token"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        }
        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
        }
        public static void ConfigureCors(this IServiceCollection services, IConfiguration config)
        {
            services.AddCors(options =>
         {
             options.AddDefaultPolicy(policy =>
             {
                 policy.WithOrigins(
                         config["JWT:Audience"]!)
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials();
             });
         });
        }
        public static void ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<AppDbSeeder>();
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IAuthServices, AuthServices>();
        }
        public static void ConfigureOptionsPattern(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SudoData>(configuration.GetSection(SudoData.Section));
        }
        public static void ConfigureLocalizator(this IServiceCollection services)
        {
            services.AddJsonLocalization(options =>
            {
                options.ResourcesPath = "Resources";
                options.FileEncoding = Encoding.UTF8;
                options.CacheDuration = TimeSpan.FromMinutes(30);
            });
        }

    }
}
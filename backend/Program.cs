using Audit.Core;
using backend.Configuration;
using backend.Data;
using backend.Extensions;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

// 1. Build a temporary configuration to read environment variables & appsettings
var bootstrapConfig = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 2. Initialize the Bootstrap Logger using your custom configuration
Log.Logger = SerilogConfigurator.CreateConfiguration(bootstrapConfig)
    .CreateBootstrapLogger();


try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddServiceExtensions(builder.Configuration);

    // 1. Build the app safely first
    var app = builder.Build();
    // 2. NOW configure Audit.NET using the fully built service provider
    var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
    Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
    {
        // Use the safely resolved accessor from outside the lambda
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userName = httpContext.User.Identity.Name ?? "Unknown User";
            scope.SetCustomField("User", userName);
        }
        else
        {
            scope.SetCustomField("User", "Anonymous/System");
        }
    });
    Audit.Core.Configuration.Setup()
      .UsePostgreSql(config => config
          .ConnectionString(builder.Configuration.GetConnectionString("Default"))
          .Schema("public")
          .TableName("AuditLogs")
          .IdColumnName("Id")
          .DataJsonColumn("AuditData")
          .CustomColumn("EventDate", ev => ev.StartDate)
          .CustomColumn("User", ev => ev.CustomFields.ContainsKey("User") ? ev.CustomFields["User"]?.ToString() : "System")
          .CustomColumn("Action", ev => ev.EventType));

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // localhost:5088/scalar/v1
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    await app.InitialiseDatabaseAsync();
    app.UseHttpsRedirection();
    app.UseCustomLocalization();
    app.UseExceptionHandler();
    app.MapControllers();

    app.Run();
}
catch (HostAbortedException)
{
    // Don't log EF Core CLI tooling aborts as fatal errors
    throw;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during application startup lifecycle.");
}
finally
{
    Log.CloseAndFlush();
}
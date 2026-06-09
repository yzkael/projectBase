using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

public static class SerilogConfigurator
{
    public static LoggerConfiguration CreateConfiguration(IConfiguration configuration)
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

        return new LoggerConfiguration()
            .MinimumLevel.Information()
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
                // Matches your configuration path: ConnectionStrings:Default
                connectionString: configuration.GetConnectionString("Default"),
                tableName: "Logs",
                columnOptions: columnOptions,
                needAutoCreateTable: true,
                restrictedToMinimumLevel: LogEventLevel.Warning
            );
    }
}
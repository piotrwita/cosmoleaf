using System.Data;
using Erp.WmsConnector.Shared.Abstractions.HttpContextAccessors;
using Erp.WmsConnector.Shared.Infrastructure.Helpers;
using Erp.WmsConnector.Shared.Infrastructure.HttpContextAccessors;
using Erp.WmsConnector.Shared.Infrastructure.Logging.Enrichers;
using Erp.WmsConnector.Shared.Infrastructure.Logging.Options;
using Erp.WmsConnector.Shared.Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Erp.WmsConnector.Shared.Infrastructure.Logging;

public static class RegistrationExtensions
{
    public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration config)
    {
        services.AddHttpContextAccessor();
        services.TryAddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddSerilog((context, loggerConfiguration) =>
            ConfigureSerilog(loggerConfiguration, config));

        return services;
    }

    public static WebApplicationBuilder UseLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.TryAddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            ConfigureSerilog(loggerConfiguration, builder.Configuration));

        return builder;
    }

    private static void ConfigureSerilog(LoggerConfiguration configuration, IConfiguration config)
    {
        var loggerOptions = config.GetRequiredOptions<LoggerOptions>();
        var mainDbOptions = config.GetRequiredOptions<DatabaseOptions>();

        LogEventLevel level = ParseLogLevel(loggerOptions.Level);
        configuration.MinimumLevel.Is(level);
        configuration.ApplyOverrides(loggerOptions);

        configuration.Enrich.FromLogContext();
        configuration.Enrich.With<CorrelationIdEnricher>();
        configuration.UseConsoleIfEnabled(loggerOptions);
        configuration.UseFileIfEnabled(loggerOptions);
        configuration.UseMssqlDbIfEnabled(loggerOptions, mainDbOptions);
    }

    private static void ApplyOverrides(this LoggerConfiguration configuration, LoggerOptions options)
    {
        foreach ((string source, string level) in options.Overrides)
        {
            configuration.MinimumLevel.Override(source, ParseLogLevel(level));
        }
    }

    private static LogEventLevel ParseLogLevel(string? level, LogEventLevel defaultLevel = LogEventLevel.Information)
        => EnumHelper.ParseOrDefault<LogEventLevel>(level, defaultLevel);

    private static void UseConsoleIfEnabled(this LoggerConfiguration configuration, LoggerOptions options)
    {
        ConsoleLoggerOptions? consoleOptions = options.Console;
        if (consoleOptions == null || !consoleOptions.Enabled)
        {
            return;
        }

        LogEventLevel level = ParseLogLevel(options.Level);
        configuration.WriteTo.Console(restrictedToMinimumLevel: level);
    }

    private static void UseFileIfEnabled(this LoggerConfiguration configuration, LoggerOptions options)
    {
        FileLoggerOptions? fileOptions = options.File;
        if (fileOptions == null || !fileOptions.Enabled)
        {
            return;
        }

        LogEventLevel level = ParseLogLevel(options.Level);
        RollingInterval rollingInterval = EnumHelper.ParseOrDefault<RollingInterval>(fileOptions.Interval, RollingInterval.Day);
        configuration.WriteTo.File(
            fileOptions.Path,
            restrictedToMinimumLevel: level,
            rollingInterval: rollingInterval,
            retainedFileCountLimit: fileOptions.RetainedFileCountLimit);
    }

    private static void UseMssqlDbIfEnabled(this LoggerConfiguration loggerConfiguration, LoggerOptions loggerOptions, DatabaseOptions mainDbOptions)
    {
        DatabaseLoggerOptions? mssqlOptions = loggerOptions.Database;
        if (mssqlOptions == null || !mssqlOptions.Enabled)
        {
            return;
        }

        var mssqlSinkOptions = new MSSqlServerSinkOptions()
        {
            TableName = mssqlOptions.Table,
            SchemaName = mssqlOptions.Schema,
            AutoCreateSqlTable = true
        };

        string loggerConnString = string.IsNullOrWhiteSpace(mssqlOptions.ConnectionString) ? mainDbOptions.ConnectionString : mssqlOptions.ConnectionString;
        ColumnOptions columnOptions = CreateColumnOptions();
        LogEventLevel level = ParseLogLevel(loggerOptions.Level);

        loggerConfiguration.WriteTo.MSSqlServer(
            connectionString: loggerConnString,
            sinkOptions: mssqlSinkOptions,
            restrictedToMinimumLevel: level,
            columnOptions: columnOptions);
    }

    private static ColumnOptions CreateColumnOptions()
    {
        var options = new ColumnOptions
        {
            AdditionalColumns =
            [
                new()
                {
                    ColumnName = Constants.CorrelationPropertyName,
                    PropertyName = Constants.CorrelationPropertyName,
                    DataType = SqlDbType.NVarChar,
                    DataLength = 64,
                    AllowNull = true
                },
                new()
                {
                    ColumnName = Constants.CodePropertyName,
                    PropertyName = Constants.CodePropertyName,
                    DataType = SqlDbType.NVarChar,
                    DataLength = 100,
                    AllowNull = true,
                    NonClusteredIndex = true
                }
            ]
        };

        options.TimeStamp.ConvertToUtc = true;

        return options;
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using Serilog.Extensions.Logging.File;
using System.Linq;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Display;
using Serilog.Formatting;

namespace Microsoft.Extensions.Logging
{
    public static class FileLoggerExtensions
    {

        public static ILoggerFactory AddFile(this ILoggerFactory loggerFactory, IConfigurationSection configuration)
        {

            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var config = configuration.Get<FileLoggingConfiguration>();
            if (string.IsNullOrWhiteSpace(config.PathFormat))
            {
                SelfLog.WriteLine("Unable to add the file logger: no PathFormat was present in the configuration");
                return loggerFactory;
            }

            var minimumLevel = GetMinimumLogLevel(configuration);
            var levelOverrides = GetLevelOverrides(configuration);

            return loggerFactory.AddFile(config.PathFormat, config.LoggingEnabled, minimumLevel, levelOverrides, config.Json, config.FileSizeLimitBytes, config.RetainedFileCountLimit);
        }

        public static ILoggerFactory AddFile(this ILoggerFactory loggerFactory, string pathFormat,string LoggingEnabled, LogLevel minimumLevel = LogLevel.Information, IDictionary<string, LogLevel> levelOverrides = null, bool isJson = false, long? fileSizeLimitBytes = FileLoggingConfiguration.DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = FileLoggingConfiguration.DefaultRetainedFileCountLimit)
        {
            var logger = CreateLogger(pathFormat, LoggingEnabled,minimumLevel, levelOverrides, isJson, fileSizeLimitBytes, retainedFileCountLimit);
            return loggerFactory.AddSerilog(logger, dispose: true);
        }


        public static ILoggingBuilder AddFile(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            if (loggingBuilder == null) throw new ArgumentNullException(nameof(loggingBuilder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var config = configuration.Get<FileLoggingConfiguration>();
            if (string.IsNullOrWhiteSpace(config.PathFormat))
            {
                SelfLog.WriteLine("Unable to add the file logger: no PathFormat was present in the configuration");
                return loggingBuilder;
            }

            var minimumLevel = GetMinimumLogLevel(configuration);
            var levelOverrides =  GetLevelOverrides(configuration);

            return loggingBuilder.AddFile(config.PathFormat,config.LoggingEnabled, minimumLevel, levelOverrides, config.Json, config.FileSizeLimitBytes, config.RetainedFileCountLimit);
        }


        public static ILoggingBuilder AddFile(this ILoggingBuilder loggingBuilder, string pathFormat,string LoggingEnabled, LogLevel minimumLevel = LogLevel.Information, IDictionary<string, LogLevel> levelOverrides = null, bool isJson = false, long? fileSizeLimitBytes = FileLoggingConfiguration.DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = FileLoggingConfiguration.DefaultRetainedFileCountLimit)
        {
            var logger = CreateLogger(pathFormat, LoggingEnabled, minimumLevel, levelOverrides, isJson, fileSizeLimitBytes, retainedFileCountLimit);

            return loggingBuilder.AddSerilog(logger, dispose: true);
        }

        private static Serilog.Core.Logger CreateLogger(string pathFormat,string LoggingEnabled, LogLevel minimumLevel, IDictionary<string, LogLevel> levelOverrides, bool isJson, long? fileSizeLimitBytes, int? retainedFileCountLimit)
        {
            var configuration = new LoggerConfiguration();
            if (LoggingEnabled.ToLower().Equals("true"))
            {
                var formatter = isJson ? (ITextFormatter)new RenderedCompactJsonFormatter() : new MessageTemplateTextFormatter("{Timestamp:o} {RequestId,13} [{Level:u3}] {Message} ({EventId:x8}){NewLine}{Exception}", null);

                     configuration = new LoggerConfiguration().MinimumLevel.Is(Conversions.MicrosoftToSerilogLevel(minimumLevel))
                    .Enrich.FromLogContext()
                    .WriteTo.Async(w => w.RollingFile(formatter, Environment.ExpandEnvironmentVariables(pathFormat),
                     fileSizeLimitBytes: fileSizeLimitBytes
                     , retainedFileCountLimit: retainedFileCountLimit,shared:true, flushToDiskInterval: TimeSpan.FromSeconds(2)));
                ;

                if (!isJson)
                {
                    configuration.Enrich.With<EventIdEnricher>();
                }

                foreach (var levelOverride in levelOverrides ?? new Dictionary<string, LogLevel>())
                {
                    configuration.MinimumLevel.Override(levelOverride.Key, Conversions.MicrosoftToSerilogLevel(levelOverride.Value));
                }

            }


            return configuration.CreateLogger();
        }

        //private static Serilog.Core.Logger CreateLogger(string pathFormat, LogLevel minimumLevel, IDictionary<string, LogLevel> levelOverrides, bool isJson, long? fileSizeLimitBytes, int? retainedFileCountLimit)
        //{
        //    //if (pathFormat == null) throw new ArgumentNullException(nameof(pathFormat));

        //    var formatter = isJson ? (ITextFormatter)new RenderedCompactJsonFormatter() : new MessageTemplateTextFormatter("{Timestamp:o} {RequestId,13} [{Level:u3}] {Message} ({EventId:x8}){NewLine}{Exception}", null);

        //    var configuration = new LoggerConfiguration()
        //        .MinimumLevel.Is(Conversions.MicrosoftToSerilogLevel(minimumLevel))
        //        .Enrich.FromLogContext()
        //        .WriteTo.Async(w => w.RollingFile(
        //            formatter,
        //            Environment.ExpandEnvironmentVariables(pathFormat),
        //            fileSizeLimitBytes: fileSizeLimitBytes,
        //            retainedFileCountLimit: retainedFileCountLimit,
        //            shared: true,
        //            flushToDiskInterval: TimeSpan.FromSeconds(2)));

        //    if (!isJson)
        //    {
        //        configuration.Enrich.With<EventIdEnricher>();
        //    }

        //    foreach (var levelOverride in levelOverrides ?? new Dictionary<string, LogLevel>())
        //    {
        //        configuration.MinimumLevel.Override(levelOverride.Key, Conversions.MicrosoftToSerilogLevel(levelOverride.Value));
        //    }

        //    return configuration.CreateLogger();
        //}

        private static LogLevel GetMinimumLogLevel(IConfiguration configuration)
        {
            var minimumLevel = LogLevel.Information;
            var defaultLevel = configuration["LogLevel:Default"];
            if (!string.IsNullOrWhiteSpace(defaultLevel))
            {
                if (!Enum.TryParse(defaultLevel, out minimumLevel))
                {
                    SelfLog.WriteLine("The minimum level setting `{0}` is invalid", defaultLevel);
                    minimumLevel = LogLevel.Information;
                }
            }
            return minimumLevel;
        }

        private static Dictionary<string, LogLevel> GetLevelOverrides(IConfiguration configuration)
        {
            var levelOverrides = new Dictionary<string, LogLevel>();
            foreach (var overr in configuration.GetSection("LogLevel").GetChildren().Where(cfg => cfg.Key != "Default"))
            {
                if (!Enum.TryParse(overr.Value, out LogLevel value))
                {
                    SelfLog.WriteLine("The level override setting `{0}` for `{1}` is invalid", overr.Value, overr.Key);
                    continue;
                }

                levelOverrides[overr.Key] = value;
            }

            return levelOverrides;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace MessageBrokerServiceApi.Common.Helper
{
    public static class Logging
    {
        public static void InitializeSerilog(IConfiguration configuration, string source,
            string enabledIntegration = null)
        {
            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                //.ReadFrom.Settings(configuration)
                .Enrich.WithProperty("Source", source)
                .Enrich.With<ExceptionDataEnricher>();

            if (!string.IsNullOrWhiteSpace(enabledIntegration))
                loggerConfig.Enrich.WithProperty("EnabledIntegration", enabledIntegration);

            Log.Logger = loggerConfig.CreateLogger();
            Serilog.Debugging.SelfLog.Enable(Console.Error);
        }

        /// <summary>
        /// Inicializa a configuração do Serilog utilizando o nome do EntryAssembly
        /// como source
        /// </summary>
        /// <param name="configuration"></param>
        public static void InitializeSerilog(IConfiguration configuration, string enabledIntegration = null)
        {
            InitializeSerilog(configuration,
                Assembly.GetEntryAssembly().GetName().Name, enabledIntegration);
        }

        public class ExceptionDataEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                if (logEvent.Exception == null ||
                    logEvent.Exception.Data == null ||
                    logEvent.Exception.Data.Count == 0) return;

                var dataDictionary = logEvent.Exception.Data
                    .Cast<DictionaryEntry>()
                    .Where(e => e.Key is string)
                    .ToDictionary(e => (string)e.Key, e => e.Value);

                var property = propertyFactory.CreateProperty("ExceptionData", dataDictionary, destructureObjects: true);

                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}

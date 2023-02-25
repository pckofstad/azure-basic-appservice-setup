using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace BlazorApp.Server.Configuration
{
    public class ConfigurationInfo
    {
        public const string NORWEGIAN_CUTURE = "nb-no";

        private enum RequiredValues
        {
            SQL_CONNECTION_STRING,
            AzureWebJobsStorage
        }

        /// <summary>
        /// Values that should always have a system fallback set
        /// </summary>
        private enum OptionalValues
        {
            APPINSIGHTS_INSTRUMENTATIONKEY,
        }

        private static ILogger? _logger;
        private static IConfiguration _config = null!;

        public ConfigurationInfo(IConfiguration config, ILogger? logger = null)
        {
            _logger = logger;
            _config = config;
        }



        public static bool IsConfigurationHealthOk(bool throwException = true)
        {
            try
            {
                var configurationValues = Enum.GetValues(typeof(RequiredValues))
                    .Cast<RequiredValues>()
                    .ToList();

                foreach (var configurationValue in configurationValues)
                {
                    var value = _config.GetSection(configurationValue.ToString());
                    if (value == null)
                    {
                        throw new Exception($"Required configuration missing: {configurationValue}");
                    }
                }
            }
            catch
            {
                _logger?.LogError("EnviromentService - HealthCheck failing");
                if (throwException)
                {
                    throw;
                }

                return false;
            }

            return true;
        }

        #region Support functions

        private static string GetValue(RequiredValues enumKey)
        {
            var value = _config.GetSection(enumKey.ToString()).Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            _logger?.LogError("Required configuration value not found: {enumKey}", enumKey);
            throw new Exception($"Required configuration value not found: {enumKey}");

        }

        private static string GetValue(OptionalValues enumKey, string fallbackValue)
        {
            var value = _config.GetSection(enumKey.ToString()).Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                value = fallbackValue;
            }

            return value.Trim();
        }

        private static bool GetValueAsBool(OptionalValues enumKey, string fallback) =>
            GetValue(enumKey, fallback) == "1";

        #endregion


        public static string GetSqlConnectionString() => GetValue(RequiredValues.SQL_CONNECTION_STRING);

        public static string GetAPPINSIGHTS_INSTRUMENTATIONKEY() => GetValue(OptionalValues.APPINSIGHTS_INSTRUMENTATIONKEY, "");

        public static bool IsLocalDebugBuild()
        {
#if DEBUG
            return true;
#else
            return false;
#endif

        }

        public static TimeZoneInfo GetNorwegianTimeZoneInfo() => TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");

    }

}

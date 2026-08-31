using Microsoft.Extensions.Configuration;

namespace PlaywrightTests.Configuration
{
    /// <summary>
    /// Loads TestSettings from appsettings.json
    /// with environment variable override support.
    /// </summary>
    public static class TestSettingsLoader
    {
        private static TestSettings? _settings;

        public static TestSettings Load()
        {
            if (_settings != null) return _settings;

            var environment = Environment
                .GetEnvironmentVariable(
                    "ASPNETCORE_ENVIRONMENT")
                ?? "Development";

            var config = new ConfigurationBuilder()
                .AddJsonFile(
                    Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
                    optional: false,
                    reloadOnChange: false)
                .AddJsonFile(
                    Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment}.json"),
                    optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables(
                    prefix: "TESTSETTINGS__")
                .Build();

            _settings = config
                .GetSection(TestSettings.SectionName)
                .Get<TestSettings>()
                ?? new TestSettings();

            // Override credentials from env vars
            var user = Environment
                .GetEnvironmentVariable(
                    "SAUCEDEMO_USERNAME");
            var pass = Environment
                .GetEnvironmentVariable(
                    "SAUCEDEMO_PASSWORD");

            if (!string.IsNullOrEmpty(user))
                _settings.SauceDemoUsername = user;
            if (!string.IsNullOrEmpty(pass))
                _settings.SauceDemoPassword = pass;

            return _settings;
        }
    }
}
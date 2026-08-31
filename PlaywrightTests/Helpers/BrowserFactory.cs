using Microsoft.Playwright;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Helpers
{
    /// <summary>
    /// KAN-6: Config-driven browser factory.
    /// Creates IBrowser based on TestSettings.Browser.
    /// Supports: chromium, firefox, webkit.
    /// </summary>
    public static class BrowserFactory
    {
        public static async Task<IBrowser> Create(
            IPlaywright playwright,
            TestSettings settings)
        {
            var options = new BrowserTypeLaunchOptions
            {
                Headless = settings.Headless,
                SlowMo = settings.SlowMo
            };

            return settings.Browser
                .ToLower() switch
            {
                "firefox" =>
                    await playwright.Firefox
                        .LaunchAsync(options),

                "webkit" =>
                    await playwright.Webkit
                        .LaunchAsync(options),

                "chrome" or "chromium" =>
                    await playwright.Chromium
                        .LaunchAsync(
                            new BrowserTypeLaunchOptions
                            {
                                Headless = settings.Headless,
                                SlowMo = settings.SlowMo,
                                Channel =
                                    string.IsNullOrEmpty(
                                        settings.Channel)
                                    ? null
                                    : settings.Channel
                            }),

                _ =>
                    await playwright.Chromium
                        .LaunchAsync(options)
            };
        }
    }
}
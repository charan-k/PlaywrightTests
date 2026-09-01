using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Tests.Api
{
    /// <summary>
    /// KAN-8: Base class for all API tests.
    /// KAN-25: Setup APIRequestContext with
    ///         DemoQABaseUrl from TestSettings (KAN-5).
    /// </summary>
    public abstract class ApiTestBase
    {
        protected IAPIRequestContext
            ApiContext = null!;
        protected TestSettings
            Settings = null!;

        private IPlaywright _playwright = null!;

        [SetUp]
        public async Task ApiSetup()
        {
            // ✅ KAN-5: Load config from
            // TestSettings (no hardcoded URLs)
            Settings = TestSettingsLoader.Load();

            _playwright =
                await Playwright.CreateAsync();

            // ✅ KAN-8: Create APIRequestContext
            // BaseURL from TestSettings.DemoQABaseUrl
            ApiContext = await _playwright
                .APIRequest.NewContextAsync(
                    new APIRequestNewContextOptions
                    {
                        BaseURL =
                            Settings.DemoQABaseUrl,
                        ExtraHTTPHeaders =
                            new Dictionary
                            <string, string>
                        {
                            {
                                "Content-Type",
                                "application/json"
                            },
                            {
                                "Accept",
                                "application/json"
                            }
                        }
                    });

            Console.WriteLine(
                $"✅ API Context created: " +
                $"{Settings.DemoQABaseUrl}");
        }

        [TearDown]
        public async Task ApiTeardown()
        {
            if (ApiContext != null)
                await ApiContext.DisposeAsync();

            _playwright?.Dispose();

            Console.WriteLine(
                "✅ API Context disposed.");
        }
    }
}
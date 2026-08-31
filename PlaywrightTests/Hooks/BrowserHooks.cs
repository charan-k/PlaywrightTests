using Microsoft.Playwright;
using PlaywrightTests.Configuration;
using PlaywrightTests.Helpers;
using Reqnroll;
using Reqnroll.BoDi;

namespace PlaywrightTests.Hooks
{
    /// <summary>
    /// KAN-3: Reqnroll hooks for browser lifecycle.
    /// [BeforeScenario]: launches browser
    /// [AfterScenario]: screenshot on fail + closes
    /// </summary>
    [Binding]
    public class BrowserHooks
    {
        private readonly IObjectContainer _container;
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;

        public BrowserHooks(
            IObjectContainer container)
            => _container = container;

        [BeforeScenario]
        public async Task SetupBrowser()
        {
            var settings =
                TestSettingsLoader.Load();

            _playwright =
                await Playwright.CreateAsync();

            // ✅ KAN-6: Use BrowserFactory
            _browser = await BrowserFactory.Create(
                _playwright, settings);

            var context = await _browser
                .NewContextAsync(
                    new BrowserNewContextOptions
                    {
                        ViewportSize = new ViewportSize
                        {
                            Width = settings.ViewportWidth,
                            Height = settings.ViewportHeight
                        }
                    });

            var page = await context.NewPageAsync();

            // Register for DI in step definitions
            _container.RegisterInstanceAs(page);
            _container.RegisterInstanceAs(settings);
            _container.RegisterInstanceAs(context);

            Console.WriteLine(
                $"✅ BDD Browser: " +
                $"{settings.Browser.ToUpper()}");
        }

        [AfterScenario]
        public async Task TeardownBrowser(
            ScenarioContext scenarioContext)
        {
            // ✅ KAN-4: Screenshot on failure
            if (scenarioContext.TestError != null)
            {
                try
                {
                    var page = _container
                        .Resolve<IPage>();
                    var settings =
                        TestSettingsLoader.Load();
                    var dir = Path.Combine(
                        settings.ReportPath,
                        "Screenshots");
                    Directory.CreateDirectory(dir);
                    var path = Path.Combine(dir,
                        $"BDD_{scenarioContext.ScenarioInfo.Title.Replace(" ", "_")}" +
                        $"-{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    await page.ScreenshotAsync(
                        new() { Path = path });
                    Console.WriteLine(
                        $"📸 BDD Screenshot: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Screenshot: {ex.Message}");
                }
            }

            await _browser.CloseAsync();
            _playwright.Dispose();
            Console.WriteLine(
                "✅ BDD Browser closed.");
        }
    }
}
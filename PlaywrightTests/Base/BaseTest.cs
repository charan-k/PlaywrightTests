using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightTests.Configuration;
using PlaywrightTests.Helpers;

namespace PlaywrightTests.Base
{
    public class BaseTest
    {
        protected IPlaywright _playwright = null!;
        protected IBrowser _browser = null!;
        protected IBrowserContext _context = null!;
        protected IPage _page = null!;
        protected TestSettings Settings = null!;

        public static readonly object[] BrowserTestData =
        {
            new object[] { "chrome"  },
            new object[] { "firefox" },
            new object[] { "webkit"  }
        };

        // ✅ KAN-6: Browser created via BrowserFactory
        // ✅ KAN-5: All config from TestSettings
        // ✅ KAN-4: Tracing started per test
        public async Task LaunchBrowser(
            string browserName)
        {
            Settings = TestSettingsLoader.Load();

            // ✅ KAN-6: Override browser from parameter
            // (allows TestFixtureSource multi-browser)
            if (!string.IsNullOrEmpty(browserName))
                Settings.Browser = browserName;

            _playwright = await Playwright.CreateAsync();

            // ✅ KAN-6: Use BrowserFactory
            _browser = await BrowserFactory.Create(
                _playwright, Settings);

            _context = await _browser
                .NewContextAsync(
                    new BrowserNewContextOptions
                    {
                        ViewportSize = new ViewportSize
                        {
                            Width = Settings.ViewportWidth,
                            Height = Settings.ViewportHeight
                        }
                    });

            // ✅ KAN-4: Start tracing for every test
            await _context.Tracing.StartAsync(
                new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true,
                    Title =
                        TestContext.CurrentContext
                        .Test.FullName
                });

            _page = await _context.NewPageAsync();

            Console.WriteLine(
                $"✅ [{Settings.Browser.ToUpper()}]" +
                $" Launched | " +
                $"Headless={Settings.Headless} | " +
                $"SlowMo={Settings.SlowMo}ms");
        }

        [TearDown]
        public async Task Cleanup()
        {
            Settings ??= TestSettingsLoader.Load();

            var status =
                TestContext.CurrentContext
                .Result.Outcome.Status;

            var testName = SanitizeFileName(
                TestContext.CurrentContext.Test.Name);

            var ts = DateTime.Now
                .ToString("yyyyMMdd_HHmmss");

            if (status == TestStatus.Failed)
            {
                await SaveFailureArtifacts(
                    testName, ts);
            }
            else
            {
                if (_context != null)
                {
                    await _context.Tracing.StopAsync(
                        new TracingStopOptions());
                }
                Console.WriteLine(
                    $"✅ PASSED: {testName}" +
                    $" — trace discarded (no .zip)");
            }

            if (_browser != null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        private async Task SaveFailureArtifacts(
            string testName, string ts)
        {
            var screenshotDir = Path.Combine(
                Settings.ReportPath, "Screenshots");
            var traceDir = Path.Combine(
                Settings.ReportPath, "Traces");

            Directory.CreateDirectory(screenshotDir);
            Directory.CreateDirectory(traceDir);

            if (_page != null)
            {
                var shotPath = Path.Combine(
                    screenshotDir,
                    $"{testName}-{ts}.png");
                try
                {
                    await _page.ScreenshotAsync(
                        new PageScreenshotOptions
                        {
                            Path = shotPath,
                            FullPage = true
                        });
                    TestContext.AddTestAttachment(
                        shotPath, "Failure Screenshot");
                    Console.WriteLine(
                        $"📸 Screenshot: {shotPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Screenshot failed: " +
                        $"{ex.Message}");
                }
            }

            if (_context != null)
            {
                var tracePath = Path.Combine(
                    traceDir,
                    $"{testName}-{ts}.zip");
                try
                {
                    await _context.Tracing.StopAsync(
                        new TracingStopOptions
                        {
                            Path = tracePath
                        });
                    TestContext.AddTestAttachment(
                        tracePath, "Playwright Trace");
                    Console.WriteLine(
                        $"🔍 Trace: {tracePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Trace failed: {ex.Message}");
                }
            }

            Console.WriteLine(
                $"❌ FAILED: {testName}");
        }

        private static string SanitizeFileName(
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return "UnknownTest";
            var invalid = Path.GetInvalidFileNameChars();
            var clean = string.Concat(
                name.Select(c =>
                    invalid.Contains(c) ? '_' : c));
            return clean.Length > 100
                ? clean[..100] : clean;
        }
    }
}
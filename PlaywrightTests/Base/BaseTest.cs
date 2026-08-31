using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Base
{
    /// <summary>
    /// Base class for all Playwright tests.
    /// KAN-5: All config from TestSettings (no hardcoded values).
    /// KAN-4: Screenshot + trace captured ONLY on failure.
    ///        Passing tests stop tracing without saving .zip.
    ///        All paths derive from Settings.ReportPath.
    ///        File names sanitized for parameterized tests.
    /// KAN-6: BrowserFactory-driven browser selection.
    /// </summary>
    public class BaseTest
    {
        protected IPlaywright _playwright = null!;
        protected IBrowser _browser = null!;
        protected IBrowserContext _context = null!;
        protected IPage _page = null!;
        protected TestSettings Settings = null!;

        // ✅ KAN-6: Multi-browser test data
        public static readonly object[] BrowserTestData =
        {
            new object[] { "chrome"  },
            new object[] { "firefox" },
            new object[] { "webkit"  }
        };

        // ─────────────────────────────────────────────────
        // ✅ KAN-5: All values from TestSettings (no hardcoding)
        // ✅ KAN-4: Tracing started here, stopped in TearDown
        // ✅ KAN-6: Browser selected via switch from config
        // ─────────────────────────────────────────────────
        public async Task LaunchBrowser(string browserName)
        {
            Settings = TestSettingsLoader.Load();
            _playwright = await Playwright.CreateAsync();

            var opts = new BrowserTypeLaunchOptions
            {
                Headless = Settings.Headless,
                SlowMo = Settings.SlowMo
            };

            _browser = browserName.ToLower() switch
            {
                "chrome" or "chromium" =>
                    await _playwright.Chromium
                        .LaunchAsync(
                            new BrowserTypeLaunchOptions
                            {
                                Headless = Settings.Headless,
                                SlowMo = Settings.SlowMo,
                                Channel =
                                    string.IsNullOrEmpty(
                                        Settings.Channel)
                                    ? null
                                    : Settings.Channel
                            }),
                "firefox" =>
                    await _playwright.Firefox
                        .LaunchAsync(opts),
                "webkit" =>
                    await _playwright.Webkit
                        .LaunchAsync(opts),
                _ =>
                    await _playwright.Chromium
                        .LaunchAsync(opts)
            };

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

            // ✅ KAN-4: Start tracing for EVERY test
            // Will be stopped in TearDown:
            //   FAIL → trace saved as .zip
            //   PASS → trace stopped, NO .zip written
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
                $"✅ [{browserName.ToUpper()}] Launched" +
                $" | Headless={Settings.Headless}" +
                $" | SlowMo={Settings.SlowMo}ms");
        }

        // ─────────────────────────────────────────────────
        // ✅ KAN-4: Cleanup with conditional artifact capture
        //
        // FAILED test  → screenshot + trace.zip saved
        // PASSED test  → trace stopped, NO files saved
        //                (keeps Traces/ folder clean)
        //
        // ALL paths from Settings.ReportPath (KAN-5)
        // Screenshots/ and Traces/ = subfolders of ReportPath
        //
        // File names sanitized:
        //   Path.GetInvalidFileNameChars() replacement
        //   Max 100 chars (Windows MAX_PATH safe)
        //   Handles parameterized names e.g. Test("a,b/c")
        // ─────────────────────────────────────────────────
        [TearDown]
        public async Task Cleanup()
        {
            // Load settings (in case LaunchBrowser
            // was not called in this test)
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
                // ✅ PASSED: Stop tracing WITHOUT
                // saving .zip — keeps Traces/ clean
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

        // ─────────────────────────────────────────────────
        // Called only when test FAILS
        // ─────────────────────────────────────────────────
        private async Task SaveFailureArtifacts(
            string testName, string ts)
        {
            // ✅ All paths from Settings.ReportPath
            // Screenshots/ and Traces/ are SUBFOLDERS
            // of ReportPath (./TestResults/HtmlReports/)
            var screenshotDir = Path.Combine(
                Settings.ReportPath, "Screenshots");

            var traceDir = Path.Combine(
                Settings.ReportPath, "Traces");

            // Create subdirs at runtime if missing
            Directory.CreateDirectory(screenshotDir);
            Directory.CreateDirectory(traceDir);

            // ✅ Screenshot — failed tests only
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

                    // Attach to NUnit output
                    TestContext.AddTestAttachment(
                        shotPath,
                        "Failure Screenshot");

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

            // ✅ Trace .zip — failed tests only
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

                    // Attach to NUnit output
                    TestContext.AddTestAttachment(
                        tracePath,
                        "Playwright Trace");

                    Console.WriteLine(
                        $"🔍 Trace: {tracePath}");
                    Console.WriteLine(
                        "   View: " +
                        "https://trace.playwright.dev");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"⚠️ Trace failed: " +
                        $"{ex.Message}");
                }
            }

            var err =
                TestContext.CurrentContext
                .Result.Message
                ?? "No error message";

            Console.WriteLine(
                $"❌ FAILED: {testName}");
            Console.WriteLine(
                $"   Error: " +
                $"{err[..Math.Min(150, err.Length)]}");
        }

        // ─────────────────────────────────────────────────
        // ✅ Sanitize test name for file system use
        // Replaces invalid chars (,./:*?"<>|) with _
        // Handles parameterized test names:
        //   DemoQA_CheckBox_Complete("chrome,param")
        //   → DemoQA_CheckBox_Complete__chrome_param_
        // Truncated to 100 chars for Windows safety
        // ─────────────────────────────────────────────────
        private static string SanitizeFileName(
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return "UnknownTest";

            var invalidChars =
                Path.GetInvalidFileNameChars();

            var clean = string.Concat(
                name.Select(c =>
                    invalidChars.Contains(c)
                    ? '_' : c));

            return clean.Length > 100
                ? clean[..100]
                : clean;
        }
    }
}
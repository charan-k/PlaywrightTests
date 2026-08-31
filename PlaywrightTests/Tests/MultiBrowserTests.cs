using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Helpers;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class MultiBrowserTests
    {
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        private IBrowserContext _context = null!;
        private IPage _page = null!;

        // ✅ KAN-6: Browsers from config
        // Read GoogleBaseUrl from TestSettings
        private static TestSettings GetSettings()
            => TestSettingsLoader.Load();

        private static readonly object[] BrowserTestData =
        {
            new object[] { "chrome"   },
            new object[] { "firefox"  },
            new object[] { "webkit"   }
        };

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
        }

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        [Description(
            "KAN-6: Launch Google in Chrome, " +
            "Firefox and WebKit")]
        public async Task
        NavigateToGoogle_InMultipleBrowsers(
            string browserName)
        {
            var settings = GetSettings();
            settings.Browser = browserName;

            Console.WriteLine(
                $"🚀 Launching: {browserName.ToUpper()}");

            // ✅ KAN-6: Use BrowserFactory
            _browser = await BrowserFactory.Create(
                _playwright, settings);

            _context = await _browser
                .NewContextAsync(
                    new BrowserNewContextOptions
                    {
                        ViewportSize = new ViewportSize
                        {
                            Width = settings.ViewportWidth,
                            Height = settings.ViewportHeight
                        }
                    });

            _page = await _context.NewPageAsync();

            Console.WriteLine(
                $"✅ [{browserName.ToUpper()}] " +
                $"Browser Launched!");

            // ✅ KAN-5: URL from config (not hardcoded)
            await _page.GotoAsync(
                settings.GoogleBaseUrl);

            Console.WriteLine(
                $"✅ [{browserName.ToUpper()}] " +
                $"Navigated to: {_page.Url}");

            var title = await _page.TitleAsync();

            Assert.That(
                title,
                Does.Contain("Google"),
                $"[{browserName.ToUpper()}] " +
                $"Title should contain 'Google'");

            Console.WriteLine(
                $"✅ [{browserName.ToUpper()}] " +
                $"Title: {title}");

            await _page.CloseAsync();
            Console.WriteLine(
                $"✅ [{browserName.ToUpper()}] " +
                $"Page Closed.");
        }

        [TearDown]
        public async Task Cleanup()
        {
            if (_browser != null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
            Console.WriteLine("✅ Browser Closed.");
        }
    }
}
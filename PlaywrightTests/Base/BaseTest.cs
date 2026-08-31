using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Base
{
    public class BaseTest
    {
        protected IPlaywright _playwright = null!;
        protected IBrowser _browser = null!;
        protected IBrowserContext _context = null!;
        protected IPage _page = null!;
        protected TestSettings Settings = null!;

        // Test Data — Multiple Browsers
        public static readonly object[] BrowserTestData =
        {
            new object[] { "chrome"   },
            new object[] { "firefox"  },
            new object[] { "webkit"   }
        };

        public async Task LaunchBrowser(
            string browserName)
        {
            Settings = TestSettingsLoader.Load();
            _playwright = await Playwright.CreateAsync();

            var options = new BrowserTypeLaunchOptions
            {
                Headless = Settings.Headless,
                SlowMo = Settings.SlowMo
            };

            _browser = browserName switch
            {
                "chrome" => await _playwright
                    .Chromium.LaunchAsync(
                        new BrowserTypeLaunchOptions
                        {
                            Headless = Settings.Headless,
                            SlowMo = Settings.SlowMo,
                            Channel = Settings.Channel
                        }),
                "firefox" => await _playwright
                    .Firefox.LaunchAsync(options),
                "webkit" => await _playwright
                    .Webkit.LaunchAsync(options),
                _ => throw new ArgumentException(
                    $"Unknown browser: {browserName}")
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

            _page = await _context.NewPageAsync();

            Console.WriteLine(
                $"✅ [{browserName.ToUpper()}]" +
                $" Browser Launched!");
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
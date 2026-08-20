// Base/BaseTest.cs
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests.Base
{
    public class BaseTest
    {
        protected IPlaywright _playwright;
        protected IBrowser _browser;
        protected IBrowserContext _context;
        protected IPage _page;

        // ✅ Test Data — Multiple Browsers
        public static readonly object[] BrowserTestData =
        {
            new object[] { "chrome"  },
            new object[] { "firefox" },
            new object[] { "webkit"  }
        };

        public async Task LaunchBrowser(string browserName)
        {
            _playwright = await Playwright.CreateAsync();

            _browser = browserName switch
            {
                "chrome" => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Channel = "chrome",
                    Headless = false,
                    SlowMo = 800
                }),
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 800
                }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 800
                }),
                _ => throw new ArgumentException($"Unknown browser: {browserName}")
            };

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            _page = await _context.NewPageAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Browser Launched!");
        }

        [TearDown]
        public async Task Cleanup()
        {
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
            Console.WriteLine("✅ Browser Closed.");
        }
    }
}
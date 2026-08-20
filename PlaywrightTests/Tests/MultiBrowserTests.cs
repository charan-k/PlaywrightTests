using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class MultiBrowserTests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        // ✅ Test Data — Browser Name & URL
        private static readonly object[] BrowserTestData =
        {
            new object[] { "chrome",    "https://www.google.com/" },
            new object[] { "firefox",   "https://www.google.com/" },
            new object[] { "webkit",    "https://www.google.com/" }
        };

        [SetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
        }

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        [Description("Launch Google in Chrome, Firefox & WebKit")]
        public async Task NavigateToGoogle_InMultipleBrowsers(string browserName, string url)
        {
            Console.WriteLine($"🚀 Launching Browser: {browserName.ToUpper()}");

            // ✅ Step 1: Launch the correct browser
            _browser = browserName switch
            {
                "chrome" => await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Channel = "chrome",  // ✅ Real Google Chrome
                    Headless = false,
                    SlowMo = 500
                }),
                "firefox" => await _playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 500
                }),
                "webkit" => await _playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 500
                }),
                _ => throw new ArgumentException($"❌ Unknown browser: {browserName}")
            };

            // ✅ Step 2: Create Context & Page
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            _page = await _context.NewPageAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Browser Launched Successfully!");

            // ✅ Step 3: Navigate to Google
            await _page.GotoAsync(url);
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to: {_page.Url}");

            // ✅ Step 4: Assert Page Title
            var title = await _page.TitleAsync();
            Assert.That(title, Does.Contain("Google"),
                $"[{browserName.ToUpper()}] Page title should contain 'Google'");

            Console.WriteLine($"✅ [{browserName.ToUpper()}] Page Title: {title}");

            // ✅ Step 5: Close the Page
            await _page.CloseAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Page Closed Successfully.");
        }

        [TearDown]
        public async Task Cleanup()
        {
            // ✅ Close Browser after each test
            if (_browser != null)
            {
                await _browser.CloseAsync();
                Console.WriteLine("✅ Browser Closed in TearDown.");
            }

            // ✅ Dispose Playwright instance
            _playwright?.Dispose();
            Console.WriteLine("✅ Playwright Disposed.");
        }
    }
}

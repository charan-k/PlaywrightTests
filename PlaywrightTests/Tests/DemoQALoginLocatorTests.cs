// Tests/DemoQALoginTests.cs
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Base;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class DemoQALoginLocatorTests : BaseTest
    {
        private const string LoginUrl = "https://demoqa.com/login";

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQALogin_UsingAllLocators(string browserName)
        {
            await LaunchBrowser(browserName);

            // ✅ Step 1: Navigate to DemoQA Login
            await _page.GotoAsync(LoginUrl);
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to DemoQA Login");

            // ================================================
            // ✅ Locator 1: Using ID
            // ================================================
            var usernameById = _page.Locator("#userName");
            await usernameById.ClickAsync();
            await usernameById.FillAsync("testuser");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Username entered using ID");

            // ================================================
            // ✅ Locator 2: Using CSS Selector
            // ================================================
            var passwordByCSS = _page.Locator("input#password");
            await passwordByCSS.ClickAsync();
            await passwordByCSS.FillAsync("Test@1234");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Password entered using CSS");

            // ================================================
            // ✅ Locator 3: Using XPath
            // ================================================
            var loginBtnByXPath = _page.Locator("//button[@id='login']");
            await loginBtnByXPath.ClickAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Login button clicked using XPath");

            // ================================================
            // ✅ Locator 4: Using GetByPlaceholder
            // ================================================
            await _page.GotoAsync(LoginUrl);
            var usernameByPlaceholder = _page.GetByPlaceholder("UserName");
            await usernameByPlaceholder.FillAsync("testuser");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Username entered using GetByPlaceholder");

            // ================================================
            // ✅ Locator 5: Using GetByRole
            // ================================================
            var passwordByRole = _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
            await passwordByRole.FillAsync("Test@1234");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Password entered using GetByRole");

            // ================================================
            // ✅ Locator 6: Click Login Button using GetByRole
            // ================================================
            var loginByRole = _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
            await loginByRole.ClickAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Login button clicked using GetByRole");

            Console.WriteLine($"✅ [{browserName.ToUpper()}] DemoQA Login Test Completed!");
        }
    }
}
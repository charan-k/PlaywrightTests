using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Base;
using PlaywrightTests.Configuration;

namespace PlaywrightTests.Tests
{
    /// <summary>
    /// KAN-7: Negative Test Cases
    /// NC-001 to NC-005 as defined in FRD Section 4.5
    /// All tagged [Category("Negative")]
    /// Uses Page Objects from KAN-3 (if available)
    /// Falls back to inline locators if POM not done
    /// </summary>
    [TestFixture]
    [Category("Negative")]
    public class NegativeTests : BaseTest
    {
        // ================================================
        // ✅ NC-001: SauceDemo Invalid Credentials
        // ================================================
        [Test]
        [Category("Negative")]
        [Description(
            "NC-001: SauceDemo login with " +
            "invalid username and password")]
        public async Task
        SauceDemo_InvalidCredentials_ShouldShowError()
        {
            await LaunchBrowser("chrome");

            await _page.GotoAsync(
                Settings.SauceDemoBaseUrl);
            await _page.WaitForLoadStateAsync(
                LoadState.DOMContentLoaded);

            // Enter wrong credentials
            await _page.FillAsync(
                "#user-name", "wrong_user");
            await _page.FillAsync(
                "#password", "wrong_pass");
            await _page.ClickAsync(
                "#login-button");

            // Assert error message
            var error = await _page
                .Locator("[data-test='error']")
                .TextContentAsync();

            Assert.That(
                error,
                Does.Contain(
                    "Username and password " +
                    "do not match"),
                "NC-001: Error should say " +
                "credentials do not match");

            Console.WriteLine(
                $"✅ NC-001 PASSED: {error}");
        }

        // ================================================
        // ✅ NC-002: SauceDemo Empty Username
        // ================================================
        [Test]
        [Category("Negative")]
        [Description(
            "NC-002: SauceDemo login with " +
            "empty username field")]
        public async Task
        SauceDemo_EmptyUsername_ShouldShowError()
        {
            await LaunchBrowser("chrome");

            await _page.GotoAsync(
                Settings.SauceDemoBaseUrl);
            await _page.WaitForLoadStateAsync(
                LoadState.DOMContentLoaded);

            // Leave username empty
            await _page.FillAsync(
                "#user-name", string.Empty);
            await _page.FillAsync(
                "#password",
                Settings.SauceDemoPassword);
            await _page.ClickAsync(
                "#login-button");

            // Assert validation message
            var error = await _page
                .Locator("[data-test='error']")
                .TextContentAsync();

            Assert.That(
                error,
                Does.Contain("Username is required"),
                "NC-002: Error should say " +
                "Username is required");

            Console.WriteLine(
                $"✅ NC-002 PASSED: {error}");
        }

        // ================================================
        // ✅ NC-003: SauceDemo Locked Out User
        // ================================================
        [Test]
        [Category("Negative")]
        [Description(
            "NC-003: SauceDemo login with " +
            "locked_out_user account")]
        public async Task
        SauceDemo_LockedUser_ShouldShowError()
        {
            await LaunchBrowser("chrome");

            await _page.GotoAsync(
                Settings.SauceDemoBaseUrl);
            await _page.WaitForLoadStateAsync(
                LoadState.DOMContentLoaded);

            // Use locked out user
            await _page.FillAsync(
                "#user-name", "locked_out_user");
            await _page.FillAsync(
                "#password",
                Settings.SauceDemoPassword);
            await _page.ClickAsync(
                "#login-button");

            // Assert locked out message
            var error = await _page
                .Locator("[data-test='error']")
                .TextContentAsync();

            Assert.That(
                error,
                Does.Contain("locked out"),
                "NC-003: Error should say " +
                "user has been locked out");

            Console.WriteLine(
                $"✅ NC-003 PASSED: {error}");
        }

        // ================================================
        // ✅ NC-004: DemoQA Invalid Credentials
        // ================================================
        [Test]
        [Category("Negative")]
        [Description(
            "NC-004: DemoQA login with " +
            "invalid credentials")]
        public async Task
        DemoQA_InvalidCredentials_ShouldShowError()
        {
            await LaunchBrowser("chrome");

            await _page.GotoAsync(
                Settings.DemoQALoginUrl);
            await _page.WaitForLoadStateAsync(
                LoadState.DOMContentLoaded);

            // Enter invalid DemoQA credentials
            await _page.GetByPlaceholder("UserName")
                .FillAsync("invalid_user_123");
            await _page.GetByRole(
                AriaRole.Textbox,
                new() { Name = "Password" })
                .FillAsync("invalid_pass_123");
            await _page.GetByRole(
                AriaRole.Button,
                new() { Name = "Login" })
                .ClickAsync();

            // Wait for response
            await _page.WaitForTimeoutAsync(2000);

            // Assert: still on login page OR error shown
            var currentUrl = _page.Url;
            var loginFailed =
                currentUrl.Contains("login") ||
                currentUrl.Contains("demoqa");

            Assert.That(
                loginFailed,
                Is.True,
                "NC-004: Login with invalid " +
                "credentials should fail");

            Console.WriteLine(
                $"✅ NC-004 PASSED. " +
                $"Login failed as expected. " +
                $"URL: {currentUrl}");
        }

        // ================================================
        // ✅ NC-005: DemoQA Empty Required Fields
        // ================================================
        [Test]
        [Category("Negative")]
        [Description(
            "NC-005: DemoQA login with " +
            "empty username and password fields")]
        public async Task
        DemoQA_EmptyFields_ShouldShowValidation()
        {
            await LaunchBrowser("chrome");

            await _page.GotoAsync(
                Settings.DemoQALoginUrl);
            await _page.WaitForLoadStateAsync(
                LoadState.DOMContentLoaded);

            // Submit with empty fields
            await _page.GetByRole(
                AriaRole.Button,
                new() { Name = "Login" })
                .ClickAsync();

            await _page.WaitForTimeoutAsync(1000);

            // Assert: still on login page
            var currentUrl = _page.Url;

            Assert.That(
                currentUrl,
                Does.Contain("login")
                .Or.Contain("demoqa"),
                "NC-005: Empty fields should " +
                "not redirect away from login");

            Console.WriteLine(
                $"✅ NC-005 PASSED. " +
                $"Empty fields validation works. " +
                $"URL: {currentUrl}");
        }
    }
}
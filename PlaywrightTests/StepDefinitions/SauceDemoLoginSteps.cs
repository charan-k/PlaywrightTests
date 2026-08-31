using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Pages;
using Reqnroll;

namespace PlaywrightTests.StepDefinitions
{
    /// <summary>
    /// KAN-3: Step definitions for SauceDemo login.
    /// Binds to SauceDemoLoginPage Page Object.
    /// </summary>
    [Binding]
    public class SauceDemoLoginSteps
    {
        private readonly IPage _page;
        private readonly TestSettings _settings;
        private readonly SauceDemoLoginPage
            _loginPage;

        public SauceDemoLoginSteps(
            IPage page,
            TestSettings settings)
        {
            _page = page;
            _settings = settings;
            _loginPage =
                new SauceDemoLoginPage(page);
        }

        [Given(
            "the PlaywrightTests framework " +
            "is configured")]
        public void GivenFrameworkConfigured()
        {
            Assert.That(
                _settings,
                Is.Not.Null,
                "TestSettings should be loaded");
            Console.WriteLine(
                "✅ Framework configured via " +
                "TestSettings.cs (KAN-5)");
        }

        [Given(
            "the SauceDemo login page is open")]
        public async Task GivenSauceDemoOpen()
        {
            await _loginPage.NavigateTo(
                _settings.SauceDemoBaseUrl);
            Console.WriteLine(
                $"✅ SauceDemo open: {_page.Url}");
        }

        [When(
            "the user enters valid credentials")]
        public async Task WhenValidCredentials()
        {
            await _loginPage.Login(
                _settings.SauceDemoUsername,
                _settings.SauceDemoPassword);
            Console.WriteLine(
                "✅ Valid credentials entered");
        }

        [When(
            "the user enters invalid credentials" +
            " {string} and {string}")]
        public async Task WhenInvalidCredentials(
            string user, string pass)
        {
            await _loginPage.Login(user, pass);
            Console.WriteLine(
                $"✅ Invalid creds: {user}");
        }

        [When(
            "the username field is left empty")]
        public async Task WhenEmptyUsername()
        {
            await _loginPage.Login(
                string.Empty,
                _settings.SauceDemoPassword);
            Console.WriteLine(
                "✅ Empty username submitted");
        }

        [When(
            "the user enters locked out " +
            "user credentials")]
        public async Task WhenLockedOutUser()
        {
            await _loginPage.Login(
                "locked_out_user",
                _settings.SauceDemoPassword);
            Console.WriteLine(
                "✅ locked_out_user submitted");
        }

        [Then(
            "the user is on the inventory page")]
        public async Task ThenInventoryVisible()
        {
            await _page.WaitForURLAsync(
                "**/inventory**");
            Assert.That(
                _page.Url,
                Does.Contain("inventory"),
                "Should be on inventory page");
            Console.WriteLine(
                "✅ On inventory page");
        }

        [Then(
            "the error message {string}" +
            " is displayed")]
        public async Task ThenErrorShown(
            string expected)
        {
            var error = await _loginPage
                .GetErrorMessage();
            Assert.That(
                error,
                Does.Contain(expected),
                $"Expected error: {expected}");
            Console.WriteLine(
                $"✅ Error shown: {error}");
        }
    }
}
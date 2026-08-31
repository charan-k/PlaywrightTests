using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Configuration;
using PlaywrightTests.Pages;
using Reqnroll;

namespace PlaywrightTests.StepDefinitions
{
    [Binding]
    public class DemoQALoginSteps
    {
        private readonly IPage _page;
        private readonly TestSettings _settings;
        private readonly DemoQALoginPage _loginPage;

        public DemoQALoginSteps(
            IPage page,
            TestSettings settings)
        {
            _page = page;
            _settings = settings;
            _loginPage = new DemoQALoginPage(page);
        }

        [Given(
            "the DemoQA login page is open")]
        public async Task GivenDemoQAOpen()
        {
            await _loginPage.NavigateTo(
                _settings.DemoQALoginUrl);
            Console.WriteLine(
                $"✅ DemoQA open: {_page.Url}");
        }

        [Then(
            "the login form is visible")]
        public async Task ThenFormVisible()
        {
            var form = _page.GetByPlaceholder(
                "UserName");
            Assert.That(
                await form.IsVisibleAsync(),
                Is.True,
                "Login form should be visible");
            Console.WriteLine(
                "✅ DemoQA login form visible");
        }

        [When(
            "invalid DemoQA credentials " +
            "are submitted")]
        public async Task WhenInvalidDemoQA()
        {
            await _loginPage.Login(
                "invalid_user",
                "invalid_pass");
            Console.WriteLine(
                "✅ Invalid DemoQA creds submitted");
        }

        [Then(
            "an error response is shown on DemoQA")]
        public async Task ThenDemoQAError()
        {
            await _page.WaitForTimeoutAsync(2000);
            var stillOnLogin =
                _loginPage.IsOnLoginPage();
            Assert.That(
                stillOnLogin,
                Is.True,
                "Should remain on login page");
            Console.WriteLine(
                "✅ DemoQA login failed as expected");
        }
    }
}
using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// KAN-3: Page Object for SauceDemo Login.
    /// Binds step definitions to page interactions.
    /// </summary>
    public class SauceDemoLoginPage
    {
        private readonly IPage _page;

        public SauceDemoLoginPage(IPage page)
            => _page = page;

        public async Task NavigateTo(string url)
            => await _page.GotoAsync(url);

        public async Task Login(
            string username, string password)
        {
            await _page.FillAsync(
                "#user-name", username);
            await _page.FillAsync(
                "#password", password);
            await _page.ClickAsync(
                "#login-button");
        }

        public async Task<string> GetErrorMessage()
            => await _page
                .Locator("[data-test='error']")
                .TextContentAsync()
                ?? string.Empty;

        public async Task<bool>
            IsInventoryPageVisible()
            => _page.Url.Contains("inventory");

        public async Task<string> GetPageTitle()
            => await _page.TitleAsync();
    }
}
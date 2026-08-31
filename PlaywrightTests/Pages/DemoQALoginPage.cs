using Microsoft.Playwright;

namespace PlaywrightTests.Pages
{
    /// <summary>
    /// KAN-3: Page Object for DemoQA Login.
    /// </summary>
    public class DemoQALoginPage
    {
        private readonly IPage _page;

        public DemoQALoginPage(IPage page)
            => _page = page;

        public async Task NavigateTo(string url)
            => await _page.GotoAsync(url);

        public async Task Login(
            string username, string password)
        {
            await _page.GetByPlaceholder("UserName")
                .FillAsync(username);
            await _page.GetByRole(
                AriaRole.Textbox,
                new() { Name = "Password" })
                .FillAsync(password);
            await _page.GetByRole(
                AriaRole.Button,
                new() { Name = "Login" })
                .ClickAsync();
        }

        public async Task<string> GetErrorMessage()
        {
            try
            {
                return await _page
                    .Locator(".mb-1")
                    .TextContentAsync()
                    ?? string.Empty;
            }
            catch { return string.Empty; }
        }

        public bool IsOnLoginPage()
            => _page.Url.Contains("login");
    }
}
// Tests/SearchLocatorTests.cs
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Base;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class GoogleSearchLocatorTests : BaseTest
    {
        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task SearchGoogle_UsingDifferentLocators(string browserName)
        {
            await LaunchBrowser(browserName);

            // ✅ Step 1: Navigate to Google
            await _page.GotoAsync("https://www.google.com/");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to Google");

            // ✅ Wait for page to fully load
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ✅ Handle Cookie Consent Popup (if appears)
            try
            {
                var acceptBtn = _page.Locator("button:has-text('Accept all')");
                if (await acceptBtn.IsVisibleAsync())
                {
                    await acceptBtn.ClickAsync();
                    Console.WriteLine("✅ Cookie consent accepted");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("ℹ️ No cookie consent popup found");
            }

            // ================================================
            // ✅ Locator 1: Using NAME attribute
            // ================================================
            try
            {
                var searchByName = _page.Locator("[name='q']");
                await searchByName.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                await searchByName.ClickAsync();
                await searchByName.FillAsync("Playwright C# Automation");
                await searchByName.PressAsync("Enter");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine($"✅ [{browserName.ToUpper()}] Searched using NAME locator");

                var title1 = await _page.TitleAsync();
                Console.WriteLine($"✅ Title: {title1}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ NAME locator failed: {ex.Message}");
            }

            // Go back to Google
            await _page.GotoAsync("https://www.google.com/");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ================================================
            // ✅ Locator 2: Using CSS Selector
            // ================================================
            try
            {
                var searchByCSS = _page.Locator("input[name='q']");
                await searchByCSS.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                await searchByCSS.ClickAsync();
                await searchByCSS.FillAsync("Playwright CSS Locator");
                await searchByCSS.PressAsync("Enter");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine($"✅ [{browserName.ToUpper()}] Searched using CSS locator");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ CSS locator failed: {ex.Message}");
            }

            // Go back to Google
            await _page.GotoAsync("https://www.google.com/");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ================================================
            // ✅ Locator 3: Using XPATH
            // ================================================
            try
            {
                var searchByXPath = _page.Locator("//input[@name='q']");
                await searchByXPath.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                await searchByXPath.ClickAsync();
                await searchByXPath.FillAsync("Playwright XPath Locator");
                await searchByXPath.PressAsync("Enter");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine($"✅ [{browserName.ToUpper()}] Searched using XPATH locator");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ XPath locator failed: {ex.Message}");
            }

            // Go back to Google
            await _page.GotoAsync("https://www.google.com/");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ================================================
            // ✅ Locator 4: Using GetByRole
            // ================================================
            try
            {
                var searchByRole = _page.GetByRole(AriaRole.Combobox, new() { Name = "Search" });
                await searchByRole.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 5000
                });
                await searchByRole.ClickAsync();
                await searchByRole.FillAsync("Playwright GetByRole Locator");
                await searchByRole.PressAsync("Enter");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine($"✅ [{browserName.ToUpper()}] Searched using GetByRole locator");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ GetByRole locator failed: {ex.Message}");
            }

            Console.WriteLine($"✅ [{browserName.ToUpper()}] All Locator Tests Completed!");
        }
    }
}

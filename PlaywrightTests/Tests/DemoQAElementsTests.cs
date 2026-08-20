// Tests/DemoQAElementsTests.cs
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Base;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class DemoQAElementsTests : BaseTest
    {
        private const string BaseUrl = "https://demoqa.com";

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQA_Elements_TextBox(string browserName)
        {
            await LaunchBrowser(browserName);
            await _page.GotoAsync($"{BaseUrl}/text-box");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to TextBox");

            // ✅ Full Name — Using ID
            await _page.Locator("#userName").FillAsync("John Doe");

            // ✅ Email — Using CSS
            await _page.Locator("input#userEmail").FillAsync("john@example.com");

            // ✅ Current Address — Using Placeholder
            await _page.GetByPlaceholder("Current Address").FillAsync("123 Main Street, NY");

            // ✅ Permanent Address — Using XPath
            await _page.Locator("//textarea[@id='permanentAddress']").FillAsync("456 Park Avenue, LA");

            // ✅ Submit Button — Using CSS
            await _page.Locator("#submit").ClickAsync();

            Console.WriteLine($"✅ [{browserName.ToUpper()}] TextBox Form Submitted!");

            // ✅ Assert Output
            var output = await _page.Locator("#output").IsVisibleAsync();
            Assert.That(output, Is.True, "Output should be visible after submit");
        }

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQA_CheckBox_Complete(string browserName)
        {
            await LaunchBrowser(browserName);
            await _page.GotoAsync("https://demoqa.com/checkbox");
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to CheckBox");

            // ✅ Step 1: Expand the Tree
            var expandBtn = _page.Locator("span.rc-tree-switcher.rc-tree-switcher_close");
            await expandBtn.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await expandBtn.ClickAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Tree Expanded!");

            // ✅ Step 2: Click Home Checkbox
            var homeCheckbox = _page.Locator("span[aria-label='Select Home']");
            await homeCheckbox.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await homeCheckbox.ClickAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Home Checkbox Selected!");

            // ✅ Step 3: Verify Result
            await _page.WaitForSelectorAsync("#result");
            var resultText = await _page.Locator("#result").TextContentAsync();
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Result: {resultText}");

            Assert.That(resultText, Does.Contain("home").IgnoreCase,
                "Result should contain 'home'");

            Console.WriteLine($"✅ [{browserName.ToUpper()}] CheckBox Test Passed!");
        }
        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQA_Elements_RadioButton(string browserName)
        {
            await LaunchBrowser(browserName);
            await _page.GotoAsync($"{BaseUrl}/radio-button");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to RadioButton");

            // ✅ Click Yes Radio Button — Using CSS
            await _page.Locator("label[for='yesRadio']").ClickAsync();
            var yesText = await _page.Locator(".text-success").TextContentAsync();
            Assert.That(yesText, Is.EqualTo("Yes"));
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Yes Radio Selected: {yesText}");

            // ✅ Click Impressive Radio Button — Using XPath
            await _page.Locator("//label[@for='impressiveRadio']").ClickAsync();
            var impressiveText = await _page.Locator(".text-success").TextContentAsync();
            Assert.That(impressiveText, Is.EqualTo("Impressive"));
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Impressive Radio Selected: {impressiveText}");
        }

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQA_Elements_WebTable(string browserName)
        {
            await LaunchBrowser(browserName);
            await _page.GotoAsync($"{BaseUrl}/webtables");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to WebTables");

            // ✅ Click Add Button — Using ID
            await _page.Locator("#addNewRecordButton").ClickAsync();

            // ✅ Fill Form — Using ID Locators
            await _page.Locator("#firstName").FillAsync("John");
            await _page.Locator("#lastName").FillAsync("Doe");
            await _page.Locator("#userEmail").FillAsync("john@test.com");
            await _page.Locator("#age").FillAsync("30");
            await _page.Locator("#salary").FillAsync("50000");
            await _page.Locator("#department").FillAsync("QA");

            // ✅ Submit — Using ID
            await _page.Locator("#submit").ClickAsync();

            Console.WriteLine($"✅ [{browserName.ToUpper()}] WebTable Record Added!");
        }

        [Test]
        [TestCaseSource(nameof(BrowserTestData))]
        public async Task DemoQA_Elements_Buttons(string browserName)
        {
            await LaunchBrowser(browserName);
            await _page.GotoAsync($"{BaseUrl}/buttons");
            Console.WriteLine($"✅ [{browserName.ToUpper()}] Navigated to Buttons");

            // ✅ Double Click — Using ID
            await _page.Locator("#doubleClickBtn").DblClickAsync();
            var doubleClickMsg = await _page.Locator("#doubleClickMessage").TextContentAsync();
            Assert.That(doubleClickMsg, Does.Contain("double click"));
            Console.WriteLine($"✅ Double Click: {doubleClickMsg}");

            // ✅ Right Click — Using ID
            await _page.Locator("#rightClickBtn").ClickAsync(new LocatorClickOptions
            {
                Button = MouseButton.Right
            });
            var rightClickMsg = await _page.Locator("#rightClickMessage").TextContentAsync();
            Assert.That(rightClickMsg, Does.Contain("right click"));
            Console.WriteLine($"✅ Right Click: {rightClickMsg}");

            // ✅ Dynamic Click — Using XPath
            await _page.Locator("//button[text()='Click Me']").ClickAsync();
            var clickMsg = await _page.Locator("#dynamicClickMessage").TextContentAsync();
            Assert.That(clickMsg, Does.Contain("dynamic click"));
            Console.WriteLine($"✅ Dynamic Click: {clickMsg}");
        }
    }
}

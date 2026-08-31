using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTests.Base;

namespace PlaywrightTests.Tests
{
    [TestFixture]
    public class SauceDemoTests : BaseTest
    {
        // ✅ Constants
        // ADD THESE LINES instead (after class opening brace):
        private string BaseUrl => Settings.SauceDemoBaseUrl;
        private string UserName => Settings.SauceDemoUsername;
        private string Password => Settings.SauceDemoPassword;
        private string FirstName => Settings.TestUserFirstName;
        private string LastName => Settings.TestUserLastName;
        private string PostalCode => Settings.TestUserPostalCode;

        // ✅ Store price for comparison
        private string _itemPriceFromInventory = string.Empty;
        private string _itemNameFromInventory = string.Empty;

        // ================================================
        // ✅ MAIN TEST
        // ================================================
        [Test]
        [Description("BDD: SauceDemo — Login, Add to Cart, Checkout & Verify")]
        public async Task SauceDemo_Complete_Checkout_Flow()
        {
            // ✅ GIVEN: Launch Browser
            Console.WriteLine("\n================================================");
            Console.WriteLine("GIVEN: Chrome Browser is Launched");
            Console.WriteLine("================================================");
            await LaunchBrowser("chrome");

            // ✅ WHEN: Navigate to SauceDemo
            Console.WriteLine("\nWHEN: Navigating to SauceDemo");
            await NavigateToSauceDemo();

            // ✅ AND: Login with Demo Credentials
            Console.WriteLine("\nAND: Login with Demo Credentials");
            await LoginWithDemoCredentials();

            // ✅ AND: Select Item & Add to Cart
            Console.WriteLine("\nAND: Select Item & Add to Cart");
            await SelectItemAndAddToCart();

            // ✅ AND: Navigate to Cart & Verify Price
            Console.WriteLine("\nAND: Navigate to Cart & Verify Price");
            await NavigateToCartAndVerifyPrice();

            // ✅ AND: Checkout & Enter Details
            Console.WriteLine("\nAND: Checkout & Enter Details");
            await ClickCheckoutAndEnterDetails();

            // ✅ AND: Verify Item & Price on Checkout Page
            Console.WriteLine("\nAND: Verify Item & Price on Checkout Page");
            await VerifyItemAndPriceOnCheckout();

            // ✅ AND: Click Finish
            Console.WriteLine("\nAND: Click Finish");
            await ClickFinishAndVerify();

            Console.WriteLine("\n================================================");
            Console.WriteLine("✅ ALL STEPS COMPLETED SUCCESSFULLY!");
            Console.WriteLine("================================================");
        }

        // ================================================
        // ✅ Step 1: Navigate to SauceDemo
        // ================================================
        private async Task NavigateToSauceDemo()
        {
            await _page.GotoAsync(BaseUrl);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // ✅ Assert Login Page Loaded
            var loginBtn = _page.Locator("#login-button");
            await loginBtn.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 10000
            });

            Assert.That(
                await loginBtn.IsVisibleAsync(),
                Is.True,
                "Login button should be visible");

            Console.WriteLine($"✅ Navigated to : {_page.Url}");
            Console.WriteLine($"✅ Page Title   : {await _page.TitleAsync()}");
        }

        // ================================================
        // ✅ Step 2: Login with Demo Credentials
        // ================================================
        private async Task LoginWithDemoCredentials()
        {
            // ✅ Read credentials from login page hint
            Console.WriteLine("\n📋 Demo Credentials from Login Page:");
            Console.WriteLine($"   Username : {UserName}");
            Console.WriteLine($"   Password : {Password}");

            // ✅ Enter Username — Using ID
            var usernameField = _page.Locator("#user-name");
            await usernameField.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await usernameField.ClearAsync();
            await usernameField.FillAsync(UserName);
            Console.WriteLine($"✅ Username Entered : {UserName}");

            // ✅ Enter Password — Using ID
            var passwordField = _page.Locator("#password");
            await passwordField.ClearAsync();
            await passwordField.FillAsync(Password);
            Console.WriteLine($"✅ Password Entered : {Password}");

            // ✅ Click Login Button — Using ID
            var loginButton = _page.Locator("#login-button");
            await loginButton.ClickAsync();
            Console.WriteLine("✅ Login Button Clicked!");

            // ✅ Wait for Inventory Page
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await _page.WaitForTimeoutAsync(1000);

            // ✅ Assert Inventory Page Loaded
            Assert.That(
                _page.Url,
                Does.Contain("inventory"),
                "Should navigate to inventory page after login");

            var inventoryTitle = _page.Locator(".title");
            Assert.That(
                await inventoryTitle.TextContentAsync(),
                Is.EqualTo("Products"),
                "Inventory page title should be 'Products'");

            Console.WriteLine($"✅ Login Successful!");
            Console.WriteLine($"✅ Current URL  : {_page.Url}");
            Console.WriteLine($"✅ Page Heading : {await inventoryTitle.TextContentAsync()}");
        }

        // ================================================
        // ✅ Step 3: Select Item & Add to Cart
        // ================================================
        private async Task SelectItemAndAddToCart()
        {
            await _page.WaitForTimeoutAsync(1000);

            // ✅ Get First Item Name
            var firstItemName = _page
                .Locator(".inventory_item_name")
                .First;

            await firstItemName.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

            _itemNameFromInventory = await firstItemName.TextContentAsync() ?? string.Empty;
            Console.WriteLine($"\n📦 Selected Item   : {_itemNameFromInventory}");

            // ✅ Get First Item Price
            var firstItemPrice = _page
                .Locator(".inventory_item_price")
                .First;

            _itemPriceFromInventory = await firstItemPrice.TextContentAsync() ?? string.Empty;
            Console.WriteLine($"💰 Item Price      : {_itemPriceFromInventory}");

            // ✅ Assert Item Name & Price are not empty
            Assert.That(
                _itemNameFromInventory,
                Is.Not.Empty,
                "Item name should not be empty");

            Assert.That(
                _itemPriceFromInventory,
                Is.Not.Empty,
                "Item price should not be empty");

            // ✅ Click Add to Cart Button for First Item
            var addToCartBtn = _page
                .Locator(".btn_inventory")
                .First;

            await addToCartBtn.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

            await addToCartBtn.ClickAsync();
            Console.WriteLine("✅ Add to Cart Clicked!");

            // ✅ Verify Cart Badge Shows 1
            var cartBadge = _page.Locator(".shopping_cart_badge");
            await cartBadge.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

            var cartCount = await cartBadge.TextContentAsync();
            Assert.That(cartCount, Is.EqualTo("1"), "Cart badge should show 1");

            Console.WriteLine($"✅ Cart Badge Count : {cartCount}");
            Console.WriteLine($"✅ Item Added to Cart Successfully!");

            // ✅ Print Summary
            Console.WriteLine("\n================================================");
            Console.WriteLine("📋 INVENTORY PAGE SUMMARY");
            Console.WriteLine("================================================");
            Console.WriteLine($"📦 Item Name  : {_itemNameFromInventory}");
            Console.WriteLine($"💰 Item Price : {_itemPriceFromInventory}");
            Console.WriteLine("================================================");
        }

        // ================================================
        // ✅ Step 4: Navigate to Cart & Verify Price
        // ================================================
        private async Task NavigateToCartAndVerifyPrice()
        {
            // ✅ Click Cart Icon
            var cartIcon = _page.Locator(".shopping_cart_link");
            await cartIcon.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Console.WriteLine("✅ Cart Icon Clicked!");

            // ✅ Assert Cart Page Loaded
            Assert.That(
                _page.Url,
                Does.Contain("cart"),
                "Should navigate to cart page");

            var cartTitle = _page.Locator(".title");
            Assert.That(
                await cartTitle.TextContentAsync(),
                Is.EqualTo("Your Cart"),
                "Cart page title should be 'Your Cart'");

            Console.WriteLine($"✅ Cart Page URL    : {_page.Url}");
            Console.WriteLine($"✅ Cart Page Title  : {await cartTitle.TextContentAsync()}");

            // ✅ Verify Item Name in Cart
            var cartItemName = _page
                .Locator(".inventory_item_name")
                .First;

            var cartItemNameText = await cartItemName.TextContentAsync() ?? string.Empty;

            Assert.That(
                cartItemNameText,
                Is.EqualTo(_itemNameFromInventory),
                $"Cart item name should match inventory item name");

            Console.WriteLine($"✅ Cart Item Name   : {cartItemNameText}");

            // ✅ Verify Price in Cart Matches Inventory Price
            var cartItemPrice = _page
                .Locator(".inventory_item_price")
                .First;

            var cartItemPriceText = await cartItemPrice.TextContentAsync() ?? string.Empty;

            Assert.That(
                cartItemPriceText,
                Is.EqualTo(_itemPriceFromInventory),
                $"Cart price ({cartItemPriceText}) should match " +
                $"inventory price ({_itemPriceFromInventory})");

            Console.WriteLine($"✅ Cart Item Price  : {cartItemPriceText}");

            // ✅ Price Match Verification
            Console.WriteLine("\n================================================");
            Console.WriteLine("💰 CART PRICE VERIFICATION");
            Console.WriteLine("================================================");
            Console.WriteLine($"📦 Item Name              : {cartItemNameText}");
            Console.WriteLine($"💰 Price on Inventory Page: {_itemPriceFromInventory}");
            Console.WriteLine($"💰 Price on Cart Page     : {cartItemPriceText}");
            Console.WriteLine($"🔍 Prices Match           : ✅ YES");
            Console.WriteLine("================================================");
        }

        // ================================================
        // ✅ Step 5: Checkout & Enter Details
        // ================================================
        private async Task ClickCheckoutAndEnterDetails()
        {
            // ✅ Click Checkout Button
            var checkoutBtn = _page.Locator("#checkout");
            await checkoutBtn.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await checkoutBtn.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Console.WriteLine("✅ Checkout Button Clicked!");

            // ✅ Assert Checkout Page Loaded
            Assert.That(
                _page.Url,
                Does.Contain("checkout-step-one"),
                "Should navigate to checkout step one");

            var checkoutTitle = _page.Locator(".title");
            Assert.That(
                await checkoutTitle.TextContentAsync(),
                Is.EqualTo("Checkout: Your Information"),
                "Checkout page title should be correct");

            Console.WriteLine($"✅ Checkout URL     : {_page.Url}");
            Console.WriteLine($"✅ Checkout Title   : {await checkoutTitle.TextContentAsync()}");

            // ✅ Enter First Name — Using ID
            var firstNameField = _page.Locator("#first-name");
            await firstNameField.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await firstNameField.FillAsync(FirstName);
            Console.WriteLine($"✅ First Name Entered : {FirstName}");

            // ✅ Enter Last Name — Using ID
            var lastNameField = _page.Locator("#last-name");
            await lastNameField.FillAsync(LastName);
            Console.WriteLine($"✅ Last Name Entered  : {LastName}");

            // ✅ Enter Postal Code — Using ID
            var postalCodeField = _page.Locator("#postal-code");
            await postalCodeField.FillAsync(PostalCode);
            Console.WriteLine($"✅ Postal Code Entered: {PostalCode}");

            // ✅ Click Continue Button
            var continueBtn = _page.Locator("#continue");
            await continueBtn.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Console.WriteLine("✅ Continue Button Clicked!");

            // ✅ Assert Checkout Step Two Loaded
            Assert.That(
                _page.Url,
                Does.Contain("checkout-step-two"),
                "Should navigate to checkout step two");

            Console.WriteLine($"✅ Checkout Step 2 URL: {_page.Url}");
        }

        // ================================================
        // ✅ Step 6: Verify Item & Price on Checkout Page
        // ================================================
        private async Task VerifyItemAndPriceOnCheckout()
        {
            await _page.WaitForTimeoutAsync(1000);

            // ✅ Assert Checkout Overview Title
            var overviewTitle = _page.Locator(".title");
            Assert.That(
                await overviewTitle.TextContentAsync(),
                Is.EqualTo("Checkout: Overview"),
                "Checkout overview title should be correct");

            Console.WriteLine($"✅ Overview Title   : {await overviewTitle.TextContentAsync()}");

            // ✅ Verify Item Name on Checkout Page
            var checkoutItemName = _page
                .Locator(".inventory_item_name")
                .First;

            var checkoutItemNameText = await checkoutItemName.TextContentAsync() ?? string.Empty;

            Assert.That(
                checkoutItemNameText,
                Is.EqualTo(_itemNameFromInventory),
                $"Checkout item name should match inventory item name");

            Console.WriteLine($"✅ Checkout Item    : {checkoutItemNameText}");

            // ✅ Verify Price on Checkout Page
            var checkoutItemPrice = _page
                .Locator(".inventory_item_price")
                .First;

            var checkoutItemPriceText = await checkoutItemPrice.TextContentAsync() ?? string.Empty;

            Assert.That(
                checkoutItemPriceText,
                Is.EqualTo(_itemPriceFromInventory),
                $"Checkout price ({checkoutItemPriceText}) should match " +
                $"inventory price ({_itemPriceFromInventory})");

            Console.WriteLine($"✅ Checkout Price   : {checkoutItemPriceText}");

            // ✅ Verify Item Total
            var itemTotal = _page.Locator(".summary_subtotal_label");
            var itemTotalText = await itemTotal.TextContentAsync() ?? string.Empty;
            Console.WriteLine($"✅ Item Total       : {itemTotalText}");

            // ✅ Verify Tax
            var tax = _page.Locator(".summary_tax_label");
            var taxText = await tax.TextContentAsync() ?? string.Empty;
            Console.WriteLine($"✅ Tax              : {taxText}");

            // ✅ Verify Total
            var total = _page.Locator(".summary_total_label");
            var totalText = await total.TextContentAsync() ?? string.Empty;
            Console.WriteLine($"✅ Total            : {totalText}");

            // ✅ Print Checkout Summary
            Console.WriteLine("\n================================================");
            Console.WriteLine("💰 CHECKOUT PAGE VERIFICATION");
            Console.WriteLine("================================================");
            Console.WriteLine($"📦 Item Name              : {checkoutItemNameText}");
            Console.WriteLine($"💰 Price on Inventory     : {_itemPriceFromInventory}");
            Console.WriteLine($"💰 Price on Checkout      : {checkoutItemPriceText}");
            Console.WriteLine($"💰 Item Total             : {itemTotalText}");
            Console.WriteLine($"💰 Tax                    : {taxText}");
            Console.WriteLine($"💰 Grand Total            : {totalText}");
            Console.WriteLine($"🔍 Prices Match           : ✅ YES");
            Console.WriteLine("================================================");
        }

        // ================================================
        // ✅ Step 7: Click Finish & Verify
        // ================================================
        private async Task ClickFinishAndVerify()
        {
            // ✅ Click Finish Button
            var finishBtn = _page.Locator("#finish");
            await finishBtn.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });
            await finishBtn.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            Console.WriteLine("✅ Finish Button Clicked!");

            // ✅ Assert Order Complete Page
            Assert.That(
                _page.Url,
                Does.Contain("checkout-complete"),
                "Should navigate to checkout complete page");

            // ✅ Verify Success Message
            var successHeader = _page.Locator(".complete-header");
            await successHeader.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 5000
            });

            var successText = await successHeader.TextContentAsync() ?? string.Empty;

            Assert.That(
                successText,
                Is.EqualTo("Thank you for your order!"),
                "Success message should be 'Thank you for your order!'");

            // ✅ Verify Complete Text
            var completeText = _page.Locator(".complete-text");
            var completeMsg = await completeText.TextContentAsync() ?? string.Empty;

            Console.WriteLine("\n================================================");
            Console.WriteLine("🎉 ORDER COMPLETION SUMMARY");
            Console.WriteLine("================================================");
            Console.WriteLine($"✅ Success Header   : {successText}");
            Console.WriteLine($"✅ Complete Message : {completeMsg}");
            Console.WriteLine($"✅ Final URL        : {_page.Url}");
            Console.WriteLine("================================================");
            Console.WriteLine("🎉 ORDER PLACED SUCCESSFULLY!");
            Console.WriteLine("================================================");
        }
    }
}

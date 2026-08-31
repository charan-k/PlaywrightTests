namespace PlaywrightTests.Configuration
{
    /// <summary>
    /// Strongly-typed configuration settings for 
    /// the Playwright test framework.
    /// Loaded from appsettings.json via IOptions<T>.
    /// </summary>
    public class TestSettings
    {
        public const string SectionName = "TestSettings";

        // URLs
        public string SauceDemoBaseUrl { get; set; } = string.Empty;
        public string DemoQABaseUrl { get; set; } = string.Empty;
        public string DemoQALoginUrl { get; set; } = string.Empty;
        public string DemoQACheckboxUrl { get; set; } = string.Empty;
        public string GoogleBaseUrl { get; set; } = string.Empty;

        // Browser config
        public string Browser { get; set; } = "chromium";
        public string Channel { get; set; } = string.Empty;
        public bool Headless { get; set; } = false;
        public int SlowMo { get; set; } = 0;
        public int ViewportWidth { get; set; } = 1280;
        public int ViewportHeight { get; set; } = 720;

        // Test credentials
        public string SauceDemoUsername { get; set; } = string.Empty;
        public string SauceDemoPassword { get; set; } = string.Empty;

        // Test data
        public string TestUserFirstName { get; set; } = string.Empty;
        public string TestUserLastName { get; set; } = string.Empty;
        public string TestUserPostalCode { get; set; } = string.Empty;

        // Reporting
        public string ReportPath { get; set; } = "./TestResults/HtmlReports/";
    }
}
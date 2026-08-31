Feature: DemoQA Login
  As a QA Engineer
  I want to verify DemoQA login with locators
  So that multiple locator strategies are validated

  Background:
    Given the PlaywrightTests framework is configured

  Scenario: DemoQA login page loads correctly
    Given the DemoQA login page is open
    Then the login form is visible

  Scenario: DemoQA login with invalid credentials fails
    Given the DemoQA login page is open
    When invalid DemoQA credentials are submitted
    Then an error response is shown on DemoQA
Feature: SauceDemo Login
  As a QA Engineer
  I want to verify SauceDemo login scenarios
  So that positive and negative cases are covered

  Background:
    Given the PlaywrightTests framework is configured

  Scenario: Successful login with valid credentials
    Given the SauceDemo login page is open
    When the user enters valid credentials
    Then the user is on the inventory page

  Scenario: Login fails with invalid credentials
    Given the SauceDemo login page is open
    When the user enters invalid credentials "wrong_user" and "wrong_pass"
    Then the error message "Username and password do not match" is displayed

  Scenario: Login fails when username is empty
    Given the SauceDemo login page is open
    When the username field is left empty
    Then the error message "Username is required" is displayed

  Scenario: Login fails for locked out user
    Given the SauceDemo login page is open
    When the user enters locked out user credentials
    Then the error message "Sorry, this user has been locked out" is displayed
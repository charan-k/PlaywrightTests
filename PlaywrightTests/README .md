## Description
Automated end-to-end testing project using 
Playwright for .NET with C#. This project contains 
UI automation tests for web applications built 
using the Playwright testing framework.

## Tech Stack
- Language: C#
- Runtime: .NET 8.0
- Framework: Playwright for .NET
- Build Tool: dotnet CLI / MSBuild
- Test Framework: NUnit with Playwright
- IDE: Visual Studio

## Project Structure
- PlaywrightTests/ - Main test project folder
- PlaywrightTests.slnx - Visual Studio solution file

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or later

### Installation
1. Clone the repository
git clone https://github.com/charan-k/PlaywrightTests

2. Navigate to project folder
cd PlaywrightTests

3. Restore dependencies
dotnet restore

4. Install Playwright browsers
pwsh bin/Debug/net8.0/playwright.ps1 install

### Running Tests
dotnet test

## Author
- Charan Kumar
- Organization: EPAM

## GitHub Repository
https://github.com/charan-k/PlaywrightTests
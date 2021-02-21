using System;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;

namespace ContactBook_AndroidAppTests
{
    public class WinAppAppiumTestsContactBook
    {
        const string AppForTesting = @"C:\Trash\ContactBook-DesktopClient\ContactBook-DesktopClient.exe";
        const string ApiServiceUrl = "https://contactbook.nakov.repl.co/api";

        private AppiumLocalService appiumLocalService;
        private WindowsDriver<WindowsElement> driver;

        [OneTimeSetUp]
        public void SetupLocalService()
        {
            // Start the Appium server as local Node.js app
            appiumLocalService = new AppiumServiceBuilder().UsingAnyFreePort().Build();
            appiumLocalService.Start();

            var appiumOptions = new AppiumOptions() { PlatformName = "Windows" };
            appiumOptions.AddAdditionalCapability("app", AppForTesting);
            driver = new WindowsDriver<WindowsElement>(appiumLocalService, appiumOptions);
        }

        [Test]
        public void Test_WindowsApp_SearchContacts()
        {
            // Connect to the RESTful service
            var textBoxApiUrl = driver.FindElementByAccessibilityId("textBoxApiUrl");
            textBoxApiUrl.Clear();
            textBoxApiUrl.SendKeys(ApiServiceUrl);

            var buttonConnect = driver.FindElementByAccessibilityId("buttonConnect");
            buttonConnect.Click();

            // Switch to the new active window (which have just changed)
            var activeWindow = driver.WindowHandles[0];
            driver.SwitchTo().Window(activeWindow);

            // Search for "steve"
            var textBoxSearch = driver.FindElementByAccessibilityId("textBoxSearch");
            textBoxSearch.Clear();
            textBoxSearch.SendKeys("steve");

            var buttonSearch = driver.FindElementByAccessibilityId("buttonSearch");
            buttonSearch.Click();

            // Wait until the search results appear
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => { 
                return driver.FindElementByAccessibilityId("labelResult")
                    .Text.Contains("Contacts found:");
            });

            // Assert that the first contact in the table is "Steve Jobs"
            var dataGridViewContacts =
                driver.FindElementByAccessibilityId("dataGridViewContacts");
            var cellFirstName = dataGridViewContacts.FindElementByXPath(
                "//Edit[@Name='FirstName Row 0, Not sorted.']");
            Assert.AreEqual("Steve", cellFirstName.Text);

            var cellLastName = dataGridViewContacts.FindElementByXPath(
                "//Edit[@Name='LastName Row 0, Not sorted.']");
            Assert.AreEqual("Jobs", cellLastName.Text);
        }

        [OneTimeTearDown]
        public void ShutDown()
        {
            driver.Quit();
            appiumLocalService?.Dispose();
        }
    }
}
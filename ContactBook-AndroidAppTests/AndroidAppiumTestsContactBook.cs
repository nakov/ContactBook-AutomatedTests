using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Service;
using System;

namespace ContactBook_AndroidAppTests
{
    public class AndroidAppiumTestsContactBook
    {
        const string AppForTesting = @"C:\Trash\contactbook-androidclient.apk";
        const string ApiServiceUrl = "https://contactbook.nakov.repl.co/api";

        private AppiumLocalService appiumLocalService;
        private AndroidDriver<AndroidElement> driver;

        [OneTimeSetUp]
        public void SetupLocalService()
        {
            // Start the Appium server as local Node.js app
            //appiumLocalService = new AppiumServiceBuilder().UsingAnyFreePort().Build();
            //appiumLocalService.Start();

            var appiumOptions = new AppiumOptions() { PlatformName = "Android" };
            appiumOptions.AddAdditionalCapability("app", AppForTesting);
            driver = new AndroidDriver<AndroidElement>(
                new Uri("http://[::1]:4723/wd/hub"), appiumOptions);

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }

        [Test]
        public void Test_AndroidApp_SearchContacts()
        {
            // Connect to the RESTful service
            var editTextApiUrl = driver.FindElementById(
                "contactbook.androidclient:id/editTextApiUrl");
            editTextApiUrl.Clear();
            editTextApiUrl.SendKeys(ApiServiceUrl);

            var buttonConnect = driver.FindElementById(
                "contactbook.androidclient:id/buttonConnect");
            buttonConnect.Click();

            // Search for "steve"
            var editTextKeyword = driver.FindElementById(
                "contactbook.androidclient:id/editTextKeyword");
            editTextKeyword.Clear();
            editTextKeyword.SendKeys("steve");

            var buttonSearch = driver.FindElementById(
                "contactbook.androidclient:id/buttonSearch");
            buttonSearch.Click();

            // Assert that one or several contacts are displayed
            var textViewSearchResult = driver.FindElementById(
                "contactbook.androidclient:id/textViewSearchResult");
            StringAssert.Contains("Contacts found:", textViewSearchResult.Text);

            // Assert that the first contact in the list is "Steve Jobs"
            var textViewFirstName = driver.FindElementById(
                "contactbook.androidclient:id/textViewFirstName");
            Assert.AreEqual("Steve", textViewFirstName.Text);

            var textViewLastName = driver.FindElementById(
                "contactbook.androidclient:id/textViewLastName");
            Assert.AreEqual("Jobs", textViewLastName.Text);
        }

        [OneTimeTearDown]
        public void ShutDown()
        {
            driver.Quit();
            appiumLocalService?.Dispose();
        }
    }
}
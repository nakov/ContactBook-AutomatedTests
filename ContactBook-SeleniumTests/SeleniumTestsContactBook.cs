using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace ContactBook_SeleniumTests
{
    public class SeleniumTestsContactBook
    {
        const string AppBaseUrl = "https://contactbook.nakov.repl.co";
        RemoteWebDriver driver;

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [Test]
        public void Test_ListContacts()
        {
            // Arrange
            string contactsUrl = AppBaseUrl + "/contacts";

            // Act
            driver.Navigate().GoToUrl(contactsUrl); 
            
            // Assert
            var textBoxFirstName = driver.FindElement(
                By.CssSelector("table tr.fname > td"));
            Assert.AreEqual("Steve", textBoxFirstName.Text);
            var textBoxLastName = driver.FindElement(
                By.CssSelector("table tr.lname > td"));
            Assert.AreEqual("Jobs", textBoxLastName.Text);
        }

        [Test]
        public void Test_FindContactByKeyword_ValidResults()
        {
            // Arrange
            string keyword = "albert";
            string searchUrl = AppBaseUrl + "/contacts/search";

            // Act
            driver.Navigate().GoToUrl(searchUrl);
            var keywordTextBox = driver.FindElementByCssSelector("input#keyword");
            keywordTextBox.SendKeys(keyword);
            var buttonSeàrch = driver.FindElementByCssSelector("button#search");
            buttonSeàrch.Click();

            // Assert
            var searchResultsDiv = driver.FindElement(
                By.CssSelector("main > div"));
            StringAssert.Contains("contacts found", searchResultsDiv.Text);
            var textBoxFirstName = driver.FindElement(
                By.CssSelector("table tr.fname > td"));
            Assert.AreEqual("Albert", textBoxFirstName.Text);
            var textBoxLastName = driver.FindElement(
                By.CssSelector("table tr.lname > td"));
            Assert.AreEqual("Einstein", textBoxLastName.Text);
        }

        [Test]
        public void Test_FindContactByKeyword_NoResults()
        {
            // Arrange
            string keyword = "missing" + DateTime.Now.Ticks;
            string searchUrl = AppBaseUrl + "/contacts/search";

            // Act
            driver.Navigate().GoToUrl(searchUrl);
            var keywordTextBox = driver.FindElementByCssSelector("input#keyword");
            keywordTextBox.SendKeys(keyword);
            var buttonSeàrch = driver.FindElementByCssSelector("button#search");
            buttonSeàrch.Click();

            // Assert
            var searchResultsDiv = driver.FindElement(
                By.CssSelector("main > div"));
            Assert.AreEqual("No contacts found.", searchResultsDiv.Text);
        }

        [Test]
        public void Test_CreateContact_InvalidData()
        {
            // Arrange
            string firstName = "Maria";
            string lastName = "Green";
            string createContactUrl = AppBaseUrl + "/contacts/create";

            // Act
            driver.Navigate().GoToUrl(createContactUrl);
            var textBoxFirstName = driver.FindElementByCssSelector("input#firstName");
            textBoxFirstName.SendKeys(firstName);
            var textBoxLastName = driver.FindElementByCssSelector("input#lastName");
            textBoxLastName.SendKeys(lastName);
            var buttonCreate = driver.FindElementByCssSelector("button#create");
            buttonCreate.Click();

            // Assert
            var errDiv = driver.FindElement(By.CssSelector("div.err"));
            Assert.AreEqual("Error: Invalid email!", errDiv.Text);
        }

        [Test]
        public void Test_CreateContact_ValidData()
        {
            // Arrange
            string firstName = "Maria";
            string lastName = "Green";
            string email = "mgreen@yahoo.com";
            string phone = "+359 888 888";
            string comments = "my friend";
            string createContactUrl = AppBaseUrl + "/contacts/create";

            // Act
            driver.Navigate().GoToUrl(createContactUrl);

            var textBoxFirstName = driver.FindElementByCssSelector("input#firstName");
            textBoxFirstName.SendKeys(firstName);

            var textBoxLastName = driver.FindElementByCssSelector("input#lastName");
            textBoxLastName.SendKeys(lastName);

            var textBoxEmail = driver.FindElementByCssSelector("input#email");
            textBoxEmail.SendKeys(email);

            var textBoxPhone = driver.FindElementByCssSelector("input#phone");
            textBoxPhone.SendKeys(phone);

            var textBoxComments = driver.FindElementByCssSelector("textarea#comments");
            textBoxComments.SendKeys(comments);

            var buttonCreate = driver.FindElementByCssSelector("button#create");
            buttonCreate.Click();

            // Assert
            var pageHeading = driver.FindElementByCssSelector("header.top h1");
            Assert.AreEqual("View Contacts", pageHeading.Text);

            var contactTables = driver.FindElements(By.CssSelector("table.contact-entry"));
            var lastContactTable = contactTables[contactTables.Count - 1];
            var textFieldFirstName = lastContactTable.FindElement(
                By.CssSelector("tr.fname td"));
            Assert.AreEqual(firstName, textFieldFirstName.Text);

            var textFieldLastName = lastContactTable.FindElement(
                By.CssSelector("tr.lname td"));
            Assert.AreEqual(lastName, textFieldLastName.Text);

            var textFieldEmail = lastContactTable.FindElement(
                By.CssSelector("tr.email td"));
            Assert.AreEqual(email, textFieldEmail.Text);

            var textFieldPhone = lastContactTable.FindElement(
                By.CssSelector("tr.phone td"));
            Assert.AreEqual(phone, textFieldPhone.Text);

            var textFieldComments = lastContactTable.FindElement(
                By.CssSelector("tr.comments td"));
            Assert.AreEqual(comments, textFieldComments.Text);
        }

        [OneTimeTearDown]
        public void ShutDown()
        {
            driver.Quit();
        }
    }
}

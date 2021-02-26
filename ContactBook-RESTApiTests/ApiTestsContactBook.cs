using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;

namespace ContactBook_SeleniumTests
{
    public class ApiTestsContactBook
    {
        const string ApiBaseUrl = "https://contactbook.nakov.repl.co/api";

        private RestClient client = new RestClient(ApiBaseUrl) { Timeout = 3000 };

        [Test]
        public void Test_ListContacts()
        {
            // Arrange
            string contactsUrl = ApiBaseUrl + "/contacts";
            var request = new RestRequest(contactsUrl, Method.GET);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contacts = new JsonDeserializer()
                .Deserialize<List<ContactResponse>>(response);
            Assert.IsTrue(contacts.Count > 0);
            Assert.IsTrue(contacts[0].id > 0);
            Assert.AreEqual(contacts[0].firstName, "Steve");
            Assert.AreEqual(contacts[0].lastName, "Jobs");
        }

        [Test]
        public void Test_FindContactByKeyword_ValidResults()
        {
            // Arrange
            string keyword = "albert";
            string searchUrl = ApiBaseUrl + "/contacts/search/{keyword}";
            var request = new RestRequest(searchUrl, Method.GET);
            request.AddUrlSegment("keyword", keyword);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contacts = new JsonDeserializer()
                .Deserialize<List<ContactResponse>>(response);
            Assert.IsTrue(contacts.Count > 0);
            var firstContact = contacts[0];
            Assert.AreEqual("Albert", firstContact.firstName);
            Assert.AreEqual("Einstein", firstContact.lastName);
        }

        [Test]
        public void Test_FindContactByKeyword_NoResults()
        {
            // Arrange
            string keyword = "missing" + DateTime.Now.Ticks;
            string searchUrl = ApiBaseUrl + "/contacts/search/{keyword}";
            var request = new RestRequest(searchUrl, Method.GET);
            request.AddUrlSegment("keyword", keyword);

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var contacts = new JsonDeserializer()
                .Deserialize<List<ContactResponse>>(response);
            Assert.IsTrue(contacts.Count == 0);
        }

        [Test]
        public void Test_CreateContact_InvalidData()
        {
            // Arrange
            string firstName = "Maria";
            string lastName = "Green";
            string email = "invalid email";
            string createContactUrl = ApiBaseUrl + "/contacts";
            var request = new RestRequest(createContactUrl, Method.POST);
            request.AddJsonBody(new { firstName, lastName, email });

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
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
            string createContactUrl = ApiBaseUrl + "/contacts";
            var request = new RestRequest(createContactUrl, Method.POST);
            request.AddJsonBody(
                new { firstName, lastName, email, phone, comments });

            // Act
            var response = client.Execute(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            string contactsUrl = ApiBaseUrl + "/contacts";
            var contactsRequest = new RestRequest(contactsUrl, Method.GET);
            var contactsResponse = client.Execute(contactsRequest);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, contactsResponse.StatusCode);
            var contacts = new JsonDeserializer()
                .Deserialize<List<ContactResponse>>(contactsResponse);
            var lastContact = contacts[contacts.Count - 1];

            Assert.IsTrue(lastContact.id > 0);
            Assert.AreEqual(firstName, lastContact.firstName);
            Assert.AreEqual(lastName, lastContact.lastName);
            Assert.AreEqual(email, lastContact.email);
            Assert.AreEqual(phone, lastContact.phone);
            Assert.AreEqual(comments, lastContact.comments);
        }
    }
}

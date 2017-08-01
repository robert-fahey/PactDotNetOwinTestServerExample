using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using PactNet.Mocks.MockHttpService.Models;
using SampleApp.Infrastructure.Clients;
using SampleApp.Infrastructure.Models;

namespace SampleApp.ConsoleApp.PACT.Consumer.Tests
{
    public class DateFormatterPactTests
    {
        private DateFormatterWebApiConsumerPact _pactData;
        private HttpClient _httpClient;
        private DateApiClient _dateApiClient;

        [SetUp]
        public void Setup()
        {
            _pactData = new DateFormatterWebApiConsumerPact();
            _pactData.MockProviderService.ClearInteractions();
                
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(_pactData.MockProviderServiceBaseUri)
            };            
        }

        [TearDown]
        public void TearDown()
        {
            _pactData?.Dispose();
        }

        [Test]
        public async Task GetFormattedDate_ShouldReturnFoundResult()
        {
            //Arrange
            var expectedResult = new FormattedDateModel
            {
                FormattedDate = "20 March 2017 12:00:01",
                Date = DateTime.Parse("2017-03-20T12:00:01.00Z"),
                Language = "en-GB"
            };

            var testAuthToken = "SomeValidAuthToken";

            _pactData.MockProviderService
                .Given("A utc date '2017-03-20T12:00:01.00Z' and language 'en-GB'")
                .UponReceiving("A GET date formatting request")
                .With(RequestBuilder.Request(HttpVerb.Get, "/api/dates/formattedDate")
                    .WithHeader("Authorization", $"Bearer {testAuthToken}")
                    .WithQueryParameter("date", "2017-03-20T12:00:01.00Z")
                    .WithQueryParameter("lang", "en-GB")
                    .Build())
                .WillRespondWith(new ProviderServiceResponse
                    {
                        Headers = new Dictionary<string, object>
                        {
                            {$"Content-Type", "application/json; charset=utf-8"}
                        },
                        Status = (int) HttpStatusCode.OK,
                        Body = expectedResult
                    });

            _dateApiClient = new DateApiClient(_httpClient, testAuthToken);

            //Act

            var result = await _dateApiClient.GetFormatedDate("2017-03-20T12:00:01.00Z", "en-GB");

            //Assert
            result.FormattedDate.Should().Be(expectedResult.FormattedDate);
            result.Date.Should().Be(expectedResult.Date);
            result.Language.Should().Be(expectedResult.Language);
            
            _pactData.MockProviderService.VerifyInteractions();

        }

        [Test]
        public void GetFormattedDate_NoAuthorisationShouldReturnUnauthorised()
        {
            //Arrange
            
            _pactData.MockProviderService
                .Given("A utc date '2017-03-20T12:00:01.00Z' and language 'en-GB'")
                .UponReceiving("A GET date formatting request")
                .With(RequestBuilder.Request(HttpVerb.Get, "/api/dates/formattedDate")
                    
                    .WithQueryParameter("date", "2017-03-20T12:00:01.00Z")
                    .WithQueryParameter("lang", "en-GB")
                    .Build())
                 .WillRespondWith(new ProviderServiceResponse
                 {
                     Status = 401,
                     Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                     Body = new
                     {
                         message = "Authorization has been denied for this request."
                     }
                 });

            _dateApiClient = new DateApiClient(_httpClient);

            

            //Act //Assert
            Assert.Throws<AggregateException>(() => _dateApiClient.GetFormatedDate("2017-03-20T12:00:01.00Z", "en-GB").Wait());

            _pactData.MockProviderService.VerifyInteractions();

        }
    }
}
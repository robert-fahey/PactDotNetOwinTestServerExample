using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using SampleApp.Infrastructure.Models;
using TechTalk.SpecFlow;

namespace SampleApp.Specs.StepDefinitions
{
    [Binding]
    public class GetFormattedDateSteps
    {
        private readonly HttpClient _httpClient;
        private readonly List<HttpResponseMessage> _responseHistory;

        public GetFormattedDateSteps(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _responseHistory = new List<HttpResponseMessage>();
        }

        [BeforeScenario("Authorised", Order = 2)]
        public void AddAuthorisation()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer SomeValidAuthToken");
        }

        [When(@"I request the formatted date '(.*)' for language '(.*)'")]
        public void WhenIRequestTheFormattedDateForLanguage(string date, string language)
        {
            var response = _httpClient.GetAsync($"api/dates/formattedDate?date={date}&lang={language}").Result;

            _responseHistory.Add(response);
        }

        [Then(@"the response status code should be '(.*)'")]
        public void ThenTheResponseStatusCodeShouldBe(HttpStatusCode statusCode)
        {
            _responseHistory.Should().NotBeEmpty();
            var lastResponse = _responseHistory.Last();
            lastResponse.StatusCode.Should().Be(statusCode);
        }

        [Then(@"the response formatted date should be '(.*)'")]
        public void ThenTheResponseFormattedDateShouldBe(string formattedDate)
        {
            var model = GetLastResponseContents<FormattedDateModel>();
            model.FormattedDate.Should().Be(formattedDate);
        }

        [Then(@"the response formatted date should be")]
        public void ThenTheResponseFormattedDateShouldBe(FormattedDateModel expectedFormattedDateModel)
        {
            var model = GetLastResponseContents<FormattedDateModel>();
            model.ShouldBeEquivalentTo(expectedFormattedDateModel);
        }

        private T GetLastResponseContents<T>()
        {
            var lastResponse = GetLastResponseMessage();
            return lastResponse.Content.ReadAsAsync<T>().Result;
        }

        private HttpResponseMessage GetLastResponseMessage()
        {
            _responseHistory.Should().NotBeEmpty();
            var lastResponse = _responseHistory.Last();
            return lastResponse;
        }
    }
}

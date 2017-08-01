using System;
using System.Configuration;
using System.Net.Http;
using SampleApp.Infrastructure.Clients;

namespace SampleApp.ConsoleApp
{
    public class CompositionRoot
    {
        private static readonly Func<string, string> GetConnectionString =
            (name) => ConfigurationManager.AppSettings[name];

        private HttpClient _httpClient;

        public DateApiClient GetDateApiClient()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(GetConnectionString("DateApiService"))
            };

            return new DateApiClient(_httpClient, "SomeValidAuthToken");
        }
    }
}
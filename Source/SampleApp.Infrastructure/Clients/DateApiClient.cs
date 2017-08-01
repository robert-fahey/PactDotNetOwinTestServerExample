using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SampleApp.Infrastructure.Models;

namespace SampleApp.Infrastructure.Clients
{
    public class DateApiClient
    {
        private readonly HttpClient _httpClient;

        public Uri BaseUri { get; set; }

        public DateApiClient(HttpClient client, string authToken = null)
        {
            _httpClient = client;
            
            //Workaround for singleton httpclient when dns changes - see http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
            var sp = ServicePointManager.FindServicePoint(_httpClient.BaseAddress);
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute

            if (authToken != null)
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
            }
        }

        public async Task<FormattedDateModel> GetFormatedDate(string date, string language)
        {
            var path = $"api/dates/formattedDate?date={date}&lang={language}";
            
            var request = new HttpRequestMessage(HttpMethod.Get, path) { };
            
            var response = _httpClient.SendAsync(request);
            
            try
            {                
                var result = response.Result;

                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<FormattedDateModel>(data);
                }

                RaiseResponseError(request, result);
            }
            finally
            {
                Dispose(request, response);
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(_httpClient);
        }

        public void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables.Where(d => d != null))
            {
                disposable.Dispose();
            }
        }

        private static void RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            throw new HttpRequestException(
                $"The Events API request for {failedRequest.Method.ToString().ToUpperInvariant()} " +
                $"{failedRequest.RequestUri} failed. Response Status: {(int) failedResponse.StatusCode}, " +
                $"Response Body: {failedResponse.Content.ReadAsStringAsync().Result}");
        }
    }
}
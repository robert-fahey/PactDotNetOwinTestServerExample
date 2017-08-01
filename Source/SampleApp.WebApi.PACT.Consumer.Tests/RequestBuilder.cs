using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace SampleApp.ConsoleApp.PACT.Consumer.Tests
{
    public class RequestBuilder
    {
        private readonly IDictionary<string, object> _headers;
        private readonly Dictionary<string, string> _queryParameters;
        private readonly ProviderServiceRequest _serviceRequest;

        private RequestBuilder(ProviderServiceRequest serviceRequest)
        {
            _headers = new Dictionary<string, object>();
            _queryParameters = new Dictionary<string, string>();
            _serviceRequest = serviceRequest;
        }

        public static RequestBuilder Request(HttpVerb method, string path)
        {
            return new RequestBuilder(new ProviderServiceRequest
            {
                Method = method,
                Path = path
            });
        }

        public RequestBuilder WithHeader(string key, string value)
        {
            _headers.Add(key, value);
            return this;
        }

        public RequestBuilder WithQueryParameter(string key, string value)
        {
            _queryParameters.Add(key, value);
            return this;
        }

        public ProviderServiceRequest Build()
        {
            _serviceRequest.Headers = _headers;
            string query = "";
            if (_queryParameters.Count > 0)
            {
                foreach (var queryKey in _queryParameters.Keys)
                {
                    query += (query == "" ? "" : "&") + queryKey + "=" + Uri.EscapeUriString(_queryParameters[queryKey]);
                }
                _serviceRequest.Query = query;
            }

            return _serviceRequest;
        }

    }
}
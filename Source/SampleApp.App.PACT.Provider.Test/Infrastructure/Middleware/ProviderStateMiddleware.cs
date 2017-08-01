using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.Middleware
{
    public class ProviderState
    {
        public string Consumer { get; set; }
        public string State { get; set; }
    }

    public class ProviderStateMiddleware
    {
        private const string ConsumerName = "SampleApp.ConsoleApp";
        private readonly Func<IDictionary<string, object>, Task> _mNext;
        private readonly IDictionary<string, Action> _providerStates;
        
        public ProviderStateMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _mNext = next;
            _providerStates = new Dictionary<string, Action>
            {
                {
                    "A utc date '2017-03-20T12:00:01.00Z' and language 'en-GB'",
                    AddTesterIfItDoesntExist
                }
            };
        }

        private void AddTesterIfItDoesntExist()
        {
            //Add code to go an inject or insert the tester data
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            if (context.Request.Path.Value == "/provider-states")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                if (context.Request.Method == HttpMethod.Post.ToString() &&
                    context.Request.Body != null)
                {
                    string jsonRequestBody;
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        jsonRequestBody = reader.ReadToEnd();
                    }

                    var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                    //A null or empty provider state key must be handled
                    if (!string.IsNullOrEmpty(providerState?.State) && providerState.Consumer == ConsumerName)
                    {
                        _providerStates[providerState.State].Invoke();
                    }

                    await context.Response.WriteAsync("");
                }
            }
            else
            {
                await _mNext.Invoke(environment);
            }
        }
    }
}
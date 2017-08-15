using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.ProviderState;

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
        
        public ProviderStateMiddleware(Func<IDictionary<string, object>, Task> next, IProviderStateManager providerStateManager)
        {
            _mNext = next;
            _providerStates = providerStateManager.GetStates();        
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
                        try
                        {
                            _providerStates[providerState.State].Invoke();
                        }
                        catch
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            var message =
                                $"{{\"Message\":\"Provider state not found\", \"ProviderState\": \"{providerState.State}\" }}";
                            await context.Response.WriteAsync(message);
                        }
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
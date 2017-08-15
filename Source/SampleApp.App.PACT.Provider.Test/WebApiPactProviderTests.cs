using System;
using System.IO;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using NUnit.Framework;
using Owin;
using PactNet;
using SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.Middleware;
using SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.ProviderState;
using SampleApp.Infrastructure.Auth;
using SampleApp.WebApi;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests
{
    public class WebApiPactProviderTests
    {
        const string ServiceUri = "http://localhost:9222";
        private readonly IProviderStateManager _providerStateManager = new ProviderStateManager();

        private IDisposable StartServer(IProviderStateManager providerStateManager)
        {
            return WebApp.Start(ServiceUri, (appBuilder) =>
            {
                var startup = new Startup();

                appBuilder.Use(typeof(ProviderStateMiddleware), providerStateManager);
                appBuilder.Use(typeof(AuthorizationTokenReplacementMiddleware),
                    appBuilder.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace, "Access_Token", "v1"));
                startup.Configuration(appBuilder);
            });
        }
        
        [SetUp]
        public void Setup()
        {
            //Nunit resharper runner hack
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Test]
        public void EnsureSomethingApiHonoursPactWithConsumer()
        {
            //Arrange
            var config = new PactVerifierConfig();
            _providerStateManager.AddState(
                "A utc date '2017-03-20T12:00:01.00Z' and language 'en-GB'",
                () => DateTime.Parse("2017-03-20T12:00:01.00Z"));
            
            using (StartServer(_providerStateManager))
            {
                //Act //Assert
                IPactVerifier pactVerifier = new PactVerifier(config);

                pactVerifier
                    .ProviderState($"{ServiceUri}/provider-states")
                    .ServiceProvider("DateFormatter.WebApi", ServiceUri)
                    .HonoursPactWith("SampleApp.ConsoleApp")
                    .PactUri($"C:/temp/PACT/sampleapp.consoleapp-dateformatter.webapi.json")
                    .Verify();
            }
        }
    }
}
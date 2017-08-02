using System;
using System.Net.Http;
using BoDi;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using SampleApp.Infrastructure.Clients;
using SampleApp.WebApi;
using TechTalk.SpecFlow;

[assembly: Parallelizable(ParallelScope.Fixtures)]
namespace SampleApp.Specs.Configuration
{
    [Binding]

    public class ConfigurationBinding
    {
        private readonly IObjectContainer _objectContainer;

        public ConfigurationBinding(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario(Order = 0)]
        public void InitializeDependencies()
        {
            var testServer = CreateTestServer();
            _objectContainer.RegisterInstanceAs(testServer);
            _objectContainer.RegisterInstanceAs(testServer.HttpClient);
        }

        [AfterScenario]
        public void CleanDependencies()
        {
            var testServer = _objectContainer.Resolve<TestServer>();
            testServer.Dispose();
        }

        private TestServer CreateTestServer()
        {
            var testServer = TestServer.Create(appBuilder =>
            {
                var startup = new Startup();
                startup.Configuration(appBuilder);
            });
            testServer.BaseAddress = new Uri("http://sampleapp.localhost");
            return testServer;
        }
    }
}

using System;
using BoDi;
using Microsoft.Owin.Testing;
using SampleApp.WebApi;
using TechTalk.SpecFlow;

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

        [BeforeScenario( Order = 0)]
        public void InitializeDependencies()
        {
            var testServer = CreateTestServer();
            _objectContainer.RegisterInstanceAs(testServer);
            _objectContainer.RegisterInstanceAs(testServer.HttpClient );
        }

        [AfterScenario]
        public void CleanDependencies()
        {
            var testServer = _objectContainer.Resolve<TestServer>();
            testServer.Dispose();
        }

        private static TestServer CreateTestServer()
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

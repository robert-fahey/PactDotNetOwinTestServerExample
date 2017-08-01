using System.IO;
using Microsoft.Owin.Hosting;
using NUnit.Framework;
using PactNet;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests
{
    public class WebApiPactProviderTests
    {
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

            const string serviceUri = "http://localhost:9222";


            using (WebApp.Start<TestServerApi>(serviceUri))
            {
                //Act //Assert
                IPactVerifier pactVerifier = new PactVerifier(config);

                pactVerifier
                    .ProviderState($"{serviceUri}/provider-states")
                    .ServiceProvider("DateFormatter.WebApi", serviceUri)
                    .HonoursPactWith("SampleApp.ConsoleApp")
                    .PactUri($"C:/temp/PACT/sampleapp.consoleapp-dateformatter.webapi.json")
                    .Verify();
            }
        }
    }
}
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace SampleApp.ConsoleApp.PACT.Consumer.Tests
{
    public class DateFormatterWebApiConsumerPact
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort => 1236;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public DateFormatterWebApiConsumerPact()
        {
            string pactDir = System.Text.RegularExpressions.Regex.Replace(ConfigurationManager.AppSettings["DateFormatter.WebApiPact.Dir"], @"^.\\", TestContext.CurrentContext.WorkDirectory + "/");
            string logDir = System.Text.RegularExpressions.Regex.Replace(ConfigurationManager.AppSettings["DateFormatter.WebApiPact.LogDir"], @"^.\\", TestContext.CurrentContext.WorkDirectory + "/");

            //Nuint resharper runner hack
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

            PactBuilder = new PactBuilder(new PactConfig { PactDir = pactDir, LogDir = logDir, SpecificationVersion = "2.0.0" });

            PactBuilder
                .ServiceConsumer("SampleApp.ConsoleApp")
                .HasPactWith("DateFormatter.WebApi");

            MockProviderService = PactBuilder.MockService(MockServerPort, new JsonSerializerSettings());

        }

        public void Dispose()
        {
            PactBuilder.Build(); //Will save the pact file once finished
        }
    }
}
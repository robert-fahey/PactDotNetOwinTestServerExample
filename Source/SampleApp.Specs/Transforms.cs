using Newtonsoft.Json;
using SampleApp.Infrastructure.Models;
using TechTalk.SpecFlow;

namespace SampleApp.Specs
{
    [Binding]
    public class Transforms
    {
        [StepArgumentTransformation]
        public FormattedDateModel StringToFormattedDateModel(string value)
        {
            return JsonConvert.DeserializeObject<FormattedDateModel>(value);
        }
    }
}

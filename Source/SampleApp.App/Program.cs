using System.Threading.Tasks;

namespace SampleApp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var compositionRoot = new CompositionRoot();

            var client = compositionRoot.GetDateApiClient();
            
            var task = client.GetFormatedDate("2017-03-20T12:00:01.00Z", "en-GB");

            task.Wait();

            System.Console.WriteLine(task.Result.FormattedDate);

        }
    }
}
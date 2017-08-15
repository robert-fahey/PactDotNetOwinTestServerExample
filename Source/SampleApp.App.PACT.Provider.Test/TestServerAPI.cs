using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SampleApp.ConsoleApp.PACT.Provider.Tests.Infrastructure.Middleware;
using SampleApp.Infrastructure.Auth;
using SampleApp.WebApi;

namespace SampleApp.ConsoleApp.PACT.Provider.Tests
{
    public class TestServerApi
    {        
        public void Configuration(IAppBuilder app)
        {
            var startup = new Startup();
            app.Use<ProviderStateMiddleware>();
            app.Use(typeof(AuthorizationTokenReplacementMiddleware),
               app.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace, "Access_Token", "v1"));
            startup.Configuration(app);          
        }
    }
}

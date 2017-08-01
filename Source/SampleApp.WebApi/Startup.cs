using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using SampleApp.Infrastructure.Auth;
using SampleApp.WebApi;
using SampleApp.WebApi.Infrastructure.Swagger;
using Swashbuckle.Application;

[assembly: OwinStartup("ApiConfiguration", typeof(Startup))]
namespace SampleApp.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            // Owin Middleware; we use token middleware for requests that require authorization.
            
            app.Use(typeof(AuthorizationTokenReplacementMiddleware),
            app.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace, "Access_Token", "v1"));

            var oAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            app.UseOAuthBearerAuthentication(oAuthBearerOptions);
 

            config.MapHttpAttributeRoutes();

            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Date localizer");
                    c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
                })
                .EnableSwaggerUi();

            app.UseWebApi(config);
        }
    }
}
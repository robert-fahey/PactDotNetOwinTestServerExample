using System;
using System.Globalization;
using System.Net;
using System.Web.Http;
using SampleApp.Infrastructure.Models;
using Swashbuckle.Swagger.Annotations;

namespace SampleApp.WebApi.Controllers
{
    [RoutePrefix("api")]
    public class DatesController : ApiController
    {
        [Authorize]
        [Route("dates/formattedDate")]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public IHttpActionResult GetFormattedDate(string date, string lang)
        {
            var dateTime = DateTime.Parse(date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            var formattedDate = dateTime.ToString("F", CultureInfo.GetCultureInfo(lang));
            return Ok(new FormattedDateModel
            {
                Language = lang,
                Date = dateTime,
                FormattedDate = formattedDate
            });
        }
    }
}
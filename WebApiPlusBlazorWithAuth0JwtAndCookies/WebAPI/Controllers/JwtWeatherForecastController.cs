using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiPlusBlazorWithAuth0JwtAndCookies
{
    [ApiController]
    [Route("[controller]")]
    // Marking this class with the [Authorize] means that any requesters must be authorized (logged in).
    // By setting the AuthenticationSchemes param to JwtBearerDefaults.AuthenticationScheme, we force this controller to require JWT-based authentication.
    // In Startup.cs, we have set 'options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme'. The AuthenticationSchemes param here overrides that.
    // In order to send valid authenticated requests against this conteoller, you'll need to pass your full JWT toke in the Authorization HTTP Header:
    // Authorization: Bearer {your full JWT token}
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // If have have role-based authentication setup in Auth0, you can protect controllers or individual endpoints by using the Roles param:
    // [Authorize(Roles = "Admin, WeatherUser, SomeOtherRole")]
    // Those roles would be confgured in the Auth0 portal under the Users & Roles section.
    public class JwtWeatherForecastController : Controller
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<PublicWeatherForecastController> _logger;

        public JwtWeatherForecastController(ILogger<PublicWeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

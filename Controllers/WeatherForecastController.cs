using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TelemetryHttpTester.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly CustomMetrics _customMetrics;
    private readonly ILogger<WeatherForecastController> _logger;
    private static readonly ActivitySource Activity = new("CustomActivityName");


    public WeatherForecastController(CustomMetrics customMetrics, ILogger<WeatherForecastController> logger)
    {
        _customMetrics = customMetrics;
        _logger = logger;
    }
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };




    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using (var activity =
               Activity.StartActivity("GetWeatherForecast Message", ActivityKind.Server))
        {

            _logger.LogInformation("WeatherCount getting Incremented");
            _customMetrics.IncrementWeatherCheckCount();
            AddLog();

            AddActivityTags(activity);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
                .ToArray();
        }
    }

    private void AddLog()
    {
        _logger.LogInformation("WeatherCount Incremented");

    }
    private static void AddActivityTags(Activity activity)
    {
        activity?.SetTag("messaging.system", "test");
        activity?.SetTag("messaging.destination_kind", "test queue");
    }
}
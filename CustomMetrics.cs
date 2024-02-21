using System.Diagnostics.Metrics;

namespace TelemetryHttpTester
{
    public class CustomMetrics
    {
        private Counter<int> WeatherCheckCounter { get; }
        public CustomMetrics(IMeterFactory meterFactory)
        {
            Meter meter = meterFactory.Create("CustomMeter");
            WeatherCheckCounter = meter.CreateCounter<int>("WeatherCheck", "Times");
        }

        public void IncrementWeatherCheckCount() => WeatherCheckCounter.Add(1);
    }
}

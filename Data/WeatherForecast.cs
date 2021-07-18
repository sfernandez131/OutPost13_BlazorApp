using System;
using static OutPost13.Data.WeatherPointsResponse;

namespace OutPost13.Data
{
    public class WeatherForecast
    {
        public string Date { get; set; }

        public int TemperatureC => (TemperatureF - 32) * 5 / 9;

        public int TemperatureF { get; set; }//=> 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
        public string Icon { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DailyWeatherData DailyWeather { get; set; }
    }
    public class DailyWeatherForecast
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public int TemperatureC => (TemperatureF - 32) * 5 / 9;

        public int TemperatureF { get; set; }//=> 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
        public string Icon { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DailyWeatherData DailyWeather { get; set; }
    }
}

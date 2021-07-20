using System;

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
    }
    public class DailyWeatherForecast
    {
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsDayTime { get; set; }
        public string WindSpeed { get; set; }
        public string WindDirection { get; set; }
        public string ShortForecast { get; set; }
        public int TemperatureC => (TemperatureF - 32) * 5 / 9;
        public int TemperatureF { get; set; }
        public string Summary { get; set; }
        public string Icon { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}

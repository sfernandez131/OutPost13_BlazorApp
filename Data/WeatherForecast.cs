using System;

namespace OutPost13.Data
{
    public class WeatherForecast
    {
        public string Date { get; set; }

        public int TemperatureC => (TemperatureF - 32) * 5 / 9;

        public int TemperatureF { get; set; }//=> 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}

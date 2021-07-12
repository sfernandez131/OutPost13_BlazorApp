using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace OutPost13.Data
{
    public class WeatherForecastService
    {
        public List<WeatherForecast> GetForecast(string zip)
        {
            var WeatherListData = new List<WeatherForecast>();

            string latLong = GetLatLong(zip);

            var client = new RestClient($"https://api.weather.gov/points/{latLong}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.UserAgent = "(outpost13.net, sfernandez131@gmail.com)";
            request.AddHeader("Accept", "application/cap+xml");
            IRestResponse response = client.Execute(request);

            var results = JsonConvert.DeserializeObject<WeatherPointsResponse>(response.Content);

            var client2 = new RestClient(results.properties.forecastHourly);
            client2.Timeout = -1;
            var request2 = new RestRequest(Method.GET);
            client2.UserAgent = "(outpost13.net, sfernandez131@gmail.com)";
            IRestResponse response2 = client2.Execute(request2);

            var results2 = JsonConvert.DeserializeObject<HourlyWeatherData>(response2.Content);

            foreach(var result in results2.properties.periods)
            {
                WeatherListData.Add(new WeatherForecast()
                {
                    Date = result.startTime.ToString("MM/dd/yyyy H:mm:ss") + " to " + result.endTime.ToString("H:mm:ss"),
                    Summary = result.shortForecast,
                    TemperatureF = result.temperature
                });
            }

            return WeatherListData;
        }

        private string GetLatLong(string zip)
        {
            var csvTable = new DataTable();
            string latLon = "";

            using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(@"wwwroot\zip_LATLONG_db.csv")), true))
            {
                csvTable.Load(csvReader);
            }

            for (int c = 0; c == 0; c++)
            {
                for (int r = 0; r < csvTable.Rows.Count; r++)
                {
                    if (csvTable.Rows[r].ItemArray[0].ToString() == zip)
                    {
                        latLon += csvTable.Rows[r].ItemArray[2].ToString() + ",";
                        latLon += csvTable.Rows[r].ItemArray[3].ToString();
                    }
                }
            }

            return latLon;
        }
    }


    public class WeatherPointsResponse
    {
        public object[] context { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }


        public class Geometry
        {
            public string type { get; set; }
            public float[] coordinates { get; set; }
        }

        public class Properties
        {
            public string id { get; set; }
            public string type { get; set; }
            public string cwa { get; set; }
            public string forecastOffice { get; set; }
            public string gridId { get; set; }
            public int gridX { get; set; }
            public int gridY { get; set; }
            public string forecast { get; set; }
            public string forecastHourly { get; set; }
            public string forecastGridData { get; set; }
            public string observationStations { get; set; }
            public Relativelocation relativeLocation { get; set; }
            public string forecastZone { get; set; }
            public string county { get; set; }
            public string fireWeatherZone { get; set; }
            public string timeZone { get; set; }
            public string radarStation { get; set; }
        }

        public class Relativelocation
        {
            public string type { get; set; }
            public Geometry1 geometry { get; set; }
            public Properties1 properties { get; set; }
        }

        public class Geometry1
        {
            public string type { get; set; }
            public float[] coordinates { get; set; }
        }

        public class Properties1
        {
            public string city { get; set; }
            public string state { get; set; }
            public Distance distance { get; set; }
            public Bearing bearing { get; set; }
        }

        public class Distance
        {
            public float value { get; set; }
            public string unitCode { get; set; }
        }

        public class Bearing
        {
            public int value { get; set; }
            public string unitCode { get; set; }
        }
    }


    public class HourlyWeatherData
    {
        public object[] context { get; set; }
        public string type { get; set; }
        public Geometry geometry { get; set; }
        public Properties properties { get; set; }


        public class Geometry
        {
            public string type { get; set; }
            public float[][][] coordinates { get; set; }
        }

        public class Properties
        {
            public DateTime updated { get; set; }
            public string units { get; set; }
            public string forecastGenerator { get; set; }
            public DateTime generatedAt { get; set; }
            public DateTime updateTime { get; set; }
            public string validTimes { get; set; }
            public Elevation elevation { get; set; }
            public Period[] periods { get; set; }
        }

        public class Elevation
        {
            public float value { get; set; }
            public string unitCode { get; set; }
        }

        public class Period
        {
            public int number { get; set; }
            public string name { get; set; }
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
            public bool isDaytime { get; set; }
            public int temperature { get; set; }
            public string temperatureUnit { get; set; }
            public object temperatureTrend { get; set; }
            public string windSpeed { get; set; }
            public string windDirection { get; set; }
            public string icon { get; set; }
            public string shortForecast { get; set; }
            public string detailedForecast { get; set; }
        }
    }

}

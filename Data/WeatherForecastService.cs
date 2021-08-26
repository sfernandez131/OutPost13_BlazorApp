using Json.Net;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace OutPost13.Data
{
    public class WeatherForecastService
    {
        public HashSet<int> hashMap = new HashSet<int>();
        public List<WeatherForecast> GetHourlyForecast(string zip, TimeZoneInfo timeZoneInfo)
        {
            var WeatherListData = new List<WeatherForecast>();

            string latLong = GetLatLong(zip);
            if (string.IsNullOrEmpty(latLong) || latLong.Equals(","))
            {
                return null;
            }

            var results = GetLatLongData(latLong);
            var hourlyWeather = GetHourlyForecast(results, timeZoneInfo);

            if (hourlyWeather?.properties is null)
            {
                return WeatherListData;
            }

            foreach (var result in hourlyWeather.properties.periods)
            {
                WeatherListData.Add(new WeatherForecast()
                {
                    Date = result.startTime.ToString("MM/dd/yyyy H:mm") + " to " + result.endTime.ToString("H:mm"),
                    Summary = result.shortForecast,
                    TemperatureF = result.temperature,
                    Icon = result.icon,
                    City = results.properties?.relativeLocation?.properties?.city,
                    State = results.properties?.relativeLocation?.properties?.state
                });
            }

            return WeatherListData;
        }

        public List<DailyWeatherForecast> GetDailyForecast(string zip, TimeZoneInfo timeZoneInfo)
        {
            var WeatherListDataDaily = new List<DailyWeatherForecast>();

            string latLong = GetLatLong(zip);
            if (string.IsNullOrEmpty(latLong) || latLong.Equals(","))
            {
                return null;
            }

            var results = GetLatLongData(latLong);
            var dFore = GetDailyForecast(results, timeZoneInfo);

            if (dFore?.properties is null)
                return WeatherListDataDaily;

            foreach (var result in dFore.properties?.periods)
            {
                WeatherListDataDaily.Add(new DailyWeatherForecast()
                {
                    City = results.properties.relativeLocation.properties.city,
                    State = results.properties.relativeLocation.properties.state,
                    ShortForecast = result.shortForecast,
                    StartTime = result.startTime,
                    EndTime = result.endTime,
                    Icon = result.icon,
                    IsDayTime = result.isDaytime,
                    Summary = result.detailedForecast,
                    WindSpeed = result.windSpeed,
                    Name = result.name,
                    WindDirection = result.windDirection,
                    TemperatureF = result.temperature
                });
            }

            return WeatherListDataDaily;
        }

        private DailyWeatherData GetDailyForecast(WeatherPointsResponse results, TimeZoneInfo clientTZ)
        {
            var client = new RestClient(results?.properties?.forecast);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.UserAgent = "(outpost13.net, sfernandez131@gmail.com)";
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // Process results back to local time from UTC.
                var resp = JsonConvert.DeserializeObject<DailyWeatherData>(response.Content);


                if (resp?.properties?.periods != null)
                {
                    foreach (var r in resp.properties.periods)
                    {
                        r.startTime = DateTime.SpecifyKind(r.startTime, DateTimeKind.Utc);
                        r.endTime = DateTime.SpecifyKind(r.endTime, DateTimeKind.Utc);
                        r.startTime = TimeZoneInfo.ConvertTimeFromUtc(r.startTime, clientTZ);
                        r.endTime = TimeZoneInfo.ConvertTimeFromUtc(r.endTime, clientTZ);
                    }
                }

                return resp;
            }
            else
                return null;
        }

        private HourlyWeatherData GetHourlyForecast(WeatherPointsResponse results, TimeZoneInfo clientTZ)
        {
            // These are off since it's using the server time and not the local time.
            var client = new RestClient(results.properties.forecastHourly);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.UserAgent = "(outpost13.net, sfernandez131@gmail.com)";
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // Process results back to local time from UTC.
                var resp = JsonConvert.DeserializeObject<HourlyWeatherData>(response.Content);

                if (resp?.properties?.periods != null)
                {
                    foreach (var r in resp.properties.periods)
                    {
                        r.startTime = DateTime.SpecifyKind(r.startTime, DateTimeKind.Utc);
                        r.endTime = DateTime.SpecifyKind(r.endTime, DateTimeKind.Utc);
                        r.startTime = TimeZoneInfo.ConvertTimeFromUtc(r.startTime, clientTZ);
                        r.endTime = TimeZoneInfo.ConvertTimeFromUtc(r.endTime, clientTZ);
                    }
                }

                return resp;
            }
            else
                return null;
        }

        private WeatherPointsResponse GetLatLongData(string latLong)
        {
            var client = new RestClient($"https://api.weather.gov/points/{latLong}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            client.UserAgent = "(outpost13.net, sfernandez131@gmail.com)";
            request.AddHeader("Accept", "application/cap+xml");
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
                return JsonConvert.DeserializeObject<WeatherPointsResponse>(response.Content);
            else
                return null;
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
    public class DailyWeatherData
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAPI_Test_Project.Contracts
{
    public class OpenWeatherOneCallAPI
    {
        public class OpenWeatherOnceCallAPI200Response
        {
            public class Current
            {
                public int dt { get; set; }
                public int sunrise { get; set; }
                public int sunset { get; set; }
                public double temp { get; set; }
                public double feels_like { get; set; }
                public int pressure { get; set; }
                public int humidity { get; set; }
                public double dew_point { get; set; }
                public double uvi { get; set; }
                public int clouds { get; set; }
                public int visibility { get; set; }
                public double wind_speed { get; set; }
                public int wind_deg { get; set; }
                public double wind_gust { get; set; }
                public List<Weather> weather { get; set; }
            }

            public class Minutely
            {
                public int dt { get; set; }
                public int precipitation { get; set; }
            }

            public class Rootobject
            {
                public double lat { get; set; }
                public double lon { get; set; }
                public string timezone { get; set; }
                public int timezone_offset { get; set; }
                public Current current { get; set; }
                public List<Minutely> minutely { get; set; }
            }

            public class Weather
            {
                public int id { get; set; }
                public string main { get; set; }
                public string description { get; set; }
                public string icon { get; set; }
            }
        }
    }

    public class OpenWeatherAPI401Response
    {
        public class Rootobject
        {
            public int cod { get; set; }
            public string message { get; set; }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using WeatherAPI_Test_Project.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using static WeatherAPI_Test_Project.Contracts.OpenWeatherOneCallAPI;
using System.Net;

namespace WeatherAPI_Test_Project.TestScripts
{
    [TestClass]
    public class OpenWeatherOneCallAPI_Tests
    {
        private HttpClient client;
        private string APIKey = "24309239b91ac6be65a414546a4720a2";

        [TestInitialize]
        public void Init()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("https://api.openweathermap.org/")
            };
        }

        [DataTestMethod]
        [DataRow("London", "1")]
        public void Valid_Coordinates_Return_SuccessResponse_From_OpenWeatherOneCallAPI(string cityName, string limit)
        {
            var geoCoding = GetCoordinatesFromGeocodingAPI(cityName, limit);

            var response = client.GetAsync($"data/3.0/onecall?lat={geoCoding[0].lat}&lon={geoCoding[0].lat}&appid={APIKey}").Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"StatusCode is incorrect. Error Message {response.Content.ReadAsStringAsync().Result.ToString()}");
        }

        public GeocodingAPI.Rootobject[] GetCoordinatesFromGeocodingAPI(string cityName = "London", string limit = "1")
        {
            var response = client.GetAsync($"geo/1.0/direct?q={cityName}&limit={limit}&appid={APIKey}").Result;
            return JsonConvert.DeserializeObject<GeocodingAPI.Rootobject[]>(response.Content.ReadAsStringAsync().Result);
        }

    }
}

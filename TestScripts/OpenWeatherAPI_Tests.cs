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
    public class OpenWeatherAPI_Tests
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
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit);
            var geocodingResponseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);
            var response = OpenWeatherOneCallAPIResponse(geocodingResponseData[0].lat.ToString(), geocodingResponseData[0].lon.ToString());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"StatusCode is incorrect. Error Message {response.Content.ReadAsStringAsync().Result.ToString()}");
        }

        [DataTestMethod]
        [DataRow("London", "1")]
        [DataRow("Paris", "1")]
        public void Valid_Coordinates_Return_SuccessResponse_From_CurrentWeatherAPI(string cityName, string limit)
        {
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit);
            var geocodingResponseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);
            var response = CurrentWeatherAPIResponse(geocodingResponseData[0].lat.ToString(), geocodingResponseData[0].lon.ToString());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"StatusCode is incorrect. Error Message {response.Content.ReadAsStringAsync().Result.ToString()}");
            var responseData = DeserialiseCurrentWeatherAPIResponse(response);
            Assert.AreEqual(cityName, responseData.name, "Place name returned does nt match place name requested");
        }

        [DataTestMethod]
        [DataRow("London", "1")]
        //[DataRow("Lndon", "1")]
        public void Valid_CityName_Returns_SuccessResponse_From_GeocodingAPI(string cityName, string limit)
        {
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit);
            Assert.AreEqual(HttpStatusCode.OK, geoCodingResponse.StatusCode, $"StatusCode is incorrect. Error Message {geoCodingResponse.Content.ReadAsStringAsync().Result.ToString()}");
            
            var responseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);
            Assert.AreEqual(cityName, responseData[0].name, $"Response Data returned for wrong place.");
        }

        public GeocodingAPI.Rootobject[] DeserialiseGeocodingAPIResponse(HttpResponseMessage geocodingAPIResponse)
        {
            return JsonConvert.DeserializeObject<GeocodingAPI.Rootobject[]>(geocodingAPIResponse.Content.ReadAsStringAsync().Result);
        }

        public CurrentWeatherAPI.Rootobject DeserialiseCurrentWeatherAPIResponse(HttpResponseMessage currentWeatherAPIResponse)
        {
            return JsonConvert.DeserializeObject<CurrentWeatherAPI.Rootobject>(currentWeatherAPIResponse.Content.ReadAsStringAsync().Result);
        }

        public HttpResponseMessage OpenWeatherOneCallAPIResponse(string lat, string lon)
        {
            return client.GetAsync($"data/3.0/onecall?lat={lat}&lon={lon}&appid={APIKey}").Result;
        }

        public HttpResponseMessage CurrentWeatherAPIResponse(string lat, string lon)
        {
            return client.GetAsync($"data/2.5/weather?lat={lat}&lon={lon}&appid={APIKey}").Result;
        }

        public HttpResponseMessage GeocodingAPIResponse(string cityName = "London", string limit = "1")
        {
            return client.GetAsync($"geo/1.0/direct?q={cityName}&limit={limit}&appid={APIKey}").Result;
        }

    }
}

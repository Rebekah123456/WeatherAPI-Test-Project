using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using WeatherAPI_Test_Project.Contracts;
using Newtonsoft.Json;
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

        #region TestMethods

        [DataTestMethod]
        [DataRow("London", "1")]
        public void Valid_Coordinates_Return_SuccessResponse_From_OpenWeatherOneCallAPI(string cityName, string limit)
        {
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit);
            var geocodingResponseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);
            OpenWeatherOneCallAPIResponse(geocodingResponseData[0].lat.ToString(), geocodingResponseData[0].lon.ToString(), HttpStatusCode.OK);
        }

        [DataTestMethod]
        [DataRow("London", "1")]
        [DataRow("Paris", "1")]
        public void Valid_Coordinates_Return_WeatherData_For_Requested_City_From_CurrentWeatherAPI(string cityName, string limit)
        {
            //Convert CityName to Longitude and Latitude
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit);
            var geocodingResponseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);

            //Call Current Weather API and get response
            var response = CurrentWeatherAPIResponse(geocodingResponseData[0].lat.ToString(), geocodingResponseData[0].lon.ToString(), HttpStatusCode.OK);
            
            //Deserialise response and assert weather returned for requested location 
            var responseData = DeserialiseCurrentWeatherAPIResponse(response);
            //Round co-ordinates to 4 decimal places to match CurrentWeather repsonse format
            var expectedLatitude = Math.Round((double)geocodingResponseData[0].lat, 4);
            var expectedLongitude = Math.Round((double)geocodingResponseData[0].lon, 4);

            //Assert.AreEqual(cityName, responseData.name, "Place name returned does not match place name requested");
            Assert.AreEqual(expectedLatitude, responseData.coord.lat, $"Latiude is not as expected when weather data requetsed for {cityName}");
            Assert.AreEqual(expectedLongitude, responseData.coord.lon, $"Longitude is not as expected when weather data requetsed for {cityName}");
        }

        [DataTestMethod]
        [DataRow("London", "1")]
        //[DataRow("Lndon", "1")]
        public void Valid_CityName_Returns_SuccessResponse_From_GeocodingAPI(string cityName, string limit)
        {
            var geoCodingResponse = GeocodingAPIResponse(cityName, limit, HttpStatusCode.OK);
            
            var responseData = DeserialiseGeocodingAPIResponse(geoCodingResponse);
            Assert.AreEqual(cityName, responseData[0].name, $"Response Data returned for wrong place.");
        }

        #endregion

        #region APICallsAndSharedMethods

        public GeocodingAPI.Rootobject[] DeserialiseGeocodingAPIResponse(HttpResponseMessage geocodingAPIResponse)
        {
            return JsonConvert.DeserializeObject<GeocodingAPI.Rootobject[]>(geocodingAPIResponse.Content.ReadAsStringAsync().Result);
        }

        public CurrentWeatherAPI.Rootobject DeserialiseCurrentWeatherAPIResponse(HttpResponseMessage currentWeatherAPIResponse)
        {
            return JsonConvert.DeserializeObject<CurrentWeatherAPI.Rootobject>(currentWeatherAPIResponse.Content.ReadAsStringAsync().Result);
        }

        public HttpResponseMessage OpenWeatherOneCallAPIResponse(string lat, string lon, HttpStatusCode expectedStatusCode)
        {
            var response = client.GetAsync($"data/3.0/onecall?lat={lat}&lon={lon}&appid={APIKey}").Result;
            ValidateStatusCode(response, expectedStatusCode);
            return response;
        }

        public HttpResponseMessage CurrentWeatherAPIResponse(string lat, string lon, HttpStatusCode expectedStatusCode)
        {
            var response =  client.GetAsync($"data/2.5/weather?lat={lat}&lon={lon}&appid={APIKey}&units=metric").Result;
            ValidateStatusCode(response, expectedStatusCode);
            return response;
        }

        public HttpResponseMessage GeocodingAPIResponse(string cityName = "London", string limit = "1", HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
        {
            var response = client.GetAsync($"geo/1.0/direct?q={cityName}&limit={limit}&appid={APIKey}").Result;
            ValidateStatusCode(response, expectedStatusCode);
            return response;
        }

        public void ValidateStatusCode(HttpResponseMessage apiResponse, HttpStatusCode expectedStatusCode)
        {
            Assert.AreEqual(expectedStatusCode, apiResponse.StatusCode, $"StatusCode is incorrect. Error Message {apiResponse.Content.ReadAsStringAsync().Result.ToString()}");
        }

        #endregion

    }
}

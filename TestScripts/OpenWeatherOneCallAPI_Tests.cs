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

        //Cannot progress test without subscribing
        [DataTestMethod]
        [DataRow("London", "1")]
        public void Coordinates_Return_SuccessResponse_From_OpenWeatherOneCallAPI(string cityName, string limit)
        {
            var geoCoding = GetCoordinatesFromGeocodingAPI(cityName, limit);

            var response = client.GetAsync($"data/3.0/onecall?lat={geoCoding[0].lat}&lon={geoCoding[0].lat}&appid={APIKey}").Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "StatusCode is not as expected");
        }

        [TestMethod]
        public void IfUserDoesNotHaveSubscriptionThenOpenWeatherOneCallAPIReturnsUnauthorisedResponse()
        {
            var geoCoding = GetCoordinatesFromGeocodingAPI();

            var response = client.GetAsync($"data/3.0/onecall?lat={geoCoding[0].lat}&lon={geoCoding[0].lon}&appid={APIKey}").Result; 
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode, "StatusCode is not as expected");
            var deserialisedResponse = JsonConvert.DeserializeObject<OpenWeatherAPI401Response.Rootobject>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(401, deserialisedResponse.cod, "Unauthorised respose Cod value is not as expected");
            Assert.AreEqual("Please note that using One Call 3.0 requires a separate subscription to the One Call by Call plan. Learn more here https://openweathermap.org/price. If you have a valid subscription to the One Call by Call plan, but still receive this error, then please see https://openweathermap.org/faq#error401 for more info.",
                    deserialisedResponse.message, "Unautorised message text is not as expected");
        }



        public GeocodingAPI.Rootobject[] GetCoordinatesFromGeocodingAPI(string cityName = "London", string limit = "1")
        {
            var response = client.GetAsync($"geo/1.0/direct?q={cityName}&limit={limit}&appid={APIKey}").Result;
            return JsonConvert.DeserializeObject<GeocodingAPI.Rootobject[]>(response.Content.ReadAsStringAsync().Result);
        }

    }
}

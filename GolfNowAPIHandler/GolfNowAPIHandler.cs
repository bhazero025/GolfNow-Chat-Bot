using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using GolfNowAPIHandler;
using Newtonsoft.Json.Linq;

namespace GolfNowAPI
{
    class GolfNowAPIHandler
    {
        //TODO: Dispose of HttpClient properly
        //TODO: Default constructor with credentials
		//TODO: Error catching
        const string ENDPOINT = "https://2-1-17-sandbox.api.gnsvc.com/rest";
        const string TEMP_USERNAME = "";
        const string TEMP_PASSWORD = "";

        public GolfNowAPIHandler(){
            User user = new User();
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = new HttpResponseMessage())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");
                    // Ping API to ensure we have a valid connection with our credentials
                    PingApi();
                }
            }
        }

        /// <summary>
        /// Returns the authentication token for the input User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A string holding the authentication token</returns>
        public static string CustToken(User user)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create POST request
                    trace("\n\nStarting customer token request ------------------------------------------------------ \nHeaders:");
                    trace("POST " + ENDPOINT + "/customers/" + user.getEmail() + "/authentication-token\n" + new StringContent(user.ToJSON() + Encoding.UTF8.ToString()));
                    trace(client.DefaultRequestHeaders.ToString());
                    //Obtain response and return user token as string
                    result = client.PostAsync(ENDPOINT + "/customers/" + user.getEmail() + "/authentication-token", new StringContent(user.ToJSON(), Encoding.UTF8, "application/json")).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return result.Content.ReadAsStringAsync().Result;
                }
            }
        }

        /// <summary>
        /// Uses latitude, longitude, and a proximity to find Facility objects meeting the criteria and return them in a List
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="proximity"></param>
        /// <param name="take"></param>
        /// <returns>A List of Facility objects within the input proximity of the input latitude and longitude coordinates</returns>
        public static List<Facility> GetFacilitiesByLocation(string latitude, string longitude, string proximity = "25", int take=5)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetFacilitiesByLocation search ------------------------------------------------------ \nHeaders:");
                    string formatRequest = String.Format(ENDPOINT + "/channel/7886/facilities?q=geo-location&latitude={0}&longitude={1}&proximity={2}&take={3}", latitude, longitude, proximity, take);
                    trace(formatRequest); // Output the request uri
                    trace(client.DefaultRequestHeaders.ToString());
                    //Get output and deserialize it into a List of Facility objects
                    result = client.GetAsync(formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }
        }

        /// <summary>
        /// Gets a List object holding Facility objects that represent the facilities in the City parameter
        /// </summary>
        /// <param name="city"></param>
        /// <param name="take"></param>
        /// <returns>A List of Facility objects from the input City</returns>
        public static List<Facility> GetFacilitiesByCity(City city, int take=5)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetFacilitiesByCity search ------------------------------------------------------ \nHeaders:");
                    string formatRequest = String.Format(ENDPOINT + "/channel/7886/facilities?q=country-city-state&country-code={0}&state-province-code={1}&city={2}", city.CountryCode, city.StateCode, city.CityName, take);
                    trace(formatRequest); // Output the request uri
                    trace(client.DefaultRequestHeaders.ToString());
                    //Get the response and deserialize it into a List of Facility objects
                    result = client.GetAsync(formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }
        }

        /// <summary>
        /// Returns a Facility object representing the facility with the input facility ID parameter
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="channelID"></param>
        /// <returns>A Facility object matching the input facility ID</returns>
        public static Facility GetFacilityByID(string facilityID, string channelID = "7886")
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetFacilityByID ------------------------------------------------------ \nHeaders:");
                    trace(client.DefaultRequestHeaders.ToString());
                    string formatRequest = String.Format(ENDPOINT + "/channel/{0}/facilities/{1}", channelID, facilityID);
                    trace(formatRequest);// Output the request uri
                    //Get the response and deserialize it into a Facility object
                    result = client.GetAsync(formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<Facility>(result.Content.ReadAsStringAsync().Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }
        }

        /// <summary>
        /// Returns a list of TeeTimes from the facility with the matching facility ID parameter
        /// </summary>
        /// <param name="facilityID"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <param name="holes"></param>
        /// <param name="players"></param>
        /// <returns>A List of TeeTime objects that holds TeeTimes from the facility matching the input facility ID and the parameters</returns>
        public static List<TeeTime> GetTeeTimesByFacility(string facilityID, DateTime dateMin, DateTime dateMax, float priceMin = 0, float priceMax = 200, string holes = "18", int players = 1)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetTeeTimesByFacility search ------------------------------------------------------ \nHeaders:");
                    trace(client.DefaultRequestHeaders.ToString());
                    string date1 = dateMin.AddMinutes(0).ToLocalTime().ToString();
                    string date2 = dateMax.AddDays(0).ToLocalTime().ToString();
                    string formatRequest = Uri.EscapeUriString($"/channel/7886/facilities/{facilityID}/tee-times?date-min={date1}&date-max={date2}&price-min={priceMin}&price-max={priceMax}&holes={holes}&players={players}&take=3");
                    trace("GET " + ENDPOINT + formatRequest);//Output the uri
                    //Get response and deserialize it into a list of TeeTimes
                    result = client.GetAsync(ENDPOINT + formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<TeeTimeList>(result.Content.ReadAsStringAsync().Result).TeeTimes);
                }
            }
        }

        /// <summary>
        /// Obtains a list of TeeTimes that are close in time to the one with the input TeeTimeRateID
        /// Note: The first response is the same TeeTime as the input one(Because it is most similar).
        /// </summary>
        /// <param name="teeTimeRateID"></param>
        /// <param name="facilityID"></param>
        /// <param name="players"></param>
        /// <param name="take"></param>
        /// <returns>a List of TeeTimes matching the parameters</returns>
        public static List<TeeTime> GetAdjacentTeeTimes(string teeTimeRateID, string facilityID, string players, int take = 3)
        {
             HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetAdjacentTeeTimes search ------------------------------------------------------ \nHeaders:");
                    trace(client.DefaultRequestHeaders.ToString());
                    string formatRequest = Uri.EscapeUriString($"/channel/7886/facilities/{facilityID}/tee-times/{teeTimeRateID}/adjacent-tee-times?player-count={players}&take={take}");
                    trace("GET " + ENDPOINT + formatRequest);// Output the uri
                    //Get response and deserialize it as a List of TeeTime objects
                    result = client.GetAsync(ENDPOINT + formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<List<TeeTime>>(result.Content.ReadAsStringAsync().Result));
                }
            }
        }

        /// <summary>
        /// Uses input country code, state/province code, and city name with some parameters to find TeeTimes that match
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="stateProvinceCode"></param>
        /// <param name="cityCode"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <param name="holes"></param>
        /// <param name="players"></param>
        /// <param name="take"></param>
        /// <returns>a List object of TeeTimes matching the parameters</returns>
        public static List<TeeTime> GetTeeTimesByCity(string countryCode, string stateProvinceCode, string cityCode, DateTime dateMin, DateTime dateMax, float priceMin = 0, float priceMax = 200, string holes = "18", int players = 1, int take = 3)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create request headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Create GET request
                    trace("Starting GetTeeTimesByCity search ------------------------------------------------------ \nHeaders:");
                    string date1 = dateMin.ToLocalTime().ToString();
                    string date2 = dateMax.ToLocalTime().ToString();
                    string formatRequest = Uri.EscapeUriString($"/channel/7886/facilities/tee-times?q=country-city-state&country-code={countryCode}&state-province-code={stateProvinceCode}&city={cityCode}&date-min={dateMin}&date-max={dateMax}&price-min={priceMin}&price-max={priceMax}&holes={holes}&players={players}&take={take}");
                    trace("GET " + ENDPOINT + formatRequest);// Output the uri
                    trace(client.DefaultRequestHeaders.ToString());
                    //Get response and deserialize it as a List of TeeTime objects
                    result = client.GetAsync(ENDPOINT + formatRequest).Result;
                    trace("Response headers:\n" + result.Headers.ToString());
                    return (JsonConvert.DeserializeObject<TeeTimeList>(result.Content.ReadAsStringAsync().Result).TeeTimes);
                }
            }
        }

        /// <summary>
        /// Attempts to find a city with the input arguments
        /// </summary>
        /// <param name="city"></param>
        /// <returns>A List of City objects that could match the arguments</returns>
        public static List<City> GetCityInfo(TeesByCityArgs city)
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    List<City> possibleMatchs = new List<City>();
                    //Get all countries
                    result = client.GetAsync(ENDPOINT + "/channel/7886/countries?fields=CountryCode").Result;
                    var countries = JArray.Parse(result.Content.ReadAsStringAsync().Result);
                    string[] countryCode = new string[countries.Count];
                    for (int i = 0; i < countryCode.Length; i++)
                    {
                        string value = countries[i]["CountryCode"].Value<string>();
                        if (value == "US")
                        {
                            countryCode[i] = countryCode[0];
                            countryCode[0] = value;
                        }
                        else if (value == "CA")
                        {
                            countryCode[i] = countryCode[1];
                            countryCode[1] = value;
                        }
                        else
                        {
                            countryCode[i] = value;
                        }
                    }

                    //Search all states
                    foreach (string country in countryCode)
                    {
                        result = client.GetAsync(ENDPOINT + "/channel/7886/countries/" + country + "/state-provinces?fields=StateProvinceCode").Result;
                        var states = JArray.Parse(result.Content.ReadAsStringAsync().Result);
                        string[] stateCode = new string[states.Count];
                        for (int i = 0; i < stateCode.Length; i++)
                        {
                            stateCode[i] = states[i]["StateProvinceCode"].Value<string>();
                        }

                        //search for cities
                        foreach (string state in stateCode)
                        {
                            result = client.GetAsync(ENDPOINT + "/channel/7886/countries/" + country + "/state-provinces/" + state + "/cities?fields=CityName").Result;
                            var cities = JArray.Parse(result.Content.ReadAsStringAsync().Result);
                            string[] cityName = new string[cities.Count];
                            for (int i = 0; i < cityName.Length; i++)
                            {
                                if (cities[i]["CityName"].Value<string>().ToLower() == city.CityName.ToLower())
                                {
                                    possibleMatchs.Add(new City(city.CityName, state, country));
                                }

                            }
                        }
                    }
                    return possibleMatchs;
                }
            }
        }

        /// <summary>
        /// Pings the Api to check for a connection with the credentials
        /// </summary>
        static void PingApi()
        {
            HttpResponseMessage result;
            using (HttpClient client = new HttpClient())
            {
                using (result = new HttpResponseMessage())
                {
                    //Create headers
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");
                    //Create POST request and return the response
                    trace("Ensure connection to the API with creds is possible:");
                    result = client.PostAsync(ENDPOINT + "/system/status/secure-echo", new StringContent("\"Connection established\"", Encoding.UTF8, "application/json")).Result;
                    trace(result.Content.ReadAsStringAsync().Result);
                }
            }
        }

        private static void trace(string message)
        {
            Console.WriteLine(message);
        }
    }
}
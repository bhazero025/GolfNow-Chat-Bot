using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

///<author>Andrew Millward</author>
///<author>Josh Alford</author>

namespace testAWSLambda
{
    /// <summary>
    /// Handles all connections to the backend APIs and services.
    /// </summary>
    class ApiConnectionBroker
    {
        private static ApiConnectionBroker singleton_instance;
        //Info for accessing the API backend.
        const string ENDPOINT = "https://2-1-17-sandbox.api.gnsvc.com/rest";
        const string TEMP_USERNAME = "UCF_Development";
        const string TEMP_PASSWORD = "es2QENyqPftThmJy";
        const string CHANNEL_ID = "7886";

        /// <summary>
        /// Handles all calls to the APIs and determines which is needed.
        /// </summary>
        /// <param name="args">The list of arguments that will determine the API call needed and give it the information required.</param>
        /// <returns>Key-Value paris of useful data retrived by the API.</returns>
        public IDictionary<string, string> ApiHandler(ApiArgs args)
        {
            IDictionary<string, string> output = new Dictionary<string, string>();
            output.Add( "output", "Error getting response.");
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = new HttpResponseMessage())
                {
                    //Set up headers for API call.
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("UserName", TEMP_USERNAME);
                    client.DefaultRequestHeaders.Add("Password", TEMP_PASSWORD);
                    client.DefaultRequestHeaders.Host = "sandbox.api.gnsvc.com";
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("defate"));
                    client.DefaultRequestHeaders.Add("AdvancedErrorCodes", "True");

                    //Holds temp attributes to be added to output
                    IDictionary<string, string> attributes;
                    switch (args.TYPE)
                    {
                        case ApiArgs.TYPES.Zip:
                            TeesByZipArgs teesByZip = (TeesByZipArgs)args;
                            attributes = GetFacilityByZip(client, response, teesByZip);
                            attributes.ToList().ForEach(x => output[x.Key] = x.Value);
                            break;
                        case ApiArgs.TYPES.City:
                            TeesByCityArgs teesByCity = (TeesByCityArgs)args;
                            attributes = GetFacilityByCity(client, response, teesByCity);
                            attributes.ToList().ForEach(x => output[x.Key] = x.Value);
                            break;
                        case ApiArgs.TYPES.Invoice:
                            RateInvoiceArgs invoiceArgs = (RateInvoiceArgs)args;
                            output = GetRateInvoice(client, response, invoiceArgs);
                            break;
                        case ApiArgs.TYPES.Login:
                            LoginArgs loginArgs = (LoginArgs)args;
                            output["output"] = "Loggging in...";
                            output["customerToken"] = GetCustToken(client, response, new User(loginArgs.Username, loginArgs.Password));
                            break;
                    }
                }
            }

            if (output["output"] == "" || output == null || output["output"] == "Error getting response.")
                output["output"] = "Sorry, no tee times were found with that information...";

            return output;
        }

        protected ApiConnectionBroker()
        {

        }

        public static ApiConnectionBroker Instance()
        {
            if(singleton_instance == null)
            {
                singleton_instance = new ApiConnectionBroker();
            }

            return singleton_instance;
        }

        /// <summary>
        /// Returns the customer information for the input User
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="user">The user object to fill out with data from GolfNow's API</param>
        /// <returns>A User object that stores the information for the customer</returns>
        private static User GetCustomer(HttpClient client, HttpResponseMessage result, User user)
        {
            // Variables to store response objects and create a User to return
            IEnumerable<string> value;
            User responseUser;
            string newToken;

            //Get user token if it doesn't exist
            if (user.getToken() == null)
                user.setToken(GetCustToken(client, result, user));

            //Put customer token in header
            client.DefaultRequestHeaders.Add("CustomerToken", user.getToken());

            //Obtain response
            result = client.GetAsync(ENDPOINT + "/customers/" + user.EMailAddress).Result;

            //Remove token from header and deserialize response into a User object
            client.DefaultRequestHeaders.Remove("CustomerToken");
            result.Headers.TryGetValues("CustomerToken", out value);
            newToken = value.FirstOrDefault();
            responseUser = JsonConvert.DeserializeObject<User>(result.Content.ReadAsStringAsync().Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            responseUser.setToken(newToken);

            return (responseUser);
        }


        /// <summary>
        /// Returns the authentication token for the input User
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="user">The user object to fill out with data from GolfNow's API</param>
        /// <returns>A string holding the authentication token</returns>
        private static string GetCustToken(HttpClient client, HttpResponseMessage result, User user)
        {
            //Obtain response and return user token as string
            result = client.PostAsync(ENDPOINT + "/customers/" + user.EMailAddress + "/authentication-token", new StringContent(user.ToJSON(), Encoding.UTF8, "application/json")).Result;
            return result.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
        }


        /// <summary>
        /// Returns the favorite facility information for the input User
        /// Note: You MUST authenticate and get the customer token using the GetCustToken() method before using this!
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A User object that stores the information for the customer</returns>
        public static List<Facility> GetFavoriteFacilities(HttpClient client, HttpResponseMessage result, User user)
        {
            //Obtain response and deserialize it into a list of Facility objects
            result = client.GetAsync(ENDPOINT + "/customers/" + user.EMailAddress + "/favorite-facilities?channel=" + CHANNEL_ID).Result;
            
            return (JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
           
        }

        /// <summary>
        /// Reads out the information about the users selection.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="invoiceArgs">The arguments for creating an invoice.</param>
        /// <returns>Key-value dictionary of the returned information.</returns>
        private static IDictionary<string, string> GetRateInvoice(HttpClient client, HttpResponseMessage result, RateInvoiceArgs invoiceArgs)
        {
            result = client.GetAsync(ENDPOINT + $"/channel/{CHANNEL_ID}/facilities/{invoiceArgs.FacilityID}/tee-times/{invoiceArgs.RateID}/invoice?player-count={invoiceArgs.NumOfPlayers}").Result;
            RateInvoiceResponse rateResponse = JsonConvert.DeserializeObject<RateInvoiceResponse>(result.Content.ReadAsStringAsync().Result);

            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("output", $"Ok, so that's {invoiceArgs.NumOfPlayers} Players for {rateResponse.RateName}: {rateResponse.TeeTimeNotes} - " +
                $"That'll be ${rateResponse.DueAtCourse.Summary.Total} due at the course and ${rateResponse.DueOnline.Summary.Total} due online. Sound good?"); //{rateResponse.TeeTimeRateID}");

            return keyValuePairs;
        }

        /// <summary>
        /// Gets facilities based around a given zipcode.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="zip">the arguments for the zipcode based API request. Ex. time, proximity, etc...</param>
        /// <returns>Key-Value paris of the returned TeeTimes by Facility.</returns>
        private static IDictionary<string, string> GetFacilityByZip(HttpClient client, HttpResponseMessage result, TeesByZipArgs zip)
        {
            //Call the API with the args.
            result = client.GetAsync(ENDPOINT + $"/channel/{CHANNEL_ID}/facilities?q=postal-code&postal-code={zip.ZipCode}&proximity={zip.Proximity}&minplayercount={zip.NumOfPlayers}&fields=ID,name").Result;
            //Parse response into C# objects.
            FacilitiesResponse facilities = new FacilitiesResponse(JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result));

            //Send facilites to be searched for tee times. Return K/V pair array of results.
            return getTeeTimesByFacilities(client, result, facilities, zip.Time, zip.NumOfPlayers);
        }

        /// <summary>
        /// Gets facilities based around a given city, state(optional), and country (optional).
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="zip">the arguments for the city based API request. Ex. time, proximity, etc...</param>
        /// <returns>Key-Value paris of the returned TeeTimes by Facility.</returns>
        private static IDictionary<string, string> GetFacilityByCity(HttpClient client, HttpResponseMessage result, TeesByCityArgs city)
        {
            //All cities that could match the one entered by the user.
            List<City> possibleMatchs = getCityInfo(client, result, city);
            FacilitiesResponse facilities = new FacilitiesResponse();

            //Iterate through the potentially matching cities and gather combine eligable facilites.
            foreach (City possibleCity in possibleMatchs)
            {
                result = client.GetAsync(ENDPOINT + $"/channel/{CHANNEL_ID}/facilities?q=country-city-state&country-code={possibleCity.CountryCode}&state-province-code={possibleCity.StateCode}&city={possibleCity.CityName}&players={city.NumOfPlayers}&fields=ID,name").Result;
                facilities.Facilities = facilities.Facilities.Concat(JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result)).ToList();
            }

            //Send facilites to be searched for tee times. Return K/V pair array of results.
            return getTeeTimesByFacilities(client, result, facilities, city.Time, city.NumOfPlayers);
        }

        /// <summary>
        /// Searches a list of given facilites for tee time options.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="facilities">The list of facilities to be searched.</param>
        /// <returns>Key-Value paris of the returned TeeTimes by Facility.</returns>
        private static IDictionary<string, string> getTeeTimesByFacilities(HttpClient client, HttpResponseMessage result, FacilitiesResponse facilities, string time, string players)
        {
            IDictionary<string, string> rateOptions = new Dictionary<string, string>();

            //If no facilities, return blank object.
            if (facilities.Facilities.Count == 0)
                return rateOptions;

            DateTime scheduleTime = Convert.ToDateTime(time);

            //If the time is in the past assume they ment tomorrow.
            while (scheduleTime < DateTime.Now)
                scheduleTime = scheduleTime.AddHours(24);
            
            //Define a range of time to search.
            string localDateMin = scheduleTime.AddMinutes(-15).ToLocalTime().ToString();
            string localDateMax = scheduleTime.AddHours(2).ToLocalTime().ToString();

            result = client.GetAsync(ENDPOINT + Uri.EscapeUriString($"/channel/{CHANNEL_ID}/facilities/tee-times?q=multi-facilities&facilityids={string.Join("|", facilities.GetIDs())}&date-min={localDateMin}&date-max={localDateMax}&take=3")).Result;
            TeeTimesResponse teeTimesResponse = JsonConvert.DeserializeObject<TeeTimesResponse>(result.Content.ReadAsStringAsync().Result);

            //The text to be displayed to the user.
            string output = "";
            //Keep track of which option is next.
            int optionNumb = 1;
            for (int i = 0; i < teeTimesResponse.TeeTimes.Count; i++)
            {
                //Look for the name of the facility to match the known ID.
                foreach (Facility facility in facilities.Facilities)
                {
                    if(facility.ID == teeTimesResponse.TeeTimes[i].FacilityID)
                    {
                        output += facility.Name + " " + teeTimesResponse.TeeTimes[i].Time.ToLocalTime() + " : - ";
                    }
                }

                //Display various rates.
                for (int x = 0; x < 4 && x < teeTimesResponse.TeeTimes[i].Rates.Count; x++)
                {
                    TeeTime teeTime = teeTimesResponse.TeeTimes[i];
                    Rate rate = teeTime.Rates[x];
                    output += $"(#{optionNumb}){rate.RateName}-${rate.SinglePlayerPrice.GreensFees.Value}{rate.SinglePlayerPrice.GreensFees.CurrencyCode}";
                    rateOptions.Add("rateOption" + optionNumb.ToString(), JsonConvert.SerializeObject(new TeeTimeRateSelection(teeTime.FacilityID, rate.TeeTimeRateID)));
                    if (x < 3 && x < (teeTime.Rates.Count - 1))
                    {
                        output += " | ";
                    } else
                    {
                        output += " - ";
                    }
                    optionNumb++;
                }
            }

            if (teeTimesResponse.TeeTimes.Count > 0)
                output += "Enter the number of a suitable option.";

            rateOptions.Add("players", players);
            rateOptions.Add("output", output);

            return rateOptions;
        }

        /// <summary>
        /// Returns the complete information of citties that could match the user's input.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="city">The city information provided by the user.</param>
        /// <returns>List of cities that could match the given city name.</returns>
        private static List<City> getCityInfo(HttpClient client, HttpResponseMessage result, TeesByCityArgs city)
        {
            List<City> possibleMatchs = new List<City>();
            //Get all country codes

            //If country code provided, use that.
            string[] countryCode;
            if (city.Country != null && city.Country.Length <= 2)
                countryCode = new string[] { city.Country };
            else
            {
                try
                {
                    string fields = "CountryCode";
                    if (city.Country != null && city.Country.Length > 2)
                        fields += ",CountryName";
                    result = client.GetAsync(ENDPOINT + "/channel/" + CHANNEL_ID + "/countries?fields=" + fields).Result;
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                var countries = JArray.Parse(result.Content.ReadAsStringAsync().Result);
                countryCode = new string[countries.Count];

                for (int i = 0; i < countryCode.Length; i++)
                {
                    //If user entered country is found, use only that and break out of loop.
                    if (city.Country != null && (countries[i]["CountryName"].Value<string>() == city.Country || countries[i]["CountryCode"].Value<string>() == city.Country))
                    {
                        countryCode = new string[] { countries[i]["CountryCode"].Value<string>() };
                        break;
                    }

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
            }

            //Search all states
            foreach (string country in countryCode)
            {
                //If state code provided, use that.
                string[] stateCode;
                if (city.State != null && city.State.Length <= 2)
                    stateCode = new string[] { city.State };
                else
                {
                    try
                    {
                        string fields = "StateProvinceCode";
                        if (city.State != null)
                            fields += ",StateProvinceName";
                        result = client.GetAsync(ENDPOINT + "/channel/" + CHANNEL_ID + "/countries/" + country + "/state-provinces?fields=" + fields).Result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                    var states = JArray.Parse(result.Content.ReadAsStringAsync().Result);
                    stateCode = new string[states.Count];
                    for (int i = 0; i < stateCode.Length; i++)
                    {
                        //If user entered state is found, use only that and break out of loop.
                        if (city.State != null && states[i]["StateProvinceName"].Value<string>() == city.State)
                        {
                            stateCode = new string[] { states[i]["StateProvinceCode"].Value<string>() };
                            break;
                        }
                        stateCode[i] = states[i]["StateProvinceCode"].Value<string>();
                    }
                }

                //search for cities
                foreach (string state in stateCode)
                {
                    try
                    {
                        result = client.GetAsync(ENDPOINT + "/channel/" + CHANNEL_ID + "/countries/" + country + "/state-provinces/" + state + "/cities?fields=CityName").Result;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

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

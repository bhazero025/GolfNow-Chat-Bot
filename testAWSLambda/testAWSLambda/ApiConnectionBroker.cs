using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

///<author>Andrew Millward</author>

namespace testAWSLambda
{
    /// <summary>
    /// Handles all connections to the backend APIs and services.
    /// </summary>
    class ApiConnectionBroker
    {
        //Info for accessing the API backend.
        const string ENDPOINT = "https://2-1-17-sandbox.api.gnsvc.com/rest";
        const string TEMP_USERNAME = "UCF_Development";
        const string TEMP_PASSWORD = "es2QENyqPftThmJy";

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
                    }
                }
            }

            if (output["output"] == "")
                output["output"] = "Sorry, no tee times were found with that information...";

            return output;
        }

        private IDictionary<string, string> GetRateInvoice(HttpClient client, HttpResponseMessage result, RateInvoiceArgs invoiceArgs)
        {
            result = client.GetAsync(ENDPOINT + $"/channel/7886/facilities/{invoiceArgs.FacilityID}/tee-times/{invoiceArgs.RateID}/invoice?player-count=1").Result;
            RateInvoiceResponse rateResponse = JsonConvert.DeserializeObject<RateInvoiceResponse>(result.Content.ReadAsStringAsync().Result);

            IDictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("output", $"Ok, so that's {rateResponse.RateName}: {rateResponse.TeeTimeNotes} - " +
                $"That'll be ${rateResponse.DueAtCourse.Summary.Total} due at the course and ${rateResponse.DueOnline.Summary.Total} due online. Sound good?");
            return keyValuePairs;
        }

        /// <summary>
        /// Test for checking the API.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <returns>The body of the response</returns>
        private string GetFacility(HttpClient client, HttpResponseMessage result)
        {
            result = client.GetAsync(ENDPOINT + "/channel/7886/facilities?q=geo-location&latitude=28.4158&longitude=-81.2989&proximity=25").Result;
            return result.Content.ReadAsStringAsync().Result;
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
            result = client.GetAsync(ENDPOINT + $"/channel/7886/facilities?q=postal-code&postal-code={zip.ZipCode}&proximity={zip.Proximity}&fields=ID,name").Result;
            //Parse response into C# objects.
            FacilitiesResponse facilities = new FacilitiesResponse(JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result));

            //Send facilites to be searched for tee times. Return K/V pair array of results.
            return getTeeTimesByFacilities(client, result, facilities);
        }

        /// <summary>
        /// Gets facilities based around a given city, state(optional), and country (optional).
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="zip">the arguments for the city based API request. Ex. time, proximity, etc...</param>
        /// <returns>Key-Value paris of the returned TeeTimes by Facility.</returns>
        static IDictionary<string, string> GetFacilityByCity(HttpClient client, HttpResponseMessage result, TeesByCityArgs city)
        {
            //All cities that could match the one entered by the user.
            List<City> possibleMatchs = getCityInfo(client, result, city);
            FacilitiesResponse facilities = new FacilitiesResponse();

            //Iterate through the potentially matching cities and gather combine eligable facilites.
            foreach (City possibleCity in possibleMatchs)
            {
                result = client.GetAsync(ENDPOINT + $"/channel/7886/facilities?q=country-city-state&country-code={possibleCity.CountryCode}&state-province-code={possibleCity.StateCode}&city={possibleCity.CityName}&fields=ID,name").Result;
                facilities.Facilities = facilities.Facilities.Concat(JsonConvert.DeserializeObject<List<Facility>>(result.Content.ReadAsStringAsync().Result)).ToList();
            }

            //Send facilites to be searched for tee times. Return K/V pair array of results.
            return getTeeTimesByFacilities(client, result, facilities);
        }

        /// <summary>
        /// Searches a list of given facilites for tee time options.
        /// </summary>
        /// <param name="client">Object for making HTTP requests.</param>
        /// <param name="result">Object holding the response of the HTTP request.</param>
        /// <param name="facilities">The list of facilities to be searched.</param>
        /// <returns>Key-Value paris of the returned TeeTimes by Facility.</returns>
        private static IDictionary<string, string> getTeeTimesByFacilities(HttpClient client, HttpResponseMessage result, FacilitiesResponse facilities)
        {
            DateTime currentTime = DateTime.Now;
            string localDateMin = currentTime.AddMinutes(-30).ToLocalTime().ToString();
            string localDateMax = currentTime.AddHours(23).ToLocalTime().ToString();
            result = client.GetAsync(ENDPOINT + Uri.EscapeUriString($"/channel/7886/facilities/tee-times?q=multi-facilities&facilityids={string.Join("|", facilities.GetIDs())}&date-min={localDateMin}&date-max={localDateMax}&take=3")).Result;
            TeeTimesResponse teeTimesResponse = JsonConvert.DeserializeObject<TeeTimesResponse>(result.Content.ReadAsStringAsync().Result);

            IDictionary<string, string> rateOptions = new Dictionary<string, string>();

            string output = "";
            int optionNumb = 1;
            for (int i = 0; i < teeTimesResponse.TeeTimes.Count; i++)
            {
                foreach(Facility facility in facilities.Facilities)
                {
                    if(facility.ID == teeTimesResponse.TeeTimes[i].FacilityID)
                    {
                        output += facility.Name + " : - ";
                    }
                }
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
        static List<City> getCityInfo(HttpClient client, HttpResponseMessage result, TeesByCityArgs city)
        {
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

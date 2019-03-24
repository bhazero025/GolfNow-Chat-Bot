using System;
using System.Collections.Generic;

namespace GolfNowAPI
{
    public class City
        {
            public string CityName { get; set; }
            public string StateCode { get; set; }
            public string CountryCode { get; set; }

            public City(string name, string state, string country)
            {
                CityName = name;
                StateCode = state;
                CountryCode = country;
            }

            override public string ToString()
            {
                return string.Format("{0}, {1}, {2}", CityName, StateCode, CountryCode);
            }
        }
}
using System;
using System.Collections.Generic;

namespace GolfNowAPI
{
    public abstract class TeeSearchArgs
    {
        public enum TYPES { City, Zip }
        public abstract TYPES TYPE { get; }
    }

    public class TeesByCityArgs : TeeSearchArgs
    {
        public override TYPES TYPE { get; } = TYPES.City;
        public string CityName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Time { get; set; }

        public TeesByCityArgs(string cityName, string state, string country, string time)
        {
            CityName = cityName;
            State = state;
            Country = country;
            Time = time;
        }
    }

    public class TeesByZipArgs : TeeSearchArgs
    {
        public override TYPES TYPE { get; } = TYPES.Zip;
        public string ZipCode { get; set; }
        public string Proximity { get; set; }
        public string Time { get; set; }

        public TeesByZipArgs(string zipCode, string prox, string time)
        {
            ZipCode = zipCode;
            Proximity = prox;
            Time = time;
        }
    }

    public class CustomerProfile
    {
        public string Password { get; set; }
        public string RegistrationSourceURL { get; set; }
        public string ReferredBy { get; set; }
        public string CustomerReferralToken { get; set; }
        public string EMailAddress { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Address Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public int Level { get; set; }
        public string Handicap1 { get; set; }
        public int[] Type { get; set; }
        public int HolesPreferred { get; set; }
        public int CartPreferred { get; set; }
        public bool EmailPreferred { get; set; }
        public PreferredTeeTimeRange[] PreferredTeeTimes { get; set; }
        public string GolfBalls { get; set; }
        public string Putter { get; set; }
        public string ShoeBrand { get; set; }
        public string ShoeSize { get; set; }
        public string Woods { get; set; }
        public string Irons { get; set; }
        public string RoundsMonth { get; set; }
        public string ShirtBrand { get; set; }
        public string ShirtSize { get; set; }
        public bool MilitaryVerificationOptOut { get; set; }
        public FacilityNote NewFacilityNoteEntry { get; set; }
        public int Gender { get; set; }
        public string Token { get; set; }

        public CustomerProfile(string email, string pass)
        {
            EMailAddress = email;
            Password = pass;
        }
    }

    public class PreferredTeeTimeRange
    {
        public int[] DayOfWeek { get; set; }
        public int[] TimeRange { get; set; }
    }

    public class FacilityNote
    {
        public int GolfFacilityID { get; set; }
        public string Note { get; set; }
    }

    public class TeeTimeList
    {
        public bool LimitReached { get; set; }
        public List<TeeTime> TeeTimes { get; set; }
        public int TotalTeeTimes { get; set; }
    }
}
using System;

namespace testAWSLambda
{
    public class User
    {
        public string EMailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public string RegistrationSourceURL { get; set; }
        public string ReferredBy { get; set; }
        public string CustomerReferralToken { get; set; }
        public string UserName { get; set; }
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

        public string getToken()
        {
            return this.Token;
        }

        public void setToken(string Token)
        {
            this.Token = Token;
        }

        public User()
        {
            EMailAddress = "";
            Password = "";
        }

        public User(string EMail, string Password)
        {
            this.EMailAddress = EMail;
            this.Password = Password;
        }

        public string ToJSON()
        {
            string json = String.Format("{{ \"EMail\" : \"{0}\", \"Password\" : \"{1}\"}}", EMailAddress, Password);
            return json;
        }
    }

    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string FormattedAddress { get; set; }
        public float Latitude { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public float Longitude { get; set; }
        public string PostalCode { get; set; }
        public string StateProvince { get; set; }
        public string StateProvinceCode { get; set; }
        public string SubAdministrativeArea { get; set; }

        override public string ToString()
        {
            return String.Format("{0}, {1}, {2} {3}", Line1, City, StateProvince, PostalCode);
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
}
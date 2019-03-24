using System;
using System.Collections.Generic;

namespace GolfNowAPI{

    public class Facility
    {
        public double Distance { get; set; }
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public double WeightRanking { get; set; }
        public Address Address { get; set; }
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public int FacilityFlags { get; set; }
        public string FullSizeImagePath { get; set; }
        public object GiftThisCourseUrl { get; set; }
        public int GolfRange { get; set; }
        public string Information { get; set; }
        public bool IsActive { get; set; }
        public bool IsMultiCourse { get; set; }
        public string PhoneNumber { get; set; }
        public List<object> ProductAccessCollection { get; set; }
        public List<object> Tags { get; set; }
        public string TeeTimePolicy { get; set; }
        public string ThumbnailImagePath { get; set; }
        public double TimeZoneOffset { get; set; }
        public string WebsiteAddress { get; set; }

        override public string ToString()
        {
            return String.Format("Name: {0}\nID: {1}\nAddress: {2}\nDistance: {3}\nPhone Number: {4}\nWebsite: {5}", Name, ID, Address.ToString(), Distance, PhoneNumber, WebsiteAddress);
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
            return String.Format("{0}, {1}, {2} {3}",Line1,City,StateProvince,PostalCode);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

///<author>Andrew Millward</author>

namespace testAWSLambda
{
    //Record objects that will hold information returned from and sent to the backend API

    public class RegionCurrency
    {
        public string CurrencyCode { get; set; }
        public double Value { get; set; }
    }

    public class DiscountsApplied
    {
        public RegionCurrency Amount { get; set; }
        public string DiscountName { get; set; }
        public int DiscountType { get; set; }
    }

    public class SinglePlayerPrice
    {
        public RegionCurrency DueAtCourse { get; set; }
        public RegionCurrency DueAtCourseItem { get; set; }
        public RegionCurrency DueOnline { get; set; }
        public RegionCurrency DueOnlineItem { get; set; }
        public RegionCurrency GreensFees { get; set; }
        public RegionCurrency Ipo { get; set; }
        public RegionCurrency MarketRate { get; set; }
        public RegionCurrency SalesTaxTotal { get; set; }
        public RegionCurrency TaxesAndFees { get; set; }
        public RegionCurrency TotalDue { get; set; }
        public RegionCurrency TotalDueItem { get; set; }
        public RegionCurrency TransactionFeesTotal { get; set; }
    }

    public class Rate
    {
        public bool AcceptCreditCard { get; set; }
        public int Attributes { get; set; }
        public int DiscountPercent { get; set; }
        public List<DiscountsApplied> DiscountsApplied { get; set; }
        public int FacilityFlags { get; set; }
        public int GolfRange { get; set; }
        public int HoleCount { get; set; }
        public int IpoDiscountPercent { get; set; }
        public bool IsHotDeal { get; set; }
        public int PlayerRule { get; set; }
        public List<object> ProductAccessList { get; set; }
        public string RateName { get; set; }
        public int RateSetTypeId { get; set; }
        public List<string> RateTagCodes { get; set; }
        public SinglePlayerPrice SinglePlayerPrice { get; set; }
        public int TeeTimeRateID { get; set; }
        public string Transportation { get; set; }
    }

    public class TeeTime
    {
        public int FacilityID { get; set; }
        public DateTime PlayDateUtc { get; set; }
        public DateTime Time { get; set; }
        public Rate DisplayRate { get; set; }
        public bool HasHotDealRate { get; set; }
        public bool HasMoreRates { get; set; }
        public int PlayerRule { get; set; }
        public List<Rate> Rates { get; set; }
    }

    public class TeeTimeRateSelection
    {
        public int FacilityID { get; set; }
        public int RateID { get; set; }
        
        public TeeTimeRateSelection(int facilityID, int rateID)
        {
            FacilityID = facilityID;
            RateID = rateID;
        }
    }

    public class Due
    {
        public List<InvoiceItem> Items { get; set; }
        public Summary Summary { get; set; }
    }

    public class InvoiceItem
    {
        public double Discount { get; set; }
        public double Original { get; set; }
        public double SalesTax { get; set; }
        public double SubTotal { get; set; }
        public double Total { get; set; }
        public string DisplayText { get; set; }
        public bool IsTaxIncluded { get; set; }
        public string Key { get; set; }
        public int Type { get; set; }
    }

    public class InvoiceSubItem
    {
        public List<Detail> Details { get; set; }
        public RegionCurrency Total { get; set; }
    }

    public class Detail
    {
        public RegionCurrency Amount { get; set; }
        public string Key { get; set; }
    }

    public class Summary
    {
        public double Discount { get; set; }
        public double Original { get; set; }
        public double SalesTax { get; set; }
        public double SubTotal { get; set; }
        public double Total { get; set; }
    }

    public class PolicyItem
    {
        public string Details { get; set; }
        public string Key { get; set; }
    }

    public class Pricing
    {
        public RegionCurrency DueAtCourse { get; set; }
        public InvoiceSubItem DueAtCourseItem { get; set; }
        public RegionCurrency DueOnline { get; set; }
        public InvoiceSubItem DueOnlineItem { get; set; }
        public RegionCurrency GreensFees { get; set; }
        public RegionCurrency Ipo { get; set; }
        public RegionCurrency MarketRate { get; set; }
        public RegionCurrency SalesTaxTotal { get; set; }
        public RegionCurrency TaxesAndFees { get; set; }
        public RegionCurrency TotalDue { get; set; }
        public InvoiceSubItem TotalDueItem { get; set; }
        public RegionCurrency TransactionFeesTotal { get; set; }
    }

    public class TeeTimesResponse
    {
        public bool LimitReached { get; set; }
        public List<TeeTime> TeeTimes { get; set; }
        public int TotalTeeTimes { get; set; }
    }

    public class FacilitiesResponse
    {
        public List<Facility> Facilities { get; set; }

        public FacilitiesResponse()
        {
            Facilities = new List<Facility>();
        }

        public FacilitiesResponse(List<Facility> f)
        {
            Facilities = f;
        }

        public string[] GetIDs()
        {
            string[] fIds = new string[Facilities.Count];
            for (int i = 0; i < Facilities.Count; i++)
            {
                fIds[i] = Facilities[i].ID.ToString();
            }
            return fIds;
        }
    }

    public class RateInvoiceResponse
    {
        public int FacilityID { get; set; }
        public DateTime PlayDateUtc { get; set; }
        public DateTime Time { get; set; }
        public string CurrencyCode { get; set; }
        public List<DiscountsApplied> DiscountsApplied { get; set; }
        public Due DueAtCourse { get; set; }
        public Due DueOnline { get; set; }
        public DateTime EstimatedChargeDate { get; set; }
        public int FacilityFlags { get; set; }
        public int GroupSizeLimit { get; set; }
        public bool HasMembershipDiscount { get; set; }
        public int HoleCount { get; set; }
        public bool IsHotDeal { get; set; }
        public bool IsPayNow { get; set; }
        public bool IsReservationRestricted { get; set; }
        public bool IsScheduledPaymentEnabled { get; set; }
        public bool IsSplitReservationEnabled { get; set; }
        public int PlayerCount { get; set; }
        public int PlayerRule { get; set; }
        public List<PolicyItem> PolicyItems { get; set; }
        public Pricing PreDiscountPricing { get; set; }
        public Pricing Pricing { get; set; }
        public object PricingByNumberOfPlayers { get; set; }
        public List<object> ProductAccessList { get; set; }
        public object ProductTypeIds { get; set; }
        public object ProfileSummary { get; set; }
        public object PromoCodeApplied { get; set; }
        public object PromoCodeResults { get; set; }
        public string PromoCodeSuccessMessage { get; set; }
        public string RateName { get; set; }
        public int RateSetTypeId { get; set; }
        public List<string> RateTagCodes { get; set; }
        public object ReservationGroup { get; set; }
        public object RestrictedRateMessage { get; set; }
        public string TeeTimeNotes { get; set; }
        public object TeeTimeOffer { get; set; }
        public int TeeTimeRateID { get; set; }
        public string TermsAndConditions { get; set; }
        public Due TotalDue { get; set; }
        public RegionCurrency TotalReservationPrice { get; set; }
        public bool TransactionFeeWaived { get; set; }
        public string Transportation { get; set; }
        public int WorryFreeEligibilityStatus { get; set; }
    }

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
    }

    public abstract class ApiArgs
    {
        public enum TYPES { City, Zip, Invoice, Login }
        public abstract TYPES TYPE { get; }
    }

    public class TeesByCityArgs : ApiArgs
    {
        public override TYPES TYPE { get; } = TYPES.City;
        public string CityName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Time { get; set; }
        public string NumOfPlayers { get; set; }

        public TeesByCityArgs(string cityName, string state, string country, string time, string numOfPlayers)
        {
            CityName = cityName;
            State = state;
            Country = country;
            Time = time;
            NumOfPlayers = numOfPlayers;
        }
    }

    public class TeesByZipArgs : ApiArgs
    {
        public override TYPES TYPE { get; } = TYPES.Zip;
        public string ZipCode { get; set; }
        public string Proximity { get; set; }
        public string Time { get; set; }
        public string NumOfPlayers { get; set; }

        public TeesByZipArgs(string zipCode, string prox, string time, string numOfPlayers)
        {
            ZipCode = zipCode;
            Proximity = prox;
            Time = time;
            NumOfPlayers = numOfPlayers;
        }
    }

    public class RateInvoiceArgs : ApiArgs
    {
        public override TYPES TYPE { get; } = TYPES.Invoice;
        public int FacilityID { get; set; }
        public int RateID { get; set; }
        public string NumOfPlayers { get; set; }

        public RateInvoiceArgs(int facilityID, int rateID, string numOfPlayers)
        {
            FacilityID = facilityID;
            RateID = rateID;
            NumOfPlayers = numOfPlayers;
        }
    }

    public class LoginArgs : ApiArgs
    {
        public override TYPES TYPE { get; } = TYPES.Login;
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginArgs (string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}

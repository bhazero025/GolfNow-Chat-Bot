using System.Collections.Generic;
using GolfNowAPI;

namespace GolfNowAPIHandler{
    public class DiscountsApplied
    {
        public RegionCurrency Amount { get; set; }
        public string DiscountName { get; set; }
        public int DiscountType { get; set; }
    }

    public class DisplayRate
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
        public object ProductAccessList { get; set; }
        public string RateName { get; set; }
        public int RateSetTypeId { get; set; }
        public List<string> RateTagCodes { get; set; }
        public SinglePlayerPrice SinglePlayerPrice { get; set; }
        public int TeeTimeRateID { get; set; }
        public string Transportation { get; set; }
    }

    public class RegionCurrency
    {
        public string CurrencyCode { get; set; }
        public double Value { get; set; }
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
}
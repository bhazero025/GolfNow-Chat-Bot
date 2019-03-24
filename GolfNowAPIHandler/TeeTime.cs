using System;
using System.Collections.Generic;
using GolfNowAPIHandler;

namespace GolfNowAPI
{
    public class TeeTime
    {
        public int FacilityID { get; set; }
        public DateTime PlayDateUtc { get; set; }
        public DateTime Time { get; set; }
        public DisplayRate DisplayRate { get; set; }
        public bool HasHotDealRate { get; set; }
        public bool HasMoreRates { get; set; }
        public int PlayerRule { get; set; }
        public List<Rate> Rates { get; set; }
    
        override public string ToString()
        {
            return string.Format("Tee Time Rate ID: {0}\nFacility ID: {1}\nTime: {2}\nPlayerRule: {3}", DisplayRate.TeeTimeRateID, FacilityID, Time.ToString(), PlayerRule);
        }

        public string GetPrice()
        {
            return DisplayRate.SinglePlayerPrice.TotalDue.Value.ToString();
        }
    }
}
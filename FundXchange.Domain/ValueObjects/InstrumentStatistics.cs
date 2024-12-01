using System;
using FundXchange.Domain.Base;

namespace FundXchange.Domain.ValueObjects
{
    public class InstrumentStatistics : InstrumentBase
    {
        public double RP { get; set; }
        public double Movement { get; set; }
        public double PercentageMoved { get; set; }
        public string ShortName { get; set; }
        public string DateStamp { get; set; }
        public string Time { get; set; }
        public string MovementIndicator { get; set; }
        public string Currency { get; set; }

        public DateTime Date
        {
            get
            {
                //string[] dateSplit = DateStamp.Split('/');
                if (String.IsNullOrEmpty(Time))
                    Time = "00:00";
                string[] timeSplit = Time.Split(':');

                //DateTime date = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]), Convert.ToInt32(timeSplit[0]), Convert.ToInt32(timeSplit[1]), 0);
                DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(timeSplit[0]), Convert.ToInt32(timeSplit[1]), 0);

                return date;
            }
        }
    }
}

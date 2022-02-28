using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ListenerAPI.Models
{
    public class Common
    {
        public DateTime epochUTCtoReadableUTC(string epochUTC)
        {
            long epoch = Convert.ToInt64(epochUTC) + 19800;

            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return UnixEpoch + TimeSpan.FromMilliseconds((epoch) * 1000);

        }

        public string ConvertValue(string input, string Formula)
        {
            string[] arrData;
            string sensorValue = string.Empty;

            if (Formula.Contains('*'))
            {
                arrData = Formula.Split('*');

                sensorValue = Convert.ToString(Convert.ToDouble(input) * Convert.ToDouble(arrData[1]));
            }
            else if (Formula.Contains('/'))
            {
                arrData = Formula.Split('/');

                sensorValue = Convert.ToString(Convert.ToDouble(input) / Convert.ToDouble(arrData[1]));
            }
            else if (Formula.Contains('+'))
            {
                arrData = Formula.Split('+');

                sensorValue = Convert.ToString(Convert.ToDouble(input) + Convert.ToDouble(arrData[1]));
            }
            else if (Formula.Contains('-'))
            {
                arrData = Formula.Split('-');

                sensorValue = Convert.ToString(Convert.ToDouble(input) - Convert.ToDouble(arrData[1]));
            }
            return sensorValue;
        }

        public string EpochTimeStamp(string Time)
        {
            string Date = new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(Time)).ToShortDateString();
            //string DateTime = Date.ToString();
            DateTime EpochTime = Convert.ToDateTime(Date);
            TimeSpan t = EpochTime - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            string Timestamp = secondsSinceEpoch.ToString();
            return Timestamp;
        }
        public double ConvertToUnixTimestamp()
        {
            DateTime date = DateTime.Now;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);


            DateTime dt = new DateTime();
            dt = Convert.ToDateTime(date.ToString("MM-dd-yyyy HH:mm")).ToUniversalTime();

            //.ToUniversalTime() 
            TimeSpan diff = dt - origin;
            return Math.Floor(diff.TotalSeconds);
        }


    }


    public class CommanResponse
    {
        public string sts { get; set; }
        public string msg { get; set; }
    }

  
}
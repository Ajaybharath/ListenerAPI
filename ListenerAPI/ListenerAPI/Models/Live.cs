using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ListenerAPI.Models
{
    public class Live
    {
    }

    public class LiveResp
    {
        public int pollingFrequency { get; set; }
    }


    public class SensorLive
    {
        public string deviceNo { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string time { get; set; }
    }

    public class SensorLiveResp
    {
        public string sts { get; set; }
        public List<SensorLive> live { get; set; }
    }

    public class rawdata
    {
        public string data { get; set; }
        public string time { get; set; }
    }
}
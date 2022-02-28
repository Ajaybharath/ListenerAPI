using ListenerAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ListenerAPI.Controllers
{
    [RoutePrefix("Rawdata")]
    public class RawdataController : ApiController
    {
        SqlHelper sH = new SqlHelper();

        [HttpPost]
        [Route("Data")]
        public dynamic rawdata()
        {
            List<rawdata> lstdata = new List<rawdata>();
            try
            {

                sH.InitializeDataConnecion();

                DataSet ds = sH.GetDatasetByCommand("GET_RAWDATA");

                sH.CloseConnection();

                if (ds != null)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        rawdata data = new rawdata();

                        data.data = Convert.ToString(ds.Tables[0].Rows[i]["DeviceData"]);
                        data.time = Convert.ToString(ds.Tables[0].Rows[i]["ReceivedTime"]);

                        lstdata.Add(data);
                    }
                }

                return lstdata;
            }
            catch (Exception ex)
            {
                ex = null;

                return lstdata;
            }
        }
    }
}

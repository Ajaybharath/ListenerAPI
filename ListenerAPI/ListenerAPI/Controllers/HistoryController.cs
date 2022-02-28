using ListenerAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ListenerAPI.Controllers
{
    [RoutePrefix("History")]
    public class HistoryController : ApiController
    {
        SqlHelper sH = new SqlHelper();
        CommanResponse cr = new CommanResponse();
        Common cm = new Common();


        [Route("Data")]
        [HttpPost]
        public dynamic Data(dynamic data)
        {

            LiveResp resp = new LiveResp();

            try
            {



                //Insert into Rawdata
                try
                {
                    sH.InitializeDataConnecion();

                    sH.AddParameterToSQLCommand("@DeviceData", SqlDbType.VarChar);
                    sH.SetSQLCommandParameterValue("@DeviceData", Convert.ToString(data));

                    DataSet ds = sH.GetDatasetByCommand("INSERT_RawData");

                    sH.CloseConnection();

                }
                catch (Exception exLive)
                {
                    exLive = null;

                }



                string HData = Convert.ToString(data);

                HData = Convert.ToString(HData).Replace("\r\n", "");
                HData = Convert.ToString(HData).Replace(" ", "");
                HData = HData.Substring(1, HData.Length - 2);//[{"abc"}]

                HData = HData.Replace("},{", "|");

                string[] DataArr = HData.Split('|');


                for (int ig = 0; ig < DataArr.Length; ig++)
                {
                    if (DataArr[ig] != "")
                    {
                        DataTable dt = new DataTable();


                        bool Insert = false;

                        dt.Columns.Add("SensorType", typeof(string));
                        dt.Columns.Add("SensorId", typeof(string));
                        dt.Columns.Add("SensorValue", typeof(string));
                        dt.Columns.Add("SensorUom", typeof(string));
                        dt.Columns.Add("SensorShow", typeof(string));
                        dt.Columns.Add("SensorIcon", typeof(string));

                        string Data = DataArr[ig];

                        string deviceId = string.Empty;

                        string[] arrDeviceId;

                        string shortCode = string.Empty;

                        Data = Convert.ToString(Data).Replace("\r\n", "");
                        Data = Data.Replace("{", "");
                        Data = Data.Replace("}", "");
                        Data = Data.Replace("\"", "");
                        string[] ArrData = Data.Split(',');

                        string[] arrSnr;

                        string sensorTime = string.Empty;


                        DataSet dsFormat = new DataSet();

                        bool hasTms = false;

                        try
                        {
                            for (int i = 0; i < ArrData.Length; i++)
                            {
                                if (ArrData[i].ToLower().Contains("deviceid"))
                                {
                                    arrSnr = ArrData[i].Split(':');
                                    deviceId = arrSnr[1].Trim(' ');

                                    arrDeviceId = deviceId.Split('_');
                                    shortCode = arrDeviceId[0] + "_";

                                    //getting hardware data format

                                    sH.InitializeDataConnecion();

                                    sH.AddParameterToSQLCommand("@ShortCode", SqlDbType.VarChar);
                                    sH.SetSQLCommandParameterValue("@ShortCode", shortCode);

                                    dsFormat = sH.GetDatasetByCommand("Get_HWDataFormat");

                                    sH.CloseConnection();

                                    continue;
                                }
                                if (ArrData[i].ToLower().Contains("tms"))
                                {
                                    hasTms = true;
                                    arrSnr = ArrData[i].Split(':');
                                    sensorTime = arrSnr[1].Trim(' ');

                                    if (sensorTime.Length > 10)
                                    {
                                        sensorTime = sensorTime.Substring(0, 10);
                                    }
                                    else if (sensorTime.Length == 0)//tms:""
                                    {
                                        sensorTime = Convert.ToString(cm.ConvertToUnixTimestamp());
                                    }


                                    //if ((Convert.ToDouble(cm.ConvertToUnixTimestamp()) > Convert.ToDouble(sensorTime))
                                    //    && (Convert.ToDouble(cm.ConvertToUnixTimestamp()) - Convert.ToDouble(sensorTime)) > 5270469)//5270469 2 months difference in sec
                                    //{
                                    //    Insert = false;
                                    //    break;
                                    //}

                                    //if ((Convert.ToDouble(cm.ConvertToUnixTimestamp()) < Convert.ToDouble(sensorTime))
                                    //  && (Convert.ToDouble(sensorTime) - Convert.ToDouble(cm.ConvertToUnixTimestamp())) > 3600)//3600 1 Hr ahead
                                    //{
                                    //    Insert = false;
                                    //    break;
                                    //}



                                    Insert = true;

                                    dt.Rows.Add(arrSnr[0].Trim(' '), arrSnr[0].Trim(' '),
                                                sensorTime, "", "", "");
                                }
                                else
                                {

                                    arrSnr = ArrData[i].Split(':');

                                    string convertionFact = string.Empty;
                                    string isBitWise = string.Empty;
                                    string faultValue = string.Empty;
                                    string DisValue = string.Empty;

                                    bool isExists = false;

                                    try
                                    {
                                        convertionFact = dsFormat.Tables[0].AsEnumerable().Where(g => g.Field<string>("DataKey") == arrSnr[0].Trim(' '))
                                                                                          .Select(r => r.Field<string>("DataKey")).FirstOrDefault().ToString();
                                        isExists = true;
                                    }
                                    catch (Exception exHW)
                                    {
                                        exHW = null;

                                        isExists = false;
                                    }


                                    if (isExists)
                                    {
                                        try
                                        {
                                            convertionFact = dsFormat.Tables[0].AsEnumerable().Where(g => g.Field<string>("DataKey") == arrSnr[0].Trim(' '))
                                                                                               .Select(r => r.Field<string>("Convertion")).FirstOrDefault().ToString();

                                        }
                                        catch (Exception ex)
                                        {
                                            ex = null;
                                        }

                                        try
                                        {

                                            faultValue = dsFormat.Tables[0].AsEnumerable().Where(g => g.Field<string>("DataKey") == arrSnr[0].Trim(' '))
                                                                                              .Select(r => r.Field<string>("exceptionVal")).FirstOrDefault().ToString();

                                        }
                                        catch (Exception ex)
                                        {
                                            ex = null;
                                        }

                                        try
                                        {

                                            DisValue = dsFormat.Tables[0].AsEnumerable().Where(g => g.Field<string>("DataKey") == arrSnr[0].Trim(' '))
                                                                                          .Select(r => r.Field<string>("disableVal")).FirstOrDefault().ToString();

                                        }
                                        catch (Exception ex)
                                        {
                                            ex = null;
                                        }

                                        try
                                        {
                                            isBitWise = dsFormat.Tables[0].AsEnumerable().Where(g => g.Field<string>("DataKey") == arrSnr[0].Trim(' '))
                                                                                               .Select(r => r.Field<string>("SensorName")).FirstOrDefault().ToString();


                                        }
                                        catch (Exception ex)
                                        {
                                            ex = null;
                                        }

                                        string val = string.Empty;

                                        if (!string.IsNullOrEmpty(convertionFact))
                                        {
                                            val = cm.ConvertValue(arrSnr[1].Trim(' '), convertionFact);
                                        }
                                        else
                                        {
                                            val = arrSnr[1].Trim(' ');
                                        }



                                        if (isBitWise == "17")
                                        {
                                            string binary = Convert.ToString(Convert.ToInt32(val), 2);

                                            SqlHelper sHBw = new SqlHelper();


                                            sHBw.InitializeDataConnecion("IoTMainData");

                                            sHBw.AddParameterToSQLCommand("@ShortCode", SqlDbType.VarChar);
                                            sHBw.SetSQLCommandParameterValue("@ShortCode", shortCode);

                                            sHBw.AddParameterToSQLCommand("@DataKey", SqlDbType.VarChar);
                                            sHBw.SetSQLCommandParameterValue("@DataKey", Convert.ToString(arrSnr[0].Trim(' ')));


                                            DataSet dsbitFormat = sHBw.GetDatasetByCommand("GET_BitWiseFormat");

                                            sHBw.CloseConnection();

                                            while (binary.Length < dsbitFormat.Tables[0].Rows.Count)
                                            {
                                                binary = "0" + binary;
                                            }

                                            for (int gr = 0; gr < dsbitFormat.Tables[0].Rows.Count; gr++)
                                            {
                                                string bitVal = binary[(binary.Length - 1) - gr].ToString();

                                                if (!string.IsNullOrEmpty(Convert.ToString(dsbitFormat.Tables[0].Rows[gr]["BitName"])))
                                                {
                                                    dt.Rows.Add(Convert.ToString(dsbitFormat.Tables[0].Rows[gr]["BitName"]),
                                                        Convert.ToString(dsbitFormat.Tables[0].Rows[gr]["BitName"]),
                                                    bitVal, "", "", "");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (((Convert.ToInt32(isBitWise) >= 130 && Convert.ToInt32(isBitWise) <= 151)) ||
                                (Convert.ToInt32(isBitWise) == 83 || ((Convert.ToInt32(isBitWise) >= 112 && Convert.ToInt32(isBitWise) <= 115))))
                                            {
                                                //130 latitude
                                                //131 longitude
                                                //132 to 141 Alpha Numerics
                                                //142 to 151 Counter

                                                dt.Rows.Add(arrSnr[0].Trim(' '), arrSnr[0].Trim(' '),
                                                                  val, "", "", "");
                                            }
                                            else
                                         if (!string.IsNullOrEmpty(faultValue) && Convert.ToDouble(faultValue) == Convert.ToDouble(val))
                                            {
                                                dt.Rows.Add(arrSnr[0].Trim(' '), arrSnr[0].Trim(' '),
                                                    "FAL", "", "", "");

                                            }
                                            else if (!string.IsNullOrEmpty(DisValue) && Convert.ToDouble(DisValue) == Convert.ToDouble(val))
                                            {
                                                dt.Rows.Add(arrSnr[0].Trim(' '), arrSnr[0].Trim(' '),
                                                    "DIS", "", "", "");

                                            }
                                            else
                                            {
                                                if (val.Contains('.'))
                                                {
                                                    val = Math.Round(Convert.ToDouble(val), 2).ToString();
                                                }

                                                if (deviceId == "WTH_E868E7C80ECC01" && val == "0")
                                                {

                                                }
                                                else
                                                {
                                                    dt.Rows.Add(arrSnr[0].Trim(' '), arrSnr[0].Trim(' '),
                                                                                                           val, "", "", "");
                                                }

                                            }
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception exMain)
                        {
                            exMain = null;
                            resp.pollingFrequency = 0;
                        }

                        //

                        ////insert into live data
                        try
                        {

                            if (!string.IsNullOrEmpty(deviceId) && Insert)
                            {

                                if (!hasTms)
                                {
                                    sensorTime = Convert.ToString(cm.ConvertToUnixTimestamp());


                                    dt.Rows.Add("tms", "tms",
                                                sensorTime, "", "", "");
                                }

                                //Daily Table Creation Based on Day Change
                                //string Date = new DateTime(1970, 1, 1).AddSeconds(Convert.ToDouble(sensorTime)).ToShortDateString();
                                //string DateTime = Date.ToString();
                                //DateTime EpochTime = Convert.ToDateTime(DateTime);
                                //TimeSpan t = EpochTime - new DateTime(1970, 1, 1);
                                //int secondsSinceEpoch = (int)t.TotalSeconds;
                                //string Timestamp = secondsSinceEpoch.ToString();
                                string Timestamp = cm.EpochTimeStamp(sensorTime);
                                string Tablename = deviceId + "_" + Timestamp;

                                sH.InitializeDataConnecion();

                                sH.AddParameterToSQLCommand("@Sensor", SqlDbType.Structured);
                                sH.SetSQLCommandParameterValue("@Sensor", dt);

                                sH.AddParameterToSQLCommand("@DeviceId", SqlDbType.VarChar);
                                sH.SetSQLCommandParameterValue("@DeviceId", deviceId);

                                sH.AddParameterToSQLCommand("@ShortCode", SqlDbType.VarChar);
                                sH.SetSQLCommandParameterValue("@ShortCode", shortCode);

                                sH.AddParameterToSQLCommand("@DeviceTime", SqlDbType.VarChar);
                                sH.SetSQLCommandParameterValue("@DeviceTime", sensorTime);

                                sH.AddParameterToSQLCommand("@Tablename", SqlDbType.VarChar);
                                sH.SetSQLCommandParameterValue("@Tablename", Tablename);


                                DataSet ds = sH.GetDatasetByCommand("Insert_DeviceHistoryData");

                                sH.CloseConnection();

                                if (ds.Tables[0].Rows.Count <= 0)
                                {
                                    resp.pollingFrequency = 1;
                                }
                                else if (string.IsNullOrEmpty(Convert.ToString(ds.Tables[0].Rows[0]["pollingFrequency"])))
                                {
                                    resp.pollingFrequency = 1;
                                }
                                else
                                {
                                    resp.pollingFrequency = Convert.ToInt32(ds.Tables[0].Rows[0]["pollingFrequency"]);
                                    if (resp.pollingFrequency == 400)
                                    {
                                        resp.pollingFrequency = 0;
                                    }
                                }

                            }
                        }
                        catch (Exception exLive)
                        {
                            exLive = null;
                            sH.CloseConnection();
                            resp.pollingFrequency = 0;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                ex = null;
                resp.pollingFrequency = 0;
            }
            return resp;
        }


        public double ConvertToUnixTimestamp()
        {
            DateTime date = DateTime.Now;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);


            DateTime dt = new DateTime();

            dt = Convert.ToDateTime(date.ToString("MM-dd-yyyy HH:mm:ss")).ToUniversalTime();

            //.ToUniversalTime() 
            TimeSpan diff = dt - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;


namespace SpinItApp
{
    class CloudConnection
    {
        private readonly string Addscore = "https://spinitphone.azurewebsites.net/api/AddScore?code=2saJmlAt9DLg89XD8eLQDohEnaRR6l88atP0a7DJPlpI60u/vGUG6w==";
        private readonly string Getallscores = "https://spinitphone.azurewebsites.net/api/getalluserscores?code=istA1ulaOfmfB0tg9cogPl4sqRbQc4cKCVS4FUQt29LG42SeqmASPg==";
        private readonly string Getxscores = "https://spinitphone.azurewebsites.net/api/getUserScoresByAmount?code=3kFNARoWkv30BWbJY5iBqiRSoW9/OJpCghdsZ/1hy7cbxFO3lL4tcg==";

        public string getDate(JsonItem item)
        {
            string date = item.Date.Split('T')[0];
            string[] dateVals = date.Split('-');
            return dateVals[2] + "." + dateVals[1] + "." + dateVals[0][2] + dateVals[0][3];
        }



        public void AddScore(string name,string time, string date ,string distance )
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Addscore);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"name\":\""+ name + "\"," +"\"time\":\""+ time + "\"," +"\"date\":\"" + date + "\"," + "\"distance\":\"" + distance + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JsonWrap resp = JsonConvert.DeserializeObject<JsonWrap>(result);
                if (resp.m_StringValue != "OK"){
                    throw new Exception() ; //note : catch the exception! - maybe no internet connection (!)
                }
            }
        }


        public List<JsonItem> GetAllScores(string name)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Getallscores);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"name\":\"" + name + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JsonWrap resp = JsonConvert.DeserializeObject<JsonWrap>(result);
                List<JsonItem> res = JsonConvert.DeserializeObject<List<JsonItem>>(resp.m_StringValue);

                return res;

            }
        }


        public List<JsonItem> GetXScores(string name, int amount )
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Getxscores);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"name\":\"" + name + "\"," + "\"amount\":" + amount + "}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                JsonWrap resp = JsonConvert.DeserializeObject<JsonWrap>(result);


                List<JsonItem> res = JsonConvert.DeserializeObject<List<JsonItem>>(resp.m_StringValue);

                return res;

            }
        }

        public class JsonWrap
        {
            [JsonProperty("m_MaxCapacity")]
            public long m_MaxCapacity { get; set; }

            [JsonProperty("Capacity")]
            public int Capacity { get; set; }

            [JsonProperty("m_StringValue")]
            public string m_StringValue { get; set; }

            [JsonProperty("m_currentThread")]
            public int m_currentThread { get; set; }

        }

        public class JsonItem
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("time")]
            public string Time { get; set; }

            [JsonProperty("date")]
            public string Date { get; set; }

            [JsonProperty("distance")]
            public string Distance { get; set; }

        }


    }

}

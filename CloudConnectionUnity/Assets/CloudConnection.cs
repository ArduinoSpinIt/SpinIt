using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

//put it anywhere - helper connection class 
public class CloudConnection : object {

    private static string addScore = "https://uploadscore.azurewebsites.net/api/HttpTrigger1?code=I7utRaf2lYP2UHWJSXMCsjhC9MOsAXBxjuhoI1VNTNHOYmuw2LiiGg==";
    private static string getScoresPerUser = "https://getscoresperuser.azurewebsites.net/api/HttpTrigger2?code=K1SlyXb/VWQ6XyUjOsFy2/1BjEPlzQDMRccKVTQhNRvLHpAKMlTlhw==";
    private static string getAllBestScores = "";
    private static string getAllScores = "";

    public IEnumerator AddScore(string name, int score, string time, System.Action<string> callback)
    {
        string json = "{\"name\": \""+name+"\",\"score\":"+score+", \"time\":\""+time+"\" }";
        UnityWebRequest request = MakeRequest(addScore, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }

    private UnityWebRequest MakeRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        return uwr;
    }

    public IEnumerator GetScoresPerUser(string name,System.Action<string> callback)
    {
        string json = "{\"name\": \"" + name+"\"}";
        yield return MakeRequest(getScoresPerUser, json);
        UnityWebRequest request = MakeRequest(getScoresPerUser, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }

    class JsonItem //{"m_MaxCapacity":long,"Capacity":int,"m_StringValue":string,"m_currentThread":int}
    {
        public long m_MaxCapacity;
        public int Capacity;
        public string m_StringValue;
        public int m_currentThread;
    }
    public class JsonRowList
    {
        public List<JsonRow> rowList;
    }

    public class JsonRow
    {
        public string Name;
        public int Score;
        public string Time;
    }


    public List<JsonRow> GetRowsFromJson(string json)
    {
        json = "{\"rowList\":" + json + "}";
        Debug.Log(json);
        var resultJson = JsonUtility.FromJson<JsonRowList>(json);
        //TODO- check why this returns null
        Debug.Log(resultJson.rowList);
        return resultJson.rowList;
    }



    private string GetReturnValueFromRequest(string json)
    {
        Debug.Log("the json is: " +json);
        var resultJson = JsonUtility.FromJson<JsonItem>(json).m_StringValue;
        Debug.Log("output: " + resultJson);
        return resultJson;
    }




}

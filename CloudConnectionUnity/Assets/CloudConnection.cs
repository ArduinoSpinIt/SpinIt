using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


//put it anywhere - helper connection class 
public class CloudConnection : object {

    private static string addScore = "https://uploadscore.azurewebsites.net/api/HttpTrigger1?code=I7utRaf2lYP2UHWJSXMCsjhC9MOsAXBxjuhoI1VNTNHOYmuw2LiiGg==";
    private static string getScoresPerUser = "";
    private static string getAllBestScores = "";
    private static string getAllScores = "";

    public IEnumerator AddScore(string name, int score, string time, System.Action<string> callback)
    {
        // Debug.Log(name + " " + score + " " +time);
        string json = "{\"name\": \""+name+"\",\"score\":"+score+", \"time\":\""+time+"\" }";
        //var data = PostRequest(addScore, json, callback);
        //Debug.Log(data);
        //yield return GetReturnValueFromRequest(data.ToString());
        yield return PostRequest(addScore, json, callback);
    }

    private IEnumerator PostRequest(string url, string json, System.Action<string> callback)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            callback(GetReturnValueFromRequest(uwr.downloadHandler.text));
        }
    }

    class JsonItem //{"m_MaxCapacity":long,"Capacity":int,"m_StringValue":string,"m_currentThread":int}
    {

        public long m_MaxCapacity;
        public int Capacity;
        public string m_StringValue;
        public int m_currentThread;
    }


    private string GetReturnValueFromRequest(string json)
    {
        var resultJson = JsonUtility.FromJson<JsonItem>(json).m_StringValue;
        return resultJson;
    }




}

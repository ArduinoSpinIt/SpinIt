using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;

//put it anywhere - helper connection class 
public class CloudConnection : object {

    private static string addScore = "https://handlescoresrequests.azurewebsites.net/api/addScore?code=/NGKRrb1DOzK6MX7ZAbEu3M9UaMCGuuFrDcJ3UTDxKM/3/cdCrOFUg==";
    private static string getScoresPerUser = "https://handlescoresrequests.azurewebsites.net/api/getScoresPerUser?code=aT5kP04mPlURcCOrtBAG83L5zWbMrKKRWrxv2fGp3eGYe4lCxHGFUA==";
    private static string getAllXBestScores = "https://handlescoresrequests.azurewebsites.net/api/getAllXBestScores?code=1ylDAja5Eonqyt5dPUGRAczD2D4BoCMavoJavBpwcZCRcCj4W8acPg==";
    private static string getAllScores = "https://handlescoresrequests.azurewebsites.net/api/getAllScores?code=ya6Fz/7QnLyR8G5KX3PEvwZCx89cwnI9xYfr4fD8aj1FlcHjNgJQGw==";
    private static string getUserBestXScores = "https://handlescoresrequests.azurewebsites.net/api/getUserBestXScores?code=4GOE7FsVh15z9ArvzLZ22AvtgxS/6QtyANBPkY7QEGJKvwCoEHcGeg==";
    private static string cleanDB = "https://handlescoresrequests.azurewebsites.net/api/cleanDB?code=MFJmQdZTbGuJjp6jsax5zgcNBTqU9JZu/G/40SB22sGCMGpFpFTc/A==";

    public IEnumerator CleanDB(System.Action<string> callback)
    {
        UnityWebRequest request = MakeRequest(cleanDB);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }

    public IEnumerator AddScore(string name, int score, string time, System.Action<string> callback)
    {
        string json = "{\"name\": \""+name+"\",\"score\":"+score+", \"time\":\""+time+"\" }";
        UnityWebRequest request = MakeRequest(addScore, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }

    private UnityWebRequest MakeRequest(string url, string json = "{}")
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
        //yield return MakeRequest(getScoresPerUser, json);
        UnityWebRequest request = MakeRequest(getScoresPerUser, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }


    public IEnumerator GetAllScores(System.Action<string> callback)
    {
        UnityWebRequest request = MakeRequest(getAllScores);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); 
        }
    }

    public IEnumerator GetBestXScores(int x, System.Action<string> callback)
    {
        string json = "{\"amount\": " + x + "}";
        UnityWebRequest request = MakeRequest(getAllXBestScores, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
        }
        else
        {
            callback(GetReturnValueFromRequest(request.downloadHandler.text)); //need to return OK if all is good
        }
    }

    public IEnumerator GetUserBestXScores(string name,int x, System.Action<string> callback)
    {
        string json = "{\"name\": \"" + name + "\",\"amount\": " + x + "}";
        UnityWebRequest request = MakeRequest(getUserBestXScores, json);
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.Log("Error While Sending: " + request.error);
            callback("ERROR");
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

    public class JsonRow
    {
        public string Name;
        public int Score;
        public string Time;
    }

    public List<string> SplitJsonArray(string json)
    {
        string no_first = json.Split('[')[1];
        string no_second = no_first.Split(']')[0];
        string[] new1 = no_second.Split(new string[] { "},{" }, StringSplitOptions.None);
        List<string> jsonList = new List<string>();
        foreach (string item in new1)
        {
            string new_item = item;
            if (item[0] != '{')
            {
                new_item = "{" + new_item;
            }
            if (new_item[new_item.Length - 1] != '}')
            {
                new_item = new_item + "}";
            }
            jsonList.Add(new_item);
        }
        return jsonList;

    }

    public List<JsonRow> GetRowsFromJson(string json)
    {
        List<string> list2 = SplitJsonArray(json);
        List<JsonRow> list = new List<JsonRow>();
        foreach (string item in list2)
        {
            JsonRow item2 = JsonUtility.FromJson<JsonRow>(item);
            list.Add(item2);
        }

        return list;
    }



    private string GetReturnValueFromRequest(string json)
    {
        var resultJson = JsonUtility.FromJson<JsonItem>(json).m_StringValue;
        return resultJson;
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Remoting.Lifetime;
using System.Security.Policy;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using UnityEngine.Networking;

public class NewBehaviourScript : MonoBehaviour {

   
    //private string getScore;

    /*public string AddUserScore(string name, int score, string time)
    {
        var data = new NameValueCollection();

        string json = "{\"name\": \"check\",\"score\":10, \"time\":\"sunday-test\" }";
        var response = Add(json, addScore);
        return response;

    }

    private static string Add(string json, string url)
    {

        using (var wb = new WebClient())
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";
            Stream stream = req.GetRequestStream();

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            stream.Write(buffer, 0, buffer.Length);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            return new StreamReader(res.GetResponseStream()).ReadToEnd();
        }
    }*/
    /*
    IEnumerator PostRequest(string url, string json, Action<string> callback)
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
            callback( uwr.downloadHandler.text);
        }
    }
    */

        // Use this for initialization
    void Start () {
        CloudConnection connection = new CloudConnection();
        
        string temp1 = "3333";
        string temp2 = "22222";
        int temp3 = 5;
     
        StartCoroutine(connection.AddScore((temp1),(temp3),(temp2),(text) =>
        {
            if (text != "Error")
            {
                Debug.Log("the answer is:" + text);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
            ));

        StartCoroutine(connection.GetScoresPerUser((temp1), (text) =>
        {
            if (text != "Error")
            {
                CloudConnection.JsonRow[] array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Score;
                Debug.Log("the second answer is:" + it);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
        ));



    }


    // Update is called once per frame
    void Update () {
		
	}
}

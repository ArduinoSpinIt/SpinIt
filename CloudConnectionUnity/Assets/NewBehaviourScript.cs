
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour {

  
    void Start () {
        CloudConnection connection = new CloudConnection();
        
        string temp1 = "3333";
        string temp2 = "22222";
        int temp3 = 5;
     //example how to add score
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
        //example how to get all scores of a user
        StartCoroutine(connection.GetScoresPerUser((temp1), (text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Score; // example for getting value
                Debug.Log("the second answer is:" + it);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
        ));

        int amount = 2;
        //example how TO get x best scores from all users
        StartCoroutine(connection.GetBestXScores((amount), (text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Score; // example for getting value
                Debug.Log("the THIRD answer is:" + it);
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

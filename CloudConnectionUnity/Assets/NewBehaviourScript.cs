
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour {

  
    void Start () {
        CloudConnection connection = new CloudConnection();
        
        string temp1 = "Tal";
        string temp4 = "Aviv";
        string temp2 = "24.12 8:00";
        string temp3 = "1:00";
        int round = 5;
        
     //example how to add score
        
        StartCoroutine(connection.AddScore((temp1),(temp3),(round),(temp2),(text) =>
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
        StartCoroutine(connection.GetScoresPerUser((temp1),(round), (text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Time; // example for getting value
                Debug.Log("the second answer is:" + it);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
        ));
    
        int amount = 2;
        int rounds = 3;
        //example how TO get x best scores from all users
        StartCoroutine(connection.GetBestXScores((amount),(rounds), (text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Time; // example for getting value
                Debug.Log("the THIRD answer is:" + it);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
        ));
    
        //example how to get all scores
        StartCoroutine(connection.GetAllScores((text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Time; // example for getting value
                Debug.Log("the FORTH answer is:" + it);
            }
            else
            {
                Debug.Log("there was an error");
            }
        }
       ));
    
        int numbertoreturn = 5;
        StartCoroutine(connection.GetUserBestXScores((temp1),(numbertoreturn),(text) =>
        {
            if (text != "Error")
            {
                List<CloudConnection.JsonRow> array = connection.GetRowsFromJson(text);
                string it = "";
                it = it + array[0].Time; // example for getting value
                Debug.Log("the FORTH answer is:" + it);
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

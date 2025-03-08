using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Gato
{   //{"actual":0,"round":0,"score1":0,"board":[0,0,0,0,0,0,0,0,0]}
    public int actual;
    public int round;
    public string score1;
    public string score2;
    public int[] board;

    override public string ToString ()
    {
        string data = "actual:" + actual + "\nround:" + round + "\nscore1" + score1 + "\nscore2" + score2 + "\nboard\n";
        foreach (var item in board)
        {
            data += item + "\n";
        }
        return data;
    }
}

public class GameManager : MonoBehaviour
{   
    public string id = "id1";

    void Start()
    {
        StartCoroutine(GetStatus());
    }

    IEnumerator GetStatus()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=2&id="+id);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            Gato gato = JsonUtility.FromJson<Gato>(www.downloadHandler.text);

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;

            Debug.Log(gato.ToString());
        }

        
    }

    IEnumerator Tirada()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=3&id="+id+"&pos=4");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            if (www.downloadHandler.text == "error")
            {
                // Se hace rojito
            }

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

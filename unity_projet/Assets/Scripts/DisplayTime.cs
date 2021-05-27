using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTime : MonoBehaviour
{
    public GameObject textBox;
    private float time;
    private float curTime;
    // Start is called before the first frame update
    void Start()
    {
        curTime= time = (int)Time.time;
        time = curTime - UrlStorage.time;
        int seconds = (int)(time % 60);
        int minutes = (int)(time / 60) % 60;
        int hours = (int)(time / 3600) % 24;

        string timerString = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
        textBox.GetComponent<Text>().text = timerString;
    }

    // Update is called once per frame
    
}

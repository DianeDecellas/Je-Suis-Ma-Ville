using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time = (int)Time.time;

        int seconds = (int)(time % 60);
        int minutes = (int)(time / 60)%60;
        int hours = (int)(time / 3600) % 24;

        string timerString = string.Format("{0:0}:{1:00}:{2:00}",hours, minutes,seconds);
        GetComponent<Text>().text = timerString;

        
    }
}

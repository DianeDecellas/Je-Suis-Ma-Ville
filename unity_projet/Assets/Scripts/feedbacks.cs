using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class feedbacks : MonoBehaviour
{
    
    private float time;
    private float curTime;
    // Start is called before the first frame update
    void Start()
    {
        

        //we collect information about the user's walk
        //here we collect the date of the walk
        DateTime dt = DateTime.Now;
        //string date = dt.Day + "/" + dt.Month + "/" + dt.Year;
        string date = dt.ToString("dd/MM/yyyy");
        Debug.Log(date);

        //here we collect at what time the walk has ended
        //string hour = dt.Hour + ":"+ dt.Minute + ":"+dt.Second;
        string hour = dt.ToString("HH:mm:ss");
        Debug.Log(hour);

        //here we collect the name of the walk
        string nom = UrlStorage.idBalade;
        Debug.Log(nom);
        
        //here we collect the duration of the walk
        curTime = time = (int)Time.time;
        time = curTime - UrlStorage.time;
        string duree = texteDuree(time);
        Debug.Log(duree);

        //here we collect the grade that the user has given to the walk
        int note = donnerNote();
        Debug.Log("start:"+note);
    }

     void LateUpdate()
    {
        int note = donnerNote();
        Debug.Log("update:" + note);
    }

    string texteDuree(float time)
    {
        int seconds = (int)(time % 60);
        int minutes = (int)(time / 60) % 60;
        int hours = (int)(time / 3600) % 24;

        string duree = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
        return duree;
    }

    int donnerNote()
    {
        //we instantiate each button. Each button has a corresponding number
        GameObject button1 = transform.Find("Button1").gameObject;
        GameObject button2 = transform.Find("Button2").gameObject;
        GameObject button3 = transform.Find("Button3").gameObject;
        GameObject button4 = transform.Find("Button4").gameObject;
        GameObject button5 = transform.Find("Button5").gameObject;

        //we instantiate each full star. They are all initially switched off
        GameObject star1 = transform.Find("star1").gameObject;
        GameObject star2 = transform.Find("star2").gameObject;
        GameObject star3 = transform.Find("star3").gameObject;
        GameObject star4 = transform.Find("star4").gameObject;
        GameObject star5 = transform.Find("star5").gameObject;
        
        //we define a default grade
        //UrlStorage.note = 3;
        
        /*we define each button's behaviour. When the user clicks on a button, 
         * the corresponding number of stars are switched on. 
         * UrlStorage.note gets the value of the corresponding grade.
        */
        void click1()
        {
            star1.SetActive(true);
            star2.SetActive(false);
            star3.SetActive(false);
            star4.SetActive(false);
            star5.SetActive(false);
            UrlStorage.note = 1;
        }
        button1.GetComponent<Button>().onClick.AddListener(click1);

        void click2()
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(false);
            star4.SetActive(false);
            star5.SetActive(false);
            UrlStorage.note = 2;
        }
        button2.GetComponent<Button>().onClick.AddListener(click2);

        void click3()
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
            star4.SetActive(false);
            star5.SetActive(false);
            UrlStorage.note = 3;
        }
        button3.GetComponent<Button>().onClick.AddListener(click3);

        void click4()
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
            star4.SetActive(true);
            star5.SetActive(false);
            UrlStorage.note = 4;
        }
        button4.GetComponent<Button>().onClick.AddListener(click4);

        void click5()
        {
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
            star4.SetActive(true);
            star5.SetActive(true);
            UrlStorage.note = 5;
        }
        button5.GetComponent<Button>().onClick.AddListener(click5);

        int note = UrlStorage.note;
        Debug.Log("donnerNote:"+note);
        return note;
        
    }
}

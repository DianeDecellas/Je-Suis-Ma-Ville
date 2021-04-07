using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;

public class testmusicxml : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip myClip;
    public string Url;
    void Start()
    {
        //load xml doc
        XmlDocument baladeData = new XmlDocument();
        WWW data = new WWW(Url);
        while (!data.isDone)
        {

        }
        string xmlData = data.text;
        baladeData.LoadXml(xmlData);

        //extract audiopath from xml
        XmlNode CurrentNode = baladeData.FirstChild;
        XmlNode music = CurrentNode.NextSibling;
        
        Debug.Log(music.InnerText);
        string audiopath = music.InnerText; //visiblement il lit direct le InnerText dans <music></music>et pas dans <url></url>
                                            //XmlNode url = music.FirstChild;


        //read audiosource
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(GetAudioClip(audiopath));
        Debug.Log("Starting to download the audio...");
    }

    IEnumerator GetAudioClip(string audiopath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audiopath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;
                audioSource.Play();
                Debug.Log("Audio is playing.");
            }
        }
    }
}

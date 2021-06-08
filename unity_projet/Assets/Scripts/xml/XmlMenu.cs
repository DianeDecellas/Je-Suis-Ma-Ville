﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class XmlMenu : MonoBehaviour
{
    string UrlMenu = "https://prefigurations.com/je_suis_ma_ville/balades/MenuTest.xml";
    //string UrlMenu = "https://admiring-easley-a2c27c.netlify.app/MenuTest.xml"; //Url où je conserve le menu
    UrlStorage urlstorage;                                                                           // Start is called before the first frame update
    void readXmlMenu(XmlNode menu)
    {
        GameObject g;
        int i = 0;
        XmlNode currentBalade = menu.FirstChild;
        GameObject baladeTemplate = transform.Find("baladeChoiceTemplate").gameObject;
        while (currentBalade != null)
        {
            i++;
            XmlNode title = currentBalade.FirstChild;
            XmlNode duration = title.NextSibling;
            string url = duration.NextSibling.InnerText;
            currentBalade = currentBalade.NextSibling;
            g = Instantiate(baladeTemplate, transform);
            g.transform.GetChild(0).GetComponent<Text>().text = title.InnerText;
            g.transform.GetChild(1).GetComponent<Text>().text = duration.InnerText;
            Debug.Log("Tite - Outer = " + title.OuterXml);
            Debug.Log("Name =" + title.Name);
            GameObject curg = g.transform.parent.GetChild(i).gameObject;
            void displayBalade()
            {
                XmlDocument displayXML = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
                WWW dataDisplay = new WWW(url); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
                while (!dataDisplay.isDone)
                {
                    ///cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
                }

                displayXML.LoadXml(dataDisplay.text); ///on charge le texte de data dans le doc xml baladeData

                XmlElement root = displayXML.DocumentElement;   //Get follow node
                Debug.Log("The root element is:" + root.Name);
                XmlNode descriptif = root.FirstChild;
                XmlNode resume = descriptif.SelectSingleNode("resume");
                Debug.Log(resume.InnerText);
                
                RectTransform Rt = curg.GetComponent<RectTransform>();
                Rt.sizeDelta = new Vector2(Rt.sizeDelta.x, Rt.sizeDelta.y + 200f);
                curg.transform.GetChild(2).GetComponent<Text>().text=resume.InnerText;
                curg.transform.GetChild(2).gameObject.SetActive(true);
                curg.GetComponent<Button>().onClick.AddListener(loadBalade);
            }
            void loadBalade()
            {
                UrlStorage.url = url;
                Debug.Log("Cliquée : "+url);
                SceneManager.LoadScene("scene_finale");
            }
            curg.GetComponent<Button>().onClick.AddListener(displayBalade);
        }
        baladeTemplate.SetActive(false);
    }

    void Start()
    {
        XmlDocument baladeData = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
        WWW data = new WWW(UrlMenu); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
        while (!data.isDone)
        {
            ///cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
        }

        baladeData.LoadXml(data.text); ///on charge le texte de data dans le doc xml baladeData


        XmlNode encoding = baladeData.FirstChild; ///le premier fils de baladeData est l'encoding
        Debug.Log(encoding.InnerText);
        XmlNode menu = encoding.NextSibling; ///je ne sais pas encore pourquoi mais il faut skip 1 autres fils avant d'arriver au contenu
        Debug.Log(menu.InnerText);
        readXmlMenu(menu);

       

    }

    
}

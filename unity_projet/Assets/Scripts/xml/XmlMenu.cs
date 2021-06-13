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
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class XmlMenu : MonoBehaviour
{
    string UrlMenu;
    string nomMenu = "MenuV2.xml";
    string dossierBalades = "XML_Balades";
    string FtpURL = "https://prefigurations.com/je_suis_ma_ville/balades/";
    //string UrlMenu = "https://admiring-easley-a2c27c.netlify.app/MenuTest.xml"; //Url où je conserve le menu
    UrlStorage urlstorage;                                                                           // Start is called before the first frame update
    public Sprite[] spriteArray;
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
            XmlNode idNode = title.NextSibling;
            string idBalade = idNode.InnerText;
            string urlXmlBalade = FtpURL+dossierBalades+"/"+idBalade+"/"+idBalade+".xml";
            XmlNode colorNode = idNode.NextSibling;
            string colorButton = colorNode.InnerText.Trim(new char[] {'\n', '\r', ' '});
            XmlNode durationNode = colorNode.NextSibling;
            XmlNode descriptionNode = durationNode.NextSibling;
            string description = descriptionNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
            XmlNode departureNode = descriptionNode.NextSibling;
            string departure = departureNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
            XmlNode thumbnailNode = departureNode.NextSibling;
            string thumbnailURL = FtpURL + "/"+dossierBalades+"/" + idBalade + "/" + thumbnailNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
            //Message à Emile : si tu veux te servir de la couleur du cadre, de la vignette de la balade, tu peux le faire ici
            
            Debug.Log("idBalade : " + idBalade
                + "\nurlXmlBalade : " + urlXmlBalade
                + "\ncolor : " + colorButton
                + "\ndescription : " + description
                + "\nlieu de départ : " + departure
                + "\nURL vignette : " + thumbnailURL);

            currentBalade = currentBalade.NextSibling;
            g = Instantiate(baladeTemplate, transform);
            switch (colorButton)
            {
                case "rose":
                    g.GetComponent<Image>().sprite = spriteArray[0];
                    break;
                case "jaune":
                    g.GetComponent<Image>().sprite = spriteArray[1];
                    break;
                case "vert":
                    g.GetComponent<Image>().sprite = spriteArray[3];
                    break;
                case "rouge":
                    g.GetComponent<Image>().sprite = spriteArray[2];
                    break;

            }
            
           

            g.transform.GetChild(0).GetComponent<Text>().text = title.InnerText;
            g.transform.GetChild(1).GetComponent<Text>().text = durationNode.InnerText;
            Vector2 buttonSizeVect =g.GetComponent<RectTransform>().sizeDelta;

            GameObject parent = g.transform.parent.gameObject;
            GameObject curg = parent.transform.GetChild(i).gameObject;
            Debug.Log(curg);
            Debug.Log("gni :" + FtpURL + "/"+dossierBalades+"/" + idBalade + "/");
            void clickBalade()
            {
                MenuButtonLoadBoolean b = curg.GetComponent<MenuButtonLoadBoolean>();
                if (!b.getLoadStatus())
                {
                    /* //A dégager si jamais ça fonctionne
                    XmlDocument displayXML = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
                    WWW dataDisplay = new WWW(urlXmlBalade); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
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
                    */
                    foreach (Transform child in parent.transform)
                    {
                        child.transform.GetChild(2).gameObject.SetActive(false);
                        RectTransform rect = child.GetComponent<RectTransform>();
                        rect.sizeDelta = buttonSizeVect;
                        child.GetComponent<MenuButtonLoadBoolean>().disableLoad();

                    }
                    b.enableLoad();
                    RectTransform Rt = curg.GetComponent<RectTransform>(); //Get the rect transform of object
                    Rt.sizeDelta = new Vector2(Rt.sizeDelta.x, Rt.sizeDelta.y + 200f); //make button bigger
                    curg.transform.GetChild(2).GetComponent<Text>().text = description; //get text to display
                    curg.transform.GetChild(2).gameObject.SetActive(true); //make the summary visible
                    
                }
                else {
                    UrlStorage.urlBaladeDirectory = FtpURL+"/"+dossierBalades+"/"+idBalade+"/";
                    Debug.Log("Cliquée : " + urlXmlBalade);
                    UrlStorage.idBalade = idBalade;

                    SceneManager.LoadScene("scene_finale");
                }
            }
            void loadBalade()
            {
                UrlStorage.urlBaladeDirectory = FtpURL+"/"+dossierBalades+"/"+idBalade+"/";
                Debug.Log("Cliquée : "+urlXmlBalade);
                UrlStorage.idBalade = idBalade;

                SceneManager.LoadScene("scene_finale");
            }
            curg.GetComponent<Button>().onClick.AddListener(clickBalade);
            Debug.Log(curg.GetComponent<Button>());
        }
        baladeTemplate.SetActive(false);
    }

    void Start()
    {
        UrlMenu = FtpURL + nomMenu;
        Debug.Log("UrlMenu = " + UrlMenu);
        XmlDocument listeBalades = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
        WWW data = new WWW(UrlMenu); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
        while (!data.isDone)
        {
            ///cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
        }

        listeBalades.LoadXml(data.text); ///on charge le texte de data dans le doc xml baladeData
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);

        }
        while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        XmlNode encoding = listeBalades.FirstChild; ///le premier fils de baladeData est l'encoding
        Debug.Log(encoding.InnerText);
        XmlNode dtdNode = encoding.NextSibling;
        XmlNode menu = dtdNode.NextSibling; ///je ne sais pas encore pourquoi mais il faut skip 1 autres fils avant d'arriver au contenu
        Debug.Log(menu.InnerText);
        readXmlMenu(menu);

       

    }

    
}

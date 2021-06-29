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
    //nouveaux répertoires de travail
    string nomMenu = "Menu.xml";     
    string dossierBalades = "ressources_balades";

    string FtpURL = "https://prefigurations.com/je_suis_ma_ville/balades/";
    Storage urlstorage;                                                                           // Start is called before the first frame update
    public Sprite[] spriteArray;

    /// <summary>
    /// This method is used to parse walks from the Menu XML and display them on screen.
    /// 
    /// For each walk, we retrieve the various informations relative to the balad, namely the title, the id, the color, the duration, the description, the departure and the thumbnail's URL.
    /// Each string is trimmed to delete artifacts from the XML (such as the \n and \r which could hinder the use of these informations).
    /// We feed all of these informations into the walk's button
    /// 
    /// Each walk's button displays the walk's name and its duration. When clicked, the button expands to display the thumbnail and the place where the walk begins.
    /// If clicked a second time, the button launches the walk by loading the scene scenefinale.
    /// If the user clicks elsewhere while a button is expanded, it collapses.
    /// </summary>
    /// <param name="menu">The XmlNode that is the root of the list containing the differents walks' data</param>
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
            string idBalade = idNode.InnerText.Trim(new char[] { ' ', '\n', '\r' });
            string urlXmlBalade = FtpURL+dossierBalades+"/"+idBalade+"/"+idBalade+".xml";
            XmlNode colorNode = idNode.NextSibling;
            string colorButton = colorNode.InnerText.Trim(new char[] {'\n', '\r', ' '});
            XmlNode durationNode = colorNode.NextSibling;
            XmlNode descriptionNode = durationNode.NextSibling;
            string description = descriptionNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
            XmlNode departureNode = descriptionNode.NextSibling;
            string departure = departureNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
            XmlNode thumbnailNode = departureNode.NextSibling;
            string thumbnailURL = dossierBalades+"/" + idBalade + "/" + thumbnailNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
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
                case "rouge":
                    g.GetComponent<Image>().sprite = spriteArray[2];
                    break;
                case "vert":
                    g.GetComponent<Image>().sprite = spriteArray[3];
                    break;

            }
            
           

            g.transform.GetChild(0).GetComponent<Text>().text = title.InnerText;
            g.transform.GetChild(1).GetComponent<Text>().text = durationNode.InnerText;
            Vector2 buttonSizeVect =g.GetComponent<RectTransform>().sizeDelta;

            GameObject parent = g.transform.parent.gameObject;
            GameObject curg = parent.transform.GetChild(i).gameObject;
            Debug.Log(curg);
            Debug.Log("gni :" + FtpURL +dossierBalades+"/" + idBalade + "/");
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
                    curg.transform.GetChild(2).Find("Text").GetComponent<Text>().text = description + "\n\n"+ "Lieu de départ: "+departure; //get text to display
                    curg.transform.GetChild(2).gameObject.SetActive(true); //make the summary visible
                    RawImage raw = curg.transform.GetChild(2).Find("Image").Find("RawImage").GetComponent<RawImage>() ;
                    if (thumbnailNode.InnerText.Trim(new char[] { '\n', '\r', ' ' }) != "" )
                    {
                        StartCoroutine(DownloadImage(thumbnailURL,raw));
                    }
                    else
                    {
                        Debug.Log("Il n'y a pas d'image");
                        raw.transform.parent.gameObject.SetActive(false);
                    }
                }
                else {
                    Storage.urlBaladeDirectory = FtpURL+"/"+dossierBalades+"/"+idBalade+"/";
                    Debug.Log("Cliquée : " + urlXmlBalade);
                    Storage.idBalade = idBalade;

                    SceneManager.LoadScene("scene_finale",LoadSceneMode.Single);
                }
            }
            curg.GetComponent<Button>().onClick.AddListener(clickBalade);
            Debug.Log(curg.GetComponent<Button>());
        }
        baladeTemplate.SetActive(false);
    }

    private IEnumerator DownloadImage(string imageName, RawImage YourRawImage)
    {
        string imageUrl = FtpURL + imageName;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        Debug.Log(imageUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            float imageRatio = (float)((DownloadHandlerTexture)request.downloadHandler).texture.width / (float)((DownloadHandlerTexture)request.downloadHandler).texture.height;
            YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            YourRawImage.transform.GetComponent<AspectRatioFitter>().aspectRatio = imageRatio;
        }
    }
    /// <summary>
    /// The function launched upon loading the menu screen.
    /// 
    /// The walk is downloaded from the FTP using the URL of the file containing the Menu XML +  the Menu XML
    /// Once that file is retreived, the permissions are asked to use the camera and the GPS.
    /// We then extract the walks list and launch readXmlMenu for it to display the walks thumbnails.
    /// <see cref="readXmlMenu(XmlNode)"/>
    /// </summary>
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

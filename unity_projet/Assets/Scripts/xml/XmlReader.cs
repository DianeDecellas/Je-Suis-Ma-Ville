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

namespace XmlReader
{
    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    public static class ButtonExtension
    {
        public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
        {
            button.onClick.AddListener(delegate () {
                OnClick(param);
            });
        }
    }
    class XmlReader : MonoBehaviour
    {
        
        
        
        void ItemClicked(bool correct)

        {
            if (correct)
            {
                Debug.Log("oui");
                
            }
            else
            {
                Debug.Log("non");
            }
        }
        void Valider(int itemIndex)
        {
            Debug.Log("Bravo! C'est juste!!");
            
        }
        void Refuser (int itemIndex)
        {
            Debug.Log("Bzzt! C'est faux!");
        }
        public void creerEtapeTexte(XmlNode etape, XmlNode question, XmlNode reponse)
        {
            GameObject buttonTemplate = transform.GetChild(2).gameObject;
            GameObject g;
            GameObject titre = transform.GetChild(0).gameObject;
            GameObject suivant = transform.parent.GetChild(1).gameObject;

            titre.transform.GetChild(0).GetComponent<Text>().text = question.InnerText;
            
            GameObject input = transform.GetChild(1).gameObject;
            
            Debug.Log("etape cree");
            g = buttonTemplate;

            g.transform.GetChild(1).GetComponent<Text>().text = reponse.InnerText;
            input.SetActive(true);
            void ValiderTexte(){
                if (input.GetComponent < InputField >().text.ToString()== reponse.InnerText.ToString())
                {
                    Debug.Log("Bravo!! C'est Juste!");
                }
                else
                {
                    Debug.Log("Bzzt!! C'est faux!");
                }
            }
            
            g.transform.GetChild(0).GetComponent<Text>().text = "valider";
            g.GetComponent<Button>().onClick.AddListener( ValiderTexte);
            void EtapeSuivante()
            {
                
                EtapeReader(etape.NextSibling);
            }
            suivant.GetComponent<Button>().onClick.RemoveAllListeners();
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        }

        public void creerQrCode(XmlNode etape)
        {
            GameObject suivant = transform.parent.GetChild(1).gameObject;
            void EtapeSuivante()
            {
                

                EtapeReader(etape.NextSibling);
            }
            suivant.GetComponent<Button>().onClick.RemoveAllListeners();
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        }
        
        public void creerQCM(XmlNode etape, XmlNode question, XmlNode reponsev, XmlNode reponsef1, XmlNode reponsef2, XmlNode reponsef3)
        {
            GameObject buttonTemplate = transform.GetChild(2).gameObject;
            GameObject g;
            GameObject g2;
            GameObject g3;
            GameObject g4;
            GameObject suivant = transform.parent.GetChild(1).gameObject;
            GameObject titre = transform.GetChild(0).gameObject;

            titre.transform.GetChild(0).GetComponent<Text>().text = question.InnerText;

            GameObject input = transform.GetChild(1).gameObject;
            input.SetActive(false);
            Debug.Log("etape cree");
            g = buttonTemplate;

            g.transform.GetChild(1).GetComponent<Text>().text = reponsev.InnerText;
            g2 = Instantiate(buttonTemplate, transform);
            g3 = Instantiate(buttonTemplate, transform);
            g4 = Instantiate(buttonTemplate, transform);
            g2.transform.GetChild(1).GetComponent<Text>().text = reponsef1.InnerText;
            g3.transform.GetChild(1).GetComponent<Text>().text = reponsef2.InnerText;
            g4.transform.GetChild(1).GetComponent<Text>().text = reponsef3.InnerText;
            void EtapeSuivante()
            {
                Destroy(g2.gameObject);
                Destroy(g3);
                Destroy(g4);
                EtapeReader(etape.NextSibling);
            }
            g.transform.GetChild(0).GetComponent<Text>().text = "valider";
            g.GetComponent<Button>().AddEventListener(0, Valider);
            g2.GetComponent<Button>().AddEventListener(0, Refuser);
            g3.GetComponent<Button>().AddEventListener(0, Refuser);
            g4.GetComponent<Button>().AddEventListener(0, Refuser);
            suivant.GetComponent<Button>().onClick.RemoveAllListeners();
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
            
        }
        public void EtapeReader(XmlNode CurrentNode)
        {
            string typeEtape= CurrentNode.InnerText;

            Debug.Log(typeEtape);
            if (typeEtape == "question texte")
            {
                XmlNode etape = CurrentNode.NextSibling;
                XmlNode titre = etape.FirstChild;
                XmlNode navigation = titre.NextSibling;
                Debug.Log(navigation.InnerText);
                XmlNode epreuve = navigation.NextSibling;
                Debug.Log(epreuve.InnerText);

                XmlNode texte = epreuve.FirstChild;
                XmlNode imagetexte = texte.FirstChild;
                XmlNode question = imagetexte.NextSibling;
                XmlNode reponse = question.NextSibling;


                creerEtapeTexte(etape,question, reponse);
            }

            else if (typeEtape == "QCM")
            {
                XmlNode etape = CurrentNode.NextSibling;
                XmlNode titre = etape.FirstChild;
                XmlNode navigation = titre.NextSibling;
                Debug.Log(navigation.InnerText);
                XmlNode epreuve = navigation.NextSibling;
                Debug.Log(epreuve.InnerText);

                XmlNode qcm = epreuve.FirstChild;
                XmlNode question = qcm.FirstChild;
                XmlNode reponsev = question.NextSibling;
                XmlNode reponsef1 = reponsev.NextSibling;
                XmlNode reponsef2 = reponsef1.NextSibling;
                XmlNode reponsef3 = reponsef2.NextSibling;
                creerQCM(etape, question, reponsev, reponsef1,reponsef2,reponsef3);
            }

            else if (typeEtape == "QR Code")
            {
                Debug.Log( "QrCode");
                XmlNode etape = CurrentNode.NextSibling;
                creerQrCode(etape);
            }

            if (typeEtape=="Conclusion")
            {
                Debug.Log("c'est fini");
                
            }
            
            

        }
        
        public string Url;
        private void Start()
        {
            XmlDocument baladeData = new XmlDocument();
            WWW data = new WWW(Url);
            while (!data.isDone)
            {
                
            }
            string xmlData = data.text;
            baladeData.LoadXml(data.text);
            

            XmlNode encoding = baladeData.FirstChild;
            XmlNode test2 = encoding.NextSibling;
            XmlNode test3 = test2.NextSibling;
            
            XmlNode descriptif = test3.FirstChild;
            XmlNode etape1 = descriptif.NextSibling;
            
            EtapeReader(etape1);
            
        }
    }
}
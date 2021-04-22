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


    
    public class XmlReader : MonoBehaviour
    {
    public QRCodeReader scriptQrCode;
     
        
        void ItemClicked(bool correct) ///fonction qui permet de savoir si une réponse est juste ou non

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
        void Valider(int itemIndex) ///fonction qui affiche "Bravo! C'est juste" dans la console
        {
            Debug.Log("Bravo! C'est juste!!");
            
        }
        void Refuser (int itemIndex) ///fonction qui affiche que c'est faux dans la console
        {
            Debug.Log("Bzzt! C'est faux!");
        }
        public void creerEtapeTexte(XmlNode etape, XmlNode question, XmlNode reponse) ///fonction qui crée une etape texte avec question et reponse
        {
            GameObject buttonTemplate = transform.GetChild(2).gameObject; ///on récupère la template de bouton formée dans l'UI, il s'agit du 3eme fils de l'objet auquel le code est attaché "panel"
            buttonTemplate.SetActive(true);

            GameObject g; ///on crée un objet g
            GameObject titre = transform.GetChild(0).gameObject; ///on récupère le 1er fils de panel et on l'appelle titre
            GameObject suivant = transform.parent.GetChild(1).gameObject; ///le 2ele fils du parent de panel est le bouton suivant

            titre.transform.GetChild(0).GetComponent<Text>().text = question.InnerText; ///on récupère le texte contenu dans titre et on le remplace par le texte de la question
            
            GameObject input = transform.GetChild(1).gameObject; ///le deuxième fils de panel est une zone d'entrée de texte, on l'appelle input
            
            Debug.Log("etape cree"); ///on affiche etape cree dans la console
            g = buttonTemplate; ///g est le template de bouton défini plus haut

            g.transform.GetChild(1).GetComponent<Text>().text = reponse.InnerText; ///on remplace le contenu texte de g par le texte de la réponse, c'est pour tricher et tester plus facilement il faudra enlever cette ligne
            input.SetActive(true); ///on rend input actif : il faut qu'il soit affiché dans l'ui
            void ValiderTexte(){
                if (input.GetComponent < InputField >().text.ToString()== reponse.InnerText.ToString()) ///si le texte en entrée est égal au texte attendu
                {
                    Debug.Log("Bravo!! C'est Juste!");
                }
                else
                {
                    Debug.Log("Bzzt!! C'est faux!");
                }
            }
            
            g.transform.GetChild(0).GetComponent<Text>().text = "valider"; ///on remplace le texte du bouton par valider
            g.GetComponent<Button>().onClick.AddListener( ValiderTexte); ///le bouton déclenche la fonction ValiderTexte quand on appuye dessus
            void EtapeSuivante()
            {
            Debug.Log("odkour");
                EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
            }
            suivant.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);///on applique la fonction EtapeSuivante au bouton suivant
        }

    public void creerQrCode(XmlNode etape) ///fonction creer qrCode
        {
        Debug.Log("83");
            GameObject suivant = transform.parent.GetChild(1).gameObject;
        GameObject input = transform.GetChild(1).gameObject;
            GameObject boutonValider = transform.GetChild(2).gameObject;
            GameObject qrReader = transform.GetChild(3).gameObject;
        input.SetActive(false);
        Debug.Log("87");
            boutonValider.SetActive(false);
        Debug.Log("89");
            qrReader.SetActive(true);


        Debug.Log("On rentre dans la boucle infinie");
        while(!scriptQrCode.qrcodeValide){
            Debug.Log("J'attends...");
        } //(G) A faire : mettre un late update pour optimiser parce que là, c'est schlag !
        Debug.Log("C'est bon, le QR Code est valide !!");
        scriptQrCode.qrcodeValide = false;
        
        void EtapeSuivante()
            {
                qrReader.SetActive(false);
                EtapeReader(etape.NextSibling);
            }
            suivant.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève les anciennes fonctions du bouton avant d'ajouter la fonction etape suivante appropriée
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        }


        
        public void creerQCM(XmlNode etape, XmlNode question, XmlNode reponsev, XmlNode reponsef1, XmlNode reponsef2, XmlNode reponsef3) ///reponsev est la réponse juste, reponsefi pour i entre 1 et 3 les fausses
        {
            GameObject buttonTemplate = transform.GetChild(2).gameObject; ///on récupère le template de bouton, 3eme fils du panel
            buttonTemplate.SetActive(true);

            GameObject g;
            GameObject g2;
            GameObject g3;
            GameObject g4; ///creation des différents objets
            GameObject suivant = transform.parent.GetChild(1).gameObject; ////bouton suivant
            GameObject titre = transform.GetChild(0).gameObject;

            titre.transform.GetChild(0).GetComponent<Text>().text = question.InnerText;

            GameObject input = transform.GetChild(1).gameObject;
            input.SetActive(false); ///on désactive la barre d'entrée de texte pour qu'elle n'aparaisse pas dans l'ui
            Debug.Log("etape cree");
            g = buttonTemplate;

            g.transform.GetChild(1).GetComponent<Text>().text = reponsev.InnerText;
            g2 = Instantiate(buttonTemplate, transform); ///on copie le template de bouton pour chaque réponse fausse
            g3 = Instantiate(buttonTemplate, transform);
            g4 = Instantiate(buttonTemplate, transform);
            g2.transform.GetChild(1).GetComponent<Text>().text = reponsef1.InnerText; ///on remplit le texte de chaque bouton avec celui de la réponse correspondante
            g3.transform.GetChild(1).GetComponent<Text>().text = reponsef2.InnerText;
            g4.transform.GetChild(1).GetComponent<Text>().text = reponsef3.InnerText;
            void EtapeSuivante()
            {
                Destroy(g2); ///on détruit les boutons superflus, et on en conserve un pour toujours avoir le template disponible
                Destroy(g3);
                Destroy(g4);
                EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de cette étape
            }
            g.transform.GetChild(0).GetComponent<Text>().text = "valider";
            g.GetComponent<Button>().AddEventListener(0, Valider); ///la réponse juste appelle valider, fen vrai il faudrait enlever les fonctions déjà présentes sur le bouton avant de faire ça...
            g2.GetComponent<Button>().AddEventListener(0, Refuser);///les autres appellent refuser
            g3.GetComponent<Button>().AddEventListener(0, Refuser);
            g4.GetComponent<Button>().AddEventListener(0, Refuser);
            suivant.GetComponent<Button>().onClick.RemoveAllListeners();
            suivant.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
            
        }
        public void EtapeReader(XmlNode CurrentNode)
        {
            string typeEtape= CurrentNode.InnerText; ///on connait le type de l'étape en lisant le texte du noeud en cours (le commentaire)

            Debug.Log(typeEtape);
            if (typeEtape == "question texte")
            {
                XmlNode etape = CurrentNode.NextSibling; ///l'étape en elle même est le frère du commentaire sur le xml que vous avez fouri
                XmlNode titre = etape.FirstChild; ///le premier fils de l'étape est son titre
                XmlNode navigation = titre.NextSibling; ///le deuxième fils de l'étape est la navigation
                Debug.Log(navigation.InnerText); /// on affiche la navigation dans la console, pour vérifier que àa marche
                XmlNode epreuve = navigation.NextSibling; ///l'epreuve est le 3eme fils de l'étape
                Debug.Log(epreuve.InnerText);

                XmlNode texte = epreuve.FirstChild; ///on récup_re le texte de l'épreuve
                XmlNode imagetexte = texte.FirstChild; 
                XmlNode question = imagetexte.NextSibling; ///on récupère la question
                XmlNode reponse = question.NextSibling; ///et la réponse


                creerEtapeTexte(etape,question, reponse); ///on crée une étape à partir de la question et de la réponse
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
                XmlNode reponsef1 = reponsev.NextSibling; ///on récupère toutes les réponses juste et fausses
                XmlNode reponsef2 = reponsef1.NextSibling;
                XmlNode reponsef3 = reponsef2.NextSibling;
                creerQCM(etape, question, reponsev, reponsef1,reponsef2,reponsef3); ///on crée l'étape qcm
            }

            else if (typeEtape == "QR Code")
            {
                Debug.Log( "QrCode");
                XmlNode etape = CurrentNode.NextSibling; ///à remplir pour afficher un QRCode
                creerQrCode(etape);
            }

            if (typeEtape=="Conclusion") ///si on atteint la conclusion alors c'est fini
            {
                Debug.Log("c'est fini");
                
            }
            
            

        }


        
        public string Url;
        private void Start() ///que fait on au démarrage?
        {
            XmlDocument baladeData = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
            WWW data = new WWW(Url); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
            while (!data.isDone)
            {
                ///cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
            }
            
            baladeData.LoadXml(data.text); ///on charge le texte de data dans le doc xml baladeData
            

            XmlNode encoding = baladeData.FirstChild; ///le premier fils de baladeData est l'encoding
            XmlNode test2 = encoding.NextSibling; ///je ne sais pas encore pourquoi mais il faut skip 1 autres fils avant d'arriver au contenu
            XmlNode test3 = test2.NextSibling;///la dedans y'a le contenu
            
            XmlNode descriptif = test3.FirstChild; ///le premier fils de test3 c'est le descriptif de la balade
            XmlNode etape1 = descriptif.NextSibling; ///l'etape1 c'est le premier frere du descriptif
            
            EtapeReader(etape1); ///on lance EtapeReader sur l'etape1
            
        }
    }

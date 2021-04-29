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
    public UpdatePosition gpscalcul;
        
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
        GameObject buttonTemplate = transform.Find("TestButton").gameObject; ///on récupère la template de bouton formée dans l'UI, il s'agit du 3eme fils de l'objet auquel le code est attaché "panel"
        GameObject questionBox = transform.Find("QuestionBox").gameObject; ///on récupère le 1er fils de panel et on l'appelle titre //(G) En fait on renomme en questionBox
        GameObject nextStepButton = transform.parent.Find("NextStepButton").gameObject; ///le 2ele fils du parent de panel est le bouton suivant    
        GameObject input = transform.Find("InputField").gameObject; ///le deuxième fils de panel est une zone d'entrée de texte, on l'appelle input
        Debug.Log("etape cree"); ///on affiche etape cree dans la console

        buttonTemplate.SetActive(true);
        input.SetActive(true); ///on rend input actif : il faut qu'il soit affiché dans l'ui

        questionBox.transform.Find("Text").GetComponent<Text>().text = question.InnerText; ///on récupère le texte contenu dans titre(maintenant questionBox) et on le remplace par le texte de la question

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
        GameObject g; ///on crée un objet g
        g = buttonTemplate; ///g est le template de bouton défini plus haut
        g.transform.Find("Text (1)").GetComponent<Text>().text = reponse.InnerText;//(G) je touche pas à cette ligne si c'est du test  ///on remplace le contenu texte de g par le texte de la réponse, c'est pour tricher et tester plus facilement il faudra enlever cette ligne
        g.transform.Find("Text").GetComponent<Text>().text = "valider"; ///on remplace le texte du bouton par valider
        g.GetComponent<Button>().onClick.AddListener( ValiderTexte); ///le bouton déclenche la fonction ValiderTexte quand on appuye dessus

        void EtapeSuivante()
        {
        Debug.Log("odkour");
            EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
        }
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);///on applique la fonction EtapeSuivante au bouton suivant
    }

    public void creerQrCode(XmlNode etape, XmlNode question, XmlNode reponse) ///fonction creer qrCode
    {
        //(G) il ne reste plus qu'à modifier cette methode pour qu'elle cherche dans le XML la réponse au QRCode attendue, au lieu que ce soit nous qui la passions pour les tests
        //(G) on récupère les GameObject qui définissent la vue
        GameObject nextStepButton = transform.parent.Find("NextStepButton").gameObject;
        GameObject input = transform.Find("InputField").gameObject;
        GameObject testButton = transform.Find("TestButton").gameObject;
        GameObject qrReader = transform.Find("QRReader").gameObject;
        GameObject questionTextBox = transform.Find("QuestionBox").Find("Text").gameObject; //(G) la boite texte contenant la question

        nextStepButton.transform.GetComponent<Button>().interactable = false;
        questionTextBox.transform.GetComponent<Text>().text = question.InnerText; //(G) on met le texte dans la question et on est bons !
        
        //questionTextBox.transform.Text
        input.SetActive(false);
        testButton.SetActive(false);
        qrReader.SetActive(true); //(G) on affiche le QRCodeReader
        scriptQrCode.expectedQrCodeMessage = reponse.InnerText;
        Debug.Log("Creer QRCode : Expected Message = " + scriptQrCode.expectedQrCodeMessage);
        //qrReader.expectedMessage =    //(G) penser à passer le message attendu à qrReader à partir du code XML

        IEnumerator waitForQRCode() //(G) la routine qui est appelée pour attendre que le booléen scriptQrCode.qrCodeValide passe de false à true
        {
            //(G) On ne passe à la suite de l'iterateur / enumerateur waitForQRCode que si le booléen scriptQrCode.qrcodeValide passe à true
            yield return new WaitUntil(() => scriptQrCode.isQrCodeValid); //(G) le petit bout de " () => " permet de transformer la variable scriptQrCode.qrcodeValide en fonction
            void EtapeSuivante() //(G) la fonction qui sera appelée lorsqu'on pressera le bouton "suivant".
            {
                qrReader.SetActive(false); //(G) on désactive l'objet QRReader
                EtapeReader(etape.NextSibling);
            }
            nextStepButton.GetComponent<Button>().interactable = true;
            nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève les anciennes fonctions du bouton avant d'ajouter la fonction etape suivante appropriée
            nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        }

        //(G) La boucle où on attend que le QRCode soit bon.
        Debug.Log("En attente de QRCode");
        StartCoroutine(waitForQRCode());
    }
    public void navigate(XmlNode nav)
    {
        XmlNode imagenav = nav.FirstChild;
        XmlNode instruct = imagenav.NextSibling;
        XmlNode coords = instruct.NextSibling;
        XmlNode x = coords.FirstChild;
        XmlNode y = x.NextSibling;
        Debug.Log(x.InnerText);
        float xprev = float.Parse(x.InnerText, System.Globalization.CultureInfo.InvariantCulture );
        float yprev = float.Parse(y.InnerText.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        gpscalcul.setLocalisationPrevue(xprev, yprev);


    }

        
    public void creerQCM(XmlNode etape, XmlNode question, XmlNode reponsev, XmlNode reponsef1, XmlNode reponsef2, XmlNode reponsef3) ///reponsev est la réponse juste, reponsefi pour i entre 1 et 3 les fausses
    {
        GameObject buttonTemplate = transform.Find("TestButton").gameObject; ///on récupère le template de bouton, 3eme fils du panel
        buttonTemplate.SetActive(true);

        GameObject g;
        GameObject g2;
        GameObject g3;
        GameObject g4; ///creation des différents objets
        GameObject nextStepButton = transform.parent.Find("NextStepButton").gameObject; ////bouton suivant
        GameObject questionBox = transform.Find("QuestionBox").gameObject;
        GameObject input = transform.Find("InputField").gameObject;

        questionBox.transform.Find("Text").GetComponent<Text>().text = question.InnerText;


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
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
            
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
            navigate(navigation);
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
            XmlNode etape = CurrentNode.NextSibling; 
            XmlNode titre = etape.FirstChild;
            XmlNode navigation = titre.NextSibling;
            Debug.Log(navigation.InnerText);
            XmlNode epreuve = navigation.NextSibling;

            XmlNode qrcode = epreuve.FirstChild;
            XmlNode question = qrcode.FirstChild;
            XmlNode reponse = question.NextSibling;
            creerQrCode(etape, question, reponse); // (G) On crée l'étape QRCode
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

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
using System.Linq;
using UnityEngine.SceneManagement;


public class XmlReader : MonoBehaviour
    {
    public DeviceCameraController scriptQrCode;
    public UpdatePosition gpscalcul;
    IEnumerator ecranJuste()
    {
        transform.parent.parent.Find("true").gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        transform.parent.parent.Find("true").gameObject.SetActive(false);

    }
    IEnumerator ecranFaux()
    {
        transform.parent.parent.Find("false").gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        transform.parent.parent.Find("false").gameObject.SetActive(false);

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

    public void creerEtapeInfo(XmlNode etape, XmlNode texte, XmlNode image)
    {
        GameObject info = transform.Find("info").gameObject;
        GameObject rawImageGameObject = info.transform.Find("RawImage").gameObject;
        RawImage rawImage = rawImageGameObject.GetComponent(typeof(RawImage)) as RawImage;
        GameObject texteInfo = info.transform.Find("TextInfo").gameObject;
        GameObject nextStepButton = transform.parent.Find("bottomContainer").Find("NextStepButton").gameObject;
        Debug.Log("etape cree"); ///on affiche etape cree dans la console
        GameObject input = transform.Find("InputField").gameObject;
        GameObject answerButton = transform.Find("TestButton").gameObject;
        texteInfo.GetComponent<Text>().text = texte.InnerText.ToString();
        StartCoroutine(DownloadImage(image.InnerText.ToString(), rawImage));
        input.SetActive(false);
        answerButton.SetActive(false);
        info.SetActive(true);
        nextStepButton.SetActive(true);

        //creation of audio button
        GameObject AudioButton = transform.Find("AudioButton").gameObject;
        GameObject AudioText = transform.Find("AudioButton").Find("Text").gameObject;
        AudioText.GetComponent<Text>().text = "Jouer l'audio";
        AudioButton.SetActive(true);
        //creation of audio source
        GameObject AudioSource = transform.Find("AudioSource").gameObject;
        AudioSource audioSource = AudioSource.GetComponent<AudioSource>();
        //test with a music from the internet
        string audiopath = "https://ciihuy.com/downloads/music.mp3";
        IEnumerator i = audioPlay.GetAudioClip(audiopath, audioSource);
        StartCoroutine(i);
        Debug.Log("Starting to download the audio xmlreader...");
        //how the audio button works: starts audio with first click, then pause or continues with next clicks
        bool AudioActive = false;
        void Audio()
        {
            AudioActive = !AudioActive;
            AudioText.GetComponent<Text>().text = AudioActive ? "Pause" : "Continuer";

            if (AudioActive == true)
            {
                audioPlay.playAudio(audioSource);
            }
            else
            {
                audioPlay.pauseAudio(audioSource);
            }
        }
        AudioButton.GetComponent<Button>().onClick.AddListener(Audio);

        void EtapeSuivante()
        {
            info.SetActive(false);
            EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
            AudioButton.SetActive(false);
            AudioButton.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);

    }

    private IEnumerator DownloadImage(string image, RawImage YourRawImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(image);
        Debug.Log(image);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }


    public void creerEtapeTexte(XmlNode etape, XmlNode question, XmlNode reponse, XmlNode indice) ///fonction qui crée une etape texte avec question et reponse
    {
        GameObject buttonTemplate = transform.Find("TestButton").gameObject; ///on récupère la template de bouton formée dans l'UI, il s'agit du 3eme fils de l'objet auquel le code est attaché "panel"
        GameObject questionBox = transform.Find("QuestionBox").gameObject; ///on récupère le 1er fils de panel et on l'appelle titre //(G) En fait on renomme en questionBox
        GameObject nextStepButton = transform.parent.Find("bottomContainer").Find("NextStepButton").gameObject; ///le 2ele fils du parent de panel est le bouton suivant    
        GameObject input = transform.Find("InputField").gameObject; ///le deuxième fils de panel est une zone d'entrée de texte, on l'appelle input
        GameObject imageParent = transform.Find("ImageParent").gameObject; //(G) The image that will containt the camera
        GameObject hint = transform.parent.Find("bottomContainer").Find("Hint").gameObject;//creation of hint button object
        GameObject textHint = transform.Find("TextHint").gameObject;//creation of hint text
        Debug.Log("etape cree"); ///on affiche etape cree dans la console

        textHint.GetComponent<Text>().text = indice.InnerText.ToString();//initialize textHint content with the XmlNode indice innertext


        buttonTemplate.SetActive(true);
        nextStepButton.GetComponent<Button>().interactable = false; //(G) the Next Step Button is not interactable until the answer is right

        input.SetActive(true); ///on rend input actif : il faut qu'il soit affiché dans l'ui
        textHint.SetActive(false);//the user should not see the hint before their first answer
        hint.SetActive(true);//however they need the hint button

        questionBox.transform.Find("Text").GetComponent<Text>().text = question.InnerText; ///on récupère le texte contenu dans titre(maintenant questionBox) et on le remplace par le texte de la question


        void ValiderTexte(){
            int stringDistance;//the distance between the correct answer and the user's input, calculated according to the Damerau Levenstein formula
            stringDistance = DamerauLevensteinDistance.StringDistance.GetDamerauLevenshteinDistance(input.GetComponent<InputField>().text.ToString(), reponse.InnerText.ToString());
            int threshold;//the threshold for the correct answer, since we allow some misclicks. You are free to change its definition
            threshold = reponse.InnerText.ToString().Length / 3;

            if (stringDistance <= threshold) //if the user's input is accurate enough
            {
                Debug.Log("Bravo!! C'est Juste!");
                StartCoroutine(ecranJuste());
                nextStepButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                Debug.Log("Bzzt!! C'est faux!");
                StartCoroutine(ecranFaux());
                
                Color defaultColor = buttonTemplate.GetComponent<Button>().GetComponent<Image>().color;
                //buttonTemplate.GetComponent<Button>().GetComponent<Image>().color = Color.red; //(G) must find a way to make the change last 5 seconds or so
                //buttonTemplate.GetComponent<Button>().GetComponent<Image>().color = defaultColor;
            }
        }
        GameObject g; ///on crée un objet g
        g = buttonTemplate; ///g est le template de bouton défini plus haut
        g.transform.Find("Text (1)").GetComponent<Text>().text = "Valider";//(G) je touche pas à cette ligne si c'est du test  ///on remplace le contenu texte de g par le texte de la réponse, c'est pour tricher et tester plus facilement il faudra enlever cette ligne
        g.GetComponent<Button>().onClick.AddListener( ValiderTexte); ///le bouton déclenche la fonction ValiderTexte quand on appuye dessus

        void AfficherIndice()
        {
            textHint.SetActive(true);
        }
        hint.GetComponent<Button>().onClick.AddListener(AfficherIndice);//if the user hits the hint button, the game displays the text in textHint

        void EtapeSuivante()
        {
        
            hint.SetActive(false);
            textHint.SetActive(false);
            questionBox.transform.Find("Text").GetComponent<Text>().text = "";
            EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
        }
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);///on applique la fonction EtapeSuivante au bouton suivant
    }

    public void creerQrCode(XmlNode etape, XmlNode question, XmlNode reponse) ///fonction creer qrCode
    {
        //(G) il ne reste plus qu'à modifier cette methode pour qu'elle cherche dans le XML la réponse au QRCode attendue, au lieu que ce soit nous qui la passions pour les tests
        //(G) on récupère les GameObject qui définissent la vue
        GameObject nextStepButton = transform.parent.Find("bottomContainer").Find("NextStepButton").gameObject;
        GameObject input = transform.Find("InputField").gameObject;
        GameObject testButton = transform.Find("TestButton").gameObject;
        GameObject imageParent = transform.Find("ImageParent").gameObject;
        GameObject questionTextBox = transform.Find("QuestionBox").Find("Text").gameObject; //(G) the text box containing the instructions

        nextStepButton.transform.GetComponent<Button>().interactable = false;       //(G) can not go to the next step before having the answer
        questionTextBox.transform.GetComponent<Text>().text = question.InnerText;   //(G) instructions display

        input.SetActive(false);         //(G) input and testButton are set inactive because they are unused during this step
        testButton.SetActive(false);

        scriptQrCode.expectedQrCodeMessage = reponse.InnerText; //(G) Setting the expected answer
        scriptQrCode.isQrCodeValid = false;
        imageParent.SetActive(true);

        Debug.Log("Creer QRCode : Expected Message = " + scriptQrCode.expectedQrCodeMessage);

        IEnumerator waitForQRCode() //(G) this routine is called and waits for the scriptQrCode.isQrCodeValid to be true
        {
            //(G) the Iterator / Enumerator waitForQRCode goes on only when scriptQrCode.qrcodeValide is set to true
            yield return new WaitUntil(() => scriptQrCode.isQrCodeValid); //(G) the " () => " bit transforms the scriptQrCode.qrcodeValide variable into a function
            StartCoroutine(ecranJuste());
            void EtapeSuivante() //(G) this function will be called upon clicking on the nextStepButton button
            {
                //scriptQrCode.Interrupt(); //(G) maybe will be used someday to destroy the imageParent object ? Who knows.
                imageParent.SetActive(false); //(G) deactivating the QRReader object
                EtapeReader(etape.NextSibling);
            }
            nextStepButton.GetComponent<Button>().interactable = true;
            nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); ///on enlève les anciennes fonctions du bouton avant d'ajouter la fonction etape suivante appropriée
            nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        }

        Debug.Log("En attente de QRCode");
        StartCoroutine(waitForQRCode());
    }

    public void creerQCM(XmlNode etape, XmlNode question, XmlNode reponsev, XmlNode reponsef1, XmlNode reponsef2, XmlNode reponsef3, XmlNode indice) ///reponsev est la réponse juste, reponsefi pour i entre 1 et 3 les fausses
    {
        GameObject buttonTemplate = transform.Find("TestButton").gameObject; ///on récupère le template de bouton, 3eme fils du panel
        //buttonTemplate.SetActive(true);

        GameObject g;
        GameObject g1;
        GameObject g2;
        GameObject g3;
        GameObject g4; ///creation des différents objets
        GameObject nextStepButton = transform.parent.Find("bottomContainer").Find("NextStepButton").gameObject; ////bouton suivant
        GameObject questionBox = transform.Find("QuestionBox").gameObject;
        GameObject input = transform.Find("InputField").gameObject;
        GameObject hint = transform.parent.Find("bottomContainer").Find("Hint").gameObject;//creation of hint button object
        GameObject textHint = transform.Find("TextHint").gameObject;//creation of hint text


        nextStepButton.transform.GetComponent<Button>().interactable = false;
        questionBox.transform.Find("Text").GetComponent<Text>().text = question.InnerText;
        textHint.GetComponent<Text>().text = indice.InnerText.ToString();//initialize textHint content with the XmlNode indice innertext
        

        input.SetActive(false); ///on désactive la barre d'entrée de texte pour qu'elle n'aparaisse pas dans l'ui
        textHint.SetActive(false);//the user should not see the hint before their first answer
        hint.SetActive(true);
        Debug.Log("etape cree");

        buttonTemplate.SetActive(false);
        

        //g.transform.GetChild(1).GetComponent<Text>().text = reponsev.InnerText;
        g1 = Instantiate(buttonTemplate, transform);
        g2 = Instantiate(buttonTemplate, transform); ///on copie le template de bouton pour chaque réponse fausse
        g3 = Instantiate(buttonTemplate, transform);
        g4 = Instantiate(buttonTemplate, transform);

        g1.transform.GetChild(0).GetComponent<Text>().text = reponsev.InnerText;
        g2.transform.GetChild(0).GetComponent<Text>().text = reponsef1.InnerText; ///on remplit le texte de chaque bouton avec celui de la réponse correspondante
        g3.transform.GetChild(0).GetComponent<Text>().text = reponsef2.InnerText;
        g4.transform.GetChild(0).GetComponent<Text>().text = reponsef3.InnerText;

        //(G) Getting the buttons' indexes 
        int[] indexArray = { g1.transform.GetSiblingIndex(),
            g2.transform.GetSiblingIndex(),
            g3.transform.GetSiblingIndex(),
            g4.transform.GetSiblingIndex() };
        Debug.Log(indexArray[0].ToString() 
            +" | " + indexArray[1].ToString()
            +" | " + indexArray[2].ToString()
            +" | " + indexArray[3].ToString()); //(G) test Debug
        //(G) Shuffling the indexes array
        System.Random rnd = new System.Random();
        indexArray = indexArray.OrderBy(c => rnd.Next()).ToArray();
        Debug.Log(indexArray[0].ToString() 
            + " | " + indexArray[1].ToString()
            + " | " + indexArray[2].ToString()
            + " | " + indexArray[3].ToString()); //(G) test Debug
        //(G) switching the buttons' order 
        g1.transform.SetSiblingIndex(indexArray[0]);
        g2.transform.SetSiblingIndex(indexArray[1]);
        g3.transform.SetSiblingIndex(indexArray[2]);
        g4.transform.SetSiblingIndex(indexArray[3]);

        g1.SetActive(true);
        g2.SetActive(true); 
        g3.SetActive(true);
        g4.SetActive(true);

        void AfficherIndice()
        {
            textHint.SetActive(true);
        }
        hint.GetComponent<Button>().onClick.AddListener(AfficherIndice);//if the user hits the hint button, the game displays the text in textHint

        void EtapeSuivante()
        {
            Destroy(g1);
            Destroy(g2); ///on détruit les boutons superflus, et on en conserve un pour toujours avoir le template disponible
            Destroy(g3);
            Destroy(g4);
            hint.SetActive(false);
            textHint.SetActive(false);
            questionBox.transform.Find("Text").GetComponent<Text>().text = "";
            EtapeReader(etape.NextSibling); ///on appelle la fonction EtapeReader sur le frère suivant de cette étape
        }
        
        void Valider2()
        {
            nextStepButton.transform.GetComponent<Button>().interactable = true;
            g1.GetComponent<Button>().GetComponent<Image>().color=new Color32(0,156,55,255);
            Debug.Log("C'est bon");
            StartCoroutine(ecranJuste());
            
            
        }

        void Refuser2(GameObject wrongButton)
        {

            Debug.Log("C'est faux");
            StartCoroutine(colorChangeTimer());
            StartCoroutine(ecranFaux());
            IEnumerator colorChangeTimer() //(G): changes the button's color for a moment to indicate a wrong answer.
            {
                Color defaultColor = wrongButton.GetComponent<Button>().GetComponent<Image>().color;
                wrongButton.GetComponent<Button>().GetComponent<Image>().color = new Color32(183,53,58,255);
                yield return new WaitForSecondsRealtime(3);
                wrongButton.GetComponent<Button>().GetComponent<Image>().color = defaultColor;

            }
        }
        g1.GetComponent<Button>().onClick.AddListener(Valider2); ///la réponse juste appelle valider, fen vrai il faudrait enlever les fonctions déjà présentes sur le bouton avant de faire ça...
        g2.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g2); });///les autres appellent refuser
        g3.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g3); });
        g4.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g4); });
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
            
    }

    public void EtapeReader(XmlNode CurrentNode)
    {
        string typeEtape= CurrentNode.InnerText; ///on connait le type de l'étape en lisant le texte du noeud en cours (le commentaire)
        if (CurrentNode.Name == "conclusion") ///si on atteint la conclusion alors c'est fini
        {
            Debug.Log("c'est fini");
            transform.parent.gameObject.SetActive(false);
            SceneManager.LoadScene("ecran_felicitations", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log(typeEtape);
            XmlNode etape = CurrentNode; ///l'étape en elle même est le frère du commentaire sur le xml que vous avez fouri
            XmlNode titre = etape.FirstChild; ///le premier fils de l'étape est son titre
            XmlNode navigation = titre.NextSibling; ///le deuxième fils de l'étape est la navigation
            navigate(navigation);
            Debug.Log(navigation.InnerText);
            XmlNode epreuve = navigation.NextSibling;
            XmlNode typeEpreuve = epreuve.FirstChild;
            if (typeEpreuve.Name == "info")
            {

                XmlNode info = epreuve.FirstChild;
                XmlNode texteinfo = info.FirstChild;
                XmlNode imageUrl = texteinfo.NextSibling;
                creerEtapeInfo(etape, texteinfo, imageUrl);
            }
            if (typeEpreuve.Name == "texte")
            {
                /// on affiche la navigation dans la console, pour vérifier que àa marche

                Debug.Log(epreuve.InnerText);

                XmlNode texte = epreuve.FirstChild; ///on récup_re le texte de l'épreuve
                XmlNode imagetexte = texte.FirstChild;
                XmlNode question = imagetexte.NextSibling; ///on récupère la question
                XmlNode reponse = question.NextSibling; ///et la réponse
                XmlNode indice = reponse.NextSibling; //and the hint


                creerEtapeTexte(etape, question, reponse, indice); ///on crée une étape à partir de la question et de la réponse
            }

            else if (typeEpreuve.Name == "qcm")
            {


                Debug.Log(epreuve.InnerText);

                XmlNode qcm = epreuve.FirstChild;
                XmlNode question = qcm.FirstChild;
                XmlNode reponsev = question.NextSibling;
                XmlNode reponsef1 = reponsev.NextSibling; ///on récupère toutes les réponses juste et fausses
                XmlNode reponsef2 = reponsef1.NextSibling;
                XmlNode reponsef3 = reponsef2.NextSibling;
                XmlNode indice = reponsef3.NextSibling; ///(D) and the hint

                creerQCM(etape, question, reponsev, reponsef1, reponsef2, reponsef3, indice); ///on crée l'étape qcm
            }

            else if (typeEpreuve.Name == "qrcode")
            {



                XmlNode qrcode = epreuve.FirstChild;
                XmlNode question = qrcode.FirstChild;
                XmlNode reponse = question.NextSibling;
                creerQrCode(etape, question, reponse); // (G) On crée l'étape QRCode
            }

        }
            
            

    }


        
    public string Url;
    private void Start() ///que fait on au démarrage?
    {
        UrlStorage.time = (int)Time.time;
        Url = UrlStorage.url;
        XmlDocument baladeData = new XmlDocument(); ///on crée un nouveau doc xml nommé baladeData
        WWW data = new WWW(Url); ///oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
        while (!data.isDone)
        {
            ///cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
        }

        Debug.Log("Balade finie de charger");
        baladeData.LoadXml(data.text); ///on charge le texte de data dans le doc xml baladeData

        XmlNode encoding = baladeData.FirstChild; ///le premier fils de baladeData est l'encoding
        XmlNode test2 = encoding.NextSibling; ///je ne sais pas encore pourquoi mais il faut skip 1 autres fils avant d'arriver au contenu
        XmlNode test3 = test2.NextSibling;///la dedans y'a le contenu
         
        XmlNode descriptif = test3.FirstChild; ///le premier fils de test3 c'est le descriptif de la balade
        XmlNode etape1 = descriptif.NextSibling; ///l'etape1 c'est le premier frere du descriptif

        XmlNode nomBalade = descriptif.FirstChild;
        XmlNode duree = nomBalade.NextSibling;
        XmlNode resume = duree.NextSibling;
        XmlNode lieuDepart = resume.NextSibling;
        XmlNode coords = lieuDepart.NextSibling;
        XmlNode x = coords.FirstChild;
        XmlNode y = x.NextSibling;
        float xprev = float.Parse(x.InnerText, System.Globalization.CultureInfo.InvariantCulture);
        float yprev = float.Parse(y.InnerText.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        gpscalcul.setLocalisationPrevue(xprev, yprev);

        EtapeReader(etape1); ///on lance EtapeReader sur l'etape1
            
    }
}

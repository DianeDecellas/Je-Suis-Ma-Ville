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
using System.Text;
using System.Globalization;


public class XmlReader : MonoBehaviour
    {
    GameObject trueScreen;
    GameObject falseScreen;
    Button trueScreenButton;
    Button falseScreenButton;

    public DeviceCameraController scriptQrCode;
    public UpdatePosition gpscalcul;

    GameObject questionBoxObject;
    GameObject questionTextBox;
    GameObject imageParentObject;
    GameObject infoObject;
    GameObject inputFieldObject;
    GameObject validateButton;
    GameObject audioSourceObject;
    GameObject bottomContainerObject;
    GameObject nextStepButton;
    GameObject audioButton;
    GameObject rawImageObject;
    RawImage rawImage;
    GameObject texteInfo;
    GameObject textHint;
    GameObject hint;
    GameObject compassParent;
    AudioSource audioSource;
    GameObject previousStepButton;
    bool isItPreviousStep = false;
    int furthestNodeIndex = 0;
    int currentNodeIndex = 0;

    /// <summary>
    /// This function initialises the view once the walk starts
    /// At first, we initialise every GameObject corresponding to the different UI elements
    /// Then, we retrieve the XML file from the remote server using the url and walk ID contained inside of the UrlStorage object.
    /// Finally, we parse these elements and launch the walk by calling EtapeReader
    /// <see cref="EtapeReader(XmlNode)"/>
    /// </summary>
    private void Start() //que fait on au démarrage?
    {
        
        trueScreen = transform.parent.parent.Find("true").gameObject;
        trueScreenButton = trueScreen.transform.Find("Panel").Find("ButtonTarget").GetComponent<Button>();
        falseScreen = transform.parent.parent.Find("false").gameObject;
        falseScreenButton = falseScreen.transform.Find("Panel").Find("ButtonTarget").GetComponent<Button>();
        falseScreenButton.onClick.AddListener(killFalseScreen);

        questionBoxObject = transform.Find("QuestionBox").gameObject;
        questionTextBox = questionBoxObject.transform.Find("Text").gameObject;
        imageParentObject = transform.Find("ImageParent").gameObject;
        infoObject = transform.Find("InfoParent").gameObject;
        inputFieldObject = transform.Find("InputField").gameObject;
        validateButton = transform.Find("ValidateButton").gameObject;
        audioSourceObject = transform.Find("AudioSource").gameObject;
        bottomContainerObject = transform.parent.Find("BottomContainer").gameObject;
        nextStepButton = bottomContainerObject.transform.Find("NextStepButton").gameObject;
        previousStepButton = bottomContainerObject.transform.Find("PreviousStepButton").gameObject;
        audioButton = transform.Find("InfoParent").Find("AudioButton").gameObject;
        rawImageObject = infoObject.transform.Find("Image").Find("RawImage").gameObject;
        rawImage = rawImageObject.GetComponent<RawImage>();
        texteInfo = infoObject.transform.Find("TextInfo").gameObject;
        textHint = transform.parent.Find("HintContainer").GetChild(0).gameObject;
        hint = bottomContainerObject.transform.Find("Hint").gameObject;
        compassParent = transform.parent.Find("CompassParent").gameObject;
        audioSource = audioSourceObject.GetComponent<AudioSource>();



        Debug.Log("Start XmlReader");
        UrlStorage.time = (int)Time.time;
        string urlBalade = UrlStorage.urlBaladeDirectory + UrlStorage.idBalade + ".xml";
        Debug.Log(urlBalade);
        Debug.Log("URL chargée:" + urlBalade);
        XmlDocument baladeData = new XmlDocument(); //on crée un nouveau doc xml nommé baladeData
        WWW data = new WWW(urlBalade); //oui cette fonction est obsolète mais j'ai du mal avec la nouvelle
        while (!data.isDone)
        {
            //cette boucle while sert à attendre qu'on ait bien toutes les données, sinon on risque d'avoir des erreurs
        }

        Debug.Log("Balade finie de charger");
        baladeData.LoadXml(data.text); //on charge le texte de data dans le doc xml baladeData

        XmlNode encoding = baladeData.FirstChild; //le premier fils de baladeData est l'encoding
        XmlNode dtdNode = encoding.NextSibling; //La DTD compte comme un noeud
        XmlNode baladeNode = dtdNode.NextSibling;//la racine de la balade à proprement parler

        XmlNode etapeNode = baladeNode.FirstChild; //l'etape1 c'est le premier frere du descriptif

        EtapeReader(etapeNode); //on lance EtapeReader sur l'etape1

    }

    void killFalseScreen()
    {
        falseScreen.SetActive(false);
    }
    
    IEnumerator ecranFaux()
    {
        
        falseScreen.SetActive(true);
        yield return new WaitForSecondsRealtime(2);
        
        

    }


    public void creerNavigation(XmlNode etapeNode, XmlNode imageNameNode, XmlNode instructionsNode, XmlNode CoordsNode)
    {
        string imagePath = imageNameNode.InnerText.Trim(new char[] {' ', '\n', '\r' });
        string instructions = instructionsNode.InnerText;
        float xprev = float.Parse(CoordsNode.FirstChild.InnerText, System.Globalization.CultureInfo.InvariantCulture);
        float yprev = float.Parse(CoordsNode.LastChild.InnerText.ToString(), System.Globalization.CultureInfo.InvariantCulture);
        gpscalcul.setLocalisationPrevue(xprev, yprev);

        //the part were we unload what we don't want and load what we want : 

        inputFieldObject.SetActive(false);
        validateButton.SetActive(false);
        textHint.SetActive(false);


        infoObject.SetActive(true);
        StartCoroutine(DownloadImage(imagePath, rawImage));

        compassParent.SetActive(true);

        questionTextBox.GetComponent<Text>().text = instructions;
        void EtapePrecedente()
        {
            if (etapeNode.PreviousSibling != null)
            {
                currentNodeIndex = currentNodeIndex - 1;
                isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
                infoObject.SetActive(false);
                compassParent.SetActive(false);
                trueScreen.SetActive(false);
                EtapeReader(etapeNode.PreviousSibling);
            }
        }
        void EtapeSuivante()
        {
            currentNodeIndex += 1;
            furthestNodeIndex = Mathf.Max(furthestNodeIndex, currentNodeIndex);
            isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
            infoObject.SetActive(false);
            compassParent.SetActive(false);
            trueScreen.SetActive(false);
            EtapeReader(etapeNode.NextSibling); //on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
        }
        previousStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        previousStepButton.GetComponent<Button>().onClick.AddListener(EtapePrecedente);
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); //on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);//on applique la fonction EtapeSuivante au bouton suivant
    }

    public void creerEtapeInfo(XmlNode etape, XmlNode texte, XmlNode imageNode, XmlNode audioNode)
    {
        Debug.Log("etape cree"); //on affiche etape cree dans la console

        texteInfo.GetComponent<Text>().text = texte.InnerText.ToString();
        string imagePath = imageNode.InnerText.Trim(new char[] { '\n', '\r', ' ' });
        StartCoroutine(DownloadImage(imagePath, rawImage));
        inputFieldObject.SetActive(false);
        validateButton.SetActive(false);
        infoObject.SetActive(true);
        nextStepButton.SetActive(true);

        
        if (audioNode != null)
        {
            audioButton.SetActive(true);
            //creation of audio source

            //test with a music from the internet
            string audiopath = UrlStorage.urlBaladeDirectory + audioNode.InnerText.Trim(new char[] { ' ', '\n', '\r' });
            Debug.Log("AudioPath = " + audiopath);
            IEnumerator i = audioPlay.GetAudioClip(audiopath, audioSource);
            StartCoroutine(i);
            Debug.Log("Starting to download the audio xmlreader...");
            //how the audio button works: starts audio with first click, then pause or continues with next clicks
            bool AudioActive = false;
            void Audio()
            {
                AudioActive = !AudioActive;
                

                if (AudioActive == true)
                {
                    audioPlay.playAudio(audioSource);
                    audioButton.GetComponent<changeButton>().setBool(true);
                }
                else
                {
                    audioPlay.pauseAudio(audioSource);
                    audioButton.GetComponent<changeButton>().setBool(false);
                }
            }
            audioButton.GetComponent<Button>().onClick.AddListener(Audio);
        }
        void EtapeSuivante()
        {
            currentNodeIndex += 1;
            furthestNodeIndex = Mathf.Max(currentNodeIndex, furthestNodeIndex);
            isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
            texteInfo.GetComponent<Text>().text = "";
            infoObject.SetActive(false);
            audioButton.SetActive(false);
            audioButton.GetComponent<Button>().onClick.RemoveAllListeners();
            trueScreen.SetActive(false);
            EtapeReader(etape.NextSibling); //on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)

        }

        void EtapePrecedente()
        {
            if (etape.PreviousSibling != null) {
                currentNodeIndex -= 1;
                isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
                texteInfo.GetComponent<Text>().text = "";
                infoObject.SetActive(false);
                audioButton.SetActive(false);
                audioButton.GetComponent<Button>().onClick.RemoveAllListeners();
                trueScreen.SetActive(false);
                EtapeReader(etape.PreviousSibling);
            }
        }

        previousStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        previousStepButton.GetComponent<Button>().onClick.AddListener(EtapePrecedente);
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); //on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);

    }

    private IEnumerator DownloadImage(string imageName, RawImage YourRawImage)
    {
        string imageUrl = UrlStorage.urlBaladeDirectory + imageName;
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
    /// Removes all characters that are not letters or numbers from a string
    /// </summary>
    /// <example><c>stripString("Aàëo@ - !ï")</c> returns <c>"Aaeoi"</c></example>
    /// <param name="inString">The string to strip</param>
    /// <returns>The input string with only the numbers and letters</returns>
    private string stripString(string inString)
    {
        StringBuilder outString = new StringBuilder();
        foreach (char c in inString)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                outString.Append(c);
            }
        }
        return outString.ToString();
    }

    /// <summary>
    /// Removes all diacritics symbols from a string and replace them with their standard alphabet equivalent
    /// </summary>
    /// <example>
    /// <c>RemoveDiactrics("Aàëo@ - !ï")</c> returns <c>"Aaeo@ - !i"</c>
    /// 
    /// </example>
    /// <param name="stIn"></param>
    /// <returns>the input string without any accent</returns>
    static string RemoveDiacritics(string stIn)
    {
        string stFormD = stIn.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        for (int ich = 0; ich < stFormD.Length; ich++)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(stFormD[ich]);
            }
        }
        return (sb.ToString().Normalize(NormalizationForm.FormC));
    }

    /// <summary>
    /// Replaces all diacritics symbol with their non-accentuated equivalent, then removes all characters that are neither numbers nor letters, and then finally casts every character into lower case 
    /// </summary>
    /// <example><c>normalize("L'île de la cité de Paris!")</c> returns <c>"liledelacitedeparis"</c></example>
    /// <see cref="RemoveDiacritics"/>
    /// <see cref="stripString"/>
    /// <param name="stIn"></param>
    /// <returns>The input string with the accentuated symbols replaced by their non-accentuated equivalent, and without all the characters that are neither letters nor numbers, and with all remaining letters in lower case</returns>
    private string normalize(string stIn)
    {
        return stripString(RemoveDiacritics(stIn)).ToLower();
    }

    public void creerEtapeTexte(XmlNode etape, XmlNode question, List<XmlNode> reponseArray, XmlNode indice) //fonction qui crée une etape texte avec question et reponse
    {

        Debug.Log("etape cree"); //on affiche etape cree dans la console

        textHint.GetComponent<Text>().text = indice.InnerText.ToString();//initialize textHint content with the XmlNode indice innertext

        validateButton.SetActive(true);
        nextStepButton.GetComponent<Button>().interactable = false; //(G) the Next Step Button is not interactable until the answer is right
        if (isItPreviousStep)
        {
            nextStepButton.GetComponent<Button>().interactable = false;
        }
        inputFieldObject.SetActive(true); //on rend input actif : il faut qu'il soit affiché dans l'ui
        textHint.SetActive(false);//the user should not see the hint before their first answer
        hint.SetActive(true);
        hint.GetComponent<Button>().interactable =true;//however they need the hint button

        questionTextBox.GetComponent<Text>().text = question.InnerText;

        List<string> normalizedAnswerList = new List<string>();
        foreach(XmlNode answer in reponseArray)
        {
            normalizedAnswerList.Add(normalize(answer.InnerText));
        }
        void ValiderTexte(){
            string normalizedAttempt = normalize(inputFieldObject.GetComponent<InputField>().text.ToString());
            Debug.Log(normalizedAttempt);
            bool isRightAnswer = false;
            foreach (string answer in normalizedAnswerList)
            {
                int distanceBetweenStrings;//the distance between the correct answer and the user's input, calculated according to the Damerau Levenstein formula
                distanceBetweenStrings = DamerauLevensteinDistance.StringDistance.GetDamerauLevenshteinDistance(normalizedAttempt, answer);
                int threshold;//the threshold for the correct answer, since we allow some misclicks. You are free to change its definition
                threshold = answer.Length / 3;
                isRightAnswer = (distanceBetweenStrings <= threshold);
                if (isRightAnswer)
                    break;
            }
            if (isRightAnswer) //if the user's input is accurate enough
            {
                Debug.Log("Bravo!! C'est Juste!");
                trueScreen.SetActive(true);
                
                trueScreenButton.onClick.RemoveAllListeners();
                trueScreenButton.onClick.AddListener(EtapeSuivante);
               
            }
            else
            {
                Debug.Log("Bzzt!! C'est faux!");
                StartCoroutine(ecranFaux());
                
                Color defaultColor = validateButton.GetComponent<Button>().GetComponent<Image>().color;
                //buttonTemplate.GetComponent<Button>().GetComponent<Image>().color = Color.red; //(G) must find a way to make the change last 5 seconds or so
                //buttonTemplate.GetComponent<Button>().GetComponent<Image>().color = defaultColor;
            }

        }
        GameObject g; //on crée un objet g
        g = validateButton; //g est le template de bouton défini plus haut
        g.transform.Find("Sous-titre").GetComponent<Text>().text = "Valider";//(G) je touche pas à cette ligne si c'est du test  //on remplace le contenu texte de g par le texte de la réponse, c'est pour tricher et tester plus facilement il faudra enlever cette ligne
        g.GetComponent<Button>().onClick.AddListener( ValiderTexte); //le bouton déclenche la fonction ValiderTexte quand on appuye dessus

        void AfficherIndice()
        {
            //This method displays the Hint if it is hidden, and hides it if it is displayed.
            //I do not know why this works with "textHint.activeInHierarchy" rather than "!textHint.activeInHierarchy":
            //My intuition tells me this right below should not work, but it does...
            textHint.SetActive(!textHint.activeInHierarchy);
            Debug.Log("Boup");
        }
        hint.GetComponent<Button>().onClick.RemoveAllListeners();
        hint.GetComponent<Button>().onClick.AddListener(AfficherIndice);//if the user hits the hint button, the game displays the text in textHint

        void EtapeSuivante()
        {
            currentNodeIndex += 1;
            furthestNodeIndex = Mathf.Max(furthestNodeIndex, currentNodeIndex);
            isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
            hint.GetComponent<Button>().interactable = false;
            trueScreen.SetActive(false);
            textHint.SetActive(false);
            //questionBoxObject.transform.Find("Text").GetComponent<Text>().text = "";
            questionTextBox.GetComponent<Text>().text = "";
            EtapeReader(etape.NextSibling); //on appelle la fonction EtapeReader sur le frère suivant de l'étape en cours (imaginez un arbre)
        }

        void EtapePrecedente()
        {
            if (etape.PreviousSibling != null) {
                currentNodeIndex -= 1;
                isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
                hint.GetComponent<Button>().interactable = false;
                trueScreen.SetActive(false);
                textHint.SetActive(false);
                //questionBoxObject.transform.Find("Text").GetComponent<Text>().text = "";
                questionTextBox.GetComponent<Text>().text = "";
                EtapeReader(etape.PreviousSibling);
            }
        }

        previousStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        previousStepButton.GetComponent<Button>().onClick.AddListener(EtapePrecedente);
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); //on enlève tous les attribus du bouton suivant avant de lui appliquer la fonction EtapeReader sinon le bouton suivant se retrouve avec 1000 fonctions différentes dessus
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);//on applique la fonction EtapeSuivante au bouton suivant
    }

    public void creerQrCode(XmlNode etape, XmlNode question, XmlNode reponse) //fonction creer qrCode
    {
        GameObject frame = imageParentObject.transform.Find("CameraImage").Find("Frame").gameObject;
        
        nextStepButton.transform.GetComponent<Button>().interactable = false;       //(G) can not go to the next step before having the answer
        if (isItPreviousStep)
        {
            nextStepButton.GetComponent<Button>().interactable = true;
        }
        questionTextBox.transform.GetComponent<Text>().text = question.InnerText;   //(G) instructions display
        previousStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        previousStepButton.GetComponent<Button>().onClick.AddListener(EtapePrecedente);
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners(); //on enlève les anciennes fonctions du bouton avant d'ajouter la fonction etape suivante appropriée
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
        inputFieldObject.SetActive(false);         //(G) input and testButton are set inactive because they are unused during this step
        validateButton.SetActive(false);
        scriptQrCode.SetActiveCamera(new WebCamTexture());
        scriptQrCode.expectedQrCodeMessage = reponse.InnerText.Trim(new char[] { ' ', '\n', '\r' }); //(G) Setting the expected answer
        scriptQrCode.isQrCodeValid = false;
        imageParentObject.SetActive(true);
        frame.SetActive(true);
        Debug.Log("Creer QRCode : Expected Message = " + scriptQrCode.expectedQrCodeMessage);
        void EtapePrecedente() //(G) this function will be called upon clicking on the nextStepButton button
        {
            if (currentNodeIndex != 0)
            {
                currentNodeIndex -= 1;
                isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
                scriptQrCode.stopCamera();
                //scriptQrCode.Interrupt(); //(G) maybe will be used someday to destroy the imageParent object ? Who knows.
                imageParentObject.SetActive(false); //(G) deactivating the QRReader object
                EtapeReader(etape.PreviousSibling);
                nextStepButton.transform.GetComponent<Button>().interactable = true;
                trueScreen.SetActive(false);
            }
            
        }
        void EtapeSuivante() //(G) this function will be called upon clicking on the nextStepButton button
        {
            currentNodeIndex += 1;
            furthestNodeIndex = Mathf.Max(furthestNodeIndex, currentNodeIndex);
            frame.SetActive(false);
            scriptQrCode.stopCamera();
            //scriptQrCode.Interrupt(); //(G) maybe will be used someday to destroy the imageParent object ? Who knows.
            imageParentObject.SetActive(false); //(G) deactivating the QRReader object
            EtapeReader(etape.NextSibling);
            trueScreen.SetActive(false);
            isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
        }

        IEnumerator waitForQRCode() //(G) this routine is called and waits for the scriptQrCode.isQrCodeValid to be true
        {
            //(G) the Iterator / Enumerator waitForQRCode goes on only when scriptQrCode.qrcodeValide is set to true
            yield return new WaitUntil(() => scriptQrCode.isQrCodeValid); //(G) the " () => " bit transforms the scriptQrCode.qrcodeValide variable into a function
            frame.SetActive(false);
            trueScreen.SetActive(true);
            
            trueScreenButton.onClick.RemoveAllListeners();
            trueScreenButton.onClick.AddListener(EtapeSuivante);
            
            nextStepButton.GetComponent<Button>().interactable = true;
            
        }

        Debug.Log("En attente de QRCode");
        StartCoroutine(waitForQRCode());
    }

    public void creerQCM(XmlNode etape, XmlNode question, XmlNode reponsev, XmlNode reponsef1, XmlNode reponsef2, XmlNode reponsef3, XmlNode indice) //reponsev est la réponse juste, reponsefi pour i entre 1 et 3 les fausses
    {
        GameObject g;
        GameObject g1;
        GameObject g2;
        GameObject g3;
        GameObject g4; //creation des différents objets

        nextStepButton.transform.GetComponent<Button>().interactable = false;
        if (isItPreviousStep)
        {
            nextStepButton.GetComponent<Button>().interactable = true;
        }
        questionTextBox.GetComponent<Text>().text = question.InnerText;
        textHint.GetComponent<Text>().text = indice.InnerText.ToString(); //initialize textHint content with the XmlNode indice innertext
        

        inputFieldObject.SetActive(false); //on désactive la barre d'entrée de texte pour qu'elle n'aparaisse pas dans l'ui
        textHint.SetActive(false);//the user should not see the hint before their first answer
        hint.GetComponent<Button>().interactable= true;
        Debug.Log("etape cree");
        validateButton.SetActive(false);

        g1 = Instantiate(validateButton, transform);
        g2 = Instantiate(validateButton, transform); //on copie le template de bouton pour chaque réponse fausse
        g3 = Instantiate(validateButton, transform);
        g4 = Instantiate(validateButton, transform);

        g1.transform.GetChild(0).GetComponent<Text>().text = reponsev.InnerText;
        g2.transform.GetChild(0).GetComponent<Text>().text = reponsef1.InnerText; //on remplit le texte de chaque bouton avec celui de la réponse correspondante
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
            textHint.SetActive(!textHint.activeInHierarchy);
        }
        hint.GetComponent<Button>().onClick.RemoveAllListeners();
        hint.GetComponent<Button>().onClick.AddListener(AfficherIndice);//if the user hits the hint button, the game displays the text in textHint

        void EtapePrecedente()
        {
            if (etape.PreviousSibling != null) {
                currentNodeIndex -= 1;
                isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
                Destroy(g1);
                Destroy(g2); //on détruit les boutons superflus, et on en conserve un pour toujours avoir le template disponible
                Destroy(g3);
                Destroy(g4);
                hint.GetComponent<Button>().interactable = false;
                textHint.SetActive(false);
                trueScreen.SetActive(false);
                //questionBoxObject.transform.Find("Text").GetComponent<Text>().text = "";
                questionTextBox.GetComponent<Text>().text = "";
                EtapeReader(etape.PreviousSibling); //on appelle la fonction EtapeReader sur le frère précédent de cette étape
            }
        }
        void EtapeSuivante()
        {
            currentNodeIndex += 1;
            furthestNodeIndex = Mathf.Max(furthestNodeIndex, currentNodeIndex);
            isItPreviousStep = (furthestNodeIndex < currentNodeIndex);
            Destroy(g1);
            Destroy(g2); //on détruit les boutons superflus, et on en conserve un pour toujours avoir le template disponible
            Destroy(g3);
            Destroy(g4);
            hint.GetComponent<Button>().interactable = false;
            textHint.SetActive(false);
            trueScreen.SetActive(false);
            //questionBoxObject.transform.Find("Text").GetComponent<Text>().text = "";
            questionTextBox.GetComponent<Text>().text = "";
            EtapeReader(etape.NextSibling); //on appelle la fonction EtapeReader sur le frère suivant de cette étape
        }
        
        void Valider2()
        {
            nextStepButton.transform.GetComponent<Button>().interactable = true;
            g1.GetComponent<Button>().GetComponent<Image>().color=new Color32(0,156,55,255);
            Debug.Log("C'est bon");
            trueScreen.SetActive(true);
            trueScreenButton.onClick.RemoveAllListeners();
            trueScreenButton.onClick.AddListener(EtapeSuivante);


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
                

            }
        }
        g1.GetComponent<Button>().onClick.AddListener(Valider2); //la réponse juste appelle valider, fen vrai il faudrait enlever les fonctions déjà présentes sur le bouton avant de faire ça...
        g2.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g2); });//les autres appellent refuser
        g3.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g3); });
        g4.GetComponent<Button>().onClick.AddListener(delegate { Refuser2(g4); });
        previousStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        previousStepButton.GetComponent<Button>().onClick.AddListener(EtapePrecedente);
        nextStepButton.GetComponent<Button>().onClick.RemoveAllListeners();
        nextStepButton.GetComponent<Button>().onClick.AddListener(EtapeSuivante);
            
    }

    public void EtapeReader(XmlNode CurrentNode)
    {
        if (CurrentNode == null) //si on atteint la conclusion alors c'est fini
        {
            Debug.Log("c'est fini");
            transform.parent.gameObject.SetActive(false);
            SceneManager.LoadScene("ecran_felicitations", LoadSceneMode.Single);
        }
        else
        {
            
            string typeEtape = CurrentNode.InnerText; //on connait le type de l'étape en lisant le texte du noeud en cours (le commentaire)
            XmlNode etape = CurrentNode; //l'étape en elle même est le frère du commentaire sur le xml que vous avez fouri
            XmlNode titre = etape.FirstChild; //le premier fils de l'étape est son titre
            Debug.Log("Titre Etape = " + titre.InnerText);

            XmlNode typeEpreuve = titre.NextSibling;
            Debug.Log("TypeEpreuve = "+typeEpreuve.Name);
            if (typeEpreuve.Name == "info")
            {

                //XmlNode info = typeEpreuve.FirstChild;
                XmlNode texteinfo = typeEpreuve.FirstChild;
                XmlNode imageNode = texteinfo.NextSibling;
                XmlNode audioNode = imageNode.NextSibling;

                Debug.Log("texteInfo : " + texteinfo.InnerText + "\nimageUrl : " + imageNode.InnerText + "\naudioUrl : " + ((audioNode == null) ? "Rien" : audioNode.InnerText));

                creerEtapeInfo(etape, texteinfo, imageNode, audioNode);

            } else if (typeEpreuve.Name == "texte")
            {
                // on affiche la navigation dans la console, pour vérifier que àa marche

                Debug.Log(typeEpreuve.InnerText);

                XmlNode imagetexte = typeEpreuve.FirstChild;
                XmlNode question = imagetexte.NextSibling; //on récupère la question

                List<XmlNode> reponseArray = new List<XmlNode>();
                XmlNode curNode = question.NextSibling;
                while (curNode.Name == "reponse")
                {
                    reponseArray.Add(curNode);
                    curNode = curNode.NextSibling;
                }
                XmlNode indice = curNode; //and the hint

                Debug.Log("urlImage : "+imagetexte.InnerText+"\nquestion : "+question.InnerText+"\nreponse : "+reponseArray.ToString()+"\nindice : "+indice.InnerText);

                creerEtapeTexte(etape, question, reponseArray, indice); //on crée une étape à partir de la question et de la réponse
            }

            else if (typeEpreuve.Name == "qcm")
            {


                Debug.Log(typeEpreuve.InnerText);

                XmlNode question = typeEpreuve.FirstChild;
                XmlNode reponsev = question.NextSibling;
                XmlNode reponsef1 = reponsev.NextSibling; //on récupère toutes les réponses juste et fausses
                XmlNode reponsef2 = reponsef1.NextSibling;
                XmlNode reponsef3 = reponsef2.NextSibling;
                XmlNode indice = reponsef3.NextSibling; //(D) and the hint

                Debug.Log("question : " + question.InnerText + "\nreponse :" + reponsev.InnerText + "\n"+reponsef1.InnerText+"\n"+reponsef2.InnerText+"\n"+reponsef3.InnerText);

                creerQCM(etape, question, reponsev, reponsef1, reponsef2, reponsef3, indice); //on crée l'étape qcm
            }

            else if (typeEpreuve.Name == "qrcode")
            {

                XmlNode question = typeEpreuve.FirstChild;
                XmlNode reponse = question.NextSibling;

                Debug.Log("question : " + question.InnerText + "\nreponse : "+reponse.InnerText);

                creerQrCode(etape, question, reponse); // (G) On crée l'étape QRCode
            }

            else if (typeEpreuve.Name == "navigation")
            {
                XmlNode urlImageNode = typeEpreuve.FirstChild;
                XmlNode instructionsNode = urlImageNode.NextSibling;
                XmlNode coordsNode = instructionsNode.NextSibling;
                Debug.Log("urlImage : " + urlImageNode.InnerText + "\ninstructions :" + instructionsNode.InnerText + "\ncoords : " + coordsNode.FirstChild.InnerText + " " + coordsNode.LastChild.InnerText);

                creerNavigation(etape, urlImageNode, instructionsNode, coordsNode);
            }
            else //si jamais on a eu un beug
            {
                SceneManager.LoadScene("ecran_accueil",LoadSceneMode.Single);
            }
        }           
    }
}

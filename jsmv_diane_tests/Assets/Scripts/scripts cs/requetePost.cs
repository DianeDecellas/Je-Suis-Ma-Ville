using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class requetePost : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //appeler Upload après avoir fait les bonnes modifs
        StartCoroutine(Upload());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();//penser à dl le csv, mettre chemin d accès dans le php// cest fait
        //là c pour le test, à modifier pour automatiser

        form.AddField("date", "26/05/2021");
        form.AddField("heure", "13:40");
        form.AddField("nom", "super balade");
        form.AddField("duree", "0:12:34");
        form.AddField("note", "5");

        using (UnityWebRequest www = UnityWebRequest.Post("localhost:8000/statistiquesJeSuisMaVille.php", form))
            //modifier chemin d accès et lancer en localhost/ c est fait
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

}

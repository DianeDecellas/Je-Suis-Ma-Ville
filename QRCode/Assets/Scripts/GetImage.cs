using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetImage : MonoBehaviour
{
    public UnityEngine.UI.RawImage rImage;
    
    void Start()
    {
        string url = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/cb/Flag_of_Picardie.svg/1920px-Flag_of_Picardie.svg.png";
        StartCoroutine(DownloadImage(url));
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            rImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}

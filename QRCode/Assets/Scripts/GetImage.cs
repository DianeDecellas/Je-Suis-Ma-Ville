using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetImage : MonoBehaviour
{
    public RawImage YourRawImage;
    public string url;

    private Vector3 speed = new Vector3(10, 0, 0);
    void Start()
    {
        StartCoroutine(DownloadImage(url));
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
            YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }

    private void Update()
    {
        if (YourRawImage.rectTransform.position.x >=960 || YourRawImage.rectTransform.position.x<=0)
        {
            this.speed.x *= -1;
        }
        YourRawImage.rectTransform.position += speed;
        YourRawImage.rectTransform.Rotate(new Vector3(-1, 0, 10));
    }
}

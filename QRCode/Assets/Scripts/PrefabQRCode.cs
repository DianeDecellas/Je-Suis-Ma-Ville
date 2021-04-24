using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class PrefabQRCode : MonoBehaviour
{
    public string message;
    private WebCamTexture camTexture;
    private Rect screenRect;
    private Rect myRect;

    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height);
        camTexture = new WebCamTexture();
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        if (camTexture != null)
        {
            camTexture.Play();
        }
    }

    void OnGUI()
    {
        // drawing the camera on screen
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
        //GUI.DrawTexture(myRect, camTexture, ScaleMode.ScaleToFit);
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("Lu!");
                Debug.Log("DECODED TEXT FROM QR: " +result.Text);
                if (result.Text == message)
                {
                    Debug.Log("True");
                }
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }
}

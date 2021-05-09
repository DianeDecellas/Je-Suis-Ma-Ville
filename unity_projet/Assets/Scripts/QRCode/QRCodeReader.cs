using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode; 

public class QRCodeReader : MonoBehaviour
{
    public string expectedQrCodeMessage; // (G) La solution attendue du QRCode
    public bool isQrCodeValid = false; // (G) le booléen que le script XmlReader utilise pour savoir si le qrCode scanné correspond au message attendu
    private WebCamTexture camTexture;
    private Rect screenRect; //(G) le rectangle qui contiendra la caméra.   
    private string str;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("QRCodeReader : Start");
        screenRect = new Rect(0, 0, Screen.width, Screen.height); //(G) rectangle défini par son origine (0,0), sa largeur et sa hauteur
        camTexture = new WebCamTexture(); //(G) La caméra
        camTexture.requestedHeight = Screen.height;
        camTexture.requestedWidth = Screen.width;
        str = "Not stopped";

        if (camTexture != null)
        {
            camTexture.Play();
        }
    }

    void OnGUI() //(G) Fonction appelée 
    {
        // drawing the camera on screen
        Debug.Log(str);
        GUIUtility.RotateAroundPivot(90, screenRect.center);
        GUI.DrawTexture(screenRect, camTexture, ScaleMode.ScaleToFit);
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var qrCodeResult = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height); //(G) le résultat du QR Code
            if (qrCodeResult != null) //(G) on traite ici le résultat lu
            {
                Debug.Log("QRCodeReader : DECODED TEXT FROM QR: " +qrCodeResult.Text);
                if (qrCodeResult.Text == expectedQrCodeMessage)
                {
                    Debug.Log("QRCodeReader : True");
                    isQrCodeValid = true;
                }
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }

    public void Interrupt() //(G)
    {
        camTexture.Stop();
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode; 

public class QRCodeReader : MonoBehaviour
{
    public string expectedMessage; // (G) La solution attendue du QRCode
    public bool qrcodeValide = false; // (G) le booléen que le script XmlReader utilise pour savoir si le qrCode scanné correspond au message attendu
    private WebCamTexture camTexture;
    private Rect screenRect; //(G) le rectangle qui contiendra la caméra.   

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("QRCodeReader : Start");
        screenRect = new Rect(0, 0, Screen.width, Screen.height); //(G) rectangle défini par son origine (0,0), sa largeur et sa hauteur
        camTexture = new WebCamTexture(); //(G) La caméra
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
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height); //(G) le résultat du QR Code
            if (result != null) //(G) on traite ici le résultat lu
            {
                Debug.Log("QRCodeReader : DECODED TEXT FROM QR: " +result.Text);
                if (result.Text == expectedMessage)
                {
                    Debug.Log("QRCodeReader : True");
                    qrcodeValide = true;
                }
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }
}
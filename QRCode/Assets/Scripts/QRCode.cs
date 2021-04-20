﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class QRCode : MonoBehaviour
{
    public MessageDisplay fenetre;
    private WebCamTexture camTexture;
    private Rect screenRect;
    private Rect cameraRect;
    void Start()
    {
        screenRect = new Rect(0, 0, Screen.width, Screen.height); //le rectangle qui contient le retour caméra, défini par son origine (0,0) et sa largeur et sa hauteur
        //cameraRect = new Rect(Screen.width / 4, 0, Screen.width / 2, Screen.height);
        cameraRect = new Rect(-500, -10, 2*Screen.width, Screen.height);
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
        //GUI.DrawTexture(cameraRect, camTexture, ScaleMode.ScaleToFit);
        // do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " + result.Text);
                fenetre.Display(result.Text);

            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }
}

/*
public class QRCode : MonoBehaviour
{


    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 500, 500), "I am a button"))
        {
            print("You clicked the button!");
        }
    }
}*/
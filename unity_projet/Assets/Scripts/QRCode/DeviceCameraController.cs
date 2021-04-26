using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using ZXing;
using ZXing.QrCode;

public class DeviceCameraController : MonoBehaviour
{
    private RawImage cameraImage;
    private RectTransform imageParent;
    private AspectRatioFitter imageFitter;

    public string reponseEpreuveQrCode; //(G) la réponse que le QR Code est censée décoder.
    public bool qrCodeEstValide; //(G) le booléen qu'on lit pour confirmer si le QR Code renvoie bien ce qui est attendu

    // Device cameras
    WebCamDevice frontCameraDevice;
    WebCamDevice backCameraDevice;
    WebCamDevice activeCameraDevice;

    WebCamTexture frontCameraTexture;
    WebCamTexture backCameraTexture;
    WebCamTexture activeCameraTexture;

    // Image rotation
    Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    Vector3 fixedScale = new Vector3(-1f, 1f, 1f);


    void Start()
    {
        cameraImage = transform.GetComponent<RawImage>();
        imageParent = GetComponentInParent<RectTransform>();
        imageFitter = transform.GetComponent<AspectRatioFitter>();

        // Check for device cameras
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.Log("No devices cameras found");
            return;
        }

        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.Last();
        backCameraDevice = WebCamTexture.devices.First();

        frontCameraTexture = new WebCamTexture(frontCameraDevice.name);
        backCameraTexture = new WebCamTexture(backCameraDevice.name);

        // Set camera filter modes for a smoother looking image
        frontCameraTexture.filterMode = FilterMode.Trilinear;
        backCameraTexture.filterMode = FilterMode.Trilinear;

        // Set the camera to use by default
        SetActiveCamera(backCameraTexture);
    }

    // Set the device camera to use and start it
    public void SetActiveCamera(WebCamTexture cameraToUse)
    {
        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
        }

        activeCameraTexture = cameraToUse;
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);

        cameraImage.texture = activeCameraTexture;
        //cameraImage.material.mainTexture = activeCameraTexture; // /!\ la ligne qui fait buguer tout

        activeCameraTexture.Play();
    }

    // Switch between the device's front and back camera
    public void SwitchCamera()
    {
        Debug.Log("SwitchCamera");
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
            backCameraTexture : frontCameraTexture);
    }

    // Make adjustments to image every frame to be safe, since Unity isn't 
    // guaranteed to report correct data as soon as device camera is started
    void Update()
    {
        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100)
        {
            Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        cameraImage.rectTransform.localEulerAngles = rotationVector;


        // Set AspectRatioFitter's ratio
        float videoRatio =(float)activeCameraTexture.width / (float)activeCameraTexture.height;
        imageFitter.aspectRatio = videoRatio;

        // Unflip if vertically flipped
        cameraImage.uvRect = activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        imageParent.localScale =
            activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;

        //(G) la partie où on lit effectivement le code barre :
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            // decode the current frame
            var result = barcodeReader.Decode(activeCameraTexture.GetPixels32(),
              activeCameraTexture.width, activeCameraTexture.height);
            if (result != null)
            {
                Debug.Log("DECODED TEXT FROM QR: " +result.Text);
                if (result.Text == reponseEpreuveQrCode)
                {
                    qrCodeEstValide = true;
                }
            }
        }
        catch (System.Exception ex) { Debug.LogWarning(ex.Message); }
    }
}

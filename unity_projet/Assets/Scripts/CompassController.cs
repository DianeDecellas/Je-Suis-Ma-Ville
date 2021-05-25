using System;
using UnityEngine;
using UnityEngine.UI; //required for Input.compass

public class CompassController : MonoBehaviour
{
    public float compassSmooth = 0.5f;
    private float m_lastMagneticHeading = 0f;
    
    private Vector2 North;
    private Vector2 Phone;
    private Vector2 Objective;
    private Vector2 PhoneToNorth;
    private Vector2 PhoneToObjective;
    private float anglePhoneToObjective;
   
    public GameObject textBox;
    private Text textContent;


    // Use this for initialization
    void Start()
    {
        textContent = textBox.GetComponent<Text>();
        textContent.text = "Coucou";

        // Shareloc  : 48.62360625887856f, 2.446877750478768f
        Phone = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude); //phone coords
        Objective = new Vector2(UrlStorage.xprevu, UrlStorage.yprevu); //Coords of place to go
        North = new Vector2(90, Phone.y);

        PhoneToNorth = North - Phone;
        PhoneToObjective = Objective - Phone;

        // If you need an accurate heading to true north, 
        // start the location service so Unity can correct for local deviations:
        Input.location.Start();
        // Start the compass.
        Input.compass.enabled = true;
        //measuresArray = new float[measureNb];
        //float currentTrueHeading = (float)Math.Round(Input.compass.magneticHeading, 2);
        /*for (int i=0; i<measureNb; i++)
        {
            measuresArray[i] = currentTrueHeading;
        }*/
    }
    // Update is called once per frame
    private void Update()
    {
        Phone = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude); //coords of the phone
        Objective = new Vector2(UrlStorage.xprevu, UrlStorage.yprevu); // coords of place to reach

        //do rotation based on compass
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2);
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            anglePhoneToObjective = -Vector2.SignedAngle(PhoneToNorth, PhoneToObjective) + m_lastMagneticHeading;
            transform.localRotation = Quaternion.Euler(0, 0, anglePhoneToObjective);

            textContent.text = "Vers Nord: " + currentMagneticHeading.ToString() + "\n"
                + "Angle Nord - Objectif: " + Vector2.SignedAngle(PhoneToNorth, PhoneToObjective).ToString() + "\n"
                + "Angle Phone - Objectif: " + anglePhoneToObjective.ToString();

        }
    }
}

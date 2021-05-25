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
        textContent = textBox.GetComponent<Text>(); //The debug box that is used to display the informations.
        textContent.text = "Coucou";

        Phone = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude); //The current coordinates of the mobile phone.
        Objective = new Vector2(UrlStorage.xprevu, UrlStorage.yprevu); //Coords of place to go
        North = new Vector2(90, Phone.y); //The coordinates of the North. This may seem wierd, but it works to accomodate the model of a 2D plan, and works on the scale of a city or a region

        PhoneToNorth = North - Phone; //The vector between the phone and North
        PhoneToObjective = Objective - Phone; //The vector between the Phone and the Objective

        // If you need an accurate heading to true north, 
        // start the location service so Unity can correct for local deviations:
        Input.location.Start();
        // Start the compass.
        Input.compass.enabled = true;

    }
    // Update is called once per frame
    private void Update()
    {
        Phone = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude); //to refresh the phone's current coordinates.
        Objective = new Vector2(UrlStorage.xprevu, UrlStorage.yprevu); // coords of place to reach. May need to be changed in the future.   

        //do rotation based on compass
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2); // Direction to the magnetic north. Seems to be the Angle clockwise from the phone's direction to the north's direction
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            // /!\Beware, in this code, many things measure the angles clockwise, and many counter clockwise. Always check to avoid a headache.
             anglePhoneToObjective = -Vector2.SignedAngle(PhoneToNorth, PhoneToObjective) + m_lastMagneticHeading; //updates the direction to the objective based on the north. 
            transform.localRotation = Quaternion.Euler(0, 0, anglePhoneToObjective); // rotate the compass on screen

            textContent.text = "Vers Nord: " + currentMagneticHeading.ToString() + "\n" //displays handy informations
                + "Angle Nord - Objectif: " + Vector2.SignedAngle(PhoneToNorth, PhoneToObjective).ToString() + "\n"
                + "Angle Phone - Objectif: " + anglePhoneToObjective.ToString() + "\n"
                + "Objectif: " + Objective.ToString() + "\n"
                + "Phone:" + Phone.ToString() + "\n";

        }
    }
}

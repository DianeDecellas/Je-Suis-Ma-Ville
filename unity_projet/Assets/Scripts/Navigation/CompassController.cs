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

    private int measureNb = 1;
    private int cursor = 0;
    private float[] AngleTab;
    private Vector2[] vectorTab;
    private Vector2 movingAvgVector;
    private float movingAvgAngle;

    public GameObject textBox;
    private Text textContent;

    private Vector2 newVectorFromAngle(float angle)
    {
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

    // Use this for initialization
    void Start()
    {
        textContent = textBox.GetComponent<Text>(); //The debug box that is used to display the informations.
        textContent.text = "Coucou";

        //init mean
        AngleTab = new float[measureNb];
        vectorTab = new Vector2[measureNb];
        for (int i = 0; i < measureNb; i++)
        {
            AngleTab[i] = 0f;
            vectorTab[i] = newVectorFromAngle(0f);
        }
        movingAvgVector = newVectorFromAngle(0f);
        movingAvgAngle = 0f;

        Phone = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude); //The current coordinates of the mobile phone.
        Objective = new Vector2(Storage.xprevu, Storage.yprevu); //Coords of place to go
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
        Objective = new Vector2(Storage.xprevu, Storage.yprevu); // coords of place to reach. May need to be changed in the future.  

        PhoneToNorth = North - Phone; //The vector between the phone and North
        PhoneToObjective = Objective - Phone; //The vector between the Phone and the Objective

        //do rotation based on compass
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2); // Direction to the magnetic north. Seems to be the Angle clockwise from the phone's direction to the north's direction
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            // /!\Beware, in this code, many things measure the angles clockwise, and many counter clockwise. Always check to avoid a headache.
            anglePhoneToObjective = Vector2.SignedAngle(PhoneToNorth, PhoneToObjective) - m_lastMagneticHeading; //updates the direction to the objective based on the north. 
            transform.localRotation = Quaternion.Euler(0, 0, -anglePhoneToObjective); // rotate the compass on screen

            textContent.text = "Vers Nord: " + currentMagneticHeading.ToString() + "\n" //displays handy informations
                + "Angle Nord - Objectif: " + Vector2.SignedAngle(PhoneToNorth, PhoneToObjective).ToString() + "\n"
                + "Angle Phone - Objectif: " + anglePhoneToObjective.ToString() + "\n"
                + "Objectif: " + Objective.x.ToString() + "," + Objective.y.ToString() + "\n"
                + "Phone:" + Phone.x.ToString() + "," + Phone.y.ToString() + "\n";
        }
    }

    /*private void Update()
    {
        //do rotation based on compass
        float currentMagneticHeading = (float)Math.Round(Input.compass.magneticHeading, 2); // Direction to the magnetic north. Seems to be the Angle clockwise from the phone's direction to the north's direction
        if (m_lastMagneticHeading < currentMagneticHeading - compassSmooth || m_lastMagneticHeading > currentMagneticHeading + compassSmooth)
        {
            m_lastMagneticHeading = currentMagneticHeading;
            Vector2 currentVector = newVectorFromAngle(m_lastMagneticHeading);
            movingAvgVector += (currentVector / measureNb) - (vectorTab[cursor] / measureNb);
            vectorTab[cursor] = currentVector;
            cursor = (cursor + 1) % measureNb;
            movingAvgAngle = Vector2.SignedAngle(newVectorFromAngle(0f), movingAvgVector);

            // /!\Beware, in this code, many things measure the angles clockwise, and many counter clockwise. Always check to avoid a headache.
            transform.localRotation = Quaternion.Euler(0, 0, -movingAvgAngle); // rotate the compass on screen


        }
    }*/
}


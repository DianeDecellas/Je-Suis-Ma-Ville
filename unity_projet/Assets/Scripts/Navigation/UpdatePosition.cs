using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdatePosition : MonoBehaviour
{
    // Start is called before the first frame update
    public Text position;
    float xprevu = 0; //latitude = 90-phi ici la latitude du point à trouver
    float yprevu = 0; //longitude = theta ici la longitude
    float k = 60*1852; //le facteur k pour avoir la distance en m
    float distancex = 0;
    float distancey = 0;
    float distance = 0;
    
    public void setLocalisationPrevue(float x,float y) //fonction qui met à jour les coordonées du point à trouver
    {
        xprevu = x;
        yprevu = y;
        Storage.xprevu = x;
        Storage.yprevu = y;

    }
    bool getDistanceIsOk()
    {
        return (distance<5);
    }
    
   
    
    // Update is called once per frame
    void LateUpdate()
    {
        
        float lat = GPS.Instance.latitude;
        float lon = GPS.Instance.longitude;
        distancex = k * (yprevu - GPS.Instance.longitude);
        distancey = k * (xprevu - GPS.Instance.latitude) * Mathf.Cos(yprevu / 2 + GPS.Instance.longitude / 2);
        distance = Mathf.Sqrt(Mathf.Pow(distancex,2)+Mathf.Pow(distancey,2));

        position.text = (distance > 1000) ? (distance/1000).ToString("#.0") + "km" : ((int)distance).ToString()+"m";
        //position.text = ((int)distance).ToString() + "m" ;
        if (getDistanceIsOk())
        {

        }
    }
}

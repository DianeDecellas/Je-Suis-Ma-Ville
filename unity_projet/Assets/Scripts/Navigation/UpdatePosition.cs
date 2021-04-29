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
    public void setLocalisationPrevue(float x,float y) //fonction qui met à jour les coordonées du point à trouver
    {
        xprevu = x;
        yprevu = y;

    }
    
   
    
    // Update is called once per frame
    void LateUpdate()
    {
        
        float lat = GPS.Instance.latitude;
        float lon = GPS.Instance.longitude;
        position.text = "Nord-Sud:" + (k*(yprevu-GPS.Instance.longitude)).ToString() + "\nEst-Ouest" + (k*(xprevu-GPS.Instance.latitude)*Mathf.Cos(yprevu/2+GPS.Instance.longitude/2)).ToString();
    }
}

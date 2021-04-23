using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//ce code sert à afficher les coordonnées GPS utilisateur

public class updatePositionText : MonoBehaviour
{
    public Text position;
    

    // Update is called once per frame
    private void Update()
    {
        position.text = "Lat:" + GPS.Instance.latitude.ToString()+"\nLon:"+ GPS.Instance.longitude.ToString();
    }
}

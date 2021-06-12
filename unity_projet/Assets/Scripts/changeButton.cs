using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class changeButton : MonoBehaviour
{
    public bool activate;
    public Sprite fullStar;
    public Sprite emptyStar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activate)
        {
            GetComponent<SVGImage>().sprite = fullStar;
        } else
        {
            GetComponent<SVGImage>().sprite = emptyStar;
        }
    }

    public void setBool(bool b)
    {
        activate = b;
    }
}

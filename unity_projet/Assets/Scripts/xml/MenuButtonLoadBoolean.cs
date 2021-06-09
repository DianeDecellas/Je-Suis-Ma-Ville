using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonLoadBoolean : MonoBehaviour
{
    private bool load;
    // Start is called before the first frame update
    void Start()
    {
        load = false;
    }

    public void enableLoad()
    {
        load=true;
    }
    public void disableLoad()
    {
        load = false;
    }
    public bool getLoadStatus()
    {
        return load;
    }
}

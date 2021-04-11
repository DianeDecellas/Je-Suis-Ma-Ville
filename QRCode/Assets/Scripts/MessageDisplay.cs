using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    private UnityEngine.UI.Text textComponent;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponent<UnityEngine.UI.Text>(); //On initialise textComponent à la valeur
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Display(string message)
    {
        textComponent.text = message;
    }
}

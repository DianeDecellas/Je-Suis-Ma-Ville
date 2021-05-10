using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EcranAccueil : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.GetComponent<Button>().onClick.AddListener(accessChoice);
    }
    void accessChoice()
    {
        SceneManager.LoadScene("scene de chargement");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

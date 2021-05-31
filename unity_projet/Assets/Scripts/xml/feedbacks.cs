using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class feedbacks : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.GetComponent<Button>().onClick.AddListener(accessChoice);
       
    }
    void accessChoice()
    {
        SceneManager.LoadScene("ecran_feedbacks", LoadSceneMode.Single);
    }

}

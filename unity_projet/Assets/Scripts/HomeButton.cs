using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeButton : MonoBehaviour
{
    public void escape()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("scene de chargement", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}

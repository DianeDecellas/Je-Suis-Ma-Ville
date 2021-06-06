using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class audioPlay : MonoBehaviour
{
    public static IEnumerator GetAudioClip(string audiopath, AudioSource audioSource)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(audiopath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;

            }
        }
    }

    public static void pauseAudio(AudioSource audioSource)
    {
        audioSource.Pause();
    }

    public static void playAudio(AudioSource audioSource)
    {
        audioSource.Play();
    }
}

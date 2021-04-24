using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraButtonScript : MonoBehaviour
{
    public void OnPress() {
        Debug.Log("Hey");
        //transform.parent.GetChild(3).GetComponent<DeviceCameraController>().SwitchCamera();
        transform.parent.Find("ImageParent").GetChild(0).GetComponent<DeviceCameraController>().SwitchCamera();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyar;

public class CameraPermissionHandler : MonoBehaviour
{
    ARKitCameraDevice asd;

    void Update()
    {
    }

    private void OnEnable()
    {
        requestCameraPermission();
    }

    public void requestCameraPermission()
    {
        if (Application.platform == RuntimePlatform.Android && 
            !UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera)) 
        { 
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera); 
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer && 
                !Application.HasUserAuthorization(UserAuthorization.WebCam)) 
        { 
            Application.RequestUserAuthorization(UserAuthorization.WebCam); 
        }
    }
}

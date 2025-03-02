using easyar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARVideoCameraHandler : MonoBehaviour
{
    public ARSession arSession;
    private CameraDeviceFrameSource videoCamera;
    public static ARVideoCameraHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        videoCamera = arSession.GetComponentInChildren<CameraDeviceFrameSource>();

    }

    public void EnableCamera(bool enable)
    {
        videoCamera.enabled = enable;
    }
}

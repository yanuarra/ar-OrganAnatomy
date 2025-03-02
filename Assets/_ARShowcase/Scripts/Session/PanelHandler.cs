using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler : MonoBehaviour
{
    [SerializeField]
    private Transform userInterface;
    private Transform cam;
    private GameObject currentOverlay;
    private Action curAction;

    public static PanelHandler Instance { get; private set; }

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

    public void SetRotationToCam()
    {
        if (!isCamExist()) return;
        userInterface.rotation = Quaternion.Euler(new Vector3(
            userInterface.eulerAngles.x, cam.eulerAngles.y, userInterface.eulerAngles.z));
    }

    public void SetNewCurrentOverlay(GameObject overlay, Action _customAction = null)
    {
        DeactiveCurrentOverlay();
        currentOverlay = overlay;
        ActiveCurrentOverlay();
        if (_customAction != null) curAction = _customAction; 
    }

    public void ActiveCurrentOverlay()
    {
        if (currentOverlay == null) return;
        if (curAction != null)
        {
            curAction?.Invoke();
            curAction = null;
        }
        else
        {
            currentOverlay.SetActive(true);
        }
    }

    public void DeactiveCurrentOverlay()
    {
        if (currentOverlay == null) return;
        if (curAction != null)
        {
            curAction?.Invoke();
            curAction = null;
        }
        else { 
            currentOverlay.SetActive(false);
        }
    }

    private bool isCamExist()
    {
        if (Camera.main == null) return false;
        cam = Camera.main.transform;
        return true;
    }
}
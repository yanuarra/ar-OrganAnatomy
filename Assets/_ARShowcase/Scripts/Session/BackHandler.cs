using System;
using UnityEngine;

public class BackHandler : MonoBehaviour
{
    public event Action backEvent;
    private GameObject currentPage;

    public static BackHandler Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    private void Update() {

    }

    public void Back() {
        if (MessageHandler.Instance.IsOverlayActive())
        {
            MessageHandler.Instance.Ok();
        }
        else if (ConfirmationHandler.Instance.IsOverlayActive())
        {
            ConfirmationHandler.Instance.No();
        }
        else if (backEvent != null)
        {
            backEvent.Invoke();
        }
        else
        {
            ConfirmationHandler.Instance.ActivePanel("Apakah Anda ingin keluar dari aplikasi?", Application.Quit);
        }
    }

    public void AssignBackEvent(Action eventToAssign) {
        backEvent = eventToAssign;
    }

    public bool IsBackEventNull() {
        return backEvent == null;
    }
}

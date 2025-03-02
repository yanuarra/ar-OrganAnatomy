using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfirmationHandler : MonoBehaviour {
    [SerializeField]
    private TMP_Text messageText;
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private GameObject spaceTitle;
    [SerializeField]
    private GameObject objTitle;
    [SerializeField]
    private GameObject overlay_Confirmation;
    [SerializeField]
    private GameObject group;
    [SerializeField]
    private Button buttonYes;
    [SerializeField]
    private Button buttonNo;
    private event Action YesEvent;
    private event Action NoEvent;
    private float DefaultRectHeigth;
    [SerializeField]
    private Vector3 origin;
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private RectTransform messageRect;
    public static ConfirmationHandler Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        DefaultRectHeigth = messageRect.sizeDelta.y;
        rect = rect == null ? group.GetComponent<RectTransform>() : rect;
        origin = group.transform.localPosition;
        buttonNo?.onClick.AddListener(delegate { No(); });
        buttonYes?.onClick.AddListener(delegate { Yes(); });
        StartCoroutine(init());
    }

    IEnumerator init()
    {
        yield return new WaitForEndOfFrame();
        Vector3 newPos = new Vector3(0, group.transform.localPosition.y - rect.sizeDelta.y, 0);
        group.transform.localPosition = newPos;
        toggleOverlay(false);
    }

    void toggleOverlay(bool state)
    {
        overlay_Confirmation?.SetActive(state);
    }

    public void ActivePanel(string message, Action yes = null, Action no = null) {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        spaceTitle.SetActive(false);
        objTitle.SetActive(false);
        if (buttonYes != null) buttonYes.gameObject.SetActive(false);
        buttonNo.gameObject.SetActive(true);
        YesEvent = yes;
        NoEvent = no;
        messageText.text = message;
    }

    public void ActivePanel(string title, string message, Action yes = null, Action no = null) {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        titleText.text = title;
        messageText.text = message;
        Vector3 newPos = new Vector3(0, origin.y, 0);
        Action a = () => toggleOverlay(true);
        LeanTween.moveLocalY(group, origin.y, .3f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(a);
        spaceTitle.SetActive(true);
        objTitle.SetActive(true);
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);
        YesEvent = yes;
        NoEvent = no;
        PanelHandler.Instance.SetNewCurrentOverlay(overlay_Confirmation);
    }

    public void ActivePanelPlay(string title, string message, Action yes = null, Action no = null) {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Confirmation.SetActive(true);
        spaceTitle.SetActive(true);
        objTitle.SetActive(true);
        if (buttonYes != null) buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(false);
        YesEvent = yes;
        NoEvent = no;
        titleText.text = title;
        messageText.text = message;
    }

    public void Yes() {
        Action a = () => toggleOverlay(false);
        LeanTween.moveLocalY(group, group.transform.localPosition.y - rect.sizeDelta.y, .35f)
           .setEase(LeanTweenType.easeInCubic)
           .setOnComplete(a);
        if (YesEvent != null)
        {
            YesEvent.Invoke();
        }
    }

    public void No() {
        Action a = () => toggleOverlay(false);
        LeanTween.moveLocalY(group, group.transform.localPosition.y - rect.sizeDelta.y, .35f)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(a);
        if (NoEvent != null)
        {
            NoEvent.Invoke();
        }
        if (BackHandler.Instance.IsBackEventNull())
        {
            PanelHandler.Instance.ActiveCurrentOverlay();
        }
    }

    public bool IsOverlayActive() {
        return overlay_Confirmation.activeSelf;
    }
}

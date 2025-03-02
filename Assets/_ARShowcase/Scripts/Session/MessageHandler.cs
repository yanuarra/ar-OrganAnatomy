using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.UI.Image;
using System.Collections;

public class MessageHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text messageText;
    [SerializeField]
    private RectTransform messageRect;
    [SerializeField]
    private GameObject overlay_Message;
    [SerializeField]
    private GameObject group;
    [SerializeField]
    private Button buttonOK;
    private event Action OkEvent;
    private float DefaultRectHeigth;
    private float TextRowHeigth;
    [SerializeField]
    private Vector3 origin;
    [SerializeField]
    private RectTransform rect;

    public static MessageHandler Instance { get; private set; }

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
        DefaultRectHeigth = messageRect.sizeDelta.y;
        rect = rect==null? group.GetComponent<RectTransform>():rect;
        origin = group.transform.localPosition;
        buttonOK?.onClick.AddListener(delegate { Ok(); });
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        Vector3 newPos = new Vector3(0, group.transform.localPosition.y - rect.sizeDelta.y, 0);
        group.transform.localPosition = newPos;
        toggleOverlay(false);
    }

    public void ActivePanel(string title, string message, Action ok = null)
    {
        PanelHandler.Instance.DeactiveCurrentOverlay();
        overlay_Message.SetActive(true);
        titleText.text = title;
        messageText.text = message;
        Vector3 newPos = new Vector3(0, origin.y, 0);
        //LeanTween.moveLocal(group, newPos, .5f).setEase(LeanTweenType.easeInExpo);    
        Action a = () => toggleOverlay(true);
        LeanTween.moveLocalY(group, origin.y, .25f)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(a);
        OkEvent = ok == null? ok : null;
        messageText.text = message;
        titleText.text = title;
        PanelHandler.Instance.SetNewCurrentOverlay(overlay_Message, OkEvent);
        //int rowAmount = Mathf.CeilToInt(GetMessageRectHeight(message) / TextRowHeigth);
        //messageRect.sizeDelta = new Vector2(messageRect.sizeDelta.x, DefaultRectHeigth * rowAmount);
    }

    //public float GetMessageRectHeight(string s)
    //{
    //    TextGenerator textGen = new TextGenerator();
    //    Vector2 contentSize = new Vector2(messageText.rectTransform.rect.size.x, messageText.rectTransform.rect.size.y);
    //    TextGenerationSettings generationSettings = messageText.GetGenerationSettings(contentSize);
    //    float height = Mathf.Floor(textGen.GetPreferredHeight(s, generationSettings));
    //    return height;
    //}

    public void Ok()
    {
        if (OkEvent != null)
        {
            OkEvent.Invoke();
        }
        Action a = () => toggleOverlay(false);
        LeanTween.moveLocalY(group, group.transform.localPosition.y - rect.sizeDelta.y, .35f)
            .setEase(LeanTweenType.easeOutCubic)
            .setOnComplete(a);
        //origin = group.transform.localPosition;
    }

    void toggleOverlay(bool state)
    {
        overlay_Message?.SetActive(state);
    }

    public bool IsOverlayActive()
    {
        return overlay_Message.activeSelf;
    }
}

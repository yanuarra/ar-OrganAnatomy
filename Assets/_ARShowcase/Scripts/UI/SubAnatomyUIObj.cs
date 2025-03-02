using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unjani.Module;
using Unjani.Quiz;

public class SubAnatomyUIObj : MonoBehaviour
{
    [SerializeField] private TMP_Text text_name;
    [SerializeField] private TMP_Text text_desc;
    public Toggle toggle;
    [SerializeField] private ContentSizeFitter contentFit;
    [SerializeField] private Image img_anatomy;
    [SerializeField] private Image checkmark;
    [SerializeField] private Image unCheckmark;
    [SerializeField] private ToggleGroup toggleGroup;
    public SubAnatomyData data;
    private bool isExpand = false;
    //[SerializeField] Sprite spriteThumbnail;
    [SerializeField] Texture2D tex;
    public void Init(SubAnatomyData _data)
    {
        data = _data;
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (toggleGroup == null) toggleGroup = transform.parent.GetComponent<ToggleGroup>();
        if (text_name != null) text_name.text = data.title;
        if (text_desc != null) text_desc.text = data.description;
        gameObject.name = string.Format(("sub_{0}"), data.title.Replace("_", " "));
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(delegate { OnClick(); });
        toggle.onValueChanged.AddListener(delegate { ChangeCheckMark(); });
        toggle.group = toggleGroup;
        toggleGroup.allowSwitchOff = true;
        ChangeCheckMark();
        if (string.IsNullOrEmpty(data.thumbnail.name) &&
            string.IsNullOrEmpty(data.thumbnail.url) &&
            data.thumbnail.thumbnailSprite == null)
        {
            img_anatomy.gameObject.SetActive(false);
        }
        else
        {
            img_anatomy.gameObject.SetActive(true);
            img_anatomy.preserveAspect = true;
            img_anatomy.sprite = data.thumbnail.thumbnailSprite;
        }
    }


    public void FindSubAnatomyPopUp()
    {
        AnatomyPartObject cur = Anatomy3DObjectHandler.Instance.curActiveObj;
        if (cur != null) cur.TogglePopUpAndLabel(data.title, toggle.isOn);
     }

    public void OnClick()
    {
        ChangeSubSize();
        MenuUIHandler.Instance.FindAnatomySubInfo(data.title, toggle.isOn);
        FindSubAnatomyPopUp();
    }

    public void ChangeSubSize()
    {
        Canvas.ForceUpdateCanvases();
        if (toggle.isOn)
        {
            RectTransform rt = GetComponent(typeof(RectTransform)) as RectTransform;
            RectTransform rtc = contentFit.gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
            LayoutElement le = GetComponent(typeof(LayoutElement)) as LayoutElement;
            //if (!string.IsNullOrEmpty(data.thumbnail.name) &&
            //    !string.IsNullOrEmpty(data.thumbnail.url) &&
            //    data.thumbnail.thumbnailSprite != null) 
            //    img_anatomy.gameObject.SetActive(true);
            contentFit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            Canvas.ForceUpdateCanvases();
            le.preferredHeight = rtc.sizeDelta.y+50;
            //le.minHeight = 200;
            //float curWidth = rt.rect.width;
            //rt.sizeDelta = new Vector2(curWidth, 200);
        }
        else
        {
            RectTransform rt = GetComponent(typeof(RectTransform)) as RectTransform;
            LayoutElement le = GetComponent(typeof(LayoutElement)) as LayoutElement;
            contentFit.verticalFit = ContentSizeFitter.FitMode.MinSize;
            //img_anatomy.gameObject.SetActive(false);
            Canvas.ForceUpdateCanvases();
            le.preferredHeight = 110;
            //le.minHeight = 105;
            //float curWidth = rt.rect.width;
            //rt.sizeDelta = new Vector2(curWidth, 105);
        }
    }

    public void SubUIToggle()
    {
        toggle.SetIsOnWithoutNotify(!toggle.isOn);
    }

    void ChangeCheckMark()
    {
        checkmark.gameObject.SetActive(toggle.isOn);
        unCheckmark.gameObject.SetActive(!toggle.isOn);
    }
}

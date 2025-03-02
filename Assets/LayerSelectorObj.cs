
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System;

public class LayerSelectorObj : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text_name;
    private ToggleGroup toggleGroup;
    private Toggle toggle;
    private UnityEvent onClickEvent;
    private Action<int> onClickEvents;
    private int index;
    //private Delegate onClickEvent;
    [SerializeField]
    private GameObject textOnlyGO;
    [SerializeField]
    private GameObject iconOnlyGO;
    [SerializeField]
    private GameObject outline;
    private bool isMain;

    public void Init(string _name, bool _isActive, ToggleGroup _toggleGroup, /*/*Action<int> _event,*/ int _index)
    {
        if (toggle == null) toggle = GetComponent<Toggle>();
        if (text_name == null) text_name = GetComponentInChildren<TMP_Text>();
        text_name.text = _name.Replace("_", " ");
        //onClickEvents = _event;s
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(delegate { OnClick(); });
        toggle.group = _toggleGroup;
        isMain = _isActive;
        toggle.SetIsOnWithoutNotify(_isActive);
        index = _index;
        textOnlyGO.gameObject.SetActive(!isMain);
        iconOnlyGO.gameObject.SetActive(isMain);
        outline.gameObject.SetActive(!isMain);
        ChangeColor();
    }

    void OnClick()
    {
        if (toggleGroup == null) toggleGroup = GetComponentInParent<ToggleGroup>();
        toggle.group = toggleGroup;
        if (toggle.isOn) 
        { 
            Anatomy3DObjectHandler.Instance.DivideObject(index);
            if (!isMain) outline.gameObject.SetActive(false);
        }
        else
        {
            if (!isMain) outline.gameObject.SetActive(true);
        }
        ChangeColor();
    }

    void ChangeColor()
    {
        text_name.color = toggle.isOn ? Colors.FromHex("038D2B") : Colors.FromHex("404040");
        toggle.image.color = toggle.isOn ? Colors.FromHex("CEE9D5") : Colors.FromHex("FFFFFF");
    }
}

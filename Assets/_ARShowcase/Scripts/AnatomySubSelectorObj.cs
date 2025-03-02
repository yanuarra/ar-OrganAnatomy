using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unjani.Module;

public class AnatomySubSelectorObj : MonoBehaviour
{
    [SerializeField]
    private Toggle tgl;
    [SerializeField]
    private TMP_Text text_name;
    public AnatomyData partData;
    [SerializeField]
    ToggleGroup toggleGroup;
    AnatomyData anatomyData;

    public void Init(string _name, AnatomyData _anatomyData, bool _isActive, ToggleGroup _toggleGroup)
    {
        anatomyData = _anatomyData;
        if (tgl == null) tgl = GetComponent<Toggle>();
        if (text_name == null) text_name = GetComponentInChildren<TMP_Text>();
        text_name.text = _name.Replace("_", " ");
        partData = _anatomyData;
        tgl.onValueChanged.RemoveAllListeners();
        tgl.onValueChanged.AddListener(delegate { OnClick(); });
        tgl.group = _toggleGroup;
        tgl.SetIsOnWithoutNotify(_isActive);
        ChangeColor();
    }

    public void OnClick()
    {
        if (toggleGroup == null) toggleGroup = GetComponentInParent<ToggleGroup>();
        tgl.group = toggleGroup;
        if (tgl.isOn)
            Anatomy3DObjectHandler.Instance.OnAnatomySelected(anatomyData, true);
            //StartCoroutine( Anatomy3DObjectHandler.Instance.EnableObject(anatomyData, true));
        ChangeColor();
    }

    public void ChangeColor()
    {
        text_name.color = tgl.isOn? Colors.FromHex("038D2B"): Colors.FromHex("404040");
    }
}

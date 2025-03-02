using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unjani.Module;
using easyar;

public class AnatomyARSelectorObj : MonoBehaviour
{
    [SerializeField]
    private Button btn;
    [SerializeField]
    private TMP_Text text_name;
    private Action act;
    public AnatomyData anatomyData;

    public void Init(string _name, AnatomyData _anatomyData)
    {
        anatomyData = _anatomyData;
        //partData = ConverterDataToLocal.Instance.ConvertAnatomyJsonToAnatomyLocal(_anatomyData);
        if (btn == null) btn = GetComponentInChildren<Button>();
        if (text_name == null) text_name = GetComponentInChildren<TMP_Text>();
        text_name.text = _name.Replace("_", " ");
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(delegate { OnClick(); });
    }

    void OnClick()
    {
        Anatomy3DObjectHandler.Instance.OnAnatomySelected(anatomyData, false);
        //StartCoroutine(Anatomy3DObjectHandler.Instance.EnableObject(anatomyData, false)); 
        //Anatomy3DObjectHandler.Instance.DisableObjects();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUIObj : MonoBehaviour
{
    public Toggle tgl;
    public LookAtTransformCustom lookat;
    public string id;
    ChangeOutlineHandler outlineHandler;
    public PopUpUILabelObj labelObj;
    Camera cam;
    GameObject rootObj;
    CanvasGroup canvasGroup;
    public void Init(string _id, GameObject _root)
    {
        rootObj = _root;
        if (lookat == null) lookat = GetComponent<LookAtTransformCustom>();
        if (tgl == null) tgl = GetComponent<Toggle>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
#if PLATFORM_ANDROID
        lookat.type = LookAtTransformCustom.lookAtType.Facing;
#endif
#if UNITY_EDITOR
        lookat.type = LookAtTransformCustom.lookAtType.Facing;
#endif
        cam = Camera.main;
        lookat.target = cam.transform;
        tgl.onValueChanged.RemoveAllListeners();
        tgl.onValueChanged.AddListener(delegate { OnClick(); });
        id = _id;   
        if (outlineHandler == null) outlineHandler = GetComponentsInParent<ChangeOutlineHandler>().FirstOrDefault();
        lookat.LookAtInstant();
        SetToggleGroup();
    }

    void SetToggleGroup()
    {
        if (tgl.group == null)
        {
            tgl.group = GetComponentInParent<AnatomyPartObject>().popUPTG;
            tgl.group.allowSwitchOff = true;
        }
    }

    void LateUpdate()
    {
#if PLATFORM_ANDROID
        //if (Input.touchCount == 0) return;
        if (cam == null) return;
#endif
        if (Vector3.Distance(cam.transform.position, this.gameObject.transform.position) > 
            Vector3.Distance(cam.transform.position, rootObj.gameObject.transform.position))
        {
            canvasGroup.alpha = 0.1f;
            canvasGroup.interactable = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
        }
    }

    void OnClick()
    {
        SetToggleGroup();
        MenuUIHandler.Instance.FindAnatomySubInfo(id, tgl.isOn);
        AnatomyPartObject asd = Anatomy3DObjectHandler.Instance.curActiveObj;
        asd.TogglePopUpAndLabel(id, tgl.isOn);
    }

    public void ToggleSelectedOutline(bool _state)
    {
        tgl.SetIsOnWithoutNotify(_state);
        if (outlineHandler == null) return;
        if (_state)
        {
            outlineHandler.DefaultOutline();
        }
        else
        {
            outlineHandler.DisableOutline();
        }
    }
}

using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUILabelObj : MonoBehaviour
{
    public LookAtTransformCustom lookat;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Image labelOutline;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color defaultOutlineColor;
    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Toggle toggle;
    private ChangeOutlineHandler outlineHandler;
    private Camera cam;
    private GameObject rootObj;
    private CanvasGroup canvasGroup;
    private Color color;
    public LineRendererHandler lineRenderer;
    public PopUpUIObj popUp;
    private string id;
     
    public void InitPopUpLabel(string _name, GameObject _root, PopUpUIObj _poUp)
    {
        if (labelText == null) this.GetComponentInChildren<TMP_Text>();
        if (lookat == null) lookat = GetComponent<LookAtTransformCustom>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
#if PLATFORM_ANDROID
        lookat.type = LookAtTransformCustom.lookAtType.Facing;
#endif
#if UNITY_EDITOR
        lookat.type = LookAtTransformCustom.lookAtType.Facing;
#endif
        cam = Camera.main;
        rootObj = _root;
        lookat.target = cam.transform;
        lookat.LookAtInstant();
        id = _name;
        labelText.text = id;
        OnDeselect();
        toggle.onValueChanged.RemoveAllListeners();
        toggle.onValueChanged.AddListener(delegate { OnClick(); });
        popUp = _poUp;
        toggle.interactable = true;
        canvasGroup.interactable = true;
        SetToggleGroup();
    }

    void SetToggleGroup()
    {
        if (toggle.group == null) {
            toggle.group = GetComponentInParent<AnatomyPartObject>().labelTG;
            toggle.group.allowSwitchOff = true;
        }

    }
    void OnClick()
    {
        SetToggleGroup();
        MenuUIHandler.Instance.FindAnatomySubInfo(id, toggle.isOn);
        AnatomyPartObject asd = Anatomy3DObjectHandler.Instance.curActiveObj;
        asd.TogglePopUpAndLabel(id, toggle.isOn);
    }

    void LateUpdate()
    {
#if PLATFORM_ANDROID
        //if (Input.touchCount == 0) return;
        if (cam == null) return;
#endif
        if (Vector3.Distance(cam.transform.position, this.gameObject.transform.position) >=
            Vector3.Distance(cam.transform.position, rootObj.gameObject.transform.position))
        {
            canvasGroup.alpha = 0.05f;
            if (lineRenderer != null)
            {
                lineRenderer.line.endColor = new Color(Color.black.r, Color.black.g, Color.black.b, 0.1f);
                lineRenderer.line.startColor = new Color(Color.black.r, Color.black.g, Color.black.b, 0.1f);
            }
        }
        else
        {
            canvasGroup.alpha = 1;
            if (lineRenderer != null)
            {
                lineRenderer.line.endColor = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
                lineRenderer.line.startColor = new Color(Color.black.r, Color.black.g, Color.black.b, 1f);
            }
        }
    }

    public void OnSelect()
    {
        toggle.SetIsOnWithoutNotify(true);
        SetColorGreen();
    }

    public void OnDeselect()
    {
        toggle.SetIsOnWithoutNotify(false);
        SetColorDefault();
    }

    void SetColorDefault()
    {
        labelOutline.color = defaultOutlineColor;
        labelText.color = defaultTextColor;
    }

    void SetColorGreen()
    {
        labelOutline.color   = greenColor;
        labelText.color = greenColor;
    }
}
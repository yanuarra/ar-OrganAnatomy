using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CustomToggleProperties))]
public class CustomToggle : Toggle, IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{
    private Color defaultColor;
    private CustomToggleProperties properties;

    public void SetProperties(CustomToggleProperties _properties)
    {
        properties = _properties;
        switch (properties.type)
        {
            case CustomToggleProperties.Type.primary:
                defaultColor = properties.primaryColor;
                break;
            case CustomToggleProperties.Type.secondary:
                defaultColor = properties.secondaryColor;
                break;
        }
        CheckCurrentState();
        this.onValueChanged.AddListener(delegate { CheckCurrentState(); });
    }

    void CheckCurrentState() {
        if (interactable)
        {
            if (isOn)
            {
                SetSelectedState();
            }
            else
            {
                SetNormalState();
            }
        }
        else
        {
            SetDisabledState();
        }
    }

    public void SetDisabledState() => DoStateTransition(SelectionState.Disabled, true);
    public void SetPressedState() => DoStateTransition(SelectionState.Pressed, true);
    public void SetSelectedState() => DoStateTransition(SelectionState.Selected, true);
    public void SetNormalState() => DoStateTransition(SelectionState.Normal, true);

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (properties == null) return;
        switch (state)
        {
            case SelectionState.Normal:
                if (isOn) break;
                properties.SetColor(defaultColor, properties.enableColorContent);
                break;
            case SelectionState.Highlighted:
                //if (!isOn) properties.SetColor(properties.highlightColor, properties.enableColorContent);
                if (!isOn) properties.SetColor(properties.highlightColor, properties.enableColorContent);
                break;
            case SelectionState.Pressed:
                properties.SetColor(properties.selectedBGColor, properties.selectedColor);
                break;
            case SelectionState.Selected:
                properties.SetColor(properties.selectedBGColor, properties.selectedColor);
                break;
            case SelectionState.Disabled:
                properties.SetColor(properties.disableColor, properties.disableColorContent);
                break;
            default:
                break;
        }
        properties.ToggleCheckIcon(isOn);
    }

    public override void OnPointerDown(PointerEventData eventData) {
        //AudioHandler.Instance.PlayButtonClicked();
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        //AudioHandler.Instance.PlayButtonHighlight();
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //AudioHandler.Instance.PlayButtonClicked();
        base.OnPointerClick(eventData);
        CheckCurrentState();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Networking.PlayerConnection;

public class CustomToggleProperties : MonoBehaviour
{
    public enum Type
    {
        primary,
        secondary
    }
    public enum AffectedElement
    {
        iconBgText,
        iconText,
        icon
    }

    public Type type;
    public AffectedElement affectedElement;
    public Color primaryColor;
    public Color secondaryColor;
    public Color highlightColor;
    public Color pressedColor;
    public Color selectedColor;
    public Color selectedBGColor;
    public Color disableColor;
    public Color enableColorContent;
    public Color disableColorContent;

    [SerializeField] private Image background;
    [SerializeField] private Image iconButton;
    [SerializeField] private Sprite iconCheck, iconUncheck;
    [SerializeField] private TMP_Text textButton;
    private CustomToggle customToggle;

    private void Awake()
    {
        customToggle = this.GetComponent<CustomToggle>();
    }

    private void Start()
    {
        customToggle.SetProperties(this);
    }

    public void SetColor(Color colorBg, Color colorContent)
    {
        switch (affectedElement)
        {
            case AffectedElement.iconBgText:
                if (background != null) background.color = colorBg;
                if (textButton != null) textButton.color = colorContent;
                if (iconButton != null) iconButton.color = colorContent;
                break;
            case AffectedElement.iconText:
                if (iconButton != null) iconButton.color = colorContent;
                if (textButton != null) textButton.color = colorContent;
                break;
            case AffectedElement.icon:
                if (iconButton != null) iconButton.color = colorContent;
                break;

        }
    }

    public void ToggleCheckIcon(bool _state)
    {
        if (iconCheck != null && iconUncheck != null)
            iconButton.sprite = _state ? iconCheck : iconUncheck;
    }
}

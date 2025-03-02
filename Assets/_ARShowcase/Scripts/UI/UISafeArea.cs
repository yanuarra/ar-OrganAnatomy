using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISafeArea : MonoBehaviour
{
    RectTransform rectTransform;
    Rect safeArea;
    Vector2 minAnchor;
    Vector2 maxAnchor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        RefreshPanel(Screen.safeArea);
    }

    private void OnEnable()
    {
        UISafeAreaDetection.OnSafeAreaChanged += RefreshPanel;
    }

    private void OnDisable()
    {
        UISafeAreaDetection.OnSafeAreaChanged -= RefreshPanel;
    }

    private void RefreshPanel(Rect _safeArea)
    {
        safeArea = _safeArea;
        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

        rectTransform.anchorMin = minAnchor;
        rectTransform.anchorMax = maxAnchor;
    }
}

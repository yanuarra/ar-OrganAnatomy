using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomBarUIHandler : MonoBehaviour
{
    [SerializeField] private CustomToggle toggle_Scan;
    [SerializeField] private CustomToggle toggle_AnatomyList;
    [SerializeField] private CustomToggle toggle_Profile;
    [SerializeField] private CustomToggle toggle_Info;
    [SerializeField] private CustomToggle toggle_SubInfo;
    [SerializeField] private CustomToggle toggle_Quiz;

    private void Start()
    {
        toggle_Profile.onValueChanged.AddListener(delegate { MenuUIHandler.Instance.ToggleProfile(); });
    }
}

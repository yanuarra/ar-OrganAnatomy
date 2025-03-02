using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [SerializeField]
    private GameObject panel_Intro;
    //Login Page Manager here

    // Start is called before the first frame update
    void Start()
    {
        panel_Intro.gameObject.SetActive(true);
    }

    public void GoToLogin()
    {
    }
}

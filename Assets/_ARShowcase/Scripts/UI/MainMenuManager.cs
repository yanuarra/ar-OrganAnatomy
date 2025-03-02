using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI user_text;
    [SerializeField] private Button btn_AR, btn_info, btn_bookmarks, btn_settings, btn_exit;
    [SerializeField] private List<GameObject> panelCol = new();
    public static MainMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        DisableAll();
        btn_info.onClick.RemoveAllListeners();
        btn_info.onClick.AddListener(delegate { ToggleInfo(); });
        btn_AR.onClick.RemoveAllListeners();
        btn_AR.onClick.AddListener(delegate { GoToAR(); });
        //btn_bookmarks.onClick.RemoveAllListeners();
        //btn_bookmarks.onClick.AddListener(delegate { ExitToLogin(); });
        //btn_settings.onClick.RemoveAllListeners();
        //btn_settings.onClick.AddListener(delegate { ExitToLogin(); });
        btn_exit.onClick.RemoveAllListeners();
        btn_exit.onClick.AddListener(delegate { Quit(); });
        //AudioHandler.Instance.PlayBGM();
        //AudioHandler.Instance.ChangeBGMVolume(.1f);
        ToggleContent(true);
    }

    public void ToggleInfo()
    {
        DisableAll();
    }

    public void ToggleContent(bool _state)
    {
        DisableAll();
        GameObject content = panelCol.Where(k => k.gameObject.name.Contains("ContentScrollView")).FirstOrDefault();
        content.SetActive(_state);
    }

    public void DisableAll()
    {
        foreach (var item in panelCol)
        {
            item.SetActive(false);
        }
    }

    public void GoToAR()
    {
        SceneLoader.Instance.LoadScene("AR");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

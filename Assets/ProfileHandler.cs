using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text usernameText, nimText;
    [SerializeField]
    private Button logoutBtn, loginBtn;
    [SerializeField]
    private GameObject overlay;
    [SerializeField]
    private GameObject isLogin, isNotLogin;
    private Image profilePicImage;

    public void ToggleOverlay(bool _state) => overlay?.SetActive(_state);

    private void OnEnable()
    {
        logoutBtn.onClick.RemoveAllListeners();
        logoutBtn.onClick.AddListener(delegate { EnableLogoutPanel(); });
        loginBtn.onClick.RemoveAllListeners();
        loginBtn.onClick.AddListener(delegate { GoToLogin(); });
        usernameText.text = StaticData.IsUserLoginDataExist() ? StaticData.userLoginData.USER.username : "Guest";
        nimText.text = StaticData.IsUserLoginDataExist() ? StaticData.userLoginData.USER.nim : "Masuk untuk akses semua materi";
        logoutBtn.gameObject.SetActive(StaticData.IsUserLoginDataExist());
        loginBtn.gameObject.SetActive(!StaticData.IsUserLoginDataExist());
        isLogin.gameObject.SetActive(StaticData.IsUserLoginDataExist());
        isNotLogin.gameObject.SetActive(!StaticData.IsUserLoginDataExist());
    }

    void EnableLogoutPanel()
    {
        ConfirmationHandler.Instance.ActivePanel(
            "Keluar Aplikasi?",
            "Anda yakin ingin keluar aplikasi?",
            Logout,
            PanelHandler.Instance.DeactiveCurrentOverlay
            );
    }

    void GoToLogin()
    {
        StaticData.Logout();
        SceneLoader.Instance.LoadScene("Intro");
    }

    void Logout()
    {
        StaticData.Logout();
        SceneLoader.Instance.LoadScene("Intro");
    }
}

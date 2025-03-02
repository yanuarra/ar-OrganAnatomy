using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
class Login
{
    public string username;
    public string password;
    //public string token;

    public Login(string _username, string _password/*, string _token*/)
    {
        username = _username;
        password = _password;
        //token = _token;
    }
}

[Serializable]
class LoginErrorResponse
{
    public string message;
    public string GetErrorResponse()
    {
        return message;
    }
}

public class LoginHandler : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField]
    private GameObject loginObj;
    [SerializeField]
    private GameObject mainMenuObj;

    [Header("Login UI")]
    //[SerializeField]
    //private List<TMP_InputField> inputFieldList;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private Toggle isShowPassword;
    public TMP_InputField usernameText, passwordText;
    public TMP_Text errorTextUsername, errorTextPass;
    public Image errorImgUsername, errorImgPass;
    public string username, password;
    public TMP_Text errorText;
    public bool isAutofill = false;
    [SerializeField]
    private UnityEvent onLoginSuccess;
    [SerializeField]
    private UnityEvent onLogoutSuccess;
    //private string rootURL = "https://vr-sejarah-unjani.metanesia.id"; 
    private string rootURL2 = "https://vr-sejarah.metanesia.id";
    //private string rootURL2 = "http://192.168.1.13:4000";
    //private string token = "f8c11aa4bdeaed529da636d3d00aed27";
    private string placeholder = "●";
    
    private void Awake()
    {
        //if (!StaticData.IsUserLoginDataExist())
        //{
        //    loginObj.SetActive(true);
        //    mainMenuObj.SetActive(false);
        //}
        //else
        //{
        //    loginObj.SetActive(false);
        //    mainMenuObj.SetActive(true);
        //}
        loginObj.SetActive(false);
        mainMenuObj.SetActive(true);
    }

    private void Start()
    {
        Reset();
        RegisterPassword();
        RegisterUsername();
        MaskPasswordToggle();
        isShowPassword.onValueChanged.AddListener(delegate { MaskPasswordToggle(); });
        passwordText.onValueChanged.AddListener(delegate { MaskPasswordToggle(); });
    }

    private void Reset()
    {
        errorTextUsername.text = "";
        errorTextPass.text = "";
        errorText.text = "";
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        errorTextUsername.gameObject.SetActive(false);
        errorTextPass.gameObject.SetActive(false);
        errorImgUsername.gameObject.SetActive(false);
        errorImgPass.gameObject.SetActive(false);
        //MonoBehaviorExtensions.RebuildLayout(canvas); 
    }

    public void MaskPasswordToggle()
    {
        passwordText.asteriskChar = placeholder.ToCharArray()[0];
        passwordText.contentType = isShowPassword.isOn? TMP_InputField.ContentType.Standard:TMP_InputField.ContentType.Password;
        passwordText.ForceLabelUpdate();
    }

    public void RegisterPassword()
    {
        if (passwordText == null) return;
        if (isAutofill) AutoFill("PASS");
    }

    public void RegisterUsername()
    {
        if (username == null) return;
        if (isAutofill) AutoFill("ID");
    }

    public void Login()
    {
        Reset();
        if (usernameText.text.Length == 0 || passwordText.text.Length == 0)
        {
            //Empty Field
            ErrorHandler( "Masukkan NIM kamu disini", "Masukkan kata sandi yang sesuai" );
            return;
        }
        //if (string.IsNullOrEmpty(StaticData.GetUserToken())) StaticData.SetUserToken(token);
        APIHelper.Instance.SetRootURL(rootURL2);
        Login log = new(usernameText.text, passwordText.text);
        string postData = JsonUtility.ToJson(log);
        StartCoroutine(APIHelper.Instance.PostDataCoroutine("api/guest/v1/login", postData, false, result =>
        {
            if (StaticData.requestError)
            {
                //LoginErrorResponse err = JsonUtility.FromJson<LoginErrorResponse>(result);
                //errorText.text = err.GetErrorResponse();
                //errorText.text = result;
                ErrorHandler( "Format NIM tidak sesuai", "NIM atau kata sandi belum sesuai" );
            }
            else
            {
                Debug.Log(result);
                StaticData.SetUserTokenResponse(result);
                APIHelper.Instance.SetRootURL(rootURL2);
                StartCoroutine(APIHelper.Instance.GetDataCoroutine("api/guest/v1/kuis/ar/me", StaticData.GetUserToken(), result =>
                {
                    if (StaticData.requestError)
                    {
                        Debug.Log("Error nih");
                    }
                    else
                    {
                        Debug.Log(result);
                        StaticData.SetUserLoginData(result);
                        onLoginSuccess?.Invoke();
                    }
                }
                ));}
        }));
    }

    public void Logout()
    {
        if (errorText != null) errorText.text = "";
        StaticData.Logout();
        onLogoutSuccess?.Invoke();
        Debug.Log("Successfully Logout");
    }

    public void AutoFill(string fill)
    {
        if (fill=="ID")
        {
            usernameText.text = username;
        }
        else
        {
            passwordText.text = password;
        }
    }

    public void ErrorHandler(string _msgUsername, string _msgPass)
    {
        errorTextUsername.gameObject.SetActive(true);
        errorTextPass.gameObject.SetActive(true);
        errorImgUsername.gameObject.SetActive(true);
        errorImgPass.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        errorTextUsername.text = _msgUsername;
        errorTextPass.text = _msgPass; 

    }

}
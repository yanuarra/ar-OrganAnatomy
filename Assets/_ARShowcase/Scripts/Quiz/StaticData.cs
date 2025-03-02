
using UnityEngine;

public static class StaticData
{
    public static string ModuleId;
    public static string ModuleName;
    public static string AnatomyDataID;
    public static string SceneNameBackFromQuiz;
    public static UserLogin userLoginData;
    public static Root responseData;
    public static string errorMessage;
    public static bool requestError;
    public static bool apiError;

    public static UserLogin GetUserLogin()
    {
        return userLoginData;
    }

    public static string GetUserToken()
    {
        if (userLoginData == null)
        {
            userLoginData = new UserLogin();
            userLoginData.TOKENS_TOKEN = "";
        }
        if (userLoginData.TOKENS_TOKEN == null)
        {
            userLoginData.TOKENS_TOKEN = "";
        }
        if (userLoginData.TOKENS_TOKEN.Length > 0)
        {
            return userLoginData.TOKENS_TOKEN;
        }
        else
        {
            return "";
        }
    }

    public static bool IsUserLoginDataExist()
    {
        if (userLoginData != null)
        {
            return !string.IsNullOrEmpty(userLoginData.TOKENS_TOKEN);
        }
        return false;
    }

    public static void SetUserToken(string token)
    {
        if (userLoginData == null)
        {
            userLoginData = new UserLogin();
        }
        userLoginData.TOKENS_TOKEN = token;
    }

    public static void SetUserTokenResponse(string json)
    {
        ResponseData response = JsonUtility.FromJson<ResponseData>(json);
        Debug.Log(responseData);
        if (userLoginData == null)
            userLoginData = new UserLogin();
        userLoginData.TOKENS_TOKEN = response.token;
    }

    public static void SetUserLoginData(string json)
    {
        User userdata = JsonUtility.FromJson<User>(json);
        if (userLoginData == null)
            userLoginData = new UserLogin();
        if (userLoginData.USER == null)
            userLoginData.USER = new User();
        Debug.Log(userdata.username);
        userLoginData.USER.username = userdata.username;
        userLoginData.USER.nim = userdata.nim;
        userLoginData.USER.kuis = userdata.kuis;
    }

    public static void Logout()
    {
        userLoginData = new UserLogin();
        userLoginData.TOKENS_TOKEN = "";
    }
}

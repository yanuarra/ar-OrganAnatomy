using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

[Serializable]
public class UserLogin
{
    public User USER;
    public string TOKENS_TOKEN;
    public string SESSION_ID;
}

[Serializable]
public class User
{
    public string message;
    public string nim;
    public string username;
    public List<object> kuis = new List<object>();
}

[Serializable]
public class ResponseData
{
    public string message;
    public string token;
    public bool status;
}

public class Root
{
    public ResponseData responseData;
}
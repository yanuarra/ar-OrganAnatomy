using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIHelper : MonoBehaviour
{
    //https://api-ar-showcase-unjani.metanesia.id/modules
    [SerializeField]
    private string _rootUrl;
    public static APIHelper Instance;
    private string _apiKey;

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
    public void SetRootURL(string url) => _rootUrl = url;
    public string GetRootURL()
    {
        return _rootUrl;
    }

    public IEnumerator PostDataCoroutine(string subUri, string postData, bool _isTokenRequired, Action<string> SetDataEvent)
    {
        string uri = string.Format("{0}/{1}", _rootUrl, subUri);
        byte[] rawData = Encoding.UTF8.GetBytes(postData);
        UnityWebRequest uwr = UnityWebRequest.PostWwwForm(uri, postData);
        uwr.uploadHandler = new UploadHandlerRaw(rawData);
        uwr.SetRequestHeader("Content-Type", "application/json");
        if (_isTokenRequired)
            uwr.SetRequestHeader("Authorization", "Bearer " + StaticData.userLoginData.TOKENS_TOKEN);
        /*
        uwr.SetRequestHeader("Api-Key", _apiKey);
        if (!string.IsNullOrEmpty(StaticData.GetUserToken())) { }*/
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();  
        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                StaticData.requestError = false;
                print("url : " + uri + ", respond : " + uwr.downloadHandler.text);
                SetDataEvent?.Invoke(uwr.downloadHandler.text);
                break;
            case UnityWebRequest.Result.ProtocolError:
                if (uwr.responseCode != 200 && uwr.responseCode != 201)
                {
                    StaticData.apiError = true;
                    StaticData.errorMessage = "ProtocolError: " + uwr.error;
                }
                else
                {
                    StaticData.apiError = false;
                }
                Debug.Log(StaticData.errorMessage);
                StaticData.requestError = true;
                SetDataEvent?.Invoke(StaticData.errorMessage);
                break;
            case UnityWebRequest.Result.DataProcessingError:
                if (uwr.responseCode != 200 && uwr.responseCode != 201)
                {
                    StaticData.apiError = true;
                    StaticData.errorMessage = "ErroDataProcessingErrorr: " + uwr.error;
                }
                else
                {
                    StaticData.apiError = false;
                }
                Debug.Log(UnityWebRequest.Result.DataProcessingError);
                StaticData.requestError = true;
                SetDataEvent?.Invoke(StaticData.errorMessage);
                break;
            case UnityWebRequest.Result.ConnectionError:
                StaticData.apiError = true;
                StaticData.errorMessage = "ConnectionError: " + uwr.error;
                StaticData.requestError = true;
                SetDataEvent?.Invoke(StaticData.errorMessage);
                break;
            default:
                break;
        }
    }

    public IEnumerator GetDataCoroutine(string subUri, Action<string> setDataEvent)
    {
        yield return new WaitForEndOfFrame();
        string uri = string.Format("{0}/{1}", _rootUrl, subUri);
        Debug.Log("Get Data from : " + uri);
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            if (setDataEvent != null)   
                setDataEvent("");
        }
        else
        {
            if (uwr.responseCode != 200 && uwr.responseCode != 201)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            if (setDataEvent != null)
                setDataEvent(uwr.downloadHandler.text);
        } 
    }

    public IEnumerator GetDataCoroutine(string subUri, string token, Action<string> setDataEvent)
    {
        yield return new WaitForEndOfFrame();
        string uri = string.Format("{0}/{1}", _rootUrl, subUri);
        Debug.Log("Get Data from : " + uri);
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            if (setDataEvent != null)
                setDataEvent("");
        }
        else
        {
            if (uwr.responseCode != 200 && uwr.responseCode != 201)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            if (setDataEvent != null)
                setDataEvent(uwr.downloadHandler.text);
        }
    }

    public IEnumerator GetImageTexture(string imageUrl, Action<Texture2D> onComplete)
    {
        if (imageUrl == null)
        {
            Debug.LogError($"Url download image is null");
            onComplete?.Invoke(null);
            yield return null;
        }
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + www.error);
                onComplete?.Invoke(null);
            }

            if (www.responseCode != 200 && www.responseCode != 201)
            {
                Debug.LogError($"Error loading image: {www.error}");
                onComplete?.Invoke(null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                //Debug.Log($"height of {imageUrl}: {texture.height}");
                onComplete?.Invoke(texture);
            }
        }
    }
}
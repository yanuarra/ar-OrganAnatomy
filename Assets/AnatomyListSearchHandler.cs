using UnityEngine;
using Unjani.Quiz;
using Unjani.Module;
using UnityEngine.UI;
using TMPro;

public class AnatomyListSearchHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    private Button _searchButton;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private Module modulToSearch = new Module();
    [SerializeField]
    private AnatomyListUIHandler anaListUI;

    private void Start()
    {
        _inputField.onEndEdit.AddListener( delegate { SearchByKeyword(_inputField.text); });
        _searchButton.onClick.AddListener( delegate { SearchByKeyword(_inputField.text); });
        _backButton.onClick.AddListener( delegate { BackHandler.Instance.Back(); });
        ToggleBackBtn(false);
    }

    private void ToggleBackBtn(bool _state)
    {
        _backButton?.gameObject.SetActive(_state);
    }

    void SearchByKeyword(string _keyword)
    {
        if (string.IsNullOrEmpty(_keyword) || string.IsNullOrWhiteSpace(_keyword))
        {
            ResetSearch();
            return;
        }
        //https://api-ar-showcase-unjani.metanesia.id/modules?_where[_or][0][title_contains]=Desc&_where[_or][1][description_contains]=Desc
        //https://api-ar-showcase-unjani.metanesia.id/modules/search?search=cranium
        string sub = string.Format("search?search={0}", _keyword);
        StartCoroutine(APIHelper.Instance.GetDataCoroutine(sub, OnGetDataSearch));
    }

    void OnGetDataSearch(string _searchResult)
    {
        var temp = "{\"module\": " + _searchResult + "}";
        Debug.Log(temp);
        modulToSearch = JsonUtility.FromJson<Module>(temp);
        anaListUI.FindByAnatomyData(modulToSearch);
        BackHandler.Instance.AssignBackEvent(delegate {SearchByKeyword("");});
        ToggleBackBtn(true);
    }

    private void ResetSearch()
    {
        _inputField.text = "";
        ToggleBackBtn(false);
        anaListUI.ToggleAllSelector(true);
        anaListUI.ToggleAllSelectorSub(true);
    }
}
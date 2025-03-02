using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unjani.Module;

public class MenuUIHandler : MonoBehaviour
{
    public static MenuUIHandler Instance;
    [SerializeField] private SimpleScrollSnap pullUpSSS;
    [SerializeField] private SimpleScrollSnap pullUpSubAnatomySSS;
    [SerializeField] private PullUpSubUIHandler PullUpSubUIHandler;
    [SerializeField] private PullUpUIHandler PullUpUIHandler;
    [SerializeField] private AnatomyListUIHandler anaListHandler;
    [SerializeField] private TutorialUIHandler tutorialUIHandler;
    [SerializeField] private ProfileHandler profileHandler;
    [SerializeField] private GameObject panel_PullUp;
    [SerializeField] private GameObject panel_PullUpQuiz;
    [SerializeField] private GameObject panel_BottomBar;
    [SerializeField] private GameObject panel_BottomBarScan;
    [SerializeField] private Button btn_Tutorial;
    [SerializeField] private CustomToggle toggle_Scan;
    [SerializeField] private CustomToggle toggle_AnatomyList;
    [SerializeField] private CustomToggle toggle_Profile;
    [SerializeField] private CustomToggle toggle_Info;
    [SerializeField] private CustomToggle toggle_Sub;
    [SerializeField] private CustomToggle toggle_Quiz;
    [SerializeField] private TMP_Text text_scanInfo;
    [SerializeField] private TMP_Text text_pullUpDesc;
    private int index;
    //private AnatomyPart curAnatomyPart;
    private AnatomyData curAnatomyData;
    private List<AnatomyARSelectorObj> ARSelectorCol = new List<AnatomyARSelectorObj>();
    private List<AnatomyData> data = new List<AnatomyData>();
    [SerializeField] private List<SubAnatomyUIObj> SubAnatomyUICol = new List<SubAnatomyUIObj>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public IEnumerator INIT()
    {
        yield return new WaitForEndOfFrame();
        if (pullUpSSS == null) pullUpSSS = panel_PullUp.GetComponent<SimpleScrollSnap>();
        toggle_AnatomyList.onValueChanged.RemoveAllListeners();
        toggle_AnatomyList.onValueChanged.AddListener(delegate { ToggleBottomBar(true); });
        yield return StartCoroutine( anaListHandler.SpawnARSelector());
        PullUpUIHandler.Init();
        PullUpSubUIHandler.Init();
        tutorialUIHandler.Init();
        ResetMenuUI();
        toggle_Quiz.gameObject.SetActive(StaticData.IsUserLoginDataExist());
        ToggleBottomBar(true);
        ToggleTutorialPanel(true);
    }

    public void DisableAllPanels()
    {
        tutorialUIHandler.ToggleTutorial(false);
        panel_BottomBar.SetActive(false);
        panel_BottomBarScan.SetActive(false);
        anaListHandler.ToggleOverlay(false);
        profileHandler.ToggleOverlay(false);
        PullUpSubUIHandler.ToggleOverlay(false);
        PullUpUIHandler.ToggleOverlay(false);
        panel_PullUpQuiz.SetActive(false);
    }

    public void ToggleAnatomyList(bool _state){
        DisableAllPanels();
        toggle_Scan.SetIsOnWithoutNotify(!_state);
        toggle_Profile.SetIsOnWithoutNotify(!_state);
        anaListHandler.ToggleOverlay(_state);
        toggle_AnatomyList.isOn = _state;
        toggle_AnatomyList.SetSelectedState();
    }

    public void InitAnatomyInfoCoroutine(AnatomyPartObject _obj, AnatomyData _anatomyData, bool _isSub)
    {
        tutorialUIHandler.ToggleTutorial(false);
        PanelHandler.Instance.DeactiveCurrentOverlay();
        curAnatomyData = _anatomyData;
        anaListHandler.ToggleOverlay(false);
        PullUpUIHandler.Reset();
        PullUpUIHandler.ToggleOverlay(true);
        PullUpUIHandler.SpawnPullUpAnatomy(_anatomyData, _obj);
        PullUpSubUIHandler.ToggleOverlay(true);
        //yield return StartCoroutine( PullUpSubUIHandler.SpawnPullUpSubAnatomy(_anatomyData));
        //yield return StartCoroutine( PullUpSubUIHandler.SpawnPullUpSubSelector(_anatomyData));
        PullUpSubUIHandler.SpawnPullUpSubAnatomy(_anatomyData);
        PullUpSubUIHandler.SpawnPullUpSubSelector(_anatomyData);
        if (_isSub)
        {
            TogglePullUpSubAnatomy(true);
            ToggleBottomBarScan(true, true);
        }
        else
        {
            TogglePullUp(true);
            ToggleBottomBarScan(true, false);
        }
    }

    public void FindAnatomySubInfo(string _id, bool _state) => PullUpSubUIHandler.FindAnatomySubInfo(_id, _state);
    public AnatomyARSelectorObj ChangeToggleStatus(AnatomyData _activeData) => 
        ARSelectorCol.Find(p => p.GetComponent<AnatomyARSelectorObj>().anatomyData.Equals(_activeData));

    public void ResetMenuUI()
    {
        TogglePullUp(true);
        PullUpSubUIHandler.Reset();
        PullUpUIHandler.Reset();
        foreach (var item in SubAnatomyUICol)
        {
            Destroy(item.gameObject);
        }
        ToggleBottomBar(true);
        SubAnatomyUICol.Clear();
        PanelHandler.Instance.DeactiveCurrentOverlay();
    }

    public void ToggleTutorialPanel(bool _state) 
    { 
        panel_BottomBar.gameObject.SetActive(!_state);
        tutorialUIHandler.ToggleTutorial(_state);
        ARVideoCameraHandler.Instance.EnableCamera(!_state);
    }

    public void ToggleBottomBarScan(bool _state, bool _isSUb)
    {
        panel_BottomBar.SetActive(!_state);
        panel_BottomBarScan.SetActive(_state);
        //btn_Tutorial.gameObject.SetActive(!_state);
        toggle_Info.isOn = !_isSUb;
        toggle_Sub.isOn = _isSUb;
    }

    public void ToggleBottomBar(bool _state) {
        DisableAllPanels();
        panel_BottomBar.SetActive(_state);
        anaListHandler.ToggleOverlay(!toggle_Scan.isOn);
        btn_Tutorial.gameObject.SetActive(toggle_Scan.isOn);
        text_scanInfo.gameObject.SetActive(toggle_Scan.isOn);
        ARVideoCameraHandler.Instance.EnableCamera(toggle_Scan.isOn);   
    }

    public void ToggleProfile()
    {
        DisableAllPanels();
        panel_BottomBar.SetActive(true);
        profileHandler.ToggleOverlay(toggle_Profile.isOn);
        btn_Tutorial.gameObject.SetActive(!toggle_Profile.isOn);
        text_scanInfo.gameObject.SetActive(!toggle_Profile.isOn);
        ARVideoCameraHandler.Instance.EnableCamera(toggle_Scan.isOn);
    }

    public void DisablePullUps()
    {
        PullUpSubUIHandler.ToggleOverlay(false);
        PullUpUIHandler.ToggleOverlay(false);
        panel_PullUpQuiz.SetActive(false);
        panel_BottomBarScan.SetActive(true);
    }

    public void TogglePullUp(bool _state)
    {
        DisablePullUps();
        PullUpUIHandler.ToggleOverlay(_state);
        panel_BottomBarScan.SetActive(_state);
    }

    public void TogglePullUpSubAnatomy(bool _state)
    {
        DisablePullUps();        
        PullUpSubUIHandler.ToggleOverlay(_state);
        panel_BottomBarScan.SetActive(_state);
    }

    public void ToggleQuiz(bool _state)
    {
        DisablePullUps();        
        panel_PullUpQuiz.SetActive(_state);
        panel_BottomBarScan.SetActive(_state);
    }

    public void GoToQuiz()
    {
        ModuleData module = ModuleDataHandler.Instance.GetModuleDataByAnatomyID(curAnatomyData.anatomyId);
        StaticData.ModuleId = module.moduleId;
        StaticData.ModuleName = module.title;
        StaticData.AnatomyDataID = curAnatomyData.anatomyId;
        StaticData.SceneNameBackFromQuiz = "AR";
        SceneLoader.Instance.LoadScene("Quiz");
    }
}

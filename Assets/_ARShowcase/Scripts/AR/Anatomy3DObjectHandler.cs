using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using Unjani.Module;

public class Anatomy3DObjectHandler : MonoBehaviour
{
    Module module;
    public GameObject pivot;
    public AnatomyPartObject curActiveObj;
    AnatomyData curAnatomyData;
    ObjectRot rot;
    ObjectScaleAndPan scaler;
    [SerializeField] List<AnatomyPartObject> objCol = new List<AnatomyPartObject>();
    [SerializeField] ARPrefabHandler aRPrefab;
    public static Anatomy3DObjectHandler Instance { get; private set; }

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
    private void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("SPACE");
            LoadingHandler.Instance.ToggleLoadingPanelAnatomySelect(false);
            curActiveObj.AttachToCamera();
            ApplyInputTarget();
        }
    }

    public IEnumerator INIT()
    {
        yield return new WaitForEndOfFrame();
        rot = FindObjectOfType<ObjectRot>();
        scaler = FindObjectOfType<ObjectScaleAndPan>();
        module = ModuleDataHandler.Instance.module;
        //anatomyDataCol = AnatomyDataHandler.Instance.anatomyCol;
        //List< AnatomyDataContainer> anatomyDataCon = AnatomyDataHandler.Instance.anatomyCol;
        foreach (var module in module.module)
        {
            foreach (var anatomy in module.anatomy)
            {
                //GET AR Prefab DATA
                if (anatomy.ARPrefab != null)
                {
                    GameObject GO = Instantiate(anatomy.ARPrefab);
                    AnatomyPartObject anatomyPartObject = GO.GetComponent<AnatomyPartObject>();
                    GO.transform.SetParent(pivot.transform);
                    GO.transform.position = Vector3.zero;
                    GO.transform.localScale = Vector3.one / 100f;
                    GO.transform.localRotation = Quaternion.identity;
                    anatomyPartObject.Init(anatomy);
                    objCol.Add(anatomyPartObject);
                }
            }
        }
        DisableObjects();
    }
    
    public void DivideObject(int Number)
    {
        if (curActiveObj == null) return;
        curActiveObj.DivideParts(Number);
    }

    public void EnableAllParts()
    {
        if (curActiveObj == null) return;
        curActiveObj.ResetParts();
    }

    void RemoveInputTarget()
    {
        rot.pivot = null;
        scaler.pivot = null;
    }

    void ApplyInputTarget()
    {
        rot.pivot = curActiveObj.pivot.transform;
        scaler.pivot = curActiveObj.pivot.transform;
    }

    public void OnAnatomySelected(AnatomyData _anatomyData, bool _isSub) => StartCoroutine(EnableObject(_anatomyData, _isSub));
    IEnumerator EnableObject(AnatomyData _anatomyData, bool _isSub)
    {
        DisableObjects();
        AnatomyPartObject item = objCol.Find(x => x.partID.Trim().Equals(_anatomyData.anatomyId.Trim()));
        if (item != null) {
            LoadingHandler.Instance.ToggleLoadingPanelAnatomySelect(true);
            curActiveObj = item;
            curAnatomyData = _anatomyData;
            MenuUIHandler.Instance.InitAnatomyInfoCoroutine(curActiveObj, curAnatomyData, _isSub);
            curActiveObj.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(2f);
            LoadingHandler.Instance.ToggleLoadingPanelAnatomySelect(false);
            curActiveObj.gameObject.SetActive(true);
            curActiveObj.AttachToCamera();
            ApplyInputTarget();
            yield return new WaitForEndOfFrame();
        }
        else
        {
            MessageHandler.Instance.ActivePanel(
                "Data Tidak Ditemukan",
                "Data tidak ditemukan untuk marker terkait, coba scan beberapa saat lagi atau coba scan marker yang lain.",
                ARImageTargetHandler.Instance.UnloadTargets
            );
            ARImageTargetHandler.Instance.Reset();
        }
    }

    public void DisableObjects()
    {
        foreach (var item in objCol)
        {
            item.gameObject.SetActive(false);
        }
        if (curActiveObj != null)
        {
            ARImageTargetHandler.Instance.Reset();
            RemoveInputTarget();
            curActiveObj.ReturnToOriginal();
            MenuUIHandler.Instance.ResetMenuUI();
            curAnatomyData = null;
            curActiveObj = null;
        }
    }
}

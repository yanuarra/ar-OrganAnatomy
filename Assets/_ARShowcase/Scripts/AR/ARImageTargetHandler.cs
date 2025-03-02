using System.Collections.Generic;
using UnityEngine;
using easyar;
using System.Collections;
using Unjani.Module;
using System.IO;
using UnityEngine.Analytics;
using System;
using UnityEngine.Windows;

public class ARImageTargetHandler : MonoBehaviour
{
    public static ARImageTargetHandler Instance;
    //public List<ImageTargetController> controllers = new List<ImageTargetController>();
    private ImageTrackerFrameFilter filter;
    private ImagetTargetTrigger curTrigger;
    private ImageTargetController curController;
    private GameObject curTracked;
    [SerializeField]
    private ARPrefabHandler arPrefabHandler;
    [SerializeField]
    private List<FileInfo> markerCol = new List<FileInfo>();
    private Module anatomyDataCol;
    private ObjectRot rot;
    private ObjectScaleAndPan scaler;
    enum ImageTargetSource
    {
        Offline,
        Online
    }
    [SerializeField]
    private ImageTargetSource sourceType;
    private Dictionary<string, ImageTargetController> imageTargetDict = new Dictionary<string, ImageTargetController>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Reset()
    {
        curTrigger = null;
    }

    public IEnumerator InitSequence() {
        anatomyDataCol = ModuleDataHandler.Instance.GetModule();
        if (filter == null) filter = FindObjectOfType<ImageTrackerFrameFilter>();
        rot = FindObjectOfType<ObjectRot>();
        scaler = FindObjectOfType<ObjectScaleAndPan>();
        yield return StartCoroutine(InitMarkers());
        yield return StartCoroutine(InitImageTargetControllerEvents());
        //UnloadTargets();
    }

    IEnumerator InitMarkers()
    {
        yield return new WaitForEndOfFrame();
        imageTargetDict.Clear();
        foreach (var module in anatomyDataCol.module)
        {
            foreach (var anatomy in module.anatomy)
            {
                Anatomy tempAnatomy = arPrefabHandler.FindDataByAnatomyId(anatomy.anatomyId, module.moduleId);
                if (tempAnatomy == null) continue;
                if (!string.IsNullOrEmpty(anatomy.file.url))
                {
                    sourceType = ImageTargetSource.Online;
                    //string imagetargetPath = anatomy.file.name;
                    //string imagetargetName = Path.GetFileNameWithoutExtension(anatomy.file.name);
                    string imagetargetName = anatomy.file.name;
                    string imagetargetPath = Path.Combine(ModuleDataHandler.Instance.directory, anatomy.file.name);
                    CreateImageTarget(imagetargetName, imagetargetPath, anatomy);
                    Debug.Log(imagetargetName);
                    continue;
                }
                if (!string.IsNullOrEmpty(tempAnatomy.ARImage))
                {
                    sourceType = ImageTargetSource.Offline;
                    string imagetargetPath = tempAnatomy.ARImage + ".jpg";
                    Debug.Log(imagetargetPath);
                    CreateImageTarget(tempAnatomy.ARImage, imagetargetPath, anatomy);
                }
            }
        }
    }

    private void CreateImageTarget(string _targetName, string _targetPath, AnatomyData _curAnatomyData)
    {
        GameObject imageTarget = new GameObject("imageTarget_");
        imageTarget.name += _targetName;
        var controller = imageTarget.AddComponent<ImageTargetController>();
        controller.SourceType = ImageTargetController.DataSource.ImageFile;
        controller.ImageFileSource.PathType = sourceType == ImageTargetSource.Offline? 
            PathType.StreamingAssets : PathType.Absolute;
        controller.ImageFileSource.Name = _targetName;
        controller.ImageFileSource.Path = _targetPath;
        controller.Tracker = filter;
        //controllers.Add(controller);
        //Debug.Log(_targetName + " " + controller);
        imageTargetDict.Add(_targetName, controller);

        GameObject imageTargetTrigger = new GameObject("imageTarget_trigger");
        imageTargetTrigger.transform.parent = imageTarget.transform;
        ImagetTargetTrigger trigger = imageTargetTrigger.AddComponent<ImagetTargetTrigger>();
        trigger.SetTriggerData (_curAnatomyData);
    }
   
     IEnumerator InitImageTargetControllerEvents()
    {
        yield return new WaitForEndOfFrame();
        foreach (var item in imageTargetDict)
        {
            ImageTargetController imageTargetCon = item.Value;
            imageTargetCon.TargetFound += () =>
            {
                ImagetTargetTrigger trigger = item.Value.GetComponentInChildren<ImagetTargetTrigger>();
                if (trigger != curTrigger)
                {
                    curTrigger = trigger;
                    Anatomy3DObjectHandler.Instance.OnAnatomySelected(trigger.data, false);
                }
                Debug.LogFormat("Found target {{id = {0}, name = {1}}}", imageTargetCon.Target.runtimeID(), imageTargetCon.Target.name());
            };
            imageTargetCon.TargetLost += () =>
            {
                Debug.LogFormat("Lost target {{id = {0}, name = {1}}}", imageTargetCon.Target.runtimeID(), imageTargetCon.Target.name());
            };
            imageTargetCon.TargetLoad += (Target target, bool status) =>
            {
                Debug.LogFormat("Load target {{id = {0}, name = {1}}} into {2} => {3}", target.runtimeID(), target.name(), imageTargetCon.Tracker.name, status);
            };
            imageTargetCon.TargetUnload += (Target target, bool status) =>
            {
                Debug.LogFormat("Unload target {{id = {0}, name = {1}}} => {2}", target.runtimeID(), target.name(), status);
            };
        }
    }

    public void UnloadTargets()
    {
        Reset();
        foreach (var item in imageTargetDict)
        {
            Debug.Log(item.Value.Tracker);
            item.Value.Tracker.UnloadTarget(item.Value);
            item.Value.Tracker = null;
        }
        foreach (var item in imageTargetDict)
        {
            item.Value.Tracker = filter;
        }
    }
}

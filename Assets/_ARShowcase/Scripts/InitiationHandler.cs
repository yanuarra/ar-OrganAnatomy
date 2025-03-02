using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unjani.Module;

public class InitiationHandler : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(BeginInitSequence());
    }

    IEnumerator BeginInitSequence()
    {
        yield return new WaitForEndOfFrame();
        LoadingHandler.Instance.ToggleLoadingPanel(true);
        yield return new WaitUntil(() => ModuleDataHandler.Instance.isDone);
        //Debug.Log("ModuleDataHandler isDone");
        yield return StartCoroutine(Anatomy3DObjectHandler.Instance.INIT()); 
        //Debug.Log("Anatomy3DObjectHandler Init");
        yield return StartCoroutine(ARImageTargetHandler.Instance.InitSequence());
        //Debug.Log("ARImageTargetHandler Init");
        yield return StartCoroutine(MenuUIHandler.Instance.INIT());
        Debug.Log("AnatomyData int" + StaticData.AnatomyDataID);
        if (!string.IsNullOrEmpty (StaticData.AnatomyDataID))
        {
            MenuUIHandler.Instance.ToggleAnatomyList(true);
            //AnatomyData data = ModuleDataHandler.Instance.GetAnatomyDataByID(StaticData.AnatomyDataID);
            //Debug.Log("AnatomyData " + data.anatomyId);
            //if (data != null)
            //{
            //    Anatomy3DObjectHandler.Instance.OnAnatomySelected(data, false);
            //}
        }
        yield return new WaitForSeconds(2f);
        LoadingHandler.Instance.ToggleLoadingPanel(false);
    }
}

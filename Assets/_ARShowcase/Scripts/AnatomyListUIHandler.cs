using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unjani.Module;
using Unjani.Quiz;

public class AnatomyListUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject selectorContent;
    [SerializeField] private GameObject NotFoundPanel;
    [SerializeField] private ObjectPool selectorSubObjPool;
    [SerializeField] private ObjectPool selectorObjPool;
    [SerializeField] private List<AnatomyARSelectorObj> selectorSubCol = new List<AnatomyARSelectorObj>();
    [SerializeField] private List<AnatomySelectorContainerObj> selectorContainerCol = new List<AnatomySelectorContainerObj>();
    [SerializeField] private Module _module;
    [SerializeField] private AnatomyARSelectorObj _curSelectorSub;
    [SerializeField] private AnatomySelectorContainerObj _curSelector;
    public void ToggleOverlay(bool _state) => overlay.SetActive(_state);

    private void Start()
    {
        ToggleNotFoundPanel(false);
    }

    public IEnumerator SpawnARSelector()
    {
        selectorSubCol.Clear();
        selectorContainerCol.Clear();
        yield return new WaitForEndOfFrame();
        _module = ModuleDataHandler.Instance.GetModule();
        foreach (var module in _module.module)
        {
            GameObject mg = selectorObjPool.GetObjectFromPool();
            mg.transform.parent = selectorContent.transform;
            mg.transform.position = Vector3.zero;
            mg.transform.rotation = Quaternion.identity;
            mg.transform.localScale = Vector3.one;
            AnatomySelectorContainerObj asc = mg.GetComponentInChildren<AnatomySelectorContainerObj>();
            asc.Init(module.title, module.moduleId, module.anatomy.Count.ToString());
            if (!string.IsNullOrEmpty( module.thumbnail.url))
            {
                yield return StartCoroutine(APIHelper.Instance.GetImageTexture(module.thumbnail.url, (texture) =>
                {
                    if (texture!=null)
                        asc.SetThumbnail(texture);
                }));
            }
            selectorContainerCol.Add(asc);
            foreach (var anatomy in module.anatomy)
            {
                GameObject g = selectorSubObjPool.GetObjectFromPool();
                g.transform.parent = asc.content.transform;
                g.transform.position = Vector3.zero;
                g.transform.rotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;
                AnatomyARSelectorObj ars = g.GetComponentInChildren<AnatomyARSelectorObj>();
                if (anatomy.ARPrefab != null)
                {
                    AnatomyPartObject obj = anatomy.ARPrefab.GetComponent<AnatomyPartObject>();
                }
                ars.Init(anatomy.title, anatomy);
                selectorSubCol.Add(ars);
            }
        }
        ToggleOverlay(false);
    }

    AnatomyARSelectorObj FindLocalData(AnatomyData _data)
    {
        Debug.Log(_data.anatomyId);
        return selectorSubCol.Find(data => data.anatomyData == _data);
    }

    AnatomySelectorContainerObj FindLocalDataContainer(ModuleData _data)
    {
        return selectorContainerCol.Find(data => data.moduleId == _data.moduleId);
    }

    void ToggleNotFoundPanel (bool _state) => NotFoundPanel.gameObject.SetActive(_state);
    void EnableSelector(AnatomySelectorContainerObj _selector) => _selector.gameObject.SetActive(true);
    void EnableSelectorSub(AnatomyARSelectorObj _selector) => _selector.gameObject.SetActive(true);
    public void ToggleAllSelector(bool _state)
    {
        ToggleNotFoundPanel(false);
        foreach (var item in selectorContainerCol)
        {
            item.gameObject.SetActive(_state);
        }
    }

    public void ToggleAllSelectorSub(bool _state)
    {
        ToggleNotFoundPanel(false);
        foreach (var item in selectorSubCol)
        {
            item.gameObject.SetActive(_state);
        }
    }

    public void FindByAnatomyData(Module _moduleToFind)
    {
        Debug.Log(_moduleToFind);
        if (_moduleToFind.module.Count <= 0)
        {
            ToggleAllSelector(false);
            ToggleNotFoundPanel(true);
            return;
        }
        ToggleAllSelector(false);
        foreach (var item in selectorSubCol)
        {
            foreach (var x in _moduleToFind.module)
            {
                _curSelector = FindLocalDataContainer(x);
                if (_curSelector == null) return;
                ToggleNotFoundPanel(false);
                EnableSelector(_curSelector);
            }
        }
    }
}

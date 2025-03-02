using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Unjani.Module;

public class AnatomyPartObject : MonoBehaviour
{
    public string partID;
    public GameObject pivot;
    public GameObject content;
    public ObjectPool pointPrefab;
    public ObjectPool pointLabelPrefab;
    public ObjectPool lineRendererPrefab;
    public Material lineMaterial;
    public List<Transform> mainPoints = new List<Transform>();
    public List<Transform> subPoints = new List<Transform>();
    public Transform originalParent;
    public AnatomyData anatomyData;
    public List<GameObject> partsCol = new List<GameObject>();
    public bool _isSplitAble = true;
    float distance = .5f;
    List<ChangeOutlineHandler> outlines = new List<ChangeOutlineHandler>();
    public List<PopUpUIObj> popUpUIs = new List<PopUpUIObj>();
    Dictionary<PopUpUIObj, PopUpUILabelObj> popsDict = new Dictionary<PopUpUIObj, PopUpUILabelObj>();
    public ToggleGroup labelTG;
    public ToggleGroup popUPTG;
    Vector3 centerPos;
    Vector3 oriPos;
    List<Vector3> originPartsPos = new List<Vector3>();
    List<Vector3> originPartsScale = new List<Vector3>();
    List<Quaternion> originPartsRot = new List<Quaternion>();
    Vector3 oriScale;
    Quaternion oriRot;


    private void OnEnable()
    {
        originalParent = transform.parent;
        oriPos = pivot.transform.position;
        oriRot = pivot.transform.rotation;
        oriScale = pivot.transform.localScale;
        centerPos = new Vector3(oriPos.x, oriPos.y - 4f, oriPos.z);
    }

    public void MoveToCenter() => LeanTween.moveLocal(pivot, centerPos, .5f).setEaseInCubic();
    public void MoveToOriginal() => LeanTween.moveLocal(pivot, oriPos, .5f).setEaseInCubic();
    public void AttachToCamera()
    {
        if (originalParent == null) originalParent = transform.parent; 
        Camera cam = Camera.main;
        transform.parent = cam.transform;
        transform.position = cam.transform.position + cam.transform.forward * distance;
        transform.position += cam.transform.up * .1f;
        transform.rotation = Quaternion.LookRotation(cam.transform.position - transform.position, cam.transform.up);
        //transform.rotation = new Quaternion(0, transform.rotation.y, 0, 1);
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = Quaternion.identity;
        //transform.rotation = cam.transform.rotation;
        originPartsPos.Clear();
        originPartsScale.Clear();
        originPartsRot.Clear();
        foreach (var item in partsCol)
        {
            originPartsPos.Add(item.transform.localPosition);
            originPartsScale.Add(item.transform.localScale);
            originPartsRot.Add(item.transform.localRotation);
        }
    }

    public void ReturnToOriginal()
    {
        if (originalParent == null) return;
        transform.parent = originalParent;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        pivot.transform.localPosition = Vector3.zero;
        pivot.transform.localRotation = oriRot;
        pivot.transform.localScale = oriScale;
        foreach (var item in partsCol)
        {
            int index = partsCol.IndexOf(item);
            item.transform.localPosition = originPartsPos[index];
            item.transform.localScale = originPartsScale[index];
        }
    }

    int curPartIndex;
    Vector3 curPartOrigin;
    public void DivideParts(int _Index)
    {
        if (_Index < 0)
        {
            ResetParts();
            return;
        }
        foreach (var item in partsCol)
        {
            item.SetActive(false);
            if (partsCol[_Index].Equals(item))
            {
                item.SetActive(true);
                curPartIndex = partsCol.IndexOf(item);
                LeanTween.move(item, this.transform.position, .5f).setEase(LeanTweenType.easeInExpo);
                //Vector3 centerPos = GetCenter(this.gameObject.transform);
                //LeanTween.move(item, centerPos, .5f).setEase(LeanTweenType.easeInExpo);
            }
        }
    }

    public void ResetParts()
    {
        foreach (var item in partsCol)
        {
            curPartIndex = partsCol.IndexOf(item);
            item.SetActive(true);
            LeanTween.moveLocal(item, originPartsPos[curPartIndex], .5f).setEase(LeanTweenType.easeInExpo);
            //LeanTween.moveLocal(item, originPartsPos[curPartIndex], .5f).setEase(LeanTweenType.easeInExpo);
            LeanTween.scale(item, originPartsScale[curPartIndex], .5f).setEase(LeanTweenType.easeInExpo);
            //LeanTween.rotate(item, originPartsRot[curPartIndex], .5f).setEase(LeanTweenType.easeInExpo);
        }
    }

    Vector3 GetCenter(Transform obj)
    {
        Vector3 center = new Vector3();
        if (obj.GetComponent<MeshRenderer>() != null)
        {
            //center = obj.GetComponent<Renderer>().bounds.center;
            center = obj.GetComponent<MeshRenderer>().bounds.center;
        }
        else
        {
            foreach (Transform subObj in obj)
            {
                center += GetCenter(subObj);
            }
            center /= obj.childCount;
        }
        return center;
    }

    public void Init(AnatomyData _main)
    {
        Camera cam = Camera.main;
        originalParent = transform.parent;
        anatomyData = _main;
        //part = _part;
        partID = anatomyData.anatomyId;
        mainPoints.Clear();
        subPoints.Clear();
        outlines.Clear();
        ClearDict();
        foreach (var item in partsCol)
        {
            foreach (var curSub in anatomyData.subanatomy)
            {
                    Transform part = item.GetComponentsInChildren<Transform>().Where(
                    x => GetAlphabetOnly(x.gameObject.name).Trim().ToLower().Equals(
                        GetAlphabetOnly(curSub.title).Trim().ToLower())).FirstOrDefault();
                    //x => x.gameObject.name.ToLower().Replace(" ", "").Equals(subPart.partName.ToLower().Replace(" ", ""))).FirstOrDefault();
                    if (part != null)
                    {
                        GameObject partPoint = pointPrefab.GetObjectFromPool();
                        PopUpUIObj partPointObj = partPoint.GetComponent<PopUpUIObj>();
                        partPoint.transform.SetParent(part.transform); 
                        partPoint.transform.localPosition = Vector3.zero;
                        partPoint.transform.localRotation = Quaternion.identity;
                        partPointObj.Init(curSub.title, this.gameObject);
                        popUpUIs.Add(partPointObj);

                        GameObject subPointLabel = pointLabelPrefab.GetObjectFromPool();
                        PopUpUILabelObj subPointLabelObj = subPointLabel.GetComponent<PopUpUILabelObj>();
                        subPointLabel.transform.SetParent(part.transform);
                        subPointLabel.transform.localPosition = Vector3.zero;
                        subPointLabel.transform.localRotation = Quaternion.identity;
                        subPointLabelObj.InitPopUpLabel(curSub.title, this.gameObject, partPointObj);
                        partPointObj.labelObj = subPointLabelObj;
                        SetLabel(subPointLabelObj, partPointObj);
                        SetDict(partPointObj, subPointLabelObj);
                    }
                /*
                Transform sub = item.GetComponentsInChildren<Transform>().Where(
                   k => GetAlphabetOnly(k.gameObject.name).ToLower().Replace(" ", "").Equals(
                        GetAlphabetOnly(curSub.partName).ToLower().Replace(" ", ""))).FirstOrDefault();
                   //k.gameObject.name.ToLower().Contains(curSub.subName.ToLower())).FirstOrDefault();
                if (sub != null) { 
                   GameObject subPoint = pointPrefab.GetObjectFromPool();
                   PopUIObj subPointObj = subPoint.GetComponent<PopUIObj>();
                   subPoint.transform.SetParent(sub.transform);
                   subPoint.transform.localPosition = Vector3.zero;
                   subPoint.transform.localRotation = Quaternion.identity;
                   subPointObj.Init(curSub.partName, this.gameObject);
                   popUpUIs.Add(subPointObj);

                   GameObject subPointLabel = pointLabelPrefab.GetObjectFromPool();
                   PopUpUILabelObj subPointLabelObj = subPointLabel.GetComponent<PopUpUILabelObj>();
                   subPointLabel.transform.SetParent(sub.transform);
                   subPointLabel.transform.localPosition = Vector3.zero;
                   subPointLabel.transform.localRotation = Quaternion.identity;
                   subPointLabelObj.InitPopUpLabel(curSub.partName, this.gameObject);
                   subPointObj.labelObj = subPointLabelObj;
                   SetLabel(subPointLabelObj, subPointObj);
                }
                */
            }
            //ToggleSUbPoints(false);
        }
        outlines = GetComponentsInChildren<ChangeOutlineHandler>().ToList();
        foreach (var ol in outlines)
        {
            ol.OutlineNotBlinking();
        }
    }

    void SetLabel(PopUpUILabelObj go, PopUpUIObj pu)
    {
        GameObject lineGO = new GameObject("LineRenderer");
        LineRendererHandler lrh = lineGO.AddComponent(typeof(LineRendererHandler)) as LineRendererHandler;
        lineGO.transform.SetParent(pu.transform.parent);
        lineGO.transform.localPosition = Vector3.zero;
        lineGO.transform.localRotation = Quaternion.identity;
        go.lineRenderer = lrh;

        Vector3 newpos;
        //Vector3 direction = (go.gameObject.transform.position - getCenter(this.gameObject.transform)).normalized;        
        Vector3 direction = (go.gameObject.transform.position - this.gameObject.transform.position).normalized;
        newpos = go.gameObject.transform.position + (direction * .07f);
        go.transform.position = newpos;
        go.transform.localRotation = Quaternion.identity;
        lrh.lineMats = lineMaterial;

        lrh.pointA = pu.gameObject.transform;
        lrh.pointB = go.gameObject.transform;
        lrh.InitLR();
    }

    public void FindPopUp(SubAnatomyData _data, bool _state)
    {
        AnatomyPartObject cur = Anatomy3DObjectHandler.Instance.curActiveObj;
        foreach (var item in cur.popUpUIs)
        {
            if (item.id.Equals(_data.id))
            {
                item.ToggleSelectedOutline(_state);
            }
        }
    }

    string GetAlphabetOnly(string _s)
    {
        //string s = Regex.Replace(_s, @"[^A-Z]+", String.Empty);
        Regex MyRegex = new Regex("[^a-z]", RegexOptions.IgnoreCase);
        string s = MyRegex.Replace(_s, @"");
        Console.WriteLine(s);
        return s;
    }

    PopUpUIObj curPopUpUI;
    PopUpUILabelObj curPopUpUILabel;
    public void TogglePopUpAndLabel(string _id, bool _state)
    {
        curPopUpUI = null;
        curPopUpUI = popUpUIs.Find(x => x.id.Equals(_id));
    
        if (curPopUpUI == null) return;
        if (!popsDict.TryGetValue(curPopUpUI, out curPopUpUILabel)) return;
        curPopUpUI.ToggleSelectedOutline(_state);
        if (_state)
        {
            curPopUpUILabel.OnSelect();
        }else
        {
            curPopUpUILabel.OnDeselect();
        }
    }

    public void GetDict(PopUpUIObj _popUP)
    {
        if (popsDict.ContainsKey(_popUP))
        {
            popsDict.TryGetValue(_popUP, out curPopUpUILabel);
        }    
    }
    
    public void SetDict(PopUpUIObj _popUP, PopUpUILabelObj _label) => popsDict.Add(_popUP, _label);
    public void ClearDict() => popsDict.Clear();
}

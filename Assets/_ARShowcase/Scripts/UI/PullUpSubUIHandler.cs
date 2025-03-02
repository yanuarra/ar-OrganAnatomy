using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.EventSystems;
using Unjani.Module;

public class PullUpSubUIHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected ScrollRect scrollRect;
    protected RectTransform contentPanel;
    [SerializeField] private GameObject overlay;
    [SerializeField] private Transform selectorContent;
    private SimpleScrollSnap sss;
    [SerializeField] private Button backNormal;
    [SerializeField] private Button backPulledUp;
    [SerializeField] private ObjectPool selectorObjPool;
    [SerializeField] private RectTransform infoPullUp;
    [SerializeField] private RectTransform contentDescRT;
    [SerializeField] private GameObject panelDesc; 
    [SerializeField] private List<AnatomySubSelectorObj> col = new List<AnatomySubSelectorObj>();
    [SerializeField] private ObjectPool subAnatomyObjPool;
    [SerializeField] private List<SubAnatomyUIObj> SubAnatomyUICol = new List<SubAnatomyUIObj>();
    [SerializeField] private GameObject subAnatomyContent;
    private Unjani.Module.AnatomyData curAnatomyData;
    private Vector2 defaultPos;
    private SubAnatomyUIObj curActiveSub;
    public bool isOverlapBottomBar { get; set; }
    private enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    DraggedDirection dragDir;
    Vector2 tempPos;
    bool isFirstTime = true;
    public void Init()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;
        sss = GetComponent<SimpleScrollSnap>();
        sss.OnPanelSelected.AddListener(delegate { MovementHasStopped(); });
        scrollRect.onValueChanged.AddListener(delegate { StopElementWhenOverlap();});
        backNormal.gameObject.SetActive(true);
        backPulledUp.gameObject.SetActive(false);
        scrollRect.onValueChanged.AddListener(delegate {
        });
        defaultPos = scrollRect.content.position;
    }

    public void ToggleOverlay(bool _state) {
        overlay.SetActive(_state);
        if (curActiveSub)
            SnapTo(curActiveSub);
    }

    public void SpawnPullUpSubAnatomy(AnatomyData _anatomyData)
    {
        curAnatomyData = _anatomyData;
        if (SubAnatomyUICol.Count>0)
        {
            foreach (var item in SubAnatomyUICol)
            {
                Destroy(item.gameObject);
            }
        }
        SubAnatomyUICol.Clear();
        foreach (var sub in curAnatomyData.subanatomy)
        {
            GameObject p = subAnatomyObjPool.GetObjectFromPool();
            p.transform.parent = subAnatomyContent.transform;
            p.transform.position = Vector3.zero;
            p.transform.rotation = Quaternion.identity;
            p.transform.localScale = Vector3.one;
            SubAnatomyUIObj sap = p.GetComponentInChildren<SubAnatomyUIObj>();
            sap.Init(sub);
            SubAnatomyUICol.Add(sap);
        }
    }

    public void SpawnPullUpSubSelector(AnatomyData _anatomyData)
    {
        //Get Parent
        curAnatomyData = _anatomyData;
        ModuleData curModuleData = ModuleDataHandler.Instance.GetModuleDataByAnatomyID(curAnatomyData.anatomyId);
        if (col.Count > 0 ) Reset();
        foreach (var item in curModuleData.anatomy)
        {
            GameObject mg = selectorObjPool.GetObjectFromPool();
            mg.transform.parent = selectorContent.transform;
            mg.transform.position = Vector3.zero;
            mg.transform.rotation = Quaternion.identity;
            mg.transform.localScale = Vector3.one; 
            AnatomySubSelectorObj ars = mg.GetComponent<AnatomySubSelectorObj>();
            ToggleGroup selectorTglGroup = selectorContent.GetComponent<ToggleGroup>();
            ars.Init(item.title, item, item.Equals(curAnatomyData), selectorTglGroup);
            col.Add(ars);
        }
    }

    public void Reset()
    {
        foreach (var item in col)
        {
            Destroy(item.gameObject);
        }
        col.Clear();
        curActiveSub = null;
        sss.GoToPanel(1);
        scrollRect.content.position = defaultPos;
        isFirstTime = true;
    }

    LayoutElement l;
    AnatomyPartObject curObj;
    private void MovementHasStopped()
    {
        if (l==null) l = panelDesc.GetComponent<LayoutElement>();
        switch (sss.CenteredPanel)
        {
            case 0:
                if (Anatomy3DObjectHandler.Instance.curActiveObj != null) 
                    Anatomy3DObjectHandler.Instance.curActiveObj.MoveToOriginal();
                backNormal.gameObject.SetActive(false);
                backPulledUp.gameObject.SetActive(true);
                panelDesc.gameObject.SetActive(true);
                infoPullUp.sizeDelta = new Vector2(infoPullUp.sizeDelta.x, 1270f);
                l.preferredHeight = contentDescRT.sizeDelta.y+500f;
                break;
            case 1:
                if (Anatomy3DObjectHandler.Instance.curActiveObj != null)
                    Anatomy3DObjectHandler.Instance.curActiveObj.MoveToOriginal();
                backNormal.gameObject.SetActive(true);
                backPulledUp.gameObject.SetActive(false);
                panelDesc.gameObject.SetActive(true);
                infoPullUp.sizeDelta = new Vector2(infoPullUp.sizeDelta.x, 1270f);
                l.preferredHeight = 550;
                break;
            case 2:
                sss.GoToPanel(1);
                break;
        }
    }

    public void FindAnatomySubInfo(string _id, bool _state)
    {
        SubAnatomyUIObj subObj = SubAnatomyUICol.Find(x => x.data.title.Equals(_id));
        if (subObj) { 
            curActiveSub = subObj;
            curActiveSub.toggle.SetIsOnWithoutNotify(_state);
            SnapTo(subObj);
        }
    }

    public void ToggleSubAnatomyUI()
    {
        if (curActiveSub) { 
            curActiveSub.SubUIToggle();
        }
    }

    public void SnapTo(SubAnatomyUIObj _sub)
    {
        curActiveSub = _sub;
        Transform target = curActiveSub.GetComponent<Transform>();
        curActiveSub.ChangeSubSize();
        if (overlay.activeInHierarchy)
        {
            Canvas.ForceUpdateCanvases();
            Vector2 pos = (Vector2)scrollRect.transform.InverseTransformPoint(contentDescRT.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
            pos.x = contentDescRT.anchoredPosition.x;
            contentDescRT.anchoredPosition = pos;
        }
    }

    public void StopElementWhenOverlap()
    {
        //register position
        if (isFirstTime && isOverlapBottomBar)
        {
            tempPos = scrollRect.content.position;
            isFirstTime = false;
        }
        if (dragDir == DraggedDirection.Down && isOverlapBottomBar)
        {
            _lastSelectedGO = _lastPointerData.pointerDrag;
            _lastPointerData.pointerDrag = null;
            scrollRect.decelerationRate = 0;
            scrollRect.inertia = false;
            scrollRect.content.position = new Vector2(scrollRect.content.position.x, tempPos.y);
        }
        if (Anatomy3DObjectHandler.Instance.curActiveObj != null) 
            Anatomy3DObjectHandler.Instance.curActiveObj.MoveToCenter();
    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY)
        {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        return draggedDir;
    }

    private PointerEventData _lastPointerData;
    private GameObject _lastSelectedGO;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
        _lastPointerData = eventData;
        _lastSelectedGO = EventSystem.current.currentSelectedGameObject;
        //scrollRect.inertia = true;
        //scrollRect.decelerationRate = 0.125f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        dragDir = GetDragDirection(dragVectorDirection);
        StopElementWhenOverlap();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
    }
}

using DanielLochner.Assets.SimpleScrollSnap;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unjani.Module;
using System.Collections;

public class PullUpUIHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, ISelectHandler, IDeselectHandler, IScrollHandler
{
    private SimpleScrollSnap sss;
    private ScrollRect sr;
    private ScrollRect srDesc;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button backBefore;
    [SerializeField] private Button backAfter;
    [SerializeField] private GameObject panelDesc;
    [SerializeField] private GameObject layerSelectorContent;
    [SerializeField] private GameObject layerSelectorScrollView;
    [SerializeField] private GameObject layerSelectorPrefab;
    [Header("Tips")]
    [SerializeField] private GameObject tipsBefore;
    [SerializeField] private GameObject tipsAfter; 
    [Header("Summary")]
    [SerializeField] private Image sumIcon;
    [SerializeField] private TMP_Text sumTitle;
    [SerializeField] private TMP_Text sumSubCount;
    [SerializeField] private TMP_Text sumQuizCount;
    [SerializeField] private TMP_Text pullUpDesc;
    [SerializeField] public Camera eventCamera;
    [SerializeField] private RectTransform infoPullUp;
    private RectTransform rt;
    [SerializeField] private RectTransform contentDescRT;
    private Vector3 pos;
    private Vector2 defaultPos;
    private Canvas canvas;
    private CanvasScaler canvasScaler;
    private float deltaY;
    [SerializeField] private bool isCentering = false;
    [SerializeField] private bool isCentered = false;
    [SerializeField] private GameObject img_noresult;
    [SerializeField] private RectTransform bottomBarBorder;
    [SerializeField] private Transform pullUpTitle;
    [SerializeField] private Camera cam;
    [SerializeField] private PointerEventData _lastPointerData;
    [SerializeField] private GameObject _lastSelectedGO;
    [SerializeField] private GameObject _target;
    private List<GameObject> partSelectorCol = new List<GameObject>();
    public bool isOverlapBottomBar { get; set; }

    private enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    Vector2 tempPos;
    bool isFirstTime = true;
    DraggedDirection dragDir;

    public void Init()
    {
        rt = panelDesc.GetComponent<RectTransform>();
        sr = GetComponent<ScrollRect>();
        srDesc = panelDesc.GetComponent<ScrollRect>();
        sss = GetComponent<SimpleScrollSnap>();
        eventCamera = GetComponentInParent<Canvas>().worldCamera;
        canvas = GetComponentsInParent<Canvas>().Last();
        canvasScaler = canvas.GetComponent<CanvasScaler>();
        if (canvasScaler && canvasScaler.screenMatchMode is not CanvasScaler.ScreenMatchMode.Expand)
            Debug.LogWarning("matchMode of canvasScaler of canvas of DragUI must be expand", canvasScaler);
        //sr.onValueChanged.AddListener(delegate { StopElementWhenOverlap();});
        sss.OnPanelSelected.AddListener(delegate { MovementHasStopped(); });
        //sss.OnPanelSelecting.AddListener(delegate { print("OnPanelSelecting"); });
        //sss.OnPanelCentering.AddListener(delegate { print("OnPanelCentering"); });
        //sss.OnPanelCentered.AddListener(delegate { print("OnPanelCentered"); });
        sss.OnTransitionEffects.AddListener(delegate { Debug.Log("effect"); });
        tipsBefore.SetActive(true);
        tipsAfter.SetActive(false);
        backBefore.gameObject.SetActive(true);
        backAfter.gameObject.SetActive(false);
        //sss.Size = new Vector2(Screen.width, 12);
        cam = Camera.main;
        defaultPos = sr.content.position;
    }

    public void ToggleOverlay(bool _state) => overlay.SetActive(_state);
    //public void ToggleOverlayInteractable(bool _state) => canvasGroup.interactable = _state;

    AnatomyPartObject curObj;
    AnatomyData curData;
    public void SpawnPullUpAnatomy(AnatomyData _data, AnatomyPartObject _obj)
    {
        Reset();
        curObj = _obj;
        curData = _data;
        //sumIcon.sprite = _data.thumbnail;
        sumTitle.text = _data.title;
        sumSubCount.text = string.Format("{0} Sub Anatomi", _data.subanatomy.Count.ToString());
        sumQuizCount.text = "Soal Latihan";
        if (!string.IsNullOrEmpty(_data.description))
        {
            pullUpDesc.gameObject.SetActive(true);
            img_noresult.gameObject.SetActive(false);
            string val = _data.description;
            val = val.Replace("\r", "");
            pullUpDesc.text = val;
        }
        else
        {
            pullUpDesc.gameObject.SetActive(false);
            img_noresult.gameObject.SetActive(true);
        }

        //Setup layer selector
        layerSelectorScrollView.SetActive(false);
        if (!_obj._isSplitAble) return;
        if (_obj.partsCol.Count == 1) return;
        layerSelectorScrollView.SetActive(true);
        for (int i = -1; i < _obj.partsCol.Count; i++)
        {
            GameObject selectorGO = Instantiate(layerSelectorPrefab, layerSelectorContent.transform);
            LayerSelectorObj asd = selectorGO.GetComponent<LayerSelectorObj>();
            ToggleGroup toggleGroup = layerSelectorContent.GetComponent<ToggleGroup>();
            Toggle toggle = selectorGO.GetComponent<Toggle>();
            //int index = _obj.partsCol.IndexOf(item);
            int counter = i + 1;
            toggleGroup.allowSwitchOff = false;
            toggle.group = toggleGroup;
            //toggle.onValueChanged.AddListener(delegate { DivideParts(toggle, counter - 1); });
            bool isActive = i < 0 ? true : false;
            asd.Init(counter.ToString(), isActive, toggleGroup, i);
            selectorGO.transform.SetAsFirstSibling();
            partSelectorCol.Add(selectorGO);
        }
        //Rect re = layerSelectorScrollView.GetComponent<Rect>();
        Canvas.ForceUpdateCanvases();
        LayoutElement l = layerSelectorScrollView.GetComponent<LayoutElement>();
        float height = layerSelectorContent.GetComponent<RectTransform>().rect.height;
        l.preferredHeight = height>=755? 755:height;

        //re.height = layerSelectorContent.GetComponent<Rect>().size.y;
        //contentDescRT.LeanSetPosY(0f);
    }

    public void DivideParts(Toggle _tgl, int _Index) 
    {
        if (_tgl.isOn)
        {
            curObj.DivideParts(_Index);
        }
        else
        {
            curObj.ResetParts();
        }
    }

    public void Reset()
    {
        sss.GoToPanel(1); 
        srDesc.verticalNormalizedPosition = 1; 
        sr.content.position = defaultPos;
        sumTitle.text = "";
        sumSubCount.text = "0";
        sumQuizCount.text = "0";
        pullUpDesc.text = "";
        isFirstTime = true;
        if (partSelectorCol.Count > 0)
        {
            foreach (var item in partSelectorCol)
            {
                Destroy(item);
            }
            partSelectorCol.Clear();
        }
    }

    LayoutElement l;
    private void MovementHasStopped(){
        if (l == null) l = panelDesc.GetComponent<LayoutElement>();
        isCentering = false;
        isCentered = true;
        sr.StopMovement();
        StopAllCoroutines();
        switch (sss.CenteredPanel)
        {
            case 0:
                if (Anatomy3DObjectHandler.Instance.curActiveObj != null)
                    Anatomy3DObjectHandler.Instance.curActiveObj.MoveToOriginal();
                tipsAfter.SetActive(true);
                tipsBefore.SetActive(false);
                backBefore.gameObject.SetActive(false);
                backAfter.gameObject.SetActive(true);
                panelDesc.gameObject.SetActive(true);
                infoPullUp.sizeDelta = new Vector2(infoPullUp.sizeDelta.x, 1270f);
                l.preferredHeight = 1600f;
                break;
            case 1:
                if (Anatomy3DObjectHandler.Instance.curActiveObj != null) 
                    Anatomy3DObjectHandler.Instance.curActiveObj.MoveToOriginal();
                tipsAfter.SetActive(false);
                tipsBefore.SetActive(true);
                backBefore.gameObject.SetActive(true);
                backAfter.gameObject.SetActive(false);
                panelDesc.gameObject.SetActive(true);
                infoPullUp.sizeDelta = new Vector2(infoPullUp.sizeDelta.x, 1270f);
                l.preferredHeight = 350f;
                break;
            case 2:
                sss.GoToPanel(1);
                //if (tempPos == null) tempPos = sr.content.position;
                break;
        }
    }
    private void isContentOverlap(PointerEventData ped)
    {
        if (sr.content.position.y <= -2370)
        {
            if (dragDir == DraggedDirection.Down) { 
                _lastSelectedGO = ped.pointerDrag;
                ped.pointerDrag = null;
            }
            sr.decelerationRate = 0;
            sr.inertia = false; 
            sr.content.position = new Vector3(sr.content.position.x, -2370);
        }
    }

    public void SnapTo(Transform _target)
    {
        Transform target = _target;
        if (overlay.activeInHierarchy)
        {
            Canvas.ForceUpdateCanvases();
            //Vector2 pos = (Vector2)sr.transform.InverseTransformPoint(sr.content.position) - (Vector2)sr.transform.InverseTransformPoint(target.position);
            Vector2 pos = (Vector2)sr.content.InverseTransformPoint(sr.content.position) - (Vector2)sr.content.InverseTransformPoint(target.position);
            pos.x = sr.content.anchoredPosition.x;
            sr.content.anchoredPosition = pos;
            Debug.Log(sr.content.anchoredPosition);
        }
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

    public static bool isElementsOverlap(RectTransform _elem, RectTransform _target)
    {
        Vector3[] e_wcorners = new Vector3[4];
        Vector3[] e_lcorners = new Vector3[4];
        _elem.GetWorldCorners(e_wcorners);
        Vector2 elem_minCorner = Camera.main.WorldToScreenPoint(e_wcorners[0]);
        Vector2 elem_maxCorner = Camera.main.WorldToScreenPoint(e_wcorners[2]);

        Vector3[] t_wcorners = new Vector3[4];
        Vector3[] t_lcorners = new Vector3[4];
        _target.GetWorldCorners(t_wcorners);
        Vector2 targetMinCorner = Camera.main.WorldToScreenPoint(t_wcorners[0]);
        Vector2 targetMaxCorner = Camera.main.WorldToScreenPoint(t_wcorners[2]);
        Debug.Log(string.Format("elem {0} target {1}", elem_maxCorner.ToString(), targetMaxCorner.ToString()));
        if (elem_minCorner.y > targetMaxCorner.y) { return true; }
        if (elem_minCorner.x > targetMaxCorner.x) { return false; }
        if (elem_minCorner.y > targetMaxCorner.y) { return false; }

        if (elem_maxCorner.x < targetMinCorner.x) { return false; }
        if (elem_maxCorner.y < targetMinCorner.y) { return false; }

        return true;
    }

    public void StopElementWhenOverlap()
    {
        if (_lastPointerData == null) return;
        //register position
        if (isFirstTime && isOverlapBottomBar)
        {
            tempPos = sr.content.position;
            isFirstTime = false;
        }
        Vector3 dragVectorDirection = (_lastPointerData.position - _lastPointerData.pressPosition).normalized;
        dragDir = GetDragDirection(dragVectorDirection);
        if (dragDir == DraggedDirection.Down&&  isOverlapBottomBar)
        {
            _lastSelectedGO = _lastPointerData.pointerDrag;
            _lastPointerData.pointerDrag = null;
            sr.decelerationRate = 0;
            sr.inertia = false;
            //SnapTo(_target.transform);
            sr.content.position = new Vector2(sr.content.position.x, tempPos.y);
        }
        if (Anatomy3DObjectHandler.Instance.curActiveObj != null) 
            Anatomy3DObjectHandler.Instance.curActiveObj.MoveToCenter();
    }

    // if your viewport is screen, then keep it as 'null'
    // NOTICE - doesn't consider if the rectangles are rotating,
    // but shoudl work even if canvas's camera ISN'T aligned with world axis :)
    public static bool is_rectTransformsOverlapScreen(Camera cam, RectTransform elem, RectTransform viewport = null)
    {
        Vector2 viewportMinCorner;
        Vector2 viewportMaxCorner;

        if (viewport != null)
        {
            //so that we don't have to traverse the entire parent hierarchy (just to get screen coords relative to screen),
            //ask the camera to do it for us.
            //first get world corners of our rect:
            Vector3[] v_wcorners = new Vector3[4];
            viewport.GetWorldCorners(v_wcorners); //bot left, top left, top right, bot right

            //+ow shove it back into screen space. Now the rect is relative to the bottom left corner of screen:
            viewportMinCorner = cam.WorldToScreenPoint(v_wcorners[0]);
            viewportMaxCorner = cam.WorldToScreenPoint(v_wcorners[2]);
        }
        else
        {
            //just use the scren as the viewport
            viewportMinCorner = new Vector2(0, 0);
            viewportMaxCorner = new Vector2(Screen.width, Screen.height);
        }

        //give 1 pixel border to avoid numeric issues:
        viewportMinCorner += Vector2.one;
        viewportMaxCorner -= Vector2.one;

        //do a similar procedure, to get the "element's" corners relative to screen:
        Vector3[] e_wcorners = new Vector3[4];
        Vector3[] e_lcorners = new Vector3[4];
        elem.GetWorldCorners(e_wcorners);
        //local
        //elem.GetLocalCorners(e_lcorners);
        Vector2 elem_minCorner = cam.WorldToScreenPoint(e_wcorners[0]);
        Vector2 elem_maxCorner = cam.WorldToScreenPoint(e_wcorners[2]);

        //perform comparison:
        if (elem_minCorner.x > viewportMaxCorner.x) { return false; }//completelly outside (to the right)
        if (elem_minCorner.y > viewportMaxCorner.y) { return false; }//completelly outside (is above)

        if (elem_maxCorner.x < viewportMinCorner.x) { return false; }//completelly outside (to the left)
        if (elem_maxCorner.y < viewportMinCorner.y) { return false; }//completelly outside (is below)
        /*
             commented out, but use it if need to check if element is completely inside:
            Vector2 minDif = viewportMinCorner - elem_minCorner;
            Vector2 maxDif = viewportMaxCorner - elem_maxCorner;
            if(minDif.x < 0  &&  minDif.y < 0  &&  maxDif.x > 0  &&maxDif.y > 0) { //return "is completely inside" }
        */
        return true;//passed all checks, is inside (at least partially)
    }
   
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
        _lastPointerData = eventData;
        _lastSelectedGO = EventSystem.current.currentSelectedGameObject;
        //sr.inertia = true;
        //sr.decelerationRate = 0.125f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        dragDir = GetDragDirection(dragVectorDirection);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!overlay.activeInHierarchy) return;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        Debug.Log("OnDeselect " + pointerData);
        if (pointerData != null)
        {
            // if I'm being dragged, change the event data to say I'm no longer dragged
            if (pointerData.pointerDrag == gameObject)
            {
                // This should automatically trigger EndDrag for this card,
                // OnDrop for any hovered slot, etc.
                pointerData.pointerDrag = null;
            }
        }
        else
        {
            //have to manually reset card
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        Debug.Log("OnSelect " + pointerData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        Debug.Log("Scrolling without touching");
        //Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        //dragDir = GetDragDirection(dragVectorDirection);
        //StopElementWhenOverlap();
    }
}
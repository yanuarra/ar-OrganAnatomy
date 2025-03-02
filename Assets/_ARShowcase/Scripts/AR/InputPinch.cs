using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputPinch : MonoBehaviour {
    [Range(0, 1)]
    public float sensitivity;
    public bool smooth;
    [Header("Smooth Properties")]
    [Range(0, 10)]
    public float deceleration;
    [HideInInspector]
    public float DeltaPinch { get; private set; }
    public Vector3 centerPinch { get; private set; }
    private float pinchSpeed;
    private float avgSpeedPinch;
    public bool isPinching;
    public bool isPinchAndMove;

    public Vector3 lastPanPosition;
    private int panFingerId; // Touch mode only

    private event Action TrackInput = delegate { };

    private void Start() {
        pinchSpeed = sensitivity/100;
        if (Application.isMobilePlatform) {
            if (smooth) {
                TrackInput = MobileSmoothInput;
            } else {
                TrackInput = MobileDefaultInput;
            }
        } else {
            if (smooth) {
                TrackInput = EditorSmoothInput;
            } else {
                TrackInput = EditorDefaultInput;
            }
            pinchSpeed *= 300;
        }
    }

    private void Update() {
        TrackInput();
    }

    void MobileInput() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);
        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
        DeltaPinch = deltaMagnitudeDiff * pinchSpeed;
        centerPinch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
        if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
        {
            isPinchAndMove = true;
        }
        else
        {
            isPinchAndMove = false;
        }
        //if (touchZero.phase == TouchPhase.Began && touchOne.phase == TouchPhase.Began)
        //{
        //    centerPinch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
        //    lastPanPosition = centerPinch;
        //    panFingerId = touchZero.fingerId;
        //}
        //else 
        //{
        //    centerPinch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
        //    isPinchAndMove = true;
        //    //centerPinch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
        //}
        //if (touchZero.phase == TouchPhase.Began && touchOne.phase == TouchPhase.Began)
        //{
        //    centerPinch = Vector2.Lerp(touchZero.position, touchOne.position, .5f);
        //    lastPanPosition = centerPinch;
        //    panFingerId = touchZero.fingerId;
        //}
        //else if (touchZero.fingerId == panFingerId && touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
        //{
        //    isPinchAndMove = true;
        //}
    }

    void MobileDefaultInput() {
        if (Input.touchCount == 2) {
            MobileInput();
        } else {
            DeltaPinch = 0;
        }
    }

    void MobileSmoothInput() {
        if (Input.touchCount == 2) {
            if (!isPinching) {
                avgSpeedPinch = 0;
                isPinching = true;
            }
            MobileInput();
            avgSpeedPinch = Mathf.Lerp(avgSpeedPinch, DeltaPinch, Time.deltaTime * deceleration);
        } else {
            if (isPinching) {
                DeltaPinch = avgSpeedPinch;
                isPinching = false;
            }
            DeltaPinch = Mathf.Lerp(DeltaPinch, 0, Time.deltaTime * deceleration);
        }
    }

    void EditorInput() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        DeltaPinch = -Input.GetAxis("Mouse ScrollWheel") * pinchSpeed;
        centerPinch = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
        //centerPinch = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
    }

    void EditorDefaultInput() {
        if (Input.GetMouseButtonDown(0))
        {
            lastPanPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f)); ;
        }
        else if (Input.GetMouseButton(0))
        {
            centerPinch = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            isPinchAndMove = true;
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetMouseButton(1)) {
            EditorInput();
        } else {
            DeltaPinch = 0;
        }
    }

    void EditorSmoothInput() {
        if (Input.GetMouseButtonDown(0))
        {
            centerPinch = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            lastPanPosition = centerPinch;
        }
        else if (Input.GetMouseButton(0))
        {
            centerPinch = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            isPinchAndMove = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetMouseButton(1)) {
            if (!isPinching) {
                avgSpeedPinch = 0;
                isPinching = true;
            }
            EditorInput();
            avgSpeedPinch = Mathf.Lerp(avgSpeedPinch, DeltaPinch, Time.deltaTime * deceleration);
        } else {
            if (isPinching) {
                DeltaPinch = avgSpeedPinch;
                isPinching = false;
            }
            DeltaPinch = Mathf.Lerp(DeltaPinch, 0, Time.deltaTime * deceleration);
        }
    }
}
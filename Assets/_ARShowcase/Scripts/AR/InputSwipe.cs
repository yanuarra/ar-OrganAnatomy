using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputSwipe : MonoBehaviour {
    [Range(0, 1)]
    public float sensitivity;
    public bool smooth;
    [Header("Smooth Properties")]
    [Range(0, 10)]
    public float deceleration; 
    [HideInInspector]
    public Vector3 DeltaTouch { get; private set; }
    private float swipeSpeed;
    private Vector3 avgSpeedTouch;
    private bool swiping;

    private event Action TrackInput = delegate { };

    private void Start() {
        swipeSpeed = sensitivity/4;
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
            swipeSpeed *= 20;
        }
    }

    private void Update() {
        TrackInput();
    }

    void MobileInput() {
        foreach (Touch t in Input.touches)
        {
            int id = t.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
                return;
        }
        Touch touch = Input.GetTouch(0);
        float deltaX = touch.deltaPosition.x;
        float deltaY = touch.deltaPosition.y;
        DeltaTouch = new Vector3(-deltaX * swipeSpeed, deltaY * swipeSpeed, 0);
    }

    void MobileDefaultInput() {
        if (Input.touchCount == 1) {
            MobileInput();
        } else {
            DeltaTouch = Vector3.zero;
        }
    }

    void MobileSmoothInput() {
        if (Input.touchCount == 1) {
            if (!swiping) {
                avgSpeedTouch = Vector3.zero;
                swiping = true;
            }
            MobileInput();
            avgSpeedTouch = Vector3.Lerp(avgSpeedTouch, DeltaTouch, Time.deltaTime * deceleration);
        } else {
            if (swiping) {
                DeltaTouch = avgSpeedTouch;
                swiping = false;
            }
            DeltaTouch = Vector3.Lerp(DeltaTouch, Vector3.zero, Time.deltaTime * deceleration);
        }
    }

    void EditorInput() {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");
        DeltaTouch = new Vector3(-deltaX * swipeSpeed, deltaY * swipeSpeed, 0);
    }

    void EditorDefaultInput() {
        if (Input.GetMouseButton(0)) {
            EditorInput();
        } else {
            DeltaTouch = Vector3.zero;
        }
    }

    void EditorSmoothInput() {
        if (Input.GetMouseButton(0)) {
            if (!swiping) {
                avgSpeedTouch = Vector3.zero;
                swiping = true;
            }
            EditorInput();
            avgSpeedTouch = Vector3.Lerp(avgSpeedTouch, DeltaTouch, Time.deltaTime * deceleration);
        } else {
            if (swiping) {
                DeltaTouch = avgSpeedTouch;
                swiping = false;
            }
            DeltaTouch = Vector3.Lerp(DeltaTouch, Vector3.zero, Time.deltaTime * deceleration);
        }
    }
}

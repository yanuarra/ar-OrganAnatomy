using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRot : MonoBehaviour {
    [SerializeField]
    private InputSwipe swipeInput;
    public Transform pivot;
    public bool inverseRotate;
    public bool lockVertical;
    [Header("Vertical Properties")]
    public bool verticalClamp;
    [Tooltip("(min,max) clamp for Y rotation")]
    public Vector2 clampValue;
    public Quaternion clampValueQuat;
    private float direction = 1f;
    private float horizontalDir = 1f;
    private event Action RotateObject = delegate { };
    private Vector3 lastPanPosition;
    Camera cam;
    float DecreaseCameraPanSpeed = 1f;
    private void Start()
    {
        cam = Camera.main;
        if (inverseRotate)
            direction *= -1;
        RotateObject += RotateObjectREV;
        //RotateObject += RotateObjectDefault;
        //if (lockVertical)
        //    return; 
        //RotateObject += RotateObjectWithY;
        //if (!verticalClamp)
        //    return;
        //RotateObject += Clamp;
    }

    private void Update() {
        if (pivot == null)
            return;
        if (swipeInput == null)
            return;
        foreach (Touch t in Input.touches)
        {
            int id = t.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
                return;
        }
        RotateObject();
    }
    private void RotateObjectREV()
    {
        /*Debug.Log(pivot.transform.localEulerAngles.x);
        //if (pivot.transform.localEulerAngles.x > 180 || pivot.transform.localEulerAngles.x < 360)
        //{
        //    horizontalDir = -1f;
        //}
        //else
        //{
        //    horizontalDir = 1;
        }*/
        //Horizontal Local
        pivot.transform.Rotate(0, swipeInput.DeltaTouch.x * horizontalDir, 0, Space.Self);
        //pivot.transform.localRotation *= Quaternion.Euler(new Vector3(0, smoothSwipe.DeltaTouch.x * direction, 0));
        //Vertical Global
        if (!lockVertical)
        {
            //pivot.transform.rotation *= Quaternion.Euler(new Vector3(smoothSwipe.DeltaTouch.y * direction, 0, 0));
            //CLAMP
            pivot.transform.Rotate(swipeInput.DeltaTouch.y * direction, 0, 0, Space.World);
        }
    }

    private void Clamp() {
        float camRotX = pivot.transform.rotation.x;
        //float camRotX = pivot.transform.localEulerAngles.x;
        Debug.Log(camRotX);
        //if (pivot.transform.localEulerAngles.x >= clampValue.x || pivot.transform.localEulerAngles.x >= clampValue.y)
        //pivot.transform.rotation = Quaternion.Euler(0, 0, 0);
        //Mathf.Clamp(camRotX >= 180 ? camRotX -= 360 : camRotX, clampValue.x, clampValue.y), pivot.transform.localEulerAngles.y, 0, 0);
        //pivot.transform.localEulerAngles = new Vector3(
        //    Mathf.Clamp(camRotX >= 180 ? camRotX -= 360 : camRotX, clampValue.x, clampValue.y), pivot.transform.localEulerAngles.y, 0);
    }

    private void RotateObjectDefault() {
        pivot.transform.localRotation *= Quaternion.Euler(new Vector3(swipeInput.DeltaTouch.y * direction, 0, 0));
    }

    private void RotateObjectWithY() {
        pivot.transform.localRotation *= Quaternion.Euler(new Vector3(0, swipeInput.DeltaTouch.x * direction, 0));
    }

}

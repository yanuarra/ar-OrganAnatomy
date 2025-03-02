using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ObjectScaleAndPan : MonoBehaviour {
    [SerializeField]
    private InputPinch inputPinch;
    [SerializeField]
    private Text text;
    private Plane Plane;
    public Transform pivot;
    public Plane plane;
    [Tooltip("(min,max) clamp for Scaling")]
    public Vector2 scaleClamp;
    float DecreaseCameraPanSpeed = 1f;

    private static readonly float PanSpeed = 1;
    private static readonly float ZoomSpeedTouch = 0.1f;
    private static readonly float ZoomSpeedMouse = 0.5f;
    private int panFingerId;

    private static readonly float[] BoundsX = new float[] { -.1f, .1f };
    private static readonly float[] BoundsY = new float[] { -.1f, .1f };
    private static readonly float[] ZoomBounds = new float[] { 10f, 85f };

    private Camera cam;

    private Vector3 lastPanPosition;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update() {
        //MoveObject();
        if (pivot == null)
            return;
        if (inputPinch == null)
            return;
        if (inputPinch.isPinching == false)
            return;
        RescaleObject();
        //LerpMovement(inputPinch.centerPinch);
        //if (inputPinch.isPinchAndMove)
        //MoveObject();
        //TranslationalMoveObject(inputPinch.centerPinch);
    }

    void RescaleObject() {
        pivot.transform.localScale -= new Vector3(inputPinch.DeltaPinch, inputPinch.DeltaPinch, inputPinch.DeltaPinch);
        float scale = Mathf.Clamp(pivot.transform.localScale.x, scaleClamp.x, scaleClamp.y);
        pivot.transform.localScale = new Vector3(scale, scale, scale);
    }

    void LerpMovement(Vector3 newPanPosition)
    {
        // get the touch position from the screen touch to world point
        Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3 (newPanPosition.x, newPanPosition.y, .5f));
        // lerp and set the position of the current object to that of the touch, but smoothly over time.
        pivot.transform.position = Vector3.Lerp(pivot.transform.position, touchedPos, Time.deltaTime);
    }

    void TranslationalMoveObject(Vector3 newPanPosition)
    {
        lastPanPosition = inputPinch.lastPanPosition;

        // Determine how much to move the camera
        Vector3 offset = cam.ScreenToViewportPoint(lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(offset.x * PanSpeed, offset.y * PanSpeed, 0);
        // Perform the movement
        pivot.transform.Translate(move, Space.World);
        // Ensure the camera remains within bounds.
        Vector3 pos = pivot.transform.position;
        //pos.x = Mathf.Clamp(pivot.transform.position.x, BoundsX[0], BoundsX[1]);
        //pos.y = Mathf.Clamp(pivot.transform.position.y, BoundsY[0], BoundsY[1]);
        pos.z = 0.5f;
        pivot.transform.position = pos;
        //text.text = pivot.transform.position.ToString();    
        // Cache the position
        lastPanPosition = newPanPosition;
        //lastPanPosition = pivot.transform.position;
    }
    void MoveObject()
    {
        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;
        Delta1 = ObjectPositionDelta(Input.GetTouch(0)) / DecreaseCameraPanSpeed;
        //pivot.transform.position -= new Vector3(inputPinch.DeltaPinch, inputPinch.DeltaPinch, inputPinch.DeltaPinch);
        pivot.transform.Translate(Delta1, Space.World);
    }
    private Vector3 dragOffset;
    private Plane dragPlane;
    protected Vector3 ObjectPositionDelta(Touch touch)
    {
        //not moved
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        //delta
        var rayBefore = cam.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = cam.ScreenPointToRay(touch.position);
        Debug.Log(rayBefore + " " + rayNow);
        if (Physics.Raycast(rayNow, out var hit) && hit.transform == transform)
        {
            dragOffset = hit.point - pivot.transform.position;
            dragPlane = new Plane(-cam.transform.forward, hit.point);
        }
        if (dragPlane.Raycast(rayBefore, out var enterBefore) && dragPlane.Raycast(rayNow, out var enterNow))
                return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        //not on plane
        return Vector3.zero;
    }
}

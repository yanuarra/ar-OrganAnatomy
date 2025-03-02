using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererHandler : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public LineRenderer line;
    public Material lineMats;

    public void InitLR()
    {
        line = this.gameObject.AddComponent(typeof(LineRenderer)) as LineRenderer;
        line.material = lineMats;
        line.startWidth = .005f;
        line.endWidth = .005f;
        line.positionCount = 2;
        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);
        line.startColor = Color.black;
        line.endColor = Color.black;
    }

    private void Update()
    {
        if (line == null) return;
        line.SetPosition(0, pointA.position);
        line.SetPosition(1, pointB.position);
    }
}

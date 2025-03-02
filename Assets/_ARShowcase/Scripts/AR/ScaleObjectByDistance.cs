using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObjectByDistance : MonoBehaviour
{
    float mdistance = 0f;
    float tempt = -1f;
    float minDistance = 0f;
    float maxDistance = 10f;
    [SerializeField]
    float minScale = 1f;
    [SerializeField]
    float maxScale = 3f;
    public Transform target;

    private void Start()
    {
        if (target == null)
            target = Camera.main.transform;
    }

    void UpdateScale()
    {
        var scale = Mathf.Lerp(minScale, maxScale, Mathf.InverseLerp(minDistance, maxDistance, mdistance));
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void LateUpdate()
    {
        if (Input.touchCount == 0) return;
        if (target != null)
        {
            tempt = mdistance;
            mdistance = Vector3.Distance(transform.position, target.position);
        }
        if (mdistance == tempt) return;
        UpdateScale();
    }
}

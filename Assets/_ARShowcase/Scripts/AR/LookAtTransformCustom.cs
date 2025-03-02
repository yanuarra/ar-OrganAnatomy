using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTransformCustom: MonoBehaviour
{
    public Transform target;
    public bool UseLerp = true;
    public float Speed = 20f;
    public bool UseUpdate = false;
    public bool UseLateUpdate = true;
    [System.Serializable]
    public enum lookAtType
    {
        Facing,
        Looking
    }
    [SerializeField]
    public lookAtType type = lookAtType.Facing;
    private void Start()
    {
        lookAt();
    }

    private void OnEnable()
    {
        lookAt();
    }

    void Update()
    {
        if (UseUpdate)
        {
#if PLATFORM_ANDROID
            //if (Input.touchCount == 0) return;
#endif
            lookAt();
        }
    }

    void LateUpdate()
    {
        if (UseLateUpdate)
        {
#if PLATFORM_ANDROID
            //if (Input.touchCount == 0) return;
#endif
            lookAt();
        }
    }

    public void LookAtInstant()
    {
        Quaternion rot;
        if (target != null) { 
            rot = Quaternion.LookRotation(transform.position - target.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
        }
    }

    void lookAt()
    {
        if (target != null)
        {
            if (UseLerp)
            {
                Quaternion rot;
                if (type == lookAtType.Facing)
                {
                    rot = Quaternion.LookRotation(transform.position - target.position);
                }
                else
                {
                    rot = Quaternion.LookRotation(target.position - transform.position);
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * Speed);
            }
            else
            {
                transform.LookAt(target, transform.forward);
            }
        }
    }
}
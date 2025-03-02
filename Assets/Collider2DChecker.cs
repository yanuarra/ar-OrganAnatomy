using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Collider2DChecker : MonoBehaviour
{
    [SerializeField] private UnityEvent UnityEvent_OnTriggerEnter2D;
    [SerializeField] private UnityEvent UnityEvent_OnTriggerExit2D;
    [SerializeField] private List<Collider2D> _collidersToInclude;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_collidersToInclude.Contains(collision)) return;
        UnityEvent_OnTriggerEnter2D?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_collidersToInclude.Contains(collision)) return;
        UnityEvent_OnTriggerExit2D?.Invoke();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unjani.Quiz
{
    public class MessageFooterPopup : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectFooter;
        [SerializeField]
        private float _shrinkHeight;
        [SerializeField]
        private float _expandHeight;
        private void Awake()
        {
            _rectFooter.sizeDelta = new Vector2(_rectFooter.sizeDelta.x, _shrinkHeight);
        }

        public void OnEnableMessage()
        {
            StartCoroutine(ExpandCoroutine());
        }

        public void OnDisableMessage(Action onDisableMessageDone)
        {
            StartCoroutine(ShrinkCoroutine(onDisableMessageDone));
        }

        private IEnumerator ExpandCoroutine()
        {
            float time = 0.4f;
            float elapsedTime = 0f;
            while(elapsedTime <= time)
            {
                float newExpandHeight = Mathf.Lerp(_shrinkHeight, _expandHeight, elapsedTime / time);
                _rectFooter.sizeDelta = new Vector2(_rectFooter.sizeDelta.x, newExpandHeight);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _rectFooter.sizeDelta = new Vector2(_rectFooter.sizeDelta.x, _expandHeight);
        }

        private IEnumerator ShrinkCoroutine(Action onDisableMessageDone)
        {
            float time = 0.2f;
            float elapsedTime = 0f;
            while (elapsedTime <= time)
            {
                float newExpandHeight = Mathf.Lerp(_expandHeight, _shrinkHeight, elapsedTime / time);
                _rectFooter.sizeDelta = new Vector2(_rectFooter.sizeDelta.x, newExpandHeight);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _rectFooter.sizeDelta = new Vector2(_rectFooter.sizeDelta.x, _shrinkHeight);
            onDisableMessageDone?.Invoke();
        }
    }
}

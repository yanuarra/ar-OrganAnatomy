using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unjani.Quiz
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rect;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _greenColor;
        [SerializeField]
        private float _defaultWidth;
        [SerializeField]
        private float _selectedWidth;

        public void OnBulletSelected()
        {
            _rect.sizeDelta = new Vector2(_selectedWidth, 25);
            _image.color = _greenColor;
        }

        public void OnBulletDeselected()
        {
            _rect.sizeDelta = new Vector2(_defaultWidth, 25);
            _image.color = _defaultColor;
        }
    }
}

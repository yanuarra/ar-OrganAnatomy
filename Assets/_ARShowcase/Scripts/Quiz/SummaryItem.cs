using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unjani.Quiz
{
    public class SummaryItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Sprite _imageCorrect;
        [SerializeField]
        private Sprite _imageFalse;
        [SerializeField]
        private Color _colorCorrect;
        [SerializeField]
        private Color _colorFalse;
        private event Action<int> _onButtonClick;
        private int _index;

        public void InitSummaryItem(int index, bool isTrue, Action<int> onButtonClick)
        {
            _onButtonClick = onButtonClick;
            _index = index;
            _titleText.text = $"Soal Nomor {index + 1}";
            if (isTrue)
            {
                _titleText.color = _colorCorrect;
                _image.sprite = _imageCorrect;
            }
            else
            {
                _titleText.color = _colorFalse;
                _image.sprite = _imageFalse;
            }
        }

        public void OnButtonClick()
        {
            _onButtonClick.Invoke(_index);
        }
    }
}

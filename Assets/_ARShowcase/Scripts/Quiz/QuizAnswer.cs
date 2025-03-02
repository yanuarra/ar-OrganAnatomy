using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unjani.Quiz
{
    public class QuizAnswer : MonoBehaviour
    {
        public QuizData.Answer Answer;
        public string OptionAlphabet;
        [SerializeField]
        private TMP_Text _answerText;
        [SerializeField]
        private Image _highlightImage;
        [SerializeField]
        private TMP_Text _alphabetText;
        [SerializeField]
        private Image _alphabetBoxImage;
        [SerializeField]
        private Color _greenColor;
        [SerializeField]
        private Color _redColor;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _defaultColorHighlight;
        [SerializeField]
        private Button _button;
        private RectTransform _rect;
        private event Action<QuizAnswer> _action_OnSelect;

        private void Start()
        {
            _rect = this.GetComponent<RectTransform>();
        }

        public void InitQuizAnswer(QuizData.Answer answer, Action<QuizAnswer> action_OnSelect, int index)
        {
            Answer = answer;
            OptionAlphabet = IntToLetters(index + 1);
            _action_OnSelect = action_OnSelect;
            //_answerText.text = $"{OptionAlphabet}. {Answer.answer}";
            _answerText.text = $"{Answer.answer}";
            _alphabetText.text = OptionAlphabet;
            if (QuizDataHandler.Instance.IsQuizDone)
            {
                _button.interactable = false;
            } else
            {
                _button.interactable = true;
            }
            OnDeselect();
            StartCoroutine(RefreshHeightCoroutine());
        }

        private IEnumerator RefreshHeightCoroutine()
        {
            yield return new WaitForEndOfFrame();
            float offset = 80;
            float newY = Mathf.Abs(_answerText.rectTransform.rect.y) + offset;
            newY = newY <= 140f ? 140f : newY;
            _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, newY);
        }

        public void OnSelect()
        {
            _action_OnSelect?.Invoke(this);
            SetColorGreen();
        }

        public void OnDeselect()
        {
            SetColorDefault();
        }

        public void SetColorGreen()
        {
            _highlightImage.color = _greenColor;
            _alphabetBoxImage.color = _greenColor;
            _answerText.color = _greenColor;
        }

        public void SetColorRed()
        {
            _highlightImage.color = _redColor;
            _alphabetBoxImage.color = _redColor;
            _answerText.color = _redColor;
        }

        public void SetColorDefault()
        {
            _highlightImage.color = _defaultColorHighlight;
            _alphabetBoxImage.color = _defaultColor;
            _answerText.color = _defaultColor;
        }

        private string IntToLetters(int value)
        {
            string result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }
            return result;
        }
    }
}

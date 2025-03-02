using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Unjani.Quiz
{
    public class QuizResultPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _overlay;
        [SerializeField]
        private TMP_Text _titleTotalScore;
        [SerializeField]
        private TMP_Text _descriptionTotalScore;
        [SerializeField]
        private RectTransform _resultRect;
        [SerializeField]
        private Transform _summaryItemContent;
        [SerializeField]
        private ObjectPool _summaryItemPool;
        [SerializeField]
        private UnityEvent<int> UnityEvent_OpenQuizDisplayUserAnswer;
        [SerializeField]
        private UnityEvent UnityEvent_OnQuizRestart;
        [SerializeField]
        private UnityEvent UnityEvent_OnQuizResultDone;
        private List<QuizData.Answer> _userAnswers;

        private void Awake()
        {
            _overlay.SetActive(false);
        }

        public void InitQuizResult(List<QuizData.Answer> userAnswers)
        {
            _overlay.SetActive(true);
            _userAnswers = userAnswers;
            int questionCount = QuizDataHandler.Instance.GetQuestionsCount();
            int correctAnswer = 0;
            for (int i = 0; i < userAnswers.Count; i++)
            {
                int numberIndex = i + 1;
                if (userAnswers[i].isCorrect)
                {
                    correctAnswer = correctAnswer + 1;
                }
            }
            ResetAllSummaryItem();
            SpawnAllSummaryItem();
            _titleTotalScore.text = $"{correctAnswer}<size=96>/{questionCount}</size>";
            _descriptionTotalScore.text = $"{correctAnswer} dari {questionCount} jawabanmu benar";
            Refreshlayout();
        }

        private void ResetAllSummaryItem()
        {
            SummaryItem[] obj = _summaryItemContent.GetComponentsInChildren<SummaryItem>();
            foreach (SummaryItem summaryItem in obj)
            {
                _summaryItemPool.AddObjectToPool(summaryItem.gameObject);
            }
        }

        private void SpawnAllSummaryItem()
        {
            for (int i = 0; i < _userAnswers.Count; i++)
            {
                GameObject obj = _summaryItemPool.GetObjectFromPool();
                Transform objTransform = obj.transform;
                objTransform.SetParent(_summaryItemContent);
                objTransform.localScale = Vector3.one;
                objTransform.localRotation = Quaternion.identity;
                objTransform.localPosition = Vector3.zero;
                SummaryItem summaryItem = obj.GetComponent<SummaryItem>();
                summaryItem.InitSummaryItem(i, _userAnswers[i].isCorrect, OpenQuizDisplayUserAnswer);
            }
        }

        public void OpenQuizResult()
        {
            _overlay.SetActive(true);
        }

        public void OpenQuizDisplayUserAnswer(int index)
        {
            _overlay.SetActive(false);
            UnityEvent_OpenQuizDisplayUserAnswer.Invoke(index);
        }

        public void OnQuizRestart()
        {
            _overlay.SetActive(false);
            UnityEvent_OnQuizRestart.Invoke();
        }

        public void OnQuizResultDone()
        {
            UnityEvent_OnQuizResultDone.Invoke();
        }

        private void Refreshlayout()
        {
            for (var i = 0; i < _resultRect.childCount; i++)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_resultRect.GetChild(i).GetComponent<RectTransform>());
        }
    }
}

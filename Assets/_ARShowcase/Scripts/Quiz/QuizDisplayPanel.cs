using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Unjani.Quiz
{
    public class QuizDisplayPanel : MonoBehaviour
    {
        [SerializeField]
        private GameObject _overlay;
        [SerializeField]
        private TMP_Text _titleText;
        [SerializeField]
        private TMP_Text _questionText;
        [SerializeField]
        private TMP_Text _progressText;
        [SerializeField]
        private TMP_Text _prevButtonText;
        [SerializeField]
        private TMP_Text _anwserResultTrueText;
        [SerializeField]
        private TMP_Text _anwserResultFalseText;
        [SerializeField]
        private TMP_Text _confirmationSubmitQuizText;
        [SerializeField]
        private RawImage _questionImage;
        [SerializeField]
        private GameObject _questionFrame;
        [SerializeField]
        private AspectRatioFitter _aspectRatioFitterQuestionImage;
        [SerializeField]
        private GameObject _footerDefault;
        [SerializeField]
        private GameObject _footerResult;
        [SerializeField]
        private GameObject _doneButton;
        [SerializeField]
        private GameObject _nextButton;
        [SerializeField]
        private GameObject _confirmationSubmitQuiz;
        [SerializeField]
        private GameObject _prevButtonArrowGreen;
        [SerializeField]
        private GameObject _prevButtonArrowGray;
        [SerializeField]
        private Transform _quizAnswerContainer;
        [SerializeField]
        private RectTransform _displayRect;
        [SerializeField]
        private ObjectPool _quizAnswerObjectPool;
        [SerializeField]
        private Transform _bulletContainer;
        [SerializeField]
        private ObjectPool _bulletObjectPool;
        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _greenColor;
        [SerializeField]
        private UnityEvent UnityEvent_OpenQuizResult;
        [SerializeField]
        private MessageFooterPopup _messageFooterPopup;
        private int _currentQuizIndex;
        private List<QuizData.Question> _questions = new List<QuizData.Question>();
        private QuizAnswer _selectedQuizAnswer;
        private string _currentCorrectOption;

        [SerializeField]
        private TMP_Text _notFoundText;
        [SerializeField]
        private Button _backButton;

        private void Awake()
        {
            _overlay.SetActive(false);
            _footerDefault.SetActive(false);
            _footerResult.SetActive(false);
            _confirmationSubmitQuiz.SetActive(false);
            _anwserResultTrueText.gameObject.SetActive(false);
            _anwserResultFalseText.gameObject.SetActive(false);
            _backButton.gameObject.SetActive(false);
        }

        public void InitQuizNotFoundDisplay()
        {
            _overlay.SetActive(true);
            _backButton.gameObject.SetActive(true);
            _backButton.onClick.AddListener(delegate { QuizDataHandler.Instance.OnQuizResultDone(); }); ;
            _notFoundText.text = string.Format("Modul {0} tidak ditemukan!\nSentuh untuk kembali ke laman AR . . .",
                QuizDataHandler.Instance.GetCurrentModuleId());
        }

        public void InitQuizDisplay(List<QuizData.Question> questions)
        {
            _overlay.SetActive(true);

            _questions = questions;

            //_titleText.text = QuizDataHandler.Instance.GetCurrentModuleId();
            _progressText.text = "";
            _selectedQuizAnswer = null;

            ResetAllBullet();
            SpawnAllBullet();
            DisablePrevQuestion();
            DisableDoneButton();
            SetQuizByIndex(0);
        }

        private void SetQuizByIndex(int index)
        {
            Debug.Log(_currentQuizIndex);
            Bullet oldBullet = _bulletContainer.GetChild(_currentQuizIndex).GetComponent<Bullet>();
            oldBullet.OnBulletDeselected();
            _currentQuizIndex = index;
            Bullet newBullet = _bulletContainer.GetChild(_currentQuizIndex).GetComponent<Bullet>();
            newBullet.OnBulletSelected();

            _footerDefault.SetActive(true);
            _footerResult.SetActive(false);
            _anwserResultTrueText.gameObject.SetActive(false);
            _anwserResultFalseText.gameObject.SetActive(false);

            _currentCorrectOption = "";
            _selectedQuizAnswer = null;
            _questionText.text = _questions[_currentQuizIndex].question;
            _progressText.text = $"Soal no {_currentQuizIndex + 1}";
            ResetAllQuizAnswer();
            SpawnAllQuizAnswer();
            Debug.Log(_questions[_currentQuizIndex].image.url);
            if (string.IsNullOrEmpty(_questions[_currentQuizIndex].image.url))
            {
                _questionFrame.SetActive(false);
            }
            else
            {
                _questionFrame.SetActive(true);
                StartCoroutine(SetImageCoroutine());
            }
            StartCoroutine(RefreshLayoutCoroutine());
        }

        private IEnumerator SetImageCoroutine()
        {
            _questionImage.texture = null;
            Texture2D tex;
            tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(_questions[_currentQuizIndex].image.url);
            yield return www.SendWebRequest();
            while (!www.isDone)
            {
                yield return www;
            }
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _questionImage.texture = tex;
                _aspectRatioFitterQuestionImage.aspectRatio = (float)tex.width / (float)tex.height;
            }
        }

        private IEnumerator RefreshLayoutCoroutine()
        {
            yield return new WaitForEndOfFrame();
            for (var i = 0; i < _displayRect.childCount; i++)
                LayoutRebuilder.ForceRebuildLayoutImmediate(_displayRect.GetChild(i).GetComponent<RectTransform>());
        }

        public void SetQuizResultByIndex(int index)
        {
            _overlay.SetActive(true);
            SetQuizByIndex(index);
            _footerDefault.SetActive(false);
            _footerResult.SetActive(true);
        }

        public void OpenQuizResult()
        {
            _overlay.SetActive(false);
            UnityEvent_OpenQuizResult.Invoke();
        }

        private void ResetAllBullet()
        {
            Bullet[] obj = _bulletContainer.GetComponentsInChildren<Bullet>();
            foreach (Bullet bullet in obj)
            {
                _bulletObjectPool.AddObjectToPool(bullet.gameObject);
            }
        }

        private void SpawnAllBullet()
        {
            for (int i = 0; i < _questions.Count; i++)
            {
                GameObject obj = _bulletObjectPool.GetObjectFromPool();
                Transform objTransform = obj.transform;
                objTransform.SetParent(_bulletContainer);
                objTransform.localScale = Vector3.one;
                objTransform.localRotation = Quaternion.identity;
                objTransform.localPosition = Vector3.zero;
            }
        }

        private void ResetAllQuizAnswer()
        {
            QuizAnswer[] obj = _quizAnswerContainer.GetComponentsInChildren<QuizAnswer>();
            foreach (QuizAnswer quizAnswer in obj)
            {
                _quizAnswerObjectPool.AddObjectToPool(quizAnswer.gameObject);
            }
        }

        private void SpawnAllQuizAnswer()
        {
            List<QuizData.Answer> answers = _questions[_currentQuizIndex].answers;
            for (int i = 0; i < answers.Count; i++)
            {
                GameObject obj = _quizAnswerObjectPool.GetObjectFromPool();
                Transform objTransform = obj.transform;
                objTransform.SetParent(_quizAnswerContainer);
                objTransform.localScale = Vector3.one;
                objTransform.localRotation = Quaternion.identity;
                objTransform.localPosition = Vector3.zero;
                objTransform.SetAsLastSibling();
                QuizAnswer quizAnswer = obj.GetComponent<QuizAnswer>();
                quizAnswer.InitQuizAnswer(answers[i], OnSelectAnswer, i);
                if (QuizDataHandler.Instance.IsQuizDone)
                {
                    if (answers[i].isCorrect)
                    {
                        if (QuizDataHandler.Instance.IsUserAnswered(_currentQuizIndex, answers[i])) {
                            quizAnswer.OnSelect();
                            //_currentCorrectOption = quizAnswer.OptionAlphabet;
                            _anwserResultTrueText.text = $"Jawaban kamu benar";
                            _anwserResultTrueText.gameObject.SetActive(true);
                            _anwserResultTrueText.transform.SetSiblingIndex(objTransform.GetSiblingIndex());
                        } else
                        {
                            quizAnswer.OnSelect();
                            _anwserResultTrueText.text = $"Jawaban yang benar";
                            _anwserResultTrueText.gameObject.SetActive(true);
                            _anwserResultTrueText.transform.SetSiblingIndex(objTransform.GetSiblingIndex());
                        }
                    }
                    else if (QuizDataHandler.Instance.IsUserAnswered(_currentQuizIndex, answers[i]))
                    {
                        quizAnswer.SetColorRed();
                        _anwserResultFalseText.text = $"Jawaban kamu";
                        _anwserResultFalseText.gameObject.SetActive(true);
                        _anwserResultFalseText.transform.SetSiblingIndex(objTransform.GetSiblingIndex());
                    }
                } 
                else
                {
                    if (QuizDataHandler.Instance.IsUserAnswered(_currentQuizIndex, answers[i]))
                    {
                        quizAnswer.OnSelect();
                    }
                }
            }
        }

        private void OnSelectAnswer(QuizAnswer quizAnswer)
        {
            if (_selectedQuizAnswer != null)
            {
                _selectedQuizAnswer.OnDeselect();
            }
            _selectedQuizAnswer = quizAnswer;
            OnConfirmSelectedAnswer();

        }

        public void OnConfirmSelectedAnswer()
        {
            if (_selectedQuizAnswer == null)
                return;
            if (QuizDataHandler.Instance.IsQuizDone)
                return;
            QuizData.Answer answer = _selectedQuizAnswer.Answer;
            QuizDataHandler.Instance.AddUserAnswer(_currentQuizIndex, answer);
        }

        public void OnEnableConfirmQuiz()
        {
            _confirmationSubmitQuiz.SetActive(true);
            _messageFooterPopup.OnEnableMessage();
            int unansweredQuestion = QuizDataHandler.Instance.GetUnansweredQuestionsCount();
            _confirmationSubmitQuizText.text = "Sudah yakin dengan semua jawaban kamu?";
            if(unansweredQuestion > 0)
            {
                _confirmationSubmitQuizText.text += $"\n<color=red>masih ada {unansweredQuestion} pertanyaan yang belum dijawab</color>";
            }
        }

        public void OnDisableConfirmQuiz()
        {
            _messageFooterPopup.OnDisableMessage(delegate { _confirmationSubmitQuiz.SetActive(false); });
        }

        public void OnEnableResultQuiz()
        {
            _confirmationSubmitQuiz.SetActive(false);
            _overlay.SetActive(false);
            QuizDataHandler.Instance.OnQuizDisplayDone();
        }

        public void SetNextQuestion()
        {
            int newIndex = _currentQuizIndex + 1;
            if (newIndex >= _questions.Count)
            {
                OnEnableConfirmQuiz();
                return;
            }
            else if(newIndex == _questions.Count - 1)
            {
                ActiveDoneButton();
            }
            EnablePrevQuestion();
            SetQuizByIndex(newIndex);
        }

        public void SetPrevQuestion()
        {
            int newIndex = _currentQuizIndex - 1;
            if (newIndex < 0)
            {
                return;
            } else if (newIndex == 0)
            {
                DisablePrevQuestion();

            }
            DisableDoneButton();
            SetQuizByIndex(newIndex);
        }

        public void EnablePrevQuestion()
        {
            _prevButtonText.color = _greenColor;
            _prevButtonArrowGreen.SetActive(true);
            _prevButtonArrowGray.SetActive(false);
        }

        public void DisablePrevQuestion()
        {
            _prevButtonText.color = _defaultColor;
            _prevButtonArrowGreen.SetActive(false);
            _prevButtonArrowGray.SetActive(true);
        }

        public void ActiveDoneButton()
        {
            _nextButton.SetActive(false);
            _doneButton.SetActive(true);
        }

        public void DisableDoneButton()
        {
            if (_questions.Count == 1)
            {
                ActiveDoneButton();
                return;
            }
            _nextButton.SetActive(true);
            _doneButton.SetActive(false);
        }
    }
}

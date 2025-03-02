using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Unjani.Module;
using static Unjani.Quiz.QuizData;

namespace Unjani.Quiz 
{
    public class QuizDataHandler : MonoBehaviour 
    {
        [SerializeField]
        private string _currentModuleId;
        //[SerializeField]
        //private string _quizDataFilename;

        [Header("Data")]
        [SerializeField]
        private QuizData _quizData;
        [SerializeField]
        private List<QuizData.Module> _quizDataModule = new List<QuizData.Module>();
        [SerializeField]
        private List<QuizData.Question> _questions = new List<QuizData.Question>();
        [SerializeField]
        private List<QuizData.Answer> _userAnswer = new List<QuizData.Answer>();
        private TimerController _timerController;

        public UnityEvent<List<QuizData.Question>> UnityAction_OnGetQuizDataDone;
        public UnityEvent<List<QuizData.Answer>> UnityAction_OnQuizDisplayDone;
        public UnityEvent UnityAction_OnQuizNotFound;
        public bool IsQuizDone;
        public static QuizDataHandler Instance;

        private void Awake()
        {
            SetAsSingleton();
        }

        private void SetAsSingleton()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(StaticData.ModuleId))
            {
                _currentModuleId = StaticData.ModuleId;
            }
            APIHelper.Instance.SetRootURL("https://api-ar-showcase-unjani.metanesia.id/quizzes");
            string suburi = string.Format("?module.moduleId={0}", _currentModuleId);
            Debug.Log(suburi);
            //GetQuizData("");
            GetQuizData(suburi);

            if (_timerController == null) 
                _timerController = GetComponent<TimerController>() == null? gameObject.AddComponent<TimerController>() : GetComponent<TimerController>();
            _timerController.type = TimerController.TimerType.Stopwatch;
        }

        public void GetQuizData(string moduleId)
        {
            //_currentModuleId = moduleId;
            StartCoroutine(APIHelper.Instance.GetDataCoroutine(moduleId, OnGetQuizDataDone));
        }

        private void OnGetQuizDataDone(string data)
        {
            Debug.Log(data);
            var temp = "{\"quizzes\": " + data + "}";
            //_quizData = JsonUtility.FromJson<QuizData>(temp);
            //_quizData = JsonConvert.DeserializeObject<QuizData>(temp);
            //_quizDataModule = JsonUtility.FromJson<QuizData.Module>(data);  
            _quizDataModule = JsonConvert.DeserializeObject<List<QuizData.Module>>(data);
            foreach (var item in _quizDataModule[0].questions)
            {
                if (item.image == null)
                    item.image = new QuizData.Thumbnail();
            }
            OnInitQuizDisplayPanel();
        }

        public void OnInitQuizDisplayPanel()
        {
            //QuizData.Module module = _quizData.modules.Find(result => result._id == _currentModuleId);
            //QuizData.Module module = _quizData.modules.Find(result => result.moduleId == _currentModuleId);
            QuizData.Module module = _quizDataModule[0];
            if (module == null)
            {
                Debug.Log($"Module {_currentModuleId} not found!");
                UnityAction_OnQuizNotFound.Invoke();
                return;
            }
            else
            {
                Debug.Log($"Module {_currentModuleId} found!");
            }

            List<QuizData.Question> questions = module.questions;
            int moduleQuestionCount = questions.Count;
            int showQuestionCount = module.showQuetionCount;
            int maxQuestionCount = showQuestionCount;
            if(showQuestionCount == 0)
            {
                maxQuestionCount = moduleQuestionCount;
            }
            else if (showQuestionCount > moduleQuestionCount)
            {
                maxQuestionCount = moduleQuestionCount;
            }
            _questions = RandomizeQuestion(questions, maxQuestionCount, moduleQuestionCount);
            _userAnswer = new List<QuizData.Answer>();
            IsQuizDone = false;
            for (int i = 0; i < maxQuestionCount; i++)
            {
                QuizData.Answer answer = new QuizData.Answer();
                _userAnswer.Add(answer);
            }
            UnityAction_OnGetQuizDataDone.Invoke(_questions);

            _timerController.BeginStopwatch();
        }

        private List<QuizData.Question> RandomizeQuestion(List<QuizData.Question> questions, int maxCount, int questionCount)
        {
            List<int> randomIntList = new List<int>();
            for (int i = 0; i < questionCount; i++)
            {
                randomIntList.Add(i);
            }
            List<QuizData.Question> questionsTemp = new List<QuizData.Question>();
            for (int i = 0; i < maxCount; i++)
            {
                int randomInt = UnityEngine.Random.Range(0, randomIntList.Count);
                questionsTemp.Add(questions[randomIntList[randomInt]]);
                randomIntList.RemoveAt(randomInt);
            }
            return questionsTemp;
        }

        public string GetCurrentModuleId()
        {
            return _currentModuleId;
        }

        public int GetQuestionsCount()
        {
            return _questions.Count;
        }

        public int GetUnansweredQuestionsCount()
        {
            int unansweredQuestion = 0;
            for (int i = 0; i < _userAnswer.Count; i++)
            {
                if (string.IsNullOrEmpty(_userAnswer[i].answer))
                {
                    unansweredQuestion = unansweredQuestion + 1;
                }
            }
            return unansweredQuestion;
        }

        public void AddUserAnswer(int index, QuizData.Answer answer)
        {
            _userAnswer[index] = answer;
        }

        public bool IsUserAnswered(int index, QuizData.Answer answer)
        {
            return _userAnswer[index] == answer;
        }

        public void OnQuizDisplayDone()
        {
            IsQuizDone = true;
            UnityAction_OnQuizDisplayDone.Invoke(_userAnswer);
        }

        int score = 0;
        public void OnQuizResultDone()
        {
            //Scoring
            string rootURL = "https://vr-sejarah.metanesia.id";
            //string rootURL = "http://192.168.1.13:4000";
            APIHelper.Instance.SetRootURL(rootURL);
            Root root = new();
            root.kuis = new List<QuizUserAnswerData>();
            for (int i = 0; i < _questions.Count; i++)
            {
                QuizUserAnswerData a = new QuizUserAnswerData();
                a.anatomyData = StaticData.ModuleName;
                Debug.Log(a.anatomyData);
                a.question = _questions[i].question;
                a.answer = _userAnswer[i].answer;
                a.isCorrect = _userAnswer[i].isCorrect;
                a.time = _timerController.GetElapsedTimeAsString();
                score = _userAnswer[i].isCorrect? score +1 : score;
                a.score = score.ToString();
                root.kuis.Add(a);
            }
            //root.time = _timerController.GetElapsedTimeAsString();
            //root.score = score.ToString();
            //string postData = JsonUtility.ToJson(root);
            string postData = JsonConvert.SerializeObject(root);
            Debug.Log(postData);
            StartCoroutine(APIHelper.Instance.PostDataCoroutine("api/guest/v1/kuis/ar/me", postData, true, result =>
            {
                if (StaticData.requestError)
                {
                    Debug.Log("Error nih");
                }
                else
                {
                    Debug.Log(result);
                    //StaticData.SetUserLoginData(result);
                }
                if (string.IsNullOrEmpty(StaticData.SceneNameBackFromQuiz))
                {
                    return;
                }
                SceneManager.LoadScene(StaticData.SceneNameBackFromQuiz);
            }));
        }

        public class Root
        {
            public List<QuizUserAnswerData> kuis;
          
        }

        public class QuizUserAnswerData
        {
            public string anatomyData;
            public string question;
            //public Answer answer;
            public string answer;
            public bool isCorrect;
            public string time;
            public string score;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Unjani.Quiz 
{
    [Serializable]
    public class QuizData 
    {
        public List<Module> modules = new List<Module>();

        [Serializable]
        public class Module 
        {
            public string _id;
            public int showQuetionCount;
            public List<Question> questions = new List<Question>();
        }

        [Serializable]
        public class Question {
            [TextArea]
            public string question;
            public Thumbnail image;
            public List<Answer> answers = new List<Answer>();
        }

        [Serializable]
        public class Answer {
            [TextArea]
            public string answer;
            public bool isCorrect;
        }

        [Serializable]
        public class Thumbnail
        {
            public string name;
            public string _id;
            public string url;
            public Sprite thumbnailSprite;
        }

        [Serializable]
        public class File
        {
            public string name;
            public string url;
        }
    }
}

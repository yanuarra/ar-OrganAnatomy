using System.Collections.Generic;
using UnityEngine;
using System;
using Unjani.Quiz;
using Newtonsoft.Json;
using System.IO;

namespace Unjani.Module {
    public class ModuleDataHandler : MonoBehaviour
    {
        [SerializeField]
        private string _moduleDataFilename;
        public ARPrefabHandler arPrefabHandler;
        public Module module;
        public static ModuleDataHandler Instance;
        public bool isDone { get; private set; }
        public string directory;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            isDone = false;
            directory = Path.Combine(Application.persistentDataPath, "ImageTargetDir");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            GetModuleData();
        }

        public void GetModuleData()
        {
            StartCoroutine(APIHelper.Instance.GetDataCoroutine(_moduleDataFilename, OnGetModuleDataDone));
        }

        private void OnGetModuleDataDone(string data)
        {
            //print(data.GetType());
            // var moduleCol = JsonConvert.DeserializeObject<List<ModuleData>>(data);
            var temp = "{\"module\": "+data+"}";
            module = JsonUtility.FromJson<Module>(temp);
            foreach (var module in module.module)
            {
                foreach (var anatomy in module.anatomy)
                {
                    Anatomy tempAnatomy = arPrefabHandler.FindDataByAnatomyId(anatomy.anatomyId, module.moduleId);
                    if (tempAnatomy != null)
                        anatomy.ARPrefab = tempAnatomy.prefab;
                    if (!string.IsNullOrEmpty(anatomy.file.url)) {
                        StartCoroutine(APIHelper.Instance.GetImageTexture(anatomy.file.url, (texture) =>
                        {
                            byte[] data = texture.EncodeToJPG(80);
                            string imageName = anatomy.file.name;
                            string imagePath = Path.Combine(directory, anatomy.file.name);
                            Debug.Log(imageName + " " + imagePath);
                            System.IO.File.WriteAllBytes(imagePath, data);
                        }));
                    }
                    foreach (var sub in anatomy.subanatomy)
                    {
                        if (string.IsNullOrEmpty(sub.thumbnail.url) || string.IsNullOrWhiteSpace(sub.thumbnail.url) || sub.thumbnail.url == null)
                        {
                            sub.thumbnail.thumbnailSprite = null;
                            continue;
                        }
                        StartCoroutine(APIHelper.Instance.GetImageTexture(sub.thumbnail.url, (texture) =>
                        {
                            sub.thumbnail.thumbnailSprite = texture != null? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f): null;
                        }));

                    }
                }
            }
            isDone = true;
        }

        public Module GetModule(){
            return module;
        }

        public AnatomyData GetAnatomyDataByID(string _id)
        {
            foreach (var module in module.module)
            {
                foreach (var anatomy in module.anatomy)
                {
                    if (anatomy.anatomyId == _id)
                    {
                        return anatomy;
                    }
                }
            }
            return null;
        }

        public ModuleData GetModuleDataByAnatomyID(string _id)
        {
            foreach (var module in module.module)
            {
                foreach (var anatomy in module.anatomy)
                {
                    if (anatomy.anatomyId == _id)
                    {
                        return module;
                    }
                }
            }
            return null;
        }
    }

    [Serializable]
    public class Module 
    {
        public string nama;
        public List<ModuleData> module = new List<ModuleData>();
    }
    
    [Serializable]
    public class ModuleData
    {
        public string title;
        public string id;
        public string moduleId;
        public Thumbnail thumbnail;
        [TextArea(5, 10)]
        public string description;
        public List<AnatomyData> anatomy = new List<AnatomyData>();
        public QuizData.Module quizzes;
    }

    [Serializable]
    public class AnatomyData{
        public string title;
        public string id;
        public string anatomyId;
        //public string thumbnail;
        public Thumbnail thumbnail;
        [TextArea(5, 10)]
        public string description;
        public GameObject ARPrefab;
        public List<SubAnatomyData> subanatomy = new List<SubAnatomyData>();
        public Thumbnail file;
    }

    [Serializable]
    public class SubAnatomyData{
        public string title;
        public string id;
        public Thumbnail thumbnail;
        [TextArea(5, 10)]
        public string description;
    }

    [Serializable]
    public class Thumbnail{
        public string name;
        public string _id;
        public string url;
        public Sprite thumbnailSprite;
    }


    [Serializable]
    public class File
    {
        public string name;
        public string _id;
        public string url;
    }
}


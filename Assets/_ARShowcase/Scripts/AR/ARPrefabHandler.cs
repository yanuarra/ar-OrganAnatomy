using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Unjani.Module
{
    public class ARPrefabHandler : MonoBehaviour
    {
        public List<Data> data = new List<Data>();

        public Data FindDataByModuleId(string id)
        {
            return data.Find(data => data.module_id == id);
        }

        public Anatomy FindDataByAnatomyId(string anatomyId, string moduleId)
        {
            Data newData = FindDataByModuleId(moduleId);
            if (newData!=null)
            {
                return newData.anatomy.Find(data => data.anatomy_id == anatomyId);
            }
            return null;
        }
    }

    [Serializable]
    public class Data 
    {
        public string module_id;
        public List<Anatomy> anatomy = new List<Anatomy>();
    }

    [Serializable]
    public class Anatomy{
        public string anatomy_id;
        public string ARImage;
        public GameObject prefab;
    } 
}


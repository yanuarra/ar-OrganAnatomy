using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unjani.Module
{
    public class ConverterDataToLocal : MonoBehaviour
    {
        public static ConverterDataToLocal Instance;

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

        //public AnatomyPart ConvertAnatomyJsonToAnatomyLocal(Unjani.Module.AnatomyData data){
        //    AnatomyPart tempData = new AnatomyPart();
        //    tempData.partName = data.title;
        //    tempData.partDescription = data.desc;
        //    tempData.ARPrefab = data.ARPrefab;
        //    tempData.subPart = new List<AnatomySub>();
        //    foreach (var subanatomy in data.subanatomy)
        //    {
        //        AnatomySub tempDataSub = new AnatomySub();
        //        tempDataSub.subName = subanatomy.title;
        //        tempDataSub.subImage = null;
        //        tempDataSub.subDescription =  subanatomy.description;
        //        tempData.subPart.Add(tempDataSub);
        //    }
        //    return tempData;
        //}
    }
}


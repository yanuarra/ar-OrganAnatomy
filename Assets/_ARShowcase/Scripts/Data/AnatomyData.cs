//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "Anatomy", menuName = "ScriptableObjects/Create Anatomy Data", order = 1)]
//public class AnatomyData : ScriptableObject
//{
//    public string mainName;
//    [TextArea(5, 10)]
//    public string mainDescription;
//    public Sprite thumbnail;
//    public string video;
//    public AudioClip audio;
//    public List<AnatomyPart> anatomyPart = new List<AnatomyPart>();
//}

//[Serializable]
//public class AnatomyPart
//{
//    public string partName;
//    [TextArea(5, 10)]
//    public string partDescription;
//    public Sprite thumbnail;
//    public GameObject ARPrefab;
//    public string ARImageName;
//    public List<AnatomySub> subPart = new();
//}

//[Serializable]
//public class AnatomySub
//{
//    public string subName;
//    public Sprite subImage;
//    [TextArea(5, 10)]
//    public string subDescription;
//}
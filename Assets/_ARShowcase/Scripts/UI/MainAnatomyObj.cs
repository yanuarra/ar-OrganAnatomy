//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class MainAnatomyObj : MonoBehaviour
//{
//    public TMP_Text headerText; 
//    public TMP_Text descText;
//    public GameObject content;
//    public ObjectPool objectPool;
//    public List<MainAnatomyUI> subs = new List<MainAnatomyUI>();

//    public void Init(AnatomyData _data)
//    {
//        headerText.text = _data.mainName;
//        descText.text = descText? _data.mainDescription:"";

//        GameObject go = objectPool.GetObjectFromPool();
//        go.transform.SetParent(content.transform);
//        go.transform.position = Vector3.zero;
//        go.transform.localScale = Vector3.one;
//        go.transform.localRotation = Quaternion.identity;
//        MainAnatomyUI mainUI = go.GetComponent<MainAnatomyUI>();
//        go.name += mainUI.subText;
//        mainUI.Init(_data);
//        if (!subs.Contains(mainUI)) subs.Add(mainUI);
//        foreach (var item in _data.anatomyPart)
//        {
//        }
//    }
//}

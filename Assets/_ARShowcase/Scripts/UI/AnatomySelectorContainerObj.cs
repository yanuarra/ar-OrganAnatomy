using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnatomySelectorContainerObj : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text_name;
    [SerializeField]
    private TMP_Text text_SubCount;
    public GameObject content;
    public string moduleId;
    [SerializeField]
    private Image icon;

    public void Init(string _name, string _id, string _count)
    {
        if (icon == null) icon = GetComponentInChildren<Image>();
        if (text_name == null) text_name = GetComponentInChildren<TMP_Text>();
        text_name.text = _name.Replace("_", " ");
        moduleId = _id;
        text_SubCount.text = string.Format("{0} Sub Anatomy", _count);
        gameObject.name = string.Format(("Container_{0}"), _name.Replace("_", " "));
    }

    public void SetThumbnail(Texture2D texture)
    {
        Sprite spriteThumbnail = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        icon.sprite = spriteThumbnail;
    }
}
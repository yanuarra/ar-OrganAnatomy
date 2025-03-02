using DanielLochner.Assets.SimpleScrollSnap;
//using LottiePlugin.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialUIHandler : MonoBehaviour
{
    public Button btn_Prev;
    public Button btn_Next;
    public Button btn_Done;
    public SimpleScrollSnap sss;
    public UnityEvent done;
    public RectTransform content;
    public RectTransform viewport;
    //public List<AnimatedImage> anim = new List<AnimatedImage>();
    public GameObject overlay;
    [SerializeField] private VideoPlayer player;
    [SerializeField] private VideoClip zoom;
    [SerializeField] private VideoClip rotate;
    [SerializeField] private VideoClip click;


    public void Init()
    {
        sss.OnPanelSelected.RemoveAllListeners();
        sss.OnPanelSelected.AddListener(delegate { CheckNavs(); });
        btn_Done.onClick.AddListener(delegate { Done(); });
        CheckNavs();
    }

    public void ToggleTutorial(bool _state) 
    {
        overlay.SetActive(_state);
        Reset();
    }

    public void Reset()
    {
        sss.GoToPanel(0);
    }

    public void ChangeVideoTutorial()
    {
        switch (sss.SelectedPanel)
        {
            case 0:
                player.clip = click;
                break;
            case 1:
                player.clip = zoom;
                break;
            case 2:
                player.clip = click;
                break;
        }
    }

    public void CheckNavs()
    {
        if (sss.CenteredPanel <= 0)
        {
            btn_Prev.gameObject.SetActive(false);
        }
        else
        {
            btn_Prev.gameObject.SetActive(true);
        }
        if(sss.CenteredPanel >= sss.NumberOfPanels-1)
        {
            btn_Next.gameObject.SetActive(false);
            btn_Done.gameObject.SetActive(true);
        }
        else
        {
            btn_Next.gameObject.SetActive(true);
            btn_Done.gameObject.SetActive(false);
        }
    }

    public void Done() => done?.Invoke();
}

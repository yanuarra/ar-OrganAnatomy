using LottiePlugin;
using LottiePlugin.UI;
using UnityEngine;

public class LoadingHandler : MonoBehaviour
{
    public static LoadingHandler Instance;
    [SerializeField] private GameObject overlay;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimatedImage _animatedImage;
    [SerializeField] private LottieAnimation _lottieAnimation;
    [SerializeField] private GameObject _BG;
    [SerializeField] private GameObject _loadingPanel;
    private bool _animationEnabled = true;
    private Vector2 origin;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        _animatedImage = GetComponentInChildren<AnimatedImage>();
    }
    private void Start()
    { 
        origin = _loadingPanel.transform.position;    
    }

    internal void DoUpdate()
    {
        if (!_animationEnabled)
        {
            return;
        }
        _lottieAnimation?.Update();
    }

    public void ToggleLoadingPanelAnatomySelect(bool _state)
    {
        _BG.SetActive(!_state);
        float ydifscreen = _state ? _loadingPanel.transform.position.y + (Screen.height/4): origin.y;
        _loadingPanel.transform.position = new Vector3(
            _loadingPanel.transform.position.x,
            ydifscreen, 
            _loadingPanel.transform.position.z);
        ToggleLoadingPanel(_state);
    }

    public void ToggleLoadingPanel(bool _state)
    {
        overlay?.SetActive(_state);
        DoUpdate();
        //canvasGroup.interactable = !_state;
        canvasGroup.blocksRaycasts = !_state;
        if (_state)
        {
            _animatedImage?.Play();
        }
        else
        {
            _animatedImage?.Stop();
        }
    }
}

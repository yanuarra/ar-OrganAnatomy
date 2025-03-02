//using LottiePlugin.UI;
//using UnityEngine;
//using UnityEngine.UI;

//public class LottieAnimationController : MonoBehaviour
//{
//    [SerializeField] private AnimatedImage animatedImage;
//    internal RectTransform RectTransform => _rectTransform;

//    [SerializeField] private RectTransform _rectTransform;
//    [SerializeField] private RawImage _animationPreview;
//    [SerializeField] private TextAsset _animationJsonData;
//    [SerializeField] private uint _textureWidth;
//    [SerializeField] private uint _textureHeight;
//    private LottiePlugin.LottieAnimation _lottieAnimation;
//    [SerializeField] private bool _animationEnabled = true;

//    private void Awake()
//    {
//        animatedImage = GetComponent<AnimatedImage>();
//    }

//    private void Start()
//    {
//        InitFromData(_textureWidth, _textureHeight);
//    }

//    private void OnEnable()
//    {
//        if (animatedImage == null)
//            animatedImage = GetComponent<AnimatedImage>();
//        //InitFromData(_textureWidth, _textureHeight);
//        //EnableAnimation();
//        //DoUpdate();
//        //_lottieAnimation.Play();
//        animatedImage.Play(); // Start the animationv
//    }

//    private void OnDisable()
//    {
//        if (animatedImage == null)
//            animatedImage = GetComponent<AnimatedImage>();
//        //DisableAnimation();
//        //_lottieAnimation.Stop();
//        animatedImage.Stop(); // Stop the animation
//    }

//    internal void InitFromFile(string jsonFilePath, uint width, uint height)
//    {
//        _lottieAnimation = LottiePlugin.LottieAnimation.LoadFromJsonFile(jsonFilePath, width, height);
//        _animationPreview.texture = _lottieAnimation.Texture;
//        DoUpdate();
//    }
    
//    internal void InitFromData(uint width, uint height)
//    {
//        if (_animationJsonData == null || string.IsNullOrWhiteSpace(_animationJsonData.text))
//        {
//            Debug.LogError($"Can not initialize {nameof(LottieAnimationController)} from null or empty jsong file");
//            return;
//        }
//        _lottieAnimation = LottiePlugin.LottieAnimation.LoadFromJsonData(_animationJsonData.text, string.Empty, width, height);
//        _animationPreview.texture = _lottieAnimation.Texture;
//        DoUpdate();
//    }

//    public void Dispose()
//    {
//        _lottieAnimation.Dispose();
//        Destroy(gameObject);
//    }

//    internal void DoUpdate()
//    {
//        if (!_animationEnabled)
//        {
//            return;
//        }
//        _lottieAnimation.Update();
//        Debug.Log(string.Format("is Playing: {0}", _lottieAnimation.IsPlaying));
//    }

//    internal void DoUpdateAsync()
//    {
//        if (!_animationEnabled)
//        {
//            return;
//        }
//        _lottieAnimation.UpdateAsync();
//    }

//    internal void DoDrawOneFrameAsyncGetResult()
//    {
//        if (!_animationEnabled)
//        {
//            return;
//        }
//        _lottieAnimation.DrawOneFrameAsyncGetResult();
//    }

//    internal void DisableAnimation()
//    {
//        _animationEnabled = false;
//    }

//    internal void EnableAnimation()
//    {
//        _animationEnabled = true;
//    }


//}

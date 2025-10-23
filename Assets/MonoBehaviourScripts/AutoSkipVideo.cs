using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class AutoSkipVideo : MonoBehaviour
{
    [Header("Components")]
    public VideoPlayer videoPlayer;
    public Button skipButton; 

    private bool hasSkipped = false;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.loopPointReached += OnVideoEnd;

        skipButton.onClick.AddListener(ManualSkip);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (!hasSkipped)
        {
            hasSkipped = true;
            skipButton.onClick.Invoke();
        }
    }

    private void ManualSkip()
    {
        if (!hasSkipped)
        {
            hasSkipped = true;
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();
        }
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
        skipButton.onClick.RemoveListener(ManualSkip);
    }
}

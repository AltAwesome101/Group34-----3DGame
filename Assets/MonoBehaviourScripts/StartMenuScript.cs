using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class StartSequence : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer video1;

    [Header("UI Elements")]
    public Canvas canvas; 
    public RawImage display;

    [Header("Player Control")]
    public GameObject playerInputObject;

    private PlayerInputActions inputActions;
    private bool videoFinished = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (playerInputObject != null)
        {
            playerInputObject.SetActive(false);
        }

        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Start.performed += OnStartPressed;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Start.performed -= OnStartPressed;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        display.texture = null;
        video1.targetTexture.Release();
        display.texture = video1.targetTexture;
        video1.isLooping = false;
        video1.loopPointReached += OnVideo1Finished;
        video1.Play();
    }

    private void OnVideo1Finished(VideoPlayer vp)
    {
        videoFinished = true;
    }

    private void OnStartPressed(InputAction.CallbackContext context)
    {
        if (!videoFinished)
        {
            return;
        }

       
        if (canvas != null)
            Destroy(canvas.gameObject);

        if (playerInputObject != null)
        {
            playerInputObject.SetActive(true);
        }
    }
}

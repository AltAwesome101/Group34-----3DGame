using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelpScreenManager : MonoBehaviour
{
    public RawImage helpImage; 
    private bool isHelpVisible = false;

    void Start()
    {
        if (helpImage != null)
            helpImage.gameObject.SetActive(false);
    }

    public void ToggleHelpScreen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isHelpVisible = !isHelpVisible;
        helpImage.gameObject.SetActive(isHelpVisible);
        Time.timeScale = isHelpVisible ? 0f : 1f;
    }
}
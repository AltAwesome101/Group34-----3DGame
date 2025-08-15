using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HelpScreenManager : MonoBehaviour
{
    public RawImage helpImage; // Assign in Inspector
    private bool isHelpVisible = false;

    void Start()
    {
        if (helpImage != null)
            helpImage.gameObject.SetActive(false);
    }

    // This gets called when linked in Player Input's Unity Events
    public void ToggleHelpScreen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isHelpVisible = !isHelpVisible;
        helpImage.gameObject.SetActive(isHelpVisible);
        Time.timeScale = isHelpVisible ? 0f : 1f;
    }
}
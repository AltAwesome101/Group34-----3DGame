using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapScreenManager : MonoBehaviour
{
    public RawImage mapImage;

    private bool isMapVisible = false;

    void Start()
    {
        if (mapImage != null)
        {
            mapImage.gameObject.SetActive(false);
        }
    }

    public void ToggleMapScreen(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        isMapVisible = !isMapVisible;
        mapImage.gameObject.SetActive(isMapVisible);
        Time.timeScale = isMapVisible ? 0f : 1f;
    }
}

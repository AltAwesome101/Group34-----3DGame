using UnityEngine;

public class QuitPanelCloser : MonoBehaviour
{
    public GameObject Panel;

    public void OpenPanel()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }
}

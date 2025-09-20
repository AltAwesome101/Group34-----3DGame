using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("LorePt2");
    }
}

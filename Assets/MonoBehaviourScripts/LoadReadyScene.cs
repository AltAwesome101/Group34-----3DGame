using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadReadyScene : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level2");
    }
}

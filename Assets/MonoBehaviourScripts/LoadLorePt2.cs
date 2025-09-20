using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLorePt2 : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("ReadyScene");
    }
}

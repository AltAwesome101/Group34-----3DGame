using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuAudio : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicSource;    
    public AudioClip backgroundMusic;
    public float musicVolume = 0.7f;

    private void Start()
    {
        if (musicSource == null) 
        { 
            musicSource = GetComponent<AudioSource>(); 
        }
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
}

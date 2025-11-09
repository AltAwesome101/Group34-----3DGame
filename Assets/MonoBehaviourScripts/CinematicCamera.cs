//Title: Intuitive Guide
//Author: Bhavick Nagar
//Date: 20-11-2023
//Code Version: New-input System
//Availability: https://indieklem.com/9-creating-an-intuitive-in-game-menu/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CinematicIntro : MonoBehaviour
{
    [Header("Cinematic Settings")]
    public Transform[] viewpoints; 
    public float moveDuration = 3f;
    public float pauseDuration = 2f;
    public float rotationAmount = 10f; 

    [Header("UI Elements")]
    public TMP_Text descriptionText; 
    public Button skipButton;
    [TextArea(2, 5)] public string[] locationDescriptions; 
    public string nextSceneName = "MainGameScene"; 

    [Header("Audio")]
    public AudioSource backgroundMusic; 

    private bool isSkipping = false;

    private void Start()
    {
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipCinematic);

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
            backgroundMusic.Play();

        StartCoroutine(PlayCinematic());
    }

    private IEnumerator PlayCinematic()
    {
        for (int i = 0; i < viewpoints.Length; i++)
        {
            if (isSkipping) break;

            Transform target = viewpoints[i];
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;

            float timer = 0f;

            
            while (timer < moveDuration)
            {
                if (isSkipping) yield break;

                timer += Time.deltaTime;
                float t = timer / moveDuration;

                transform.position = Vector3.Lerp(startPos, endPos, t);

                
                Quaternion extraRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 0.5f) * rotationAmount, 0);
                transform.rotation = Quaternion.Slerp(startRot, endRot * extraRotation, t);

                yield return null;
            }

            transform.position = endPos;
            transform.rotation = endRot;

            if (descriptionText != null && i < locationDescriptions.Length)
                descriptionText.text = locationDescriptions[i];

            yield return new WaitForSeconds(pauseDuration);
        }

        if (!isSkipping)
            LoadNextScene();
    }

    public void SkipCinematic()
    {
        isSkipping = true;
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        SceneManager.LoadScene(nextSceneName);
    }
}

//Title: Designing Video Game Puzzles
//Author: Damien Allan
//Date: 29-10-2023
//Code Version: New-input System
//Availability: https://www.gamedeveloper.com/design/designing-video-game-puzzles

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class CandlePuzzleManager : MonoBehaviour
{
    [Header("Puzzle")]
    [Tooltip("Order of candle IDs the player must light (index 0 = first)")]
    public List<int> correctOrder = new List<int>();

    [Header("Outputs")]
    public UnlockableDoor doorToUnlock;         
    public TextMeshProUGUI promptText;         

    [Header("Audio")]
    public AudioClip successClip;
    public AudioClip errorClip;

    [Header("Timing")]
    public float resetDelay = 1.0f;             

    private List<int> currentSequence = new List<int>();

    private AudioSource audioSource;

    private CandleInteractable[] allCandles;

    private bool puzzleSolved = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        allCandles = FindObjectsOfType<CandleInteractable>();

        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    public void TryLight(CandleInteractable candle)
    {
        if (puzzleSolved) return;
        if (candle == null) return;
        if (candle.IsLit) return;

        currentSequence.Add(candle.candleID);
        candle.ApplyLitState(true, playFX: true);

        if (promptText != null)
        {
            promptText.text = $"Lit {currentSequence.Count}/{correctOrder.Count}";
            promptText.gameObject.SetActive(true);
        }

        for (int i = 0; i < currentSequence.Count; i++)
        {
      
            if (i >= correctOrder.Count || currentSequence[i] != correctOrder[i])
            {
                StartCoroutine(ResetSequence());
                return;
            }
        }

        if (currentSequence.Count == correctOrder.Count)
        {
            PuzzleSolved();
        }
    }

    private void PuzzleSolved()
    {
        puzzleSolved = true;
        if (audioSource != null && successClip != null) audioSource.PlayOneShot(successClip);

        if (promptText != null)
        {
            promptText.text = "You hear a mechanism unlock...";
            StartCoroutine(HidePromptAfter(3f));
        }

        if (doorToUnlock != null)
            doorToUnlock.Unlock();
    }

    private IEnumerator ResetSequence()
    {
      
        if (audioSource != null && errorClip != null) audioSource.PlayOneShot(errorClip);

        if (promptText != null)
        {
            promptText.text = "That's not right...";
            promptText.gameObject.SetActive(true);
        }

       
        yield return new WaitForSeconds(resetDelay);

      
        foreach (var c in allCandles)
        {
            if (c != null && c.IsLit)
                c.ApplyLitState(false, playFX: false);
        }

        currentSequence.Clear();

        if (promptText != null)
            StartCoroutine(HidePromptAfter(0.5f));
    }

    private IEnumerator HidePromptAfter(float secs)
    {
        if (promptText == null) yield break;
        yield return new WaitForSeconds(secs);
        promptText.gameObject.SetActive(false);
    }
}

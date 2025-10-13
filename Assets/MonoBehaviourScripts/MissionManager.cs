using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI")]
    public TextMeshProUGUI missionText;
    public float fadeDuration = 1f;
    public float visibleDuration = 3f;
    public float repeatDelay = 60f;

    [Header("Missions")]
    private Dictionary<string, bool> missions = new Dictionary<string, bool>();

    private Coroutine displayRoutine;
    private string currentMission;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        missions.Add("Find the Gas Valve", false);
        missions.Add("Investigate your room", false);
        missions.Add("Repair the generator", false);
        missions.Add("Find your brother", false);
    }

    private void Start()
    {
        if (missionText)
            missionText.alpha = 0f; 

        ShowNextMission();
    }

    public void CompleteMission(string missionName)
    {
        if (missions.ContainsKey(missionName))
        {
            missions[missionName] = true;

            
            if (missionName == currentMission)
            {
                ShowNextMission();
            }
        }
    }

    private void ShowNextMission()
    {
        
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        currentMission = null;
        foreach (var kvp in missions)
        {
            if (!kvp.Value)
            {
                currentMission = kvp.Key;
                break;
            }
        }

        if (currentMission != null)
            displayRoutine = StartCoroutine(DisplayMissionRoutine(currentMission));
        else if (missionText)
            missionText.text = ""; 
    }

    private IEnumerator DisplayMissionRoutine(string missionName)
    {
        while (true)
        {
            
            if (missionText)
            {
                missionText.text = missionName;
                yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));
            }

          
            yield return new WaitForSeconds(visibleDuration);

        
            if (missionText)
                yield return StartCoroutine(FadeText(1f, 0f, fadeDuration));

            
            yield return new WaitForSeconds(repeatDelay);

          
            if (missions[missionName])
                yield break;
        }
    }

    private IEnumerator FadeText(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            missionText.alpha = a;
            yield return null;
        }
        missionText.alpha = to;
    }
}

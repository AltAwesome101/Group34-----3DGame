using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [Header("UI")]
    public TextMeshProUGUI missionText;

    [Header("Missions")]
    private Dictionary<string, bool> missions = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        missions.Add("Find the Gas Valve", false);
        missions.Add("Investigate your room", false);
        missions.Add("Repair the generator", false);
        missions.Add("Find your brother", false);

        UpdateMissionText();
    }

    public void CompleteMission(string missionName)
    {
        if (missions.ContainsKey(missionName))
        {
            missions[missionName] = true;
            UpdateMissionText();
        }
    }

    private void UpdateMissionText()
    {
        if (!missionText) return;

        string text = "Primary Objectives:\n";
        foreach (var kvp in missions)
        {
            if (!kvp.Value)
                text += "• " + kvp.Key + "\n";
        }
        missionText.text = text.TrimEnd('\n');
    }
}

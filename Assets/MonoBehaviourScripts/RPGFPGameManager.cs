using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Spawners
{
    public GameObject go;
    public bool active;

    public Spawners(GameObject newGo, bool newBool)
    {
        go = newGo;
        active = newBool;
    }
}

public class RPGFPGameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panel;
    public GameObject settingsPanel;
    public TextMeshProUGUI panelMessageText;
    public int totalKeysRequired = 3;

    [Header("UI Elements")]
    public TextMeshProUGUI roundText;
    public Button tryAgainButton;
    public Button settingsButton;
    public Button quitButton;
    public Button continueButton;
    public Button firstSelectedButton;

    public delegate void RestartRounds();
    public static event RestartRounds RoundComplete;

    public int health;
    private int currentRound = 1;
    private DamagePlayer playerDamage;
    public List<Spawners> spawner = new List<Spawners>();

    private PlayerInputActions inputActions;
    private bool isPaused = false;
    private bool isGameOver = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void Start()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<DamagePlayer>();

        if (roundText != null)
            roundText.text = $"Round: {currentRound}";

        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name.Contains("Spawner"))
            {
                spawner.Add(new Spawners(go, true));
            }
        }

        if (tryAgainButton != null)
            tryAgainButton.onClick.AddListener(OnTryAgain);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinue);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }

        health = playerDamage.health;

        if (health <= 0 && !isGameOver)
        {
            isGameOver = true;
            ShowPanel($"Survived {currentRound - 1} Rounds", showContinue: false);
            return;
        }

        // Check if round is complete
        if (!isGameOver && AllSpawnersDead())
        {
            StartNextRound();
        }
    }

    private void StartNextRound()
    {
        currentRound++;

        // Update UI
        if (roundText != null)
            roundText.text = $"Round: {currentRound}";

        // Double enemy count for each spawner
        foreach (var s in spawner)
        {
            var spawnerScript = s.go.GetComponent<EnemySpawner>();

            spawnerScript.amount *= 2; // double enemies
            spawnerScript.ResetRound(); // reset and resize pool
        }


        RoundComplete?.Invoke();
    }

    private void TogglePauseMenu()
    {
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            ShowPanel("Quit", showContinue: true);
        }
        else
        {
            HidePanel();
        }
    }

    private void ShowPanel(string message, bool showContinue)
    {
        panel.SetActive(true);
        var panelText = panel.GetComponentInChildren<TextMeshProUGUI>();
        if (panelText != null)
            panelText.text = message;
        if (panelMessageText != null)
            panelMessageText.text = message;

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (continueButton != null)
            continueButton.gameObject.SetActive(showContinue);
        if (tryAgainButton != null)
            tryAgainButton.gameObject.SetActive(!showContinue);
        if (settingsButton != null)
            settingsButton.gameObject.SetActive(true);
        if (quitButton != null)
            quitButton.gameObject.SetActive(true);

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    private void HidePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private bool AllSpawnersDead()
    {
        foreach (var s in spawner)
        {
            EnemySpawner es = s.go.GetComponent<EnemySpawner>();
            if (!es.spawnsDead)
                return false;
        }
        return true;
    }

    private void OnTryAgain()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    private void OnSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnContinue()
    {
        if (isPaused)
        {
            isPaused = false;
            HidePanel();
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}



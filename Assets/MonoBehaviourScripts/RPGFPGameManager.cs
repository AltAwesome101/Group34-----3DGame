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
    private int roundsSurvived;
    private int currentRound;
    private DamagePlayer playerDamage;
    private TextMeshProUGUI panelText;
    public List<Spawners> spawner = new List<Spawners>();

    private PlayerInputActions inputActions;
    private bool continuePressed = false;
    private bool waitingForNextRound = false;
    private bool isPaused = false;
    private bool isGameOver = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Shoot.performed += ctx => continuePressed = true;
    }

    void Start()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<DamagePlayer>();
        panelText = panel.GetComponentInChildren<TextMeshProUGUI>();

        if (roundText != null)
            roundText.text = $"Round: {roundsSurvived + 1}";

        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name.Contains("Spawner"))
            {
                spawner.Add(new Spawners(go, true));
            }
        }

        // Button listeners
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
            ShowPanel($"Survived {roundsSurvived} Rounds", showContinue: false);
            return;
        }

        if (AllSpawnersDead() && !waitingForNextRound && !isGameOver)
        {
            waitingForNextRound = true;
            roundsSurvived++;
            ShowPanel($"Round {roundsSurvived} Finished!", showContinue: true);
        }

        if (waitingForNextRound && continuePressed && !isPaused)
        {
            continuePressed = false;
            currentRound = roundsSurvived;
            waitingForNextRound = false;
            panel.SetActive(false);
            Time.timeScale = 1;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            foreach (var s in spawner)
            {
                var spawnerScript = s.go.GetComponent<EnemySpawner>();
                spawnerScript.amount += 1;
            }

            if (roundText != null)
                roundText.text = $"Round: {currentRound + 1}";

            RoundComplete?.Invoke();
        }
    }

    private void TogglePauseMenu()
    {
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            ShowPanel("Game Paused", showContinue: true);
        }
        else
        {
            HidePanel();
        }
    }

    private void ShowPanel(string message, bool showContinue)
    {
        panel.SetActive(true);
        panelText.text = message;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Toggle button visibility
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
            if (es.spawnsDead == false)
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

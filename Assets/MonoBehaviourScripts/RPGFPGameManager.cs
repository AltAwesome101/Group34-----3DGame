//Title: Wave System for Enemy Spawning
//Author: Phiktional
//Date: 03-08-2025
//Code Version: New-input System
//Availability: https://medium.com/@phiktional/implementing-a-wave-system-for-enemy-spawning-in-unity-ebf820e7a936

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RPGFPGameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject panel;               // Pause / game-over panel
    public GameObject settingsPanel;       // Settings sits on top of pause
    public TextMeshProUGUI panelMessageText;

    [Header("Key Settings")]
    [Tooltip("How many keys the player must collect before the NPC gives the first reward.")]
    public int totalKeysRequired = 3;

    [Header("UI Elements")]
    public Button tryAgainButton;
    public Button settingsButton;
    public Button quitButton;
    public Button continueButton;
    public Button firstSelectedButton;
    public Slider mouseSensitivitySlider;
    public Slider headBobSlider;

    public int health = 100;

    private DamagePlayer playerDamage;
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

        playerDamage = GameObject.FindGameObjectWithTag("Player")
                                 ?.GetComponent<DamagePlayer>();

        // Button listeners
        if (tryAgainButton) tryAgainButton.onClick.AddListener(OnTryAgain);
        if (settingsButton) settingsButton.onClick.AddListener(OnSettings);
        if (quitButton) quitButton.onClick.AddListener(OnQuit);
        if (continueButton) continueButton.onClick.AddListener(OnContinue);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePauseMenu();

        if (playerDamage != null)
            health = playerDamage.health;

        if (health <= 0 && !isGameOver)
        {
            isGameOver = true;
            ShowPanel("Game Over", showContinue: false);
        }
    }

    private void TogglePauseMenu()
    {
        if (isGameOver) return;
        isPaused = !isPaused;

        if (isPaused) ShowPanel("Paused", showContinue: true);
        else HidePanel();
    }

    private void ShowPanel(string message, bool showContinue)
    {
        panel.SetActive(true);
        if (panelMessageText) panelMessageText.text = message;

        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (continueButton) continueButton.gameObject.SetActive(showContinue);
        if (tryAgainButton) tryAgainButton.gameObject.SetActive(!showContinue);
        if (settingsButton) settingsButton.gameObject.SetActive(true);
        if (quitButton) quitButton.gameObject.SetActive(true);
        if (firstSelectedButton) EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    private void HidePanel()
    {
        panel.SetActive(false);
        settingsPanel?.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTryAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSettings()
    {
        if (settingsPanel)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
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

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
}



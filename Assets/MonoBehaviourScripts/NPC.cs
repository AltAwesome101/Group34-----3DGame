using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public GameObject interactPromptUI;
    public TextMeshProUGUI dialogueText;
    public float interactionPauseTime = 3f;

    private ShootingScript shootingScript;
    private int questStage = 0;
    private int meleeKills = 0;
    public int requiredMeleeKills = 10;
    private bool playerInRange = false;
    private PlayerInputActions inputActions;
    private RPGFPGameManager gameManager;
    private InventoryManager inventory;

    public void RegisterMeleeKill()
    {
        if (questStage == 1)
            meleeKills++;
    }

    void Awake()
    {
        inputActions = new PlayerInputActions();
        shootingScript = FindObjectOfType<ShootingScript>();
        inventory = FindObjectOfType<InventoryManager>();
        gameManager = FindObjectOfType<RPGFPGameManager>();
    }

    void OnEnable()
    {
        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Player.Interact.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Player.Interact.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!playerInRange || shootingScript == null) return;
        StartCoroutine(HandleInteraction());
    }

    private IEnumerator HandleInteraction()
    {
        Time.timeScale = 0f;
        string msg = GetDynamicMessage();
        if (dialogueText != null) dialogueText.text = msg;
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        yield return new WaitForSecondsRealtime(interactionPauseTime);
        dialogueText.text = "";

        yield return StartCoroutine(Countdown());
        Time.timeScale = 1f;
    }

    private IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            dialogueText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }
        dialogueText.text = "Start!";
        yield return new WaitForSecondsRealtime(0.5f);
        dialogueText.text = "";
    }

    private string GetDynamicMessage()
    {
        int keys = inventory != null ? inventory.keys : 0;
        int totalKeys = gameManager != null ? gameManager.totalKeysRequired : 0;

        if (questStage == 0 && keys >= totalKeys)
        {
            questStage = 1;
            shootingScript.currentGun = ShootingScript.GunType.DualShot;
            return "Well done! Here’s a Unique Gun!";
        }
        else if (questStage == 1 && meleeKills >= requiredMeleeKills)
        {
            questStage = 2;
            shootingScript.currentGun = ShootingScript.GunType.Shotgun;
            return "You’re a melee master! Enjoy your final weapon.";
        }
        else if (questStage == 0)
        {
            return $"Collect {totalKeys} keys.\n(You Have: {keys})";
        }
        else if (questStage == 1)
        {
            return $"Kill {requiredMeleeKills} enemies by melee.\n(You Have: {meleeKills})";
        }
        else
        {
            return "All tasks complete. Use your mighty shotgun!";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
            if (dialogueText != null) dialogueText.text = "";
        }
    }
}

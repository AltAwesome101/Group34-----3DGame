using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class NPC : MonoBehaviour
{
    [Header("UI References")]
    public GameObject interactPromptUI;
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue Settings")]
    public float interactionPauseTime = 3f;
    public float fadeDuration = 1f; 

    private ShootingScript shootingScript;
    private int questStage = 0;
    private int meleeKills = 0;
    public int requiredMeleeKills = 10;

    private bool playerInRange = false;
    private PlayerInputActions inputActions;
    private RPGFPGameManager gameManager;
    private InventoryManager inventory;

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

    public void RegisterMeleeKill()
    {
        if (questStage == 1)
            meleeKills++;
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
        float displayTime = GetDisplayTimeForMessage(msg);

        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);

        yield return StartCoroutine(FadeText(msg, 0f, 1f));
        yield return new WaitForSecondsRealtime(displayTime);
        yield return StartCoroutine(FadeText("", 1f, 0f));
        yield return StartCoroutine(Countdown());
        Time.timeScale = 1f;
    }

    private IEnumerator FadeText(string newText, float fromAlpha, float toAlpha)
    {
        if (dialogueText == null) yield break;

        if (!string.IsNullOrEmpty(newText))
            dialogueText.text = newText;

        Color c = dialogueText.color;
        c.a = fromAlpha;
        dialogueText.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, timer / fadeDuration);
            dialogueText.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

      
        dialogueText.color = new Color(c.r, c.g, c.b, toAlpha);

       
        if (toAlpha == 0f)
            dialogueText.text = "";
    }

    private float GetDisplayTimeForMessage(string message)
    {
        if (message.Contains("A Spirit has been awakened")) return 10f;  
        if (message.Contains("melee master")) return 5f;           
        return interactionPauseTime;                               
    }

    private IEnumerator Countdown()
    {
        for (int i = 3; i > 0; i--)
        {
            dialogueText.text = i.ToString();
            dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 1f);
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
            return "Well done! Here’s a Dual Submachine Gun!";
        }
        else if (questStage == 1 && meleeKills >= requiredMeleeKills)
        {
            questStage = 2;
            shootingScript.currentGun = ShootingScript.GunType.Shotgun;
            return "You’re a melee master! Enjoy your ShotGun.";
        }
        else if (questStage == 0)
        {
            return $"A Spirit has been awakened and she is looking for ways to build her stregth. " +
                   $"Find your brother and defeat her before it is too late. Be careful, her spiritual energy has already invaded the house. " +
                   $"Collect {totalKeys} keys.\n(You Currently Have: {keys}) and return back to me. I might have something to help you on your Journey.";
        }
        else if (questStage == 1)
        {
            return $"Kill {requiredMeleeKills} enemies by melee.\n(You Currently Have: {meleeKills})";
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
            if (interactPromptUI != null)
                interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            if (dialogueText != null)
                dialogueText.text = "";
        }
    }
}

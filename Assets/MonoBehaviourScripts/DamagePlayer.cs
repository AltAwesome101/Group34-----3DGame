using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; 

[RequireComponent(typeof(AudioSource))]
public class DamagePlayer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI healthPanel;
    public GameObject roundCompletePanel;

    [Header("Health Settings")]
    public int health = 100;
    public int maxHealth = 100;
    public Transform respawnPoint;
    public float fallThreshold = -7f;

    [Header("Damage Feedback")]
    public Image damageFlashImage;
    public float flashDuration = 0.3f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.6f);

    [Header("Camera Shake")]
    public Camera playerCamera;
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;

    [Header("Low Health Effects")]
    public AudioClip heartbeatSound;
    public float lowHealthThreshold = 30f;
    public float heartbeatInterval = 1.2f;

    public Volume postProcessingVolume;
    private DepthOfField depthOfField;

    [Header("Audio")]
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public float hurtSoundCooldown = 0.4f; 

    private AudioSource audioSource;

    private Rigidbody rb;

    private bool isDead = false;

    private bool isLowHealthActive = false;

    private Coroutine flashRoutine;

    private Coroutine shakeRoutine;

    private Coroutine heartbeatRoutine;

    private float lastHurtTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out depthOfField);
        }

        ApplyDamage(0);
        HidePanel();
    }

    private void Update()
    {
        if (!isDead && transform.position.y < fallThreshold)
            Die();

        if (!isDead && health <= lowHealthThreshold && !isLowHealthActive)
        {
            isLowHealthActive = true;
            heartbeatRoutine = StartCoroutine(LowHealthEffects());
        }
        else if (health > lowHealthThreshold && isLowHealthActive)
        {
            StopCoroutine(heartbeatRoutine);
            isLowHealthActive = false;
            ResetLowHealthVisuals();
        }
    }

    // DAMAGE AND HEALING
   
    public void ApplyDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health < 0) health = 0;
        UpdateUI();

        if (damage > 0)
        {
            if (hurtSound != null && Time.time - lastHurtTime >= hurtSoundCooldown)
            {
                lastHurtTime = Time.time;
                audioSource.clip = hurtSound;
                audioSource.spatialBlend = 0f;
                audioSource.Play();
            }

            TriggerDamageFlash();

            if (playerCamera != null)
            {
                if (shakeRoutine != null)
                    StopCoroutine(shakeRoutine);
                shakeRoutine = StartCoroutine(CameraShake());
            }
        }

        if (health == 0)
            Die();
    }

    public void AddHealth(int amount)
    {
        if (isDead) return;

        health += amount;
        if (health > maxHealth) health = maxHealth;
        UpdateUI();
    }

    // DEATH AND RESPAWN
 
    void Die()
    {
        isDead = true;

        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
        }
        ResetLowHealthVisuals();

        if (damageFlashImage != null)
        {
            damageFlashImage.gameObject.SetActive(false);
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        ShowPanel();
        Invoke(nameof(Respawn), 3f);
    }

    void Respawn()
    {
        isDead = false;
        health = 50;
        UpdateUI();
        transform.position = respawnPoint.position;
        HidePanel();

        if (damageFlashImage != null)
            damageFlashImage.gameObject.SetActive(true); 
    }

    // UI HANDLING
  
    void UpdateUI()
    {
        if (healthPanel != null)
        {
            healthPanel.text = "Health: " + health;
        }
    }

    void ShowPanel()
    {
        if (roundCompletePanel != null)
        {
            roundCompletePanel.SetActive(true);
        }
    }

    void HidePanel()
    {
        if (roundCompletePanel != null)
        {
            roundCompletePanel.SetActive(false);
        }
    }

    // DAMAGE FLASH
    void TriggerDamageFlash()
    {
        if (damageFlashImage == null)
        {
            return;
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    IEnumerator DamageFlashRoutine()
    {
        damageFlashImage.color = flashColor;
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            damageFlashImage.color = Color.Lerp(flashColor, Color.clear, elapsed / flashDuration);
            yield return null;
        }
        damageFlashImage.color = Color.clear;
    }

    // CAMERA SHAKE
  
    IEnumerator CameraShake()
    {
        Vector3 originalPos = playerCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            playerCamera.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalPos;
    }

    // LOW HEALTH EFFECTS
   
    IEnumerator LowHealthEffects()
    {
        while (true)
        {

            if (heartbeatSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(heartbeatSound);
            }

            if (damageFlashImage != null)
            {
                damageFlashImage.color = new Color(1f, 0f, 0f, 0.3f);
                yield return new WaitForSeconds(0.1f);
                damageFlashImage.color = Color.clear;
            }


            if (depthOfField != null)
            {
                depthOfField.gaussianStart.Override(0.5f);
            }

            yield return new WaitForSeconds(heartbeatInterval);

            if (depthOfField != null)
            {
                depthOfField.gaussianStart.Override(0f);
            }
        }
    }

    void ResetLowHealthVisuals()
    {
        if (damageFlashImage != null)
        {
            damageFlashImage.color = Color.clear;
        }

        if (depthOfField != null)
        {
            depthOfField.gaussianStart.Override(0f);
        }
    }

    public void OnWaveComplete()
    {
        ShowPanel();
    }
}

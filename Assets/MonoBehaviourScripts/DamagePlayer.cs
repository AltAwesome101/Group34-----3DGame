using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePlayer : MonoBehaviour
{
    public TextMeshProUGUI healthPanel;
    public GameObject roundCompletePanel; // Panel to show on death or wave end
    public int health = 100;
    public int maxHealth = 100;
    public Transform respawnPoint;
    public float fallThreshold = -7f;

    private Rigidbody rb;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ApplyDamage(0);
        HidePanel(); // Hide panel at start
    }

    private void Update()
    {
        if (!isDead && transform.position.y < fallThreshold)
        {
            Die();
        }
    }

    public void ApplyDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        if (health < 0) health = 0;
        UpdateUI();

        if (health == 0)
        {
            Die();
        }
    }

    public void AddHealth(int amount)
    {
        if (isDead) return;

        health += amount;
        if (health > maxHealth) health = maxHealth;
        UpdateUI();
    }

    void Die()
    {
        isDead = true;
        ShowPanel(); // Show panel before respawn
        Invoke("Respawn", 3f); // Delay respawn
    }

    void Respawn()
    {
        isDead = false;
        health = 50;
        UpdateUI();
        transform.position = respawnPoint.position;
        HidePanel(); // Hide panel after respawn
    }

    void UpdateUI()
    {
        if (healthPanel != null)
            healthPanel.text = "Health: " + health.ToString();
    }

    void ShowPanel()
    {
        if (roundCompletePanel != null)
            roundCompletePanel.SetActive(true);
    }

    void HidePanel()
    {
        if (roundCompletePanel != null)
            roundCompletePanel.SetActive(false);
    }

    // Optional: Call this from your round manager to show the panel
    public void OnWaveComplete()
    {
        ShowPanel();
    }
}
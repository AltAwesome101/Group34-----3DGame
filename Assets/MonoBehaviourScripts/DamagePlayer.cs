using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamagePlayer : MonoBehaviour
{
    public TextMeshProUGUI healthPanel;

    public GameObject roundCompletePanel;
    
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
        HidePanel();
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
        ShowPanel(); 
        Invoke("Respawn", 3f); 
    }

    void Respawn()
    {
        isDead = false;
        health = 50;
        UpdateUI();
        transform.position = respawnPoint.position;
        HidePanel();
    }

    void UpdateUI()
    {
        if (healthPanel != null)
        {
            healthPanel.text = "Health: " + health.ToString();
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

    public void OnWaveComplete()
    {
        ShowPanel();
    }
}
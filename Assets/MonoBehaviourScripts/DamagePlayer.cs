using TMPro;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    public TextMeshProUGUI healthPanel;
    public int health = 100;
    public int maxHealth = 100;
    public Transform respawnPoint;
    private CharacterController controller;
    public float fallThreshold = -7f; 
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        ApplyDamage(0);
    }

    private void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    public void ApplyDamage(int damage)
    {
        if (health > 0)
        {
            health -= damage;
            if (health < 0) health = 0;
            UpdateUI();

            if (health == 0)
            {
                Respawn();
            }
        }
    }

    public void AddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthPanel != null)
            healthPanel.text = "Health: " + health.ToString();
    }

    void Respawn()
    {
        health = 50;
        UpdateUI();

        if (controller != null)
        { 
            controller.enabled = false; 
        }
        transform.position = respawnPoint.position;

        if (controller != null)
        { 
            controller.enabled = true;
        }
    }
}

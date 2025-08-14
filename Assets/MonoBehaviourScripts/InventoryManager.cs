using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public int ammo = 30;
    public int keys = 0;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI keyText;

    void Start()
    {
        UpdateUI();
    }


    public void AddAmmo(int amount)
    {
        ammo += amount;
        UpdateUI();
    }

    public void UseAmmo(int amount)
    {
        ammo = Mathf.Max(0, ammo - amount);
        UpdateUI();
    }

    public void AddKey()
    {
        keys += 1;
        UpdateUI();
    }

    public void UseKey()
    {
        if (keys > 0) keys--;
        UpdateUI();
    }

    void UpdateUI()
    {
        ammoText.text = "Ammo: " + ammo.ToString();
        keyText.text = "Keys: " + keys.ToString();
    }
}
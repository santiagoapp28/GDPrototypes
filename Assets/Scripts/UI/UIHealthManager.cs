using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthManager : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public Image healthBarImage;

    private void Start()
    {
        // Initialize the UI with the starting health values from the GameManager
        if (GameManager.Instance != null)
        {
            UpdateHealth(GameManager.Instance.health, GameManager.Instance.maxHealth);
        }
    }

    public void UpdateHealth(int health, int maxHealth)
    {
        if (healthText != null)
            healthText.text = health + " / " + maxHealth;
        if (healthBarImage != null)
            healthBarImage.fillAmount = (float)health / maxHealth;
    }
}
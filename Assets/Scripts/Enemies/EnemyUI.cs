using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    public Image healthBar;

    public void UpdateHealthBar(float healthPercentage)
    {
        healthBar.fillAmount = healthPercentage;
    }
}

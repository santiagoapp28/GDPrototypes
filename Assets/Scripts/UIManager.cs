using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    public void UpdateCoins(int coins)
    {
        coinsText.text = coins.ToString();
    }
}

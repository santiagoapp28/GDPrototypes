using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image background;
    public GameObject segmentPrefab;

    public void Initialize(Card card)
    {
        icon.sprite = card.icon;
        nameText.text = card.cardName;
        descriptionText.text = card.description;
        background.color = card.color;
        segmentPrefab = card.segmentPrefab;
    }
}

using UnityEngine;

public class DeckViewer : MonoBehaviour
{
    public GameObject deckViewerPanel;
    public Transform contentParent;
    public GameObject cardPrefab;

    private void Start()
    {
        deckViewerPanel.SetActive(false); // start hidden
    }

    public void OpenDeckView()
    {
        ClearDeckDisplay();

        foreach (Card card in DeckManager.Instance.deck)
        {
            GameObject cardObj = Instantiate(cardPrefab, contentParent);
            cardObj.GetComponent<CardUI>().Initialize(card);
        }

        deckViewerPanel.SetActive(true);
    }

    public void CloseDeckView()
    {
        deckViewerPanel.SetActive(false);
    }

    private void ClearDeckDisplay()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
}

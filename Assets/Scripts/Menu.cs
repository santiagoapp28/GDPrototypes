using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject deckSelectorPanel;

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        deckSelectorPanel.SetActive(false);
    }

    public void ShowDeckSelector()
    {
        menuPanel.SetActive(false);
        deckSelectorPanel.SetActive(true);
    }
}

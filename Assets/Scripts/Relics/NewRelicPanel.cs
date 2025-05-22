using UnityEngine;

public class NewRelicPanel : MonoBehaviour
{
    public GameObject relicContainer;
    public ShopRelicSlot[] shopRelicSlots;

    private void Start()
    {
        for (int i = 0; i < shopRelicSlots.Length; i++)
        {
            shopRelicSlots[i].Setup(RelicManager.Instance.availableRelics[
                (Random.Range(0, RelicManager.Instance.availableRelics.Count))]);
            shopRelicSlots[i].relicPrice = 0; // Set the price to 0 for the relics in the reward panel
            shopRelicSlots[i].OnBuyRelic += OnBuyRelic;
        }
    }

    public void OnBuyRelic(Relic newRelic)
    {
        FindAnyObjectByType<RelicUI>().UpdateRelicUI();
        gameObject.SetActive(false);
    }
}

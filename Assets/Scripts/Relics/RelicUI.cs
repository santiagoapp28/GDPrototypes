using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicUI : MonoBehaviour
{
    public GameObject relicPrefab; // Prefab for the relic UI element
    public Dictionary<Relic, GameObject> relics = new Dictionary<Relic, GameObject>(); // Dictionary to hold relics

    public void UpdateRelicUI()
    {
        foreach (var relic in relics)
        {
            Destroy(relic.Value);
        }
        relics.Clear();
        foreach (var relic in RelicManager.Instance.currentRelics)
        {
            if (relics.ContainsKey(relic)) return;

            GameObject relicInstance = Instantiate(relicPrefab, transform); // Instantiate the relic prefab
            relics.Add(relic, relicInstance); // Add the relic to the dictionary

            relicPrefab.GetComponent<Image>().sprite = relic.image; // Set the image of the relic
            relicPrefab.GetComponent<Image>().color = RelicManager.Instance.rarityColor[(int)relic.rarity]; // Set the color based on rarity

        }
    }
}

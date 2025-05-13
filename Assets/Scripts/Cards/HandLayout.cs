using System.Collections.Generic;
using UnityEngine;

public class HandLayout : MonoBehaviour
{
    public float cardSpacing = 160f;
    public float minSpacing = 60f;
    public float maxSpacing = 180f;
    public float maxRotation = 15f; // degrees

    public void RepositionCards()
    {
        List<Transform> cards = new List<Transform>();
        foreach (Transform child in transform)
            cards.Add(child);

        int count = cards.Count;
        if (count == 0) return;

        float spacing = Mathf.Clamp(cardSpacing - (count - 1) * 10f, minSpacing, maxSpacing);
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform card = cards[i];
            float x = startX + i * spacing;
            card.localPosition = new Vector3(x, 0f, 0f);

            // Apply rotation around Z-axis for a fanned look
            float t = (i - (count - 1) / 2f) / (count / 2f); // -1 to 1
            float angle = -t * maxRotation;
            card.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

}

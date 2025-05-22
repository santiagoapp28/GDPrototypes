using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class RelicController : MonoBehaviour
{
    public Dictionary<RelicType, Relic> currentRelics = new Dictionary<RelicType, Relic>();

    public void Initialize()
    {
        foreach (Relic relic in RelicManager.Instance.currentRelics)
        {
            if (!currentRelics.ContainsValue(relic))
            {
                currentRelics.Add(relic.relicType, relic);
            }
        }
        if (currentRelics.ContainsKey(RelicType.ExtraCoins) || currentRelics.ContainsKey(RelicType.ExtraCoinsPlus))
        {
            Enemy.OnEnemyDied += OnEnemyDied;
        }
        if(currentRelics.ContainsKey(RelicType.Handsize))
        {
            FindAnyObjectByType<DeckUI>().startingHandSize++;
        }
        if(currentRelics.ContainsKey(RelicType.HandsizePlus))
        {
            FindAnyObjectByType<DeckUI>().startingHandSize+=2;
        }

        FindAnyObjectByType<RelicUI>()?.UpdateRelicUI();
    }

    private void OnEnemyDied(Enemy enemy)
    {
        if(currentRelics.ContainsKey(RelicType.ExtraCoins))
        {
            GameManager.Instance.UpdateCoins(1);
        }
        if(currentRelics.ContainsKey(RelicType.ExtraCoinsPlus))
        {
            GameManager.Instance.UpdateCoins(2);
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public GameObject towerPrefab;
    public Dictionary<Vector2Int, Tower> towerList = new Dictionary<Vector2Int, Tower>();

    public GameObject commonBulletPrefab;
    public GameObject missileBulletPrefab;
    public GameObject iceBulletPrefab;
    public GameObject fireBulletPrefab;

    public Action<Vector2Int> OnAddedTower;

    public GameObject ChangeBulletType(CardType cardtype)
    {
        switch (cardtype)
        {
            case CardType.Base_Cannon:
                return commonBulletPrefab;
            case CardType.Base_Missile:
                return missileBulletPrefab;
            case CardType.Effect_Ice:
                return iceBulletPrefab;
            case CardType.Effect_Fire:
                return fireBulletPrefab;
            default:
                throw new ArgumentOutOfRangeException(nameof(CardType), cardtype, null);
        }
    }

    public void AddTower(Vector2Int gridPos, Vector3 snappedPos, TowerSegment segment)
    {
        if (towerPrefab == null)
        {
            Debug.LogError("TowerPrefab is not assigned in TowerManager.", this);
            return;
        }

        var obj = Instantiate(towerPrefab, snappedPos, Quaternion.identity);
        towerList.Add(gridPos, obj.GetComponent<Tower>());
        obj.GetComponent<Tower>().AddSegment(segment);
        OnAddedTower(gridPos);
    }
}

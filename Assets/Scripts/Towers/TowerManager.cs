using NUnit.Framework;
using UnityEngine;
using System;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour
{
    public GameObject towerPrefab;
    public Dictionary<Vector2Int, Tower> towerList = new Dictionary<Vector2Int, Tower>();
    public void AddTower(Vector2Int gridPos, Vector3 snappedPos, TowerSegment segment)
    {
        var obj = Instantiate(towerPrefab, snappedPos, Quaternion.identity);
        towerList.Add(gridPos, obj.GetComponent<Tower>());
        obj.GetComponent<Tower>().AddSegment(segment);
    }
}

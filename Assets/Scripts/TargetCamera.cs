using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    private TowerManager _towerManager;
    public int centerFocus = 2; //how much to focus on the center of the grid instead of tower positions

    private void Start()
    {
        _towerManager = FindAnyObjectByType<TowerManager>();
        if (_towerManager != null)
        {
            _towerManager.OnAddedTower += OnAddedTower;
        }
        else
        {
            Debug.LogWarning("TargetCamera could not find a TowerManager in the scene.", this);
        }
    }

    public void OnAddedTower(Vector2Int pos)
    {
        List<Tower> towers = new List<Tower>(_towerManager.towerList.Values);
        List<Vector3> towerPos = new List<Vector3>();
        Vector3 averagePos = Vector3.zero;
        foreach (var tower in towers)
        {
            averagePos += tower.transform.position;
            towerPos.Add(tower.transform.position);
        }
        for (int i = 0; i < centerFocus; i++)
        {
            averagePos += transform.position;
        }
        averagePos /= towers.Count + centerFocus;
        transform.position = averagePos;
    }
}

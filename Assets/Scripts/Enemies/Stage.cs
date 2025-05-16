using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStage", menuName = "Tower Defense/Stage")]
public class Stage : ScriptableObject
{
    [Tooltip("Optional name for this stage (for editor organization).")]
    public string waveName;
    public List<Wave> waves = new List<Wave>();
}
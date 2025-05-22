using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }
    public List<Color> rarityColor = new List<Color>();
    public List<Relic> currentRelics = new List<Relic>(); //currentRelics
    public List<Relic> availableRelics = new List<Relic>(); //available relics to get in current game

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        FindAnyObjectByType<RelicController>()?.Initialize();
    }


    public void AddRelic(Relic newRelic)
    {
        currentRelics.Add(newRelic);
        availableRelics.Remove(newRelic);
    }
}

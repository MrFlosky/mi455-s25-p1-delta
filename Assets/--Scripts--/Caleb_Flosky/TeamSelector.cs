using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{
    private static readonly System.Random Random = new System.Random();
    
    [SerializeField] private TimerSystem timer;
    
    public static TeamSelector Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
    }
    
    public void CreateTeams()
    {
        // Check if empty
        if (WebConnection.unityServerDict.IsEmpty)
        {
            Debug.LogError("No devices in the dictionary.");
            timer.SetTimer(10, 0, 0, "Join Room", CreateTeams);
        }
        
        // make new list of all the keys
        var keys = new List<int>(WebConnection.unityServerDict.Keys);
        
        // get the randomly chosen initial id
        var initialZombieID = keys[Random.Next(keys.Count)];
        
        print("initialZombieID: " + initialZombieID);

        timer.SetTimer(1, 1, 1, "Run", GameSetup.Instance.EndGame);
    }
}

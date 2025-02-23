using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{
    private static readonly System.Random Random = new System.Random();
    
    [SerializeField] private TimerSystem timer;
    
    public static TeamSelector Instance { get; private set; }
    
    [SerializeField] private Transform spawnPointsParent;
    private List<Transform> availableSpawns = new List<Transform>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
        
        foreach (Transform spawn in spawnPointsParent)
        {
            availableSpawns.Add(spawn);
        }
    }
    
    public void CreateTeams()
    {
        // Check if empty
        if (WebConnection.unityServerDict.IsEmpty)
        {
            Debug.LogError("No devices in the dictionary.");
            timer.SetTimer(1, 0, 0, "No player joined, restarting setup", GameSetup.Instance.StartGame);
        }
        
        // make new list of all the keys
        var keys = new List<int>(WebConnection.unityServerDict.Keys);
        
        // get the randomly chosen initial id
        var initialZombieID = keys[Random.Next(keys.Count)];

        var initialZombieObj = WebConnection.playerObjDict[initialZombieID];
        initialZombieObj.GetComponent<PlayerTeam>().team = PlayerTeam.Team.Infected;
        initialZombieObj.GetComponent<Ball>().SetColor(true);
        
        MovePlayersToSpawns(WebConnection.playerObjDict);
        
        timer.timerText.text = "Run";
    }

    public void MovePlayersToSpawns(Dictionary<int, GameObject> players)
    {
        if (availableSpawns.Count < players.Count)
        {
            Debug.LogError("not enough spawn points");
            return;
        }
        
        ShuffleList(availableSpawns);

        int index = 0;
        foreach (var player in players.Values)
        {
            player.transform.position = availableSpawns[index].position;
            index++;
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Next(i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
}

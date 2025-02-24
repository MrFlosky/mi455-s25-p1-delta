using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class TeamSelector : MonoBehaviour
{
    // Random number generator for team selection
    private static readonly System.Random Random = new System.Random();

    // Reference to the TimerSystem for countdown handling
    [SerializeField] private TimerSystem timer;

    // Singleton instance for global access
    public static TeamSelector Instance { get; private set; }

    // Parent object containing spawn point transforms
    [SerializeField] private Transform spawnPointsParent;
    
    // List to store available spawn points
    private List<Transform> availableSpawns = new List<Transform>();

    private void Awake()
    {
        // Ensure only one instance of TeamSelector exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            return;
        }

        // Assign the current instance and persist across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Populate the spawn points list from the parent object
        foreach (Transform spawn in spawnPointsParent)
        {
            availableSpawns.Add(spawn);
        }
    }

    // Assigns teams and positions players at spawn points
    public void CreateTeams()
    {
        // Ensure there are players before starting the game
        if (WebConnection.unityServerDict.IsEmpty)
        {
            Debug.LogError("No devices in the dictionary.");
            timer.SetTimer(1, 0, 0, "No player joined, restarting setup", GameSetup.Instance.StartGame);
        }

        // Create a list of player IDs
        var keys = new List<int>(WebConnection.unityServerDict.Keys);

        // Randomly select one player to start as the infected
        var initialZombieID = keys[Random.Next(keys.Count)];
        var initialZombieObj = WebConnection.playerObjDict[initialZombieID];

        // Set the initial infected player's team and update their appearance
        initialZombieObj.GetComponent<PlayerTeam>().team = PlayerTeam.Team.Infected;
        initialZombieObj.GetComponent<Ball>().SetColor(true);

        // Move players to random spawn positions
        MovePlayersToSpawns(WebConnection.playerObjDict);
        
        GameSetup.Instance.FreezePlayers(true);
        
        timer.SetTimer(5, 0, 0, "Get Ready", LetPlayersMove);
    }

    private void LetPlayersMove()
    {
        Debug.Log("Bop");
        GameSetup.Instance.FreezePlayers(false);
        timer.timerText.text = "Go!";
    }

    // Moves players to random spawn locations
    public void MovePlayersToSpawns(Dictionary<int, GameObject> players)
    {
        // Ensure there are enough spawn points for all players
        if (availableSpawns.Count < players.Count)
        {
            Debug.LogError("Not enough spawn points");
            return;
        }

        // Shuffle the list to randomize spawn locations
        ShuffleList(availableSpawns);

        // Assign each player to a spawn point
        int index = 0;
        foreach (var player in players.Values)
        {
            var trail = player.GetComponent<TrailRenderer>();
            trail.enabled = false;
            
            player.transform.position = availableSpawns[index].position;
            
            trail.enabled = true;
            index++;
        }
    }

    // Helper function to shuffle a list using Fisher-Yates algorithm
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Next(i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}

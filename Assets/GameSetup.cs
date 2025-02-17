using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    
    [SerializeField] private TimerSystem timer;

    [SerializeField] private int downtimeSeconds = 0;
    [SerializeField] private int downtimeMinutes = 0;
    
    public static GameSetup Instance { get; private set; }
    
    private PlayerTeam[] players;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
            //return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
    }

    public void StartGame()
    {
        timer.SetTimer(downtimeSeconds, downtimeMinutes, 0, "Join Room", TeamSelector.Instance.CreateTeams);
    }

    private void EndGame()
    {
        timer.SetTimer(downtimeSeconds, downtimeMinutes, 0, "Game Over", StartGame);
        
        if (players.Length <= 0) return;
        // reset the players
        foreach (var player in players)
        {
            player.team = PlayerTeam.Team.Survivor;
        }
    }

    private void UpdatePlayerList()
    {
        players = FindObjectsOfType<PlayerTeam>();
    }

    public PlayerTeam[] GetPlayers()
    {
        return players;
    }
    
    public void CheckForWinner()
    {
        UpdatePlayerList();
        
        var survivorCount = 0;

        foreach (var player in players)
        {
            if (player.team == PlayerTeam.Team.Survivor)
            {
                survivorCount++;
            }
        }

        if (survivorCount <= 1)
        {
            EndGame();
        }
    }
}

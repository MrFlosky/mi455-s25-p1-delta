using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    // Reference to the timer system for handling countdowns
    [SerializeField] private TimerSystem timer;

    // Configurable downtime before restarting the game
    [SerializeField] private int downtimeSeconds = 0;
    [SerializeField] private int downtimeMinutes = 0;

    // Singleton instance for global access
    public static GameSetup Instance { get; private set; }

    // Array to store all player teams
    private PlayerTeam[] players;

    private bool _gameStarted = false; // Private backing field


    private void Awake()
    {
        // Ensure only one instance of GameSetup exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instance
        }

        // Assign the current instance and persist across scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Starts the game with a timer countdown
    public void StartGame()
    {
        timer.SetTimer(downtimeSeconds, downtimeMinutes, 0, "Join Room", StartingGame);
    }

    // Called when the countdown finishes, initiating the game start sequence
    public void StartingGame()
    {
        UpdatePlayerList();
        FreezePlayers(true); // Freeze players

        timer.SetTimer(5, 0, 0, "Get Ready", () =>
        {
            FreezePlayers(false); // Unfreeze players
            TeamSelector.Instance.CreateTeams(); // Start the game
        });
    }


    // Ends the current game session and resets the game state
    private void EndGame(string winner = "")
    {
        timer.SetTimer(downtimeSeconds, downtimeMinutes, 0, "Game Over\n" + winner + " Wins!", StartGame);
        
        // Ensure there are players before resetting teams
        if (players.Length <= 0) return;

        // Reset all players back to survivors
        foreach (var player in players)
        {
            player.team = PlayerTeam.Team.Survivor;
        }
        
        GameStarted = false;
    }

    // Updates the player list by finding all PlayerTeam components in the scene
    private void UpdatePlayerList()
    {
        players = FindObjectsOfType<PlayerTeam>();
    }

    // Returns the list of players in the game
    public PlayerTeam[] GetPlayers()
    {
        return players;
    }
    
    // Checks if only one survivor remains, triggering the end of the game
    public void CheckForWinner()
    {
        UpdatePlayerList();
        
        int survivorCount = 0;

        List<PlayerTeam> survivors = new List<PlayerTeam>();
        
        // Count the number of survivors
        foreach (var player in players)
        {
            if (player.team == PlayerTeam.Team.Survivor)
            {
                survivorCount++;
                survivors.Add(player);
            }
        }

        // If one or zero survivors remain, end the game
        if (survivorCount <= 1)
        {
            var survivorName = survivors[0].GetComponent<Ball>().usernameTxt.text;
            EndGame(survivorName);
        }
    }
    
    public void FreezePlayers(bool freeze)
    {
        foreach (var player in players)
        {
            // get each player's has a Rigidbody
            var rb = player.GetComponent<Rigidbody>(); 
            if (rb == null) continue;
            
            if (freeze)
            {
                // freeze
                rb.velocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                // unfreeze
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }
        }
    }
    
    public bool GameStarted { get; set; }
}

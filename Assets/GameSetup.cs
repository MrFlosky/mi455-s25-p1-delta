using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    
    [SerializeField] private TimerSystem timer;

    [SerializeField] private int downtimeSeconds = 0;
    [SerializeField] private int downtimeMinutes = 0;
    
    public static GameSetup Instance { get; private set; }

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
    
    public void EndGame()
    {
        timer.SetTimer(downtimeSeconds, downtimeMinutes, 0, "Game Over", StartGame);
    }
}

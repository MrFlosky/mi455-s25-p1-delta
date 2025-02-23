using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    // Enum representing the player's team
    public enum Team
    {
        Survivor,
        Infected
    }

    // Default team assignment
    public Team team = Team.Survivor;

    // Event triggered when a player's team changes
    public static event Action OnTeamUpdated;

    // Handles player infection when colliding with an infected player
    private void OnCollisionEnter(Collision other)
    {
        // Only process collisions if the player is still a survivor
        if (team != Team.Survivor) return;

        // Ensure collision is with another player
        if (!other.collider.CompareTag("Player")) return;

        var attacker = other.collider;
        
        // Check if the other player is infected
        if (attacker.GetComponent<PlayerTeam>().team == Team.Infected)
        {
            SetTeam();
        }
    }

    public void SetTeam(bool setPlayerTeam = true)
    {
        if (!setPlayerTeam) return;
        
        // Change this player's team to infected
        team = Team.Infected;

        // Update player's visual appearance
        Ball b = this.gameObject.GetComponent<Ball>();
        b.SetColor(true);

        // Notify listeners of the team update
        OnTeamUpdated?.Invoke();

        // Check if a winner should be declared
        GameSetup.Instance.CheckForWinner();
    }
}
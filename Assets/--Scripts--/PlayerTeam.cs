using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeam : MonoBehaviour
{
    public enum Team
    {
        Survivor,
        Infected
    }
    public Team team = Team.Survivor;
    public static event Action OnTeamUpdated;
    
    private void OnCollisionEnter(Collision other)
    {
        if (team != Team.Survivor) return;
        
        if (!other.collider.CompareTag("Player")) return;
            
        var attacker = other.collider;
        if (attacker.GetComponent<PlayerTeam>().team == Team.Infected)
        {
            team = Team.Infected;
            Ball b = this.gameObject.GetComponent<Ball>();
            b.SetColor(true);

            OnTeamUpdated?.Invoke();
            
            GameSetup.Instance.CheckForWinner();
        }
    }
    
}

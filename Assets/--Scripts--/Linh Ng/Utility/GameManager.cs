using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Tooltip("The list of players' balls on screen")]
    public List<Ball> balls = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterBall(Ball newBall)
    {
        if (!Balls.Contains(newBall))
        {
            Balls.Add(newBall);
        }
    }

    public void StartGame()
    {

    }
}

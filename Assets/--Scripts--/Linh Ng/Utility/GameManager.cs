using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Tooltip("The list of players' balls on screen")]
    public List<Ball> balls = null;

    // Start is called before the first frame update
    void Start()
    {
        balls = transform.GetComponentsInChildren<Ball>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {

    }
}

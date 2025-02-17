using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Pretty simple. Checks if we are warping a marble/ball and if we are moves it to the teleport spot
/// </summary>
public class WarpPad : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject warpPadDestination;

    [Header("Dynamic")] 
    private bool coolDown;

    void OnTriggerEnter(Collider other)
    {
        Marble marble = other.GetComponent<Marble>();
        Ball ball = other.GetComponent<Ball>();
        if (marble == null && ball == null) return;
        if (!coolDown)
        {
            GameObject go = other.gameObject;
            WarpPad w = warpPadDestination.GetComponent<WarpPad>();
            go.transform.position = warpPadDestination.transform.position;
            w.coolDown = true;
            w.Invoke("Refresh", 1 );
            
        }
        
    }

    public void Refresh()
    {
        print("refreshing");
        coolDown = false;

    }
}

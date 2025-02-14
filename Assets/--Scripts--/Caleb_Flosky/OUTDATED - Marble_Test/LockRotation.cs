using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public Transform marble; // Assign the player (ball) in the Inspector

    public float height = 1;
    void LateUpdate()
    {
        if (marble == null) return;

        // Keep the nametag at player's position but prevent rotation
        transform.position = marble.position + new Vector3(0, 0, 3); // Adjust Y height as needed
        transform.rotation = Quaternion.Euler(90, 0, 0); // Always stay upright
    }
}

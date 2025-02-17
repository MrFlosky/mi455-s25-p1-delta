using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    Ball ball;
    public int bumperStrength = 2;
    public float reenableDelay = 0.05f;
 
    public void OnCollisionEnter(Collision other)
    {
        ball = other.gameObject.GetComponent<Ball>();
        if (ball == null) return;

        Vector3 velocity = ball.currentVelocity;
        print("Before launch:" + velocity);
        ball.currentVelocity = velocity * -bumperStrength;
        ball.enabled = false;
        Invoke("Enable", reenableDelay);
        
        print("After launch:" + ball.currentVelocity);
    }

    public void Enable()
    {
        ball.enabled = true;
    }
}

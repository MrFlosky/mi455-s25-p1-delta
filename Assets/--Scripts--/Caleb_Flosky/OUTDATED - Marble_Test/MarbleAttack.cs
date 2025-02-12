using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleAttack : MonoBehaviour
{
    public float deathSpeedThreshold = 10f; // Minimum impact speed to trigger death
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            float impactSpeed = collision.relativeVelocity.magnitude; // Measure impact force

            // Get momentum (momentum = mass * velocity.magnitude)
            float myMomentum = rb.mass * rb.velocity.magnitude;
            float otherMomentum = otherRb.mass * otherRb.velocity.magnitude;

            // If the impact speed is high enough and this object had *lower* momentum, "die"
            if (impactSpeed >= deathSpeedThreshold && myMomentum < otherMomentum)
            {
                DieAndRespawn();
            }
        }
    }

    void DieAndRespawn()
    {
        // Disable this object (simulating death)
        gameObject.SetActive(false);

        // Respawn after 2 seconds
        Invoke(nameof(Respawn), 2f);
    }

    void Respawn()
    {
        // Reset position and velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-30, 30), 1.66f, Random.Range(-30, 30));

        // Re-enable the object
        gameObject.SetActive(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper2 : MonoBehaviour 
{
    [SerializeField] private float baseBounciness = 5f;
    [Header("How much extra bounce to add based on impact speed MULT")]
    [SerializeField] private float impactBounceFactor = 0.1f;
    public Animator animator;
    public AnimationClip tween;

    private void OnCollisionEnter(Collision collision)
    {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball != null)
        {
            // Calculate collision normal (direction of this ball to the other)
            Vector3 normal = (ball.transform.position - transform.position).normalized;

            // Calculate how fast ball moving towards the bumper
            float impactSpeed = Mathf.Abs(Vector3.Dot(ball.currentVelocity, normal));
            
            // Scale bounciness up with impact speed
            float scaledBounciness = baseBounciness + (impactSpeed * impactBounceFactor);
            
            // Calculate impulse with scaled bounciness
            float impulseStrength = -(1 + scaledBounciness) * Vector3.Dot(ball.currentVelocity, normal);
            
            // Apply impulse to ball
            Vector3 impulse = normal * impulseStrength;
            ball.currentVelocity += impulse;
            animator.Play(tween.name);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Marble : MonoBehaviour
{
    [SerializeField] private float powerModifier = 1f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private TMP_Text nameplate;
    public void Launch(float x, float z, float timeHeld)
    {
        var v = new Vector3(x, 0, z) * (Mathf.Min(timeHeld, 300f) * powerModifier);
        
        var bulletBody = gameObject.GetComponent<Rigidbody>();
        
        bulletBody.velocity = Vector3.zero;
        bulletBody.AddForce(v);
    }

    private void Awake()
    {
        if (renderer != null)
        {
            // Generate a random color
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            renderer.material.color = randomColor;
        }
        else
        {
            Debug.LogWarning("MeshRenderer is not assigned!");
        }
    }

    public void SetNameplate(string newName)
    {
        nameplate.text = newName;
    }
}

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
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
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
            SetColor();
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

    public void SetColor()
    {
        // We will need to make sure only this instances color is being offset
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        
        float hueVal = Random.Range(-1.0f, 1.0f);
        float satVal = Random.Range(0f, 10.0f);
        
        propertyBlock.SetFloat("_hueOffset", hueVal);
        propertyBlock.SetFloat("_saturationOffset", satVal);
        
        this.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }

    private void Update()
    {
        if (transform.position.y < -10f)
        {
            DieAndRespawn();
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

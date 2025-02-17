using System.Collections;
using System.Collections.Generic;
using Avante;
using UnityEngine;

public class ToggleDome : MonoBehaviour
{
    public FulldomeCamera domeCam;
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            domeCam.enabled = !domeCam.enabled;
        }
        
    }
    
}

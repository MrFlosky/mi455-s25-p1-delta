using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleColorChange : MonoBehaviour
{
    private Material material;

    void Start()
    {
        // Get the material attached to this object's MeshRenderer
        material = GetComponent<MeshRenderer>().material;

        // Create a property block to modify only this instance's material properties
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        // Generate random values for hue and saturation to create color variation
        float hueVal = Random.Range(-1.0f, 1.0f);
        float satVal = Random.Range(0f, 10.0f);

        // Apply the random values to the material property block
        propertyBlock.SetFloat("_hueOffset", hueVal);
        propertyBlock.SetFloat("_saturationOffset", satVal);

        // Apply the modified properties to the object's MeshRenderer
        this.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}
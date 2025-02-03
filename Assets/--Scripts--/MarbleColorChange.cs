using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarbleColorChange : MonoBehaviour
{
    private Material material;
    
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        // We will need to make sure only this instances color is being offset
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        
        float hueVal = Random.Range(-1.0f, 1.0f);
        float satVal = Random.Range(0f, 10.0f);
        
        propertyBlock.SetFloat("_hueOffset", hueVal);
        propertyBlock.SetFloat("_saturationOffset", satVal);
        
        this.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);
    }
}

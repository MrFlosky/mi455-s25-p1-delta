using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rigid;
    // Key for accessing device dict
    int ID;
    // Motion data for this users ball
    WebConnection.DeviceData data;
    [SerializeField] float power;

    public void Init(int id)
    {
        ID = id;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        if(WebConnection.unityServerDict.TryGetValue(ID, out _))
        {
            data = WebConnection.unityServerDict[ID];
            rigid.AddForce(new Vector3((data.lr_tilt / 100)*power, 0, 0));
            rigid.AddForce(new Vector3(0, 0, (-data.fb_tilt / 100)*power));
        }
        else
        {
            Destroy(this.gameObject);
        }
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
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    TMP_Text usernameTxt;

    Rigidbody rigid;
    // Key for accessing device dict
    int ID;
    // Motion data for this users ball
    WebConnection.DeviceData data;

    [SerializeField] float power;

    /// <summary>
    /// /Set data upon this ball being initialized so we need only set username once.
    /// </summary>
    /// <param name="id"></param>
    public void Init(int id)
    {
        ID = id;
        data = WebConnection.unityServerDict[ID];
        usernameTxt.text = data.username; 
    }

    void Awake()
    {
        usernameTxt = transform.GetChild(0).GetComponent<TMP_Text>();
        rigid = GetComponent<Rigidbody>();
        SetColor();
    }

    /// <summary>
    /// Reccomended to use fixed update for physics updates.
    /// </summary>
    void FixedUpdate()
    {
        //MoveBallDefault(data.lr_tilt, data.fb_tilt);
        MoveBallAnimCurve(data.lr_tilt, data.fb_tilt);
    }

    /// <summary>
    /// Constantly update the data from the server.
    /// </summary>
    void Update()
    {
        if(WebConnection.unityServerDict.TryGetValue(ID, out _))
        {
            data = WebConnection.unityServerDict[ID];
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //[SerializeField] private float acceleration = 30f;
    //[SerializeField] private float deceleration = 20f;
    // Adjust to change the 3rd increment at which the ball begins moving.
    //[SerializeField] private float tiltThreshold = 3f;
    //[SerializeField] private float stoppingThreshold = 0.5f;
    //[SerializeField] private float accelerationRate = 15f;
    
    // For MoveBallAnimCurve
    private Vector3 currentVelocity;
    // Max speed ball may reach
    [SerializeField] private float maxSpeed = 10f;
    // Constant mult for acceleration strength
    [SerializeField] private float accelerationMultiplier = 15f;
    // Maps tilt strength to acceleration response, affects how sharply the ball changes speed
    [SerializeField] private AnimationCurve accelerationCurve = new AnimationCurve();


    /// <summary>
    /// The initial tilt makes the ball accelerate fast, while the steeper tilt to reach max speed accelerates slower.
    /// </summary>
    /// <param name="leftRightGyro">
    // Left right/gyro data
    // </param>
    /// <param name="upDownGyro">
    // Up/down gyro data
    // </param>
    void MoveBallAnimCurve(float leftRightGyro, float upDownGyro)
    {
        // Gyro input into movement direction
        Vector3 input = new Vector3(leftRightGyro, 0, -upDownGyro);
        // Calculate desired velocity based on input, capped at max speed
        Vector3 targetVelocity = Vector3.ClampMagnitude(input, maxSpeed) * power; // keep direction of ball, but if speed(magnitude) is too big clamp it

        // Use animation curve to evaluate acceleration
        float inputStrength = input.magnitude / (maxSpeed * power); // Calculate how much of max speed trying to reach 
        float accelerationFactor = accelerationCurve.Evaluate(inputStrength);
        
        float deltaTime = Time.deltaTime;
        Vector3 acceleration = (targetVelocity - currentVelocity) * accelerationFactor * accelerationMultiplier;
        currentVelocity += acceleration * deltaTime;

            Debug.Log($"Speed: {currentVelocity.magnitude:F2} | " +
              $"AccelFactor: {accelerationFactor:F2} | " +
              $"Target Speed: {targetVelocity.magnitude:F2}");
        rigid.velocity = currentVelocity;
    }

    // void MoveBallPeppy2(float leftRightGyro, float upDownGyro)
    // {
    //     Vector3 input = new Vector3(leftRightGyro, 0, -upDownGyro);
        
    //     Vector3 targetVelocity = Vector3.ClampMagnitude(input, maxSpeed) * power;

    //     // Apply acceleration curve instead of linear movement
    //     float deltaTime = Time.deltaTime;
    //     Vector3 acceleration = (targetVelocity - currentVelocity) * accelerationRate;
    //     currentVelocity += acceleration * deltaTime;

    //     rigid.drag = input.magnitude > 0 ? 0.5f : 2f; // More drag when not moving
    //     rigid.velocity = currentVelocity;
    // }


    // void MoveBallPeppy(float leftRightGyro, float upDownGyro)
    // {
    //     Vector3 input = new Vector3(leftRightGyro, 0, -upDownGyro);
    //     Vector3 targetVelocity = Vector3.ClampMagnitude(input, maxSpeed) * power;
    //     Debug.Log(currentVelocity);
    //     float deltaTime = Time.deltaTime;
    //     if (input.magnitude > 0)
    //     {
    //         currentVelocity = Vector3.MoveTowards(
    //             currentVelocity,
    //             targetVelocity,
    //             acceleration * deltaTime
    //         );
    //     }
    //     else
    //     {
    //         currentVelocity = Vector3.MoveTowards(
    //             currentVelocity,
    //             Vector3.zero,
    //             deceleration * deltaTime
    //         );
            
    //         if (currentVelocity.magnitude < stoppingThreshold)
    //         {
    //             currentVelocity = Vector3.zero;
    //         }
    //     }

    //     rigid.velocity = currentVelocity;
    // }

    /// <summary>
    /// Used to move the ball.
    /// This one feels more like you're rolling a ball.
    /// </summary>
    void MoveBallDefault(float leftRightGyro, float upDownGyro)
    {
        rigid.AddForce(new Vector3(data.lr_tilt/ 10 *power, 0, 0));
        rigid.AddForce(new Vector3(0, 0, -data.fb_tilt/ 10 *power));
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

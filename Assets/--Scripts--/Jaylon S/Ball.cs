using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public TMP_Text usernameTxt;
    Rigidbody rigid;
    // Transform of renderer.
    Transform rendererTrans;
    // Key for accessing device dict
    int ID;
    public float currentSeconds;
    // Motion data for this users ball
    WebConnection.DeviceData data;

    /// <summary>
    /// /Set data upon this ball being initialized so we need only set username once.
    /// </summary>
    /// <param name="id"></param>
    public void Init(int id)
    {
        ID = id;
        if(WebConnection.unityServerDict.TryGetValue(ID, out var serverData))
        {
            data = serverData;
        }
        usernameTxt.text = data.username; 
    }

    void Awake()
    {
        usernameTxt = transform.GetChild(0).GetComponent<TMP_Text>();
        rigid = GetComponent<Rigidbody>();
        rendererTrans = transform.GetChild(1).transform;
        // Register this Ball instance with the GameManager when it is spawned

        SetColor(false);
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
            UpdateBallRotation();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
    // For MoveBallAnimCurve
    public Vector3 currentVelocity;
    // Maximum tilt is 40 degrees
    float maxTiltDegrees = 40f;
    // Max speed ball may reach
    [SerializeField] private float maxSpeed = 10f;
    // Constant mult for acceleration strength
    private float accelerationMultiplier = 10f;

    /**
    Connects tilt to acceleration mult, affects how sharply the ball changes speed. 
    X-Axis is Input strength, Y-axis Acceleration Factor applied.

    InputStrength (x-axis) scales with 3° increments: (3 / maxTilt).
    tilt degree = input strength
    3° = 0.075
    6° = 0.15
    9° = 0.225
    12° = 0.3
    15° = 0.375
    18° = 0.45
    21° = 0.525
    24° = 0.6
    27° = 0.675
    30° = 0.75
    33° = 0.825
    36° = 0.9
    39° = 0.975
    40° = 1.0
    */
    [Header("X-Axis: TiltStrength(0-1), Y-Axis: Acceleration Multiplier")]
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
        // Scale gyro input to be relative to max tilt degrees
        Vector3 input = new Vector3(
            leftRightGyro / maxTiltDegrees, 
            0, 
            -upDownGyro / maxTiltDegrees
        );
        
        // Calculate desired velocity based on input, capped at max speed
        Vector3 targetVelocity = Vector3.ClampMagnitude(input, 1f) * maxSpeed; 
        
        float inputStrength = input.magnitude;
        //inputStrength = Mathf.Clamp01(inputStrength); shouldnt need this if maxtiltdegrees is accurate to js on website
        
        float accelerationFactor = accelerationCurve.Evaluate(inputStrength);         
        
        float deltaTime = Time.deltaTime;         
        Vector3 acceleration = (targetVelocity - currentVelocity) * accelerationFactor * accelerationMultiplier;         
        currentVelocity += acceleration * deltaTime;          
        
        //Debug.Log($"Input Strength: {inputStrength:F2} | Accel Factor: {accelerationFactor:F2} | Tilt: {leftRightGyro:F1}°, {upDownGyro:F1}° ");              
        
        rigid.velocity = currentVelocity;     
    }


    float power = 6;
    /// <summary>
    /// Used to move the ball.
    /// This one feels more like you're rolling a ball.
    /// </summary>
    void MoveBallDefault(float leftRightGyro, float upDownGyro)
    {
        rigid.AddForce(new Vector3(data.lr_tilt/ 10 *power, 0, 0));
        rigid.AddForce(new Vector3(0, 0, -data.fb_tilt/ 10 *power));
    }

    public void SetColor(bool isInfected)
    {
        if (!isInfected)
        {
            // We will need to make sure only this instances color is being offset
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        
            float hueVal = Random.Range(-1.0f, 1.0f);
            float satVal = Random.Range(0f, 10.0f);
        
            propertyBlock.SetFloat("_hueOffset", hueVal);
            propertyBlock.SetFloat("_saturationOffset", satVal);
        
            rendererTrans.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock);  
        }
        // Setting color for infected
        else
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        
            float hueVal = 0.69f;
            float satVal = 30f;
        
            propertyBlock.SetFloat("_hueOffset", hueVal);
            propertyBlock.SetFloat("_saturationOffset", satVal);
        
            rendererTrans.GetComponent<MeshRenderer>().SetPropertyBlock(propertyBlock); 
        }
        
    }

    void UpdateBallRotation()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, currentVelocity.normalized);
            float rotationSpeed = currentVelocity.magnitude * 360f;
            rendererTrans.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Ball otherBall = collision.gameObject.GetComponent<Ball>();
        // Only handle collision if this ball ball with lower instance ID to avoid double call
        if (otherBall != null && GetInstanceID() < otherBall.GetInstanceID())
        {
            BallCollision(otherBall);
        }
    }

    [Header("Ball-ball collision variables")]
    [SerializeField] private float mass = 1f;
    [SerializeField] private float bounce = 3f; // 0 = no bounce, 1 = perfect bounce

    // Detection: Continious Dynamic seems to have fixed weird collision bug.
    // For detection mode info:
    // https://blog.terresquall.com/2019/12/collision-detection-modes-in-unitys-rigidbody-component
    public void BallCollision(Ball otherBall)
    {
        // Calculate collision normal (direction of this ball to the other). normalized to only include direction
        Vector3 normal = (otherBall.transform.position - transform.position).normalized;

        // Calculate relative velocity
        Vector3 relativeVelocity = otherBall.currentVelocity - currentVelocity;

        // How much does relative velocity project onto the normal, i.e. how fast this ball moving towards the other
        // PROJdirVel = (Vel * dir / |dir|) * dir 
        // where dir is normalized (length of 1)
        // |dir| = 1 since magnitude || means length of vector 
        // (vel * dir) * dir
        Vector3 projectedVel = (Vector3.Dot(relativeVelocity, normal)) * normal;

        // impulse equation: j = -(1 + e)(v1 - v2) / (1/m1 + 1/m2)
        Vector3 impulse = -(1 + bounce) * projectedVel / (1/mass + 1/otherBall.mass);
        // Apply impulse
        currentVelocity -= impulse / mass;
        otherBall.currentVelocity += impulse / otherBall.mass;
    }

    public Rigidbody GetRigid()
    {
        return rigid;
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        PlayerTeam.OnTeamUpdated -= UpdateTeamInfo;
    }

    private void UpdateTeamInfo()
    {
        // Update the ball based on the new team info
        //currentTeam = FindObjectOfType<PlayerTeam>().GetTeamName();
        currentSeconds = TimerSystem.instance.GetCurrentTime();
        GameManager.instance.RegisterBall(this);
        //Debug.Log($"Ball updated to new team: {currentTeam}");
    }


}

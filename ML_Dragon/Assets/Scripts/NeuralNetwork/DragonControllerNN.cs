using UnityEngine;
public class DragonControllerNN : MonoBehaviour
{
    private float flapStrength = 10f;
    private float liftFactor = 1f;
    private float speedFactor = .5f;
    private float maxSpeed = 2.5f;
    private float maxLift = 2.5f;
    private float tailHorizontalSpeed = 50f;
    private float tailVerticalSpeed = 30f;
    private float simultaneousFlapBoost = 1.5f;

    public Rigidbody rb;
    private bool canLeftFlap = true;
    private bool canRightFlap = true;

    private float lastLeftFlapTime;
    private float lastRightFlapTime;
    private float flapTimeWindow = 0.1f;

    public TailRig tail;
    private float tail_x = 0;
    private float tail_x_increment = 3;
    private float tailReturnSpeed = 1.5f;

    public NeckRig neck;
    private float neck_x = 0;
    private float neck_x_increment = 3;
    private float neckReturnSpeed = 1.5f;

    public WingRig leftWing, rightWing;

    public NeuralNetwork neuralNetwork;

    public Transform target; // The target that the dragon should find

        public bool InCollision{get=>inCollsion;}
    private bool inCollsion = false;
    
    public bool isTraining = true;
    public FitnessEvaluator fitness;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if(!isTraining)
            neuralNetwork = new NeuralNetwork(11,7, 4); // Updated to 6 inputs and 10 hidden nodes for this example
    }

    void Start(){
        if(!isTraining){
            neuralNetwork.LoadNetworkWeights("Assets/Weights.json");
        }
    }
 void Update()
    {
        //HandleWingMovement();
        //HandleTailMovement();
        //Debug.Log($"{inCollsion}");
        if(!isTraining){
            float[] inputs = CollectInputs();
            ProcessNetworkOutputs(neuralNetwork.FeedForward(inputs));
        }

    }

    void HandleWingMovement()
    {
        // Handle left wing flap
        if (Input.GetKeyDown(KeyCode.A) && leftWing.isReady)
        {
            ApplyLeftWingFlap();
            lastLeftFlapTime = Time.time;
        
            // Check for simultaneous flap
            if (Time.time - lastRightFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        // Handle right wing flap
        if (Input.GetKeyDown(KeyCode.D) && rightWing.isReady)
        {
            ApplyRightWingFlap();
            lastRightFlapTime = Time.time;

            if (Time.time - lastLeftFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        // Cap the speed and lift
        CapVelocity();
    }

   public float[] CollectInputs()
{
    float maxDetectionRange = 100f;

    // Initialize the inputs array
    float[] inputs = new float[11]; // We now have 11 inputs including tail_x

    // 1. Vertical speed
    inputs[0] = rb.velocity.y / maxLift; // Normalized vertical speed

    // 2. Horizontal speed
    Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    inputs[1] = horizontalVelocity.magnitude / maxSpeed;

    // 3-5. Relative position to target in local space
    Vector3 localRelativePosition = transform.InverseTransformPoint(target.position) / maxDetectionRange;
    inputs[2] = Mathf.Clamp(localRelativePosition.x, -1f, 1f);
    inputs[3] = Mathf.Clamp(localRelativePosition.y, -1f, 1f);
    inputs[4] = Mathf.Clamp(localRelativePosition.z, -1f, 1f);

    // 6-8. Dragon's rotation (Pitch, Yaw, Roll)
    Vector3 eulerAngles = transform.rotation.eulerAngles;
    
    // Normalize rotation angles between -1 and 1
    inputs[5] = NormalizeAngle(eulerAngles.x); // Pitch
    inputs[6] = NormalizeAngle(eulerAngles.y); // Yaw
    inputs[7] = NormalizeAngle(eulerAngles.z); // Roll

    // 9. Tail horizontal position (tail_x)
    inputs[8] = tail_x; // Add the tail's x position as an input

    // 10. Distance to target (normalized)
    float distanceToTarget = Vector3.Distance(transform.position, target.position) / maxDetectionRange;
    inputs[9] = Mathf.Clamp01(distanceToTarget);

    // 11. Angle to target (normalized)
    Vector3 toTarget = target.position - transform.position;
    Vector3 forward = transform.forward;
    float angleToTarget = Vector3.SignedAngle(forward, toTarget, Vector3.up);
    inputs[10] = angleToTarget / 180f; // Normalized between -1 and 1

    return inputs;
}

// Helper function to normalize Euler angles from 0-360 degrees to -1 and 1
private float NormalizeAngle(float angle)
{
    // Convert angle from 0-360 range to -180 to 180 range
    if (angle > 180f) angle -= 360f;

    // Normalize to range -1 to 1
    return angle / 180f;
}




    public void ProcessNetworkOutputs(float[] outputs)
    {
        // Process network outputs and map them to control signals
        //Debug.Log(outputs[0]);
        if (outputs[0] > 0.5f && leftWing.isReady)
        {
            ApplyLeftWingFlap();
            lastLeftFlapTime = Time.time;
            

            if (Time.time - lastRightFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        if (outputs[1] > 0.5f && rightWing.isReady)
        {
            ApplyRightWingFlap();
            lastRightFlapTime = Time.time;
            

            if (Time.time - lastLeftFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        if (outputs[2] != 0)
        {
            float tailMovement = Mathf.Clamp(outputs[2], -1f, 1f);
            transform.Rotate(Vector3.up, tailHorizontalSpeed * tailMovement*5 * Time.deltaTime);
            tail_x += tail_x_increment * tailMovement * Time.deltaTime;
            neck_x -= tail_x_increment * tailMovement * Time.deltaTime;
        }

        
        if (outputs[3] != 0)
        {
            float tailMovement = Mathf.Clamp(outputs[3], -1f, 1f);
        }

        tail_x = Mathf.Clamp(tail_x, -1, 1);
        tail.UpdatePostion(-tail_x);
        

        tail_x = Mathf.Clamp(tail_x, -1, 1);
        tail.UpdatePostion(-tail_x);
        neck.UpdatePostion(tail_x);

        CapVelocity();
    }

    void ApplyLeftWingFlap()
    {
        float lift = flapStrength * liftFactor;
        float speed = flapStrength * speedFactor;

        // Apply lift and forward force
        rb.AddForce(transform.up * lift / 2f, ForceMode.Impulse);
        rb.AddForce(transform.forward * speed / 2f, ForceMode.Impulse);
        transform.Rotate(transform.up, -tailHorizontalSpeed * 10 * Time.deltaTime);

        leftWing.Flap();
    }

    void ApplyRightWingFlap()
    {
        float lift = flapStrength * liftFactor;
        float speed = flapStrength * speedFactor;

        // Apply lift and forward force
        rb.AddForce(transform.up * lift / 2f, ForceMode.Impulse);
        rb.AddForce(transform.forward * speed / 2f, ForceMode.Impulse);
        transform.Rotate(transform.up, tailHorizontalSpeed * 10 * Time.deltaTime);

        rightWing.Flap();
    }

    void ApplySimultaneousFlapBoost()
    {
        float speedBoost = flapStrength * speedFactor * simultaneousFlapBoost;
        rb.AddForce(transform.forward * speedBoost, ForceMode.Impulse);
    }

    void HandleTailMovement()
    {
        // Horizontal tail movement (rotate around up axis)
        tail_x = Mathf.Lerp(tail_x, 0f, tailReturnSpeed * Time.deltaTime);
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, tailHorizontalSpeed * Time.deltaTime);
            tail_x += tail_x_increment * Time.deltaTime;
            neck_x -= tail_x_increment * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, -tailHorizontalSpeed * Time.deltaTime);
            tail_x -= tail_x_increment * Time.deltaTime;
            neck_x += tail_x_increment * Time.deltaTime;
        }

        // Vertical tail movement (rotate around right axis)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.right, -tailVerticalSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.right, tailVerticalSpeed * Time.deltaTime);
        }

        tail_x = Mathf.Clamp(tail_x, -1, 1);
        tail.UpdatePostion(-tail_x);
        neck.UpdatePostion(tail_x);
    }

    void CapVelocity()
    {
        // Cap the upward velocity
        if (rb.velocity.y > maxLift)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxLift, rb.velocity.z);
        }

        // Cap the forward speed
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
        }
    }


    public void OnCollisionEnter(Collision other){
       
        if(other.collider.CompareTag("Wall")){
           // Debug.Log("Glass Hit");
            inCollsion= true;
        }
        else{
            //Debug.Log("Glass Not Hit");
            inCollsion = false;
        }
        
    }
    public void OnCollisionExit(Collision other){
        inCollsion = false;
        
    }

    public void OnTriggerEnter(Collider other){
        if(other.CompareTag("Food")){
            if(isTraining)
                fitness.bonus = 1000;
            GameController.instance.Reset();
        }
    }
}

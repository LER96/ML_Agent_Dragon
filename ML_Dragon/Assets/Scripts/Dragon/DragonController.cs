using UnityEngine;

public class DragonController : MonoBehaviour
{
    public float flapStrength = 10f;       // Strength of a single flap
    public float liftFactor = 45f;         // How much lift is generated by a flap
    public float speedFactor = 5f;         // How much forward speed is generated by a flap
    public float maxSpeed = 20f;           // Maximum forward speed
    public float maxLift = 10f;            // Maximum upward velocity
    public float tailHorizontalSpeed = 50f; // Speed of horizontal tail movement
    public float tailVerticalSpeed = 50f;   // Speed of vertical tail movement
    public float simultaneousFlapBoost = 1.5f; // Boost multiplier for simultaneous flaps

    private Rigidbody rb;
    private bool canLeftFlap = true;       // Controls whether the left wing can flap
    private bool canRightFlap = true;      // Controls whether the right wing can flap

    private float lastLeftFlapTime;
    private float lastRightFlapTime;
    private float flapTimeWindow = 0.1f;  // Time window to consider flaps as simultaneous

    public TailRig tail;
    private float tail_x = 0;
    private float tail_x_increment = 3;
    private float tailReturnSpeed = 1.5f;

    public NeckRig neck;
    private float neck_x = 0;
    private float neck_x_increment = 3;
    private float neckReturnSpeed = 1.5f;

    public WingRig leftWing, rightWing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleWingMovement();
        HandleTailMovement();
    }

    void HandleWingMovement()
    {
        // Handle left wing flap
        if (Input.GetKeyDown(KeyCode.A) && canLeftFlap)
        {
            ApplyLeftWingFlap();
            lastLeftFlapTime = Time.time;
            canLeftFlap = false;

            // Check for simultaneous flap
            if (Time.time - lastRightFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            canLeftFlap = true;
        }

        // Handle right wing flap
        if (Input.GetKeyDown(KeyCode.D) && canRightFlap)
        {
            ApplyRightWingFlap();
            lastRightFlapTime = Time.time;
            canRightFlap = false;

            // Check for simultaneous flap
            if (Time.time - lastLeftFlapTime <= flapTimeWindow)
            {
                ApplySimultaneousFlapBoost();
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            canRightFlap = true;
        }

        // Cap the speed and lift
        CapVelocity();
    }

    void ApplyLeftWingFlap()
    {
        float lift = flapStrength * liftFactor;
        float speed = flapStrength * speedFactor;

        // Apply lift and forward force
        rb.AddForce(transform.up * lift / 2f, ForceMode.Impulse);
        rb.AddForce(transform.forward * speed / 2f, ForceMode.Impulse);
        transform.Rotate(Vector3.up, -tailHorizontalSpeed * 10 * Time.deltaTime);

        leftWing.Flap();
    }

    void ApplyRightWingFlap()
    {
        float lift = flapStrength * liftFactor;
        float speed = flapStrength * speedFactor;

        // Apply lift and forward force
        rb.AddForce(transform.up * lift / 2f, ForceMode.Impulse);
        rb.AddForce(transform.forward * speed / 2f, ForceMode.Impulse);
        transform.Rotate(Vector3.up, tailHorizontalSpeed * 10 * Time.deltaTime);

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
}

using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator _animator;
    
    private Vector3 movementInput;

    void Start()
    {
        rb.freezeRotation = true; // Prevent physics from rotating the player
    }

    void Update()
    {
        HandleMovementInput();
    }

    void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void HandleMovementInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movementInput = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
    }

    void Move()
    {
        Vector3 moveDirection = movementInput * speed;
        rb.velocity = moveDirection;
    }

    void Rotate()
    {
        if (movementInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f);
            _animator.SetBool("IsMoving",true);
        }

        else
        {
            _animator.SetBool("IsMoving",false);
        }
    }
}
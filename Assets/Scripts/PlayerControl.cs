using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Movement")]
    
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] Transform orientation;
    [SerializeField] float jumpForce;
    private float horizontalInput, verticalInput;

    private float movementSpeed;
    private Vector3 moveDirection = Vector3.forward;
    private Rigidbody rb;
    private bool isGrounded;
    private bool toggleSprint;

    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementSpeed = walkSpeed;
        toggleSprint = true;
        isGrounded = true;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = orientation.rotation;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (moveDirection.magnitude > 0 && verticalInput != 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
        if(moveDirection.magnitude > 0 && verticalInput != 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
        if (verticalInput == 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        if (horizontalInput > 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalkingRight", true);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);

        }
        else if (horizontalInput < 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", true);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);
        }
        else if (horizontalInput > 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", true);
            animator.SetBool("isRunningLeft", false);
        }
        else if (horizontalInput < 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", true);
        }
        else
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);
        }

        if(rb.velocity.y < -1.0f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }


        if (toggleSprint)
        {
            if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (movementSpeed == walkSpeed)
                {
                    movementSpeed = sprintSpeed;
                }
                else
                {
                    movementSpeed = walkSpeed;
                }
            }
        }
        if (Input.GetAxisRaw("Jump") != 0)
        {
            Debug.Log(isGrounded);
        }
        if (isGrounded && Input.GetAxisRaw("Jump") != 0)
        {
            animator.SetTrigger("isJumping");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;        }

        if (transform.position.y < -15)
        {
            transform.position = Vector3.zero + Vector3.up * 3;
        }
    }

    void LateUpdate()
    {
        rb.AddForce(moveDirection.normalized * movementSpeed, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, movementSpeed);

    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("onGround", true);
            isGrounded = true;
            Debug.Log("Entered Ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("onGround", false);
            isGrounded = false;
            Debug.Log("Exited Ground");
        }
    }

    public void SetSprintToggle(bool toggle)
    {
        this.toggleSprint = toggle;
    }

}

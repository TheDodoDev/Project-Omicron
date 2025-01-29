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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementSpeed = walkSpeed;
        toggleSprint = true;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
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
        else if(!toggleSprint)
        {
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
               movementSpeed = sprintSpeed;
            }
            else
            {
                movementSpeed = walkSpeed;
            }
        }
        if(transform.position.y < -15)
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
        if (isGrounded && Input.GetAxisRaw("Jump") != 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Entered Ground");
        }
        rb.mass = 1;

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Exited Ground");
            rb.mass = 100;
        }
    }

    public void SetSprintToggle(bool toggle)
    {
        this.toggleSprint = toggle;
    }

}

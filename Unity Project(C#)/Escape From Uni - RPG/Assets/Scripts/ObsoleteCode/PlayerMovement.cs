using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] float movementSpeed = 8f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    [SerializeField] AudioSource jumpSound;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Rigibody rb = new Rigibody(); extends 
    }

    // Update is called once per frame
    void Update()
    {
        float horintalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horintalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);

        if (Input.GetButtonDown("Jump") && IsOnGround()) 
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpSound.Play(); 
        }

        if (ItemCollector.coins == 3)
        {
            jumpForce = 50;
        }
        
    }

    bool IsOnGround()
    {
        return Physics.CheckSphere(groundCheck.position, .1f, ground);
    }
}

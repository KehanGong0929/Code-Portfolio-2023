using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//author: Leo and Kehan
public class CharacterMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    
    private Vector3 moveDirection;
    private Vector3 verticalVelocity;
    // Set the animator as public in case we would use it in the future
    public Animator animator;
    // Determine the rotation speed
    private float _rotationFactorPerFramef= 15.0f;
    
    [SerializeField] private bool isGrounded;
    //[SerializeField] Transform groundCheck;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] AudioSource jumpSound;
    
    [SerializeField] private float jumpHeight;

    //REFERENCES
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(Attack());
        }
    }
    
    private void Move()
    {

        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;

        }

        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        moveDirection = new Vector3(moveX,0,moveZ);

        
        if(isGrounded)
        {  
            if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walk();
            }
            else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            {
                Run();
            }
            else if (moveDirection == Vector3.zero)
            {
                Idle();
            }
            moveDirection *= moveSpeed;
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
       
        

        HandleRotation();
        controller.Move(moveDirection * Time.deltaTime);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
    
    bool IsMoving()
    {
        return moveDirection.x != 0 || moveDirection.z != 0;
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = Input.GetAxis("Horizontal");
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = Input.GetAxis("Vertical");
        Quaternion currentRotation = transform.rotation;
        if (IsMoving())
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFramef * Time.deltaTime);
        }
    }

    
    private void Jump()
    {
        verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        jumpSound.Play();
    }
    
    private void Idle(){

        animator.SetFloat("MovingSpeed", 0, 0.3f, Time.deltaTime);
        
    }
    private void Run()
    {
        moveSpeed = runSpeed;
        animator.SetFloat("MovingSpeed", 1f, 0.3f, Time.deltaTime);
    }
    private void Walk()
    {
        moveSpeed = walkSpeed;
        animator.SetFloat("MovingSpeed", 0.5f, 0.3f, Time.deltaTime);
    }

    private IEnumerator Attack()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("Combat Layer"), 0.5f);
        
        
        animator.SetTrigger("MeleeAttack");
        

       
        yield return new WaitForSeconds(0.9f);
        animator.SetLayerWeight(animator.GetLayerIndex("Combat Layer"), 1);

     
       
    }
}

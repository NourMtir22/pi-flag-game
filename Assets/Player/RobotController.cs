using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 8;
    [SerializeField] float sprintSpeed = 14;

    [SerializeField] float rollSpeed = 13;

    [SerializeField] float gravity = -10;
    [SerializeField] float jumpHeight = 3;
    //[SerializeField] Transform groundCheck ;
    //[SerializeField] LayerMask groundLayer;

    bool isGrounded;



    Vector3 velocity;

    private bool ballMode=false;

    private PlayerInputsManager input;
    private CharacterController controller;
    private Animator animator;
    [SerializeField] GameObject mainCam;

    [SerializeField] Transform cameraFollowTarget;

    float xRotation;
    float yRotation;
    private int jumpCount = 0; // Tracks the number of jumps
    private int maxJumps = 2;  // Maximum number of jumps allowed
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<PlayerInputsManager>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (input == null)
        {
            Debug.LogError("PlayerInputsManager component is missing!");
        }
        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing!");
        }
    }

    // player animation
    void Update()   
    {
        SwitchMode();
        if (ballMode)
        
            MoveBall();
        else
            MoveNormal();

       JumpAndGravity();

        
    }
    private void LateUpdate()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        xRotation += input.look.y;
        yRotation += input.look.x;
        xRotation = Mathf.Clamp(xRotation, -30, 70);


        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
        cameraFollowTarget.rotation = rotation;

    }
    void MoveNormal()
    {
        float speed = 0;
        Vector3 inputDir = new Vector3(input.move.x, 0, input.move.y);
        float targetRotation = 0;

        if (input.move != Vector2.zero)
        {
            if (input.sprint)
            {
                speed = sprintSpeed;
            }
            else
            {
                speed = moveSpeed;
            }
            targetRotation = Quaternion.LookRotation(inputDir).eulerAngles.y + mainCam.transform.rotation.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20 * Time.deltaTime);
            if (input.sprint)
            {
                animator.SetFloat("speed", 2);

            }
            else
            {
                animator.SetFloat("speed", input.move.magnitude);

            }
        }
        else
        {
            if (input.sprint)
            {
                animator.SetFloat("speed", 2);

            }
            else
            {
                animator.SetFloat("speed",0);

            }

        }

       
        Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
        controller.Move(targetDirection * speed * Time.deltaTime);
    }

    void MoveBall()
    {
        float speed = 0;
        Vector3 inputDir = new Vector3(input.move.x, 0, input.move.y);
        float targetRotation = 0;

        if (input.move != Vector2.zero)
        {
            speed = rollSpeed;
            targetRotation = Quaternion.LookRotation(inputDir).eulerAngles.y + mainCam.transform.rotation.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20 * Time.deltaTime);
        }


        animator.SetFloat("rollSpeed", input.move.magnitude);
        Vector3 targetDirection = Quaternion.Euler(0, targetRotation, 0) * Vector3.forward;
        controller.Move(targetDirection * speed * Time.deltaTime);
    }
    void SwitchMode()
    {
        if (input.switchMode)
        {
            ballMode = !ballMode;
            input.switchMode = false;
            animator.SetBool("ballMode", ballMode);

        }

    }
    void JumpAndGravity()
    {
        // Allow jump if jump count is less than maxJumps
        if (input.jump && jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2 * -gravity);
            jumpCount++;
            input.jump = false;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the character
        controller.Move(velocity * Time.deltaTime);

        // Reset jump count when grounded
        if (controller.isGrounded)
        {
            jumpCount = 0;
        }
    }
 
}

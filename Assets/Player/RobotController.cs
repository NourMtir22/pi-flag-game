using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class RobotController : NetworkBehaviour
{
    [SerializeField] float moveSpeed = 8;
    [SerializeField] float sprintSpeed = 14;
    [SerializeField] float rollSpeed = 13;
    [SerializeField] float gravity = -10;
    [SerializeField] float jumpHeight = 3;

    bool isGrounded;
    Vector3 velocity;

    private bool ballMode = false;
    private PlayerInputsManager input;
    private CharacterController controller;
    private Animator animator;

    [SerializeField] Transform cameraFollowTarget;

    float xRotation;
    float yRotation;
    private int jumpCount = 0; // Tracks the number of jumps
    private int maxJumps = 2;  // Maximum number of jumps allowed

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing!");
        }

        if (isLocalPlayer)
        {
            input = GetComponent<PlayerInputsManager>();

            if (input == null)
            {
                Debug.LogError("PlayerInputsManager component is missing!");
            }

            // Find the UI Canvas in the scene and assign the PlayerInputsManager
            UICanvasControllerInput canvasController = FindObjectOfType<UICanvasControllerInput>();

            if (canvasController != null)
            {
                canvasController.SetPlayerInputsManager(input);
            }
            else
            {
                Debug.LogError("UICanvasControllerInput not found in the scene!");
            }

            // Set up Cinemachine to follow the player
            CinemachineVirtualCamera cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cinemachineCam.Follow = cameraFollowTarget;
                cinemachineCam.LookAt = cameraFollowTarget;
            }
            else
            {
                Debug.LogError("CinemachineVirtualCamera not found in the scene!");
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;  // Ensure this runs only for the local player

        SwitchMode();
        if (ballMode)
            MoveBall();
        else
            MoveNormal();

        JumpAndGravity();
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;  // Ensure this runs only for the local player

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
            speed = input.sprint ? sprintSpeed : moveSpeed;
            targetRotation = Quaternion.LookRotation(inputDir).eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 20 * Time.deltaTime);

            animator.SetFloat("speed", input.sprint ? 2 : input.move.magnitude);
        }
        else
        {
            animator.SetFloat("speed", input.sprint ? 2 : 0);
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
            targetRotation = Quaternion.LookRotation(inputDir).eulerAngles.y;
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
        if (input.jump && jumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2 * -gravity);
            jumpCount++;
            input.jump = false;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            jumpCount = 0;
        }
    }
}

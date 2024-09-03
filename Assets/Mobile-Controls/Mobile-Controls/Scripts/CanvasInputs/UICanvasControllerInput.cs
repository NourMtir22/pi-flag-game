using UnityEngine;

public class UICanvasControllerInput : MonoBehaviour
{
    [Header("Output")]
    public PlayerInputsManager inputs;

    // Method to assign the PlayerInputsManager from the player script
    public void SetPlayerInputsManager(PlayerInputsManager playerInputsManager)
    {
        inputs = playerInputsManager;

        if (inputs == null)
        {
            Debug.LogError("PlayerInputsManager not found for the local player!");
        }
    }

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        if (inputs != null)
        {
            inputs.move = virtualMoveDirection;
        }
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        if (inputs != null)
        {
            inputs.look = virtualLookDirection;
        }
    }

    public void VirtualJumpInput(bool virtualJumpState)
    {
        if (inputs != null)
        {
            inputs.jump = virtualJumpState;
        }
    }

    public void VirtualSprintInput(bool virtualSprintState)
    {
        if (inputs != null)
        {
            inputs.sprint = virtualSprintState;
        }
    }

    public void VirtualSwitchInput(bool virtualSwitchState)
    {
        if (inputs != null)
        {
            inputs.switchMode = virtualSwitchState;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;
    private static InputManager instance;
    
    public static InputManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake() 
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        playerControls = new PlayerControls();

        Cursor.visible = false;
    }

    private void OnEnable() 
    {
        playerControls.Enable();    
    }

    private void OnDisable() 
    {
        playerControls.Disable();    
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseMovement()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool IsJumping()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool IsCrouching()
    {
        return playerControls.Player.Crouch.triggered;
    }

    public bool IsAttacking()
    {
        return playerControls.Player.Attack.triggered;
    }


}

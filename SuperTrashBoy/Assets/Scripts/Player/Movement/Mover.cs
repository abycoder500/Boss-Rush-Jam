using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider capsuleCollider;

    private Vector3 movementVelocity;
    private float movementSpeed = 0f;
    private float maxSpeed;
    private bool isGrounded;
    private bool isJumping = false;
    private bool isCrouched = false;
    private Vector3 movementIntertia;
    private float startColliderHeight;
    private float startColliderYPosition;
    private float startControllerHeight;
    private float startControllerYPosition;

    private float jumpTimer = 0f;

    [SerializeField] private Transform playerBody;
    [SerializeField] private float maxMovementSpeed = 30f;
    [SerializeField] private float maxCrouchSpeed = 10f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 1f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float jumpTimeLength = 1f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float crouchAmountFraction = 0.5f;
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private double knockBackAngle = 30f;

    [SerializeField] AnimationCurve jumpCurve;

    private void Awake() 
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();  
        maxSpeed = maxMovementSpeed; 
    }

    private void Start() 
    {
        startColliderHeight = capsuleCollider.height;
        startColliderYPosition = capsuleCollider.center.y;
        startControllerHeight = controller.height;
        startControllerYPosition = controller.center.y;    
    }

    private void Update()
    {
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float time = jumpTimer / jumpTimeLength;
            movementVelocity.y = jumpCurve.Evaluate(time) * jumpHeight;
            if (time > 1f)
            {
                isJumping = false;
                jumpTimer = 0f;
            }
        }
        controller.Move(movementVelocity * Time.deltaTime);
    }

    public void Move(InputManager inputManager, Transform cameraTransform, bool knocked)
    {
        playerBody.forward = cameraTransform.forward;
        if(knocked) return;
        isGrounded = controller.isGrounded;

        if (isGrounded && movementVelocity.y < 0 && !isJumping)
        {
            movementVelocity.y = 0f;
        }

        Vector2 playerMovement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(playerMovement.x, 0, playerMovement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        move.Normalize();

        if(move.sqrMagnitude >= Mathf.Epsilon)
        {
            movementSpeed = Mathf.Lerp(movementSpeed, maxSpeed, Time.deltaTime * acceleration);
            controller.Move(move * Time.deltaTime * movementSpeed);
            movementIntertia = move;
        }
        else
        {
            movementSpeed = Mathf.Lerp(movementSpeed, 0f, Time.deltaTime * deceleration);
            controller.Move(movementIntertia * Time.deltaTime * movementSpeed);
        }
    }

    public void Jump()
    {
        if(movementVelocity.y != 0f) return;
        if(isJumping) return;
        if(isCrouched) return;
        isJumping = true;
    }

    public void ApplyGravity()
    {
        if(isJumping) return;
        movementVelocity.y += gravityValue * Time.deltaTime;
    }

    public void Crouch()
    {
        if(isCrouched)
        {
            maxSpeed = maxMovementSpeed;
            isCrouched = false;

            capsuleCollider.height = startColliderHeight;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, startColliderYPosition, capsuleCollider.center.z);

            controller.height = startControllerHeight;
            controller.center = new Vector3(controller.center.x, startControllerYPosition, controller.center.z);
           
        }
        else
        {
            maxSpeed = maxCrouchSpeed;
            isCrouched = true;

            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y - (startColliderHeight - capsuleCollider.height) / 2, capsuleCollider.center.z);
            capsuleCollider.height = startColliderHeight * (1 - crouchAmountFraction);

            controller.center = new Vector3(controller.center.x, controller.center.y - (startControllerHeight - controller.height) / 2, controller.center.z);
            controller.height = startControllerHeight * (1 - crouchAmountFraction);
        }
    }

    public void TakeKnockBack(Vector3 direction)
    {
        Vector3 knockbackDir = direction;
        knockbackDir.y = 0f;
        knockbackDir.Normalize();
        knockbackDir.y = (float)Math.Sin(knockBackAngle * Mathf.PI / 180);
        movementVelocity = knockBackForce*knockbackDir;
    }

      public void Stop()
    {
        movementVelocity.x = 0f;
        movementVelocity.z = 0f;
    }

    public Vector3 GetVelocity()
    {
        return movementVelocity;
    }
}

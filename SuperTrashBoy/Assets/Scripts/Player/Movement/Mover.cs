using System;
using System.Collections;
using System.Collections.Generic;
using Armageddump.Inputs;
using UnityEngine;

public class Mover : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider capsuleCollider;

    //private Vector3 movementVelocity;
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
    [SerializeField] private float accelerationTime = 0.3f;
    [SerializeField] private float decelerationTime = 0.2f;
    [SerializeField] private float maxCrouchSpeed = 10f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float crouchAmountFraction = 0.5f;
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private double knockBackAngle = 30f;
    [SerializeField] private float bodyRotationVelocity = 2f;
    [SerializeField] private CinemachinePOVExtension ext;

    [SerializeField] AnimationCurve accelerationCurve;
    [SerializeField] AnimationCurve decelerationCurve;

    private Vector2 windForce;
    private Vector2 horizontalVelocity = Vector2.zero;
    private float accelerationTimer = 0f;
    private float decelerationTimer = 0f;
    private Transform cameraTransform;

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

        cameraTransform = Camera.main.transform;
    }

    public Vector2 Move(Vector2 moveInputs)
    {
        Vector3 moveDirection = cameraTransform.forward * moveInputs.y + cameraTransform.right * moveInputs.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();
        if(moveInputs.sqrMagnitude >= Mathf.Epsilon)
        {
            decelerationTimer = 0f;
            accelerationTimer += Time.deltaTime;
            movementIntertia = moveDirection;

            float time = accelerationTimer/accelerationTime;
            movementSpeed = accelerationCurve.Evaluate(time) * maxSpeed;
            if(time >= 1)
            {
                movementSpeed = maxSpeed;
                accelerationTimer = accelerationTime;
            }
        }
        else
        {
            accelerationTimer = 0f;
            decelerationTimer += Time.deltaTime;
            moveDirection = movementIntertia;

            float time = decelerationTimer/decelerationTime;
            movementSpeed = decelerationCurve.Evaluate(time) * maxSpeed;
            if (time >= 1)
            {
                movementSpeed = 0f;
                decelerationTimer = decelerationTime;
            }
        }

        Vector2 movementVelocity = new Vector2(moveDirection.x, moveDirection.z);
        movementVelocity *= movementSpeed;
        
        return movementVelocity;
    }

    public void SetMaxSpeed(float newSpeed)
    {
        maxSpeed =  newSpeed;
    }

    public void ReduceSpeedByFraction(float fraction)
    {
        maxSpeed *= fraction;
    }

    public void ResetMaxSpeed()
    {
        maxSpeed = maxMovementSpeed;
    }
}

   
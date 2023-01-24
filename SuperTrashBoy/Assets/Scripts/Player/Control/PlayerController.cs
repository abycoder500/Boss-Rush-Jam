using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Fighter fighter;
    private Mover mover;
    private Vector3 playerVelocity;

    private InputManager inputManager;
    
    private bool isGrounded;
    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
    }
    
    private void Start() 
    {
        inputManager = InputManager.Instance;    
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        mover.Move(inputManager, cameraTransform);

        if(inputManager.IsJumping()) mover.Jump();
        if(inputManager.IsCrouching()) mover.Crouch();
        if(inputManager.IsAttacking()) fighter.Attack();

        mover.ApplyGravity();
    }
}

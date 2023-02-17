using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float knockBackDuration = 0.4f;
    private CharacterController controller;
    private Fighter fighter;
    private Mover mover;
    private Health health;
    private Vector3 playerVelocity;

    private InputManager inputManager;
    
    private bool isGrounded;
    private bool knocked = false;

    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mover = GetComponent<Mover>();
        fighter = GetComponent<Fighter>();
        health = GetComponent<Health>();
    }
    
    private void Start() 
    {
        inputManager = InputManager.Instance;    
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable() 
    {
        health.onTakeDamage += TakeKnockBack;   
        inputManager.enabled = true; 
    }

    private void OnDisable() 
    {
        inputManager.enabled = false;
    }

    void Update()
    {
        mover.Move(inputManager, cameraTransform, knocked);

        if(knocked) return;
        if(inputManager.IsJumping()) mover.Jump();
        if(inputManager.IsCrouching()) mover.Crouch();
        if(inputManager.IsAttackingMelee()) fighter.Attack();
        if(inputManager.IsAttackingRanged()) fighter.RangeAttack();

        mover.ApplyGravity();
    }

    public void TakeKnockBack(float damage, Transform damager)
    {
        Debug.Log("Knock");
        knocked = true;
        Vector3 knockBackDirection = transform.position - damager.position;
        mover.TakeKnockBack(knockBackDirection);
        Invoke("ResetKnockBack", knockBackDuration);
    }

    private void ResetKnockBack()
    {
        mover.Stop();
        knocked = false;
    }
}

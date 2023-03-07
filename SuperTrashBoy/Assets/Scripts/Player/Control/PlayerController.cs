using System.Collections;
using System.Collections.Generic;
using Armageddump.Inputs;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float knockBackDuration = 0.4f;

    private CharacterController controller;

    private Fighter fighter;
    private Mover mover;
    private Jumper jumper;
    private Croucher croucher;
    private Gravity gravity;
    private ForceReceiver forceReceiver;

    private Health health;
    private Vector3 playerVelocity;

    private InputManager inputManager;

    private bool isGrounded;
    private bool knocked = false;

    private Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        fighter = GetComponent<Fighter>();
        mover = GetComponent<Mover>();
        croucher = GetComponent<Croucher>();
        jumper = GetComponent<Jumper>();
        gravity = GetComponent<Gravity>();
        forceReceiver = GetComponent<ForceReceiver>();

        health = GetComponent<Health>();

        inputManager = InputManager.Instance;
    }

    private void Start()
    { 
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if(inputManager == null) inputManager = InputManager.Instance;

        health.onTakeDamage += TakeKnockBack;

        inputManager.onJumpStarted += Jump;
        inputManager.onCrouchStarted += Crouch;
        inputManager.onMainAttackStarted += Attack;
        inputManager.onSecondaryAttackStarted += RangeAttack;
    }

    private void OnDisable()
    {
        if (inputManager == null) inputManager = InputManager.Instance;

        health.onTakeDamage -= TakeKnockBack;

        inputManager.onJumpStarted -= jumper.Jump;
        inputManager.onCrouchStarted -= croucher.Crouch;
        inputManager.onMainAttackStarted -= fighter.Attack;
        inputManager.onSecondaryAttackStarted -= fighter.RangeAttack;
    }

    void Update()
    {
        Vector3 movementVelocity = Vector3.zero;
        Vector2 horizontalVelocity = Vector2.zero;

        horizontalVelocity = mover.Move(inputManager.GetPlayerMovement());
        movementVelocity.x += horizontalVelocity.x;
        movementVelocity.z += horizontalVelocity.y;

        movementVelocity.y += jumper.GetVerticalVelocity();

        if(!jumper.IsJumping()) movementVelocity.y += gravity.GetGravity();

        movementVelocity += forceReceiver.GetForce();

        
        controller.Move(movementVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        if(croucher.IsCrouched()) return;

        jumper.Jump();
    }

    private void Crouch()
    {
        croucher.Crouch();
    }

    private void Attack()
    {
        fighter.Attack();
    }

    private void RangeAttack()
    {
        fighter.RangeAttack();
    }

    public void TakeKnockBack(float damage, Transform damager)
    {
        Debug.Log("Knock");
        knocked = true;
        Vector3 knockBackDirection = transform.position - damager.position;
        //mover.TakeKnockBack(knockBackDirection);
        Invoke("ResetKnockBack", knockBackDuration);
    }

    private void ResetKnockBack()
    {
        //mover.Stop();
        knocked = false;
    }
}

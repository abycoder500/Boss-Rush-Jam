using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Armageddump.Inputs
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager instance;

        private PlayerControls playerControls;
        
        private InputAction move;
        private InputAction look;

        public event Action onJumpStarted;
        public event Action onJumpReleased;
        public event Action onMainAttackStarted;
        public event Action onMainAttackReleased;
        public event Action onSecondaryAttackStarted;
        public event Action onSecondaryAttackReleased;
        public event Action onCrouchStarted;
        public event Action onCrouchReleased;
        
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
        }

        private void OnEnable() 
        {
            move = playerControls.Player.Move;
            look = playerControls.Player.Look;

            playerControls.Player.Jump.started += StartJump;
            playerControls.Player.Jump.canceled += ReleaseJump;
            playerControls.Player.MainAttack.started += StartMainAttack;
            playerControls.Player.MainAttack.canceled += ReleaseMainAttack;
            playerControls.Player.SecondaryAttack.started += StartSecondaryAttack;
            playerControls.Player.SecondaryAttack.canceled += ReleaseSecondaryAttack;
            playerControls.Player.Crouch.started += StartCrouch;
            playerControls.Player.Crouch.canceled += ReleaseCrouch;

            playerControls.Player.Enable();    
        }

        private void OnDisable()
        {
            playerControls.Player.Jump.started -= StartJump;
            playerControls.Player.Jump.canceled -= ReleaseJump;
            playerControls.Player.MainAttack.started -= StartMainAttack;
            playerControls.Player.MainAttack.canceled -= ReleaseMainAttack;
            playerControls.Player.SecondaryAttack.started -= StartSecondaryAttack;
            playerControls.Player.SecondaryAttack.canceled -= ReleaseSecondaryAttack;
            playerControls.Player.Crouch.started -= StartCrouch;
            playerControls.Player.Crouch.canceled -= ReleaseCrouch;

            playerControls.Player.Disable();
        }

        private void StartJump(InputAction.CallbackContext obj)
        {
            onJumpStarted?.Invoke();
        }

        private void ReleaseJump(InputAction.CallbackContext obj)
        {
            onJumpReleased?.Invoke();
        }

        private void StartMainAttack(InputAction.CallbackContext obj)
        {
            onMainAttackStarted?.Invoke();
        }

        private void ReleaseMainAttack(InputAction.CallbackContext obj)
        {
            onMainAttackReleased?.Invoke();
        }

        private void StartSecondaryAttack(InputAction.CallbackContext obj)
        {
            onSecondaryAttackStarted?.Invoke();
        }

        private void ReleaseSecondaryAttack(InputAction.CallbackContext obj)
        {
            onSecondaryAttackReleased?.Invoke();
        }

        private void StartCrouch(InputAction.CallbackContext obj)
        {
            onCrouchStarted?.Invoke();
        }

        private void ReleaseCrouch(InputAction.CallbackContext obj)
        {
            onCrouchReleased?.Invoke();
        }

       

        public Vector2 GetPlayerMovement()
        {
            return move.ReadValue<Vector2>();
        }

        public Vector2 GetMouseMovement()
        {
            return look.ReadValue<Vector2>();
        }

        /*public bool IsJumping()
        {
            return playerControls.Player.Jump.triggered;
        }

        public bool IsCrouching()
        {
            return playerControls.Player.Crouch.triggered;
        }

        public bool IsAttackingMelee()
        {
            return playerControls.Player.MeleeAttack.triggered;
        }

        public bool IsAttackingRanged()
        {
            return playerControls.Player.RangeAttack.triggered;
        }*/

        public bool IsPausing()
        {
            return playerControls.Player.Pause.triggered;
        }

    }
}
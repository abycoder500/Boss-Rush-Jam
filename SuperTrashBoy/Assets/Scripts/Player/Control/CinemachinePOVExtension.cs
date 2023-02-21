using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Armageddump.Inputs;

public class CinemachinePOVExtension : CinemachineExtension
{
    public event Action onCameraRotation;
    private InputManager inputManager;
    private Vector3 startingRotation;

    [SerializeField] private float clampAngleUp = 80f;
    [SerializeField] private float clampAngleDown = 80f;
    [SerializeField] private float horizontalRotationVelocity= 10f;
    [SerializeField] private float verticalRotationVelocity = 10f;

    protected override void Awake() 
    {
        inputManager = InputManager.Instance;
        base.Awake();
    }

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (inputManager == null) return;
        
        if(vcam.Follow)
        {
            if(stage == CinemachineCore.Stage.Aim)
            {
                if (startingRotation == null) startingRotation = transform.localRotation.eulerAngles;
                Vector2 mouseDelta =  inputManager.GetMouseMovement();

                startingRotation.x += mouseDelta.x * verticalRotationVelocity * Time.deltaTime;
                startingRotation.y += mouseDelta.y * horizontalRotationVelocity * Time.deltaTime;

                startingRotation.y = Mathf.Clamp(startingRotation.y, - clampAngleDown, clampAngleUp);

                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);
                onCameraRotation?.Invoke();
            }
        }
    }
}

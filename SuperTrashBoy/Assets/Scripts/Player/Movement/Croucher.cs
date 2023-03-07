using UnityEngine;

public class Croucher : MonoBehaviour 
{
    [SerializeField] private float crouchAmountFraction = 0.5f;
    [SerializeField] private float movementSpeedFractionWhenCrouched = 0.5f;

    private CapsuleCollider capsuleCollider;
    private CharacterController controller;
    private Mover mover;

    private float startColliderHeight;
    private float startColliderYPosition;
    private float startControllerHeight;
    private float startControllerYPosition;

    private bool isCrouched;

    private void Awake() 
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        controller = GetComponent<CharacterController>();
        mover = GetComponent<Mover>();
    }

    private void Start()
    {
        startColliderHeight = capsuleCollider.height;
        startColliderYPosition = capsuleCollider.center.y;
        startControllerHeight = controller.height;
        startControllerYPosition = controller.center.y;
    }

    public void Crouch()
    {
        if (isCrouched)
        {
            mover.ResetMaxSpeed();
            isCrouched = false;

            capsuleCollider.height = startColliderHeight;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, startColliderYPosition, capsuleCollider.center.z);

            controller.height = startControllerHeight;
            controller.center = new Vector3(controller.center.x, startControllerYPosition, controller.center.z);

        }
        else
        {
            mover.ReduceSpeedByFraction(movementSpeedFractionWhenCrouched);
            isCrouched = true;

            capsuleCollider.center = new Vector3(capsuleCollider.center.x, capsuleCollider.center.y - (startColliderHeight - capsuleCollider.height) / 2, capsuleCollider.center.z);
            capsuleCollider.height = startColliderHeight * (1 - crouchAmountFraction);

            controller.center = new Vector3(controller.center.x, controller.center.y - (startControllerHeight - controller.height) / 2, controller.center.z);
            controller.height = startControllerHeight * (1 - crouchAmountFraction);
        }
    }

    public bool IsCrouched()
    {
        return isCrouched;
    }
}
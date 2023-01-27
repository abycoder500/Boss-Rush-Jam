using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyFighter : MonoBehaviour
{
    [SerializeField] HitBox clubHitBox;
    [Header("Jump attack settings")]
    [SerializeField] AnimationCurve jumpAttackCurve;
    [SerializeField] float jumpHeight;
    [SerializeField] float phase1JumpTimeLength;
    [SerializeField] float phase1JumpAttackHorizontalVelocity;
    [SerializeField] float phase2JumpTimeLength;
    [SerializeField] float phase2JumpAttackHorizontalVelocity;
    [SerializeField] float phase3JumpTimeLength;
    [SerializeField] float phase3JumpAttackHorizontalVelocity;
    [Space]
    [Space]

    [Header("Dash Attack Settings")]
    //[SerializeField] AnimationCurve stepBackCurve;
    [SerializeField] float phase1StepAwayTimeLength = 1f;
    [SerializeField] float phase1StepAwayVelocity = 3f;
    [SerializeField] float phase1DashDuration = 3f;
    [SerializeField] float phase1DashVelocity = 8f;
    [SerializeField] float phase2StepAwayTimeLength = 1f;
    [SerializeField] float phase2StepAwayVelocity = 3f;
    [SerializeField] float phase2DashDuration = 3f;
    [SerializeField] float phase2DashVelocity = 8f;
    [SerializeField] float phase3StepAwayTimeLength = 1f;
    [SerializeField] float phase3StepAwayVelocity = 3f;
    [SerializeField] float phase3DashDuration = 3f;
    [SerializeField] float phase3DashVelocity = 8f;
    [Space]
    [Space]

    [Header("Spin Attack Settings")]
    //[SerializeField] AnimationCurve stepBackCurve;
    [SerializeField] float spinAttackTimeLength = 8f;
    [SerializeField] float spinAttackVelocity = 3f;
    [Space]
    [Space]

     [Header("Club Attack Settings")]
    //[SerializeField] AnimationCurve stepBackCurve;
    [SerializeField] float clubAttackTimeLength = 3f;
    [SerializeField] float hitboxActivationTime = 0.5f;
    [SerializeField] float hitboxDeactivationTime = 2.5f;
    [SerializeField] float clubAttackDamage = 3f;
    [Space]
    [Space]

    private CharacterController controller;
    private bool isAttacking = false;
    private bool isJumping = false;
    private bool isDashing = false;
    private bool isStepping = false;
    private bool isSpinning = false;
    private bool isUsingClub = false;

    private float jumpTimeLength;
    private float jumpAttackHorizontalVelocity;
   
    private float stepAwayTimeLength = 1f;
    private float stepAwayVelocity = 3f;
    private float dashDuration = 3f;
    private float dashVelocity = 8f;

    private float attackTimer = 0f;
    private Vector3 movementVelocity;
    private Vector3 moveDirection;

    private GameObject player;

    private int stage = 1;

    private void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = GetComponent<CharacterController>();    
    }

    private void Start() 
    {
        jumpTimeLength = phase1JumpTimeLength;
        jumpAttackHorizontalVelocity = phase1JumpAttackHorizontalVelocity;

        dashDuration = phase1DashDuration;
        dashVelocity = phase1DashVelocity;

        stepAwayTimeLength = phase1StepAwayTimeLength;
        stepAwayVelocity = phase1StepAwayVelocity;    

        clubHitBox.gameObject.SetActive(false);
    }

    private void Update() 
    {
        Debug.Log(isSpinning);
        if(isAttacking) attackTimer += Time.deltaTime;

        if (isJumping)
        {    
            float time = attackTimer / jumpTimeLength;
            movementVelocity.y = jumpAttackCurve.Evaluate(time) * jumpHeight;
            movementVelocity.x = moveDirection.x * jumpAttackHorizontalVelocity;
            movementVelocity.z = moveDirection.z * jumpAttackHorizontalVelocity;
            if (time > 1f)
            {
                isJumping = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
            }
        }

        if(isStepping)
        {
            float time = attackTimer / stepAwayTimeLength;
            movementVelocity.x = moveDirection.x * stepAwayVelocity;
            movementVelocity.z = moveDirection.z * stepAwayVelocity;
            if (time > 1f)
            {
                isStepping = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
            }
        }

        if (isDashing)
        {
            float time = attackTimer / dashDuration;
            movementVelocity.x = moveDirection.x * dashVelocity;
            movementVelocity.z = moveDirection.z * dashVelocity;
            if (time > 1f)
            {
                isDashing = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
            }
        }

        if(isSpinning)
        {
            float time = attackTimer / spinAttackTimeLength;
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            movementVelocity.x = direction.x * spinAttackVelocity;
            movementVelocity.z = direction.z * spinAttackVelocity;
            if (time > 1f)
            {
                isSpinning = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
            }
        }

        if(isUsingClub)
        {
            isUsingClub = false;
            StartCoroutine(ClubRoutine());
        }

        movementVelocity.y += -9.81f * Time.deltaTime;
        if (controller.isGrounded && movementVelocity.y < 0 && !isJumping)
        {
            movementVelocity.y = 0f;
        }

        controller.Move(movementVelocity * Time.deltaTime);
    }

    private IEnumerator ClubRoutine()
    {
        clubHitBox.SetupHitBox(this.gameObject, clubAttackDamage);
        clubHitBox.gameObject.SetActive(false);
        yield return new WaitForSeconds(hitboxActivationTime);
        clubHitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitboxDeactivationTime-hitboxActivationTime);
        clubHitBox.gameObject.SetActive(false);
        yield return new WaitForSeconds(clubAttackTimeLength - hitboxDeactivationTime-hitboxActivationTime);
        AttackFinished();
    }

    public void JumpForwardAttack(Vector3 direction)
    {   
        if(isAttacking) return;
        isAttacking = true;
        isJumping = true;
        direction.y = 0f;
        moveDirection = direction.normalized;
    }

    public void StepAway(Vector3 direction)
    {
        if(isAttacking) return;
        isAttacking = true;
        isStepping = true;
        direction.y = 0f;
        moveDirection = direction.normalized;
    }

    public void DashAttack(Vector3 direction)
    {
        if(isAttacking) return;
        isAttacking = true;
        isDashing = true;
        direction.y = 0f;
        moveDirection = direction.normalized;
    }

    public void SpinAttack(Vector3 direction)
    {
        if(isAttacking) return;
        isAttacking = true;
        isSpinning = true;
    }

    public void ClubAttack()
    {
        if(isAttacking) return;
        isAttacking = true;
        isUsingClub = true;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    private void AttackFinished()
    {
       isAttacking = false;
    }

    public void AdvanceStage()
    {
        stage ++;
        if(stage == 2)
        {
             jumpTimeLength = phase1JumpTimeLength;
            jumpAttackHorizontalVelocity = phase2JumpAttackHorizontalVelocity;

            dashDuration = phase2DashDuration;
            dashVelocity = phase2DashVelocity;

            stepAwayTimeLength = phase2StepAwayTimeLength;
            stepAwayVelocity = phase2StepAwayVelocity;  
        }
        if(stage == 3)
        {
            jumpTimeLength = phase3JumpTimeLength;
            jumpAttackHorizontalVelocity = phase3JumpAttackHorizontalVelocity;

            dashDuration = phase3DashDuration;
            dashVelocity = phase3DashVelocity;

            stepAwayTimeLength = phase3StepAwayTimeLength;
            stepAwayVelocity = phase3StepAwayVelocity;  
        }
    }

    public HitBox GetHitBox()
    {
        return clubHitBox;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyFighter : MonoBehaviour
{
    [SerializeField] private SFXObject attackSFXPrefab;
    [SerializeField] private SFXObject preAttackSFXPrefab;
    [SerializeField] HitBox clubHitBox;
    [SerializeField] GameObject barrelPrefab;
    [SerializeField] Transform barrelSpawnPoint;
    [SerializeField] Animator animator;
    [SerializeField] float lookRotationVelocity = 1f;
    [Header("Jump attack settings")]
    [SerializeField] AnimationCurve jumpAttackCurve;
    [SerializeField] float jumpHeight;
    [SerializeField] float phase1JumpTimeLength;
    [SerializeField] float phase1JumpAttackHorizontalVelocity;
    [SerializeField] float phase2JumpTimeLength;
    [SerializeField] float phase2JumpAttackHorizontalVelocity;
    [SerializeField] float phase3JumpTimeLength;
    [SerializeField] float phase3JumpAttackHorizontalVelocity;
    [SerializeField] float clubBehindBackTimeFraction = 0.1f;
    [SerializeField] float clubAttackTimeFraction = 0.8f;
    [SerializeField] float jumpAttackDamage = 10f;
    [Space]
    [Space]

    [Header("Dash Attack Settings")]
    [SerializeField] AnimationCurve stepBackCurve;
    [SerializeField] AnimationCurve dashCurve;
    [SerializeField] HitBox dashAttackHitBox;
    [SerializeField] float dashAttackDamage;
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

    [Header("Barrel Hit Attack Settings")]
    [SerializeField] float barrelLaunchForce = 10f;
    [SerializeField] float barrelHitForce = 20f;
    [SerializeField] float hitBarrelWaitTime = 1f;
    [SerializeField] float barrelLifeTime = 5f;
    [SerializeField] float barrelDamage = 10f;

    [Header("Move Towards Player Attack Settings")]
    [SerializeField] float moveTowardsPlayerAttackVelocity = 20;
    [SerializeField] float distanceToPlayerToAttack = 2f;
    [SerializeField] float moveTowardsPlayerAttackDuration = 0.5f;
    [SerializeField] float moveAttackDamage = 10f;



    private CharacterController controller;
    private bool isAttacking = false;
    private bool isJumping = false;
    private bool isJumpClubBehind = false;
    private bool isJumpClubUsed = false;
    private bool isDashing = false;
    private bool isStepping = false;
    private bool isSpinning = false;
    private bool isUsingClub = false;
    private bool hitBarrel = true;
    private bool isMovingTowardsPlayer = false;
    private bool isAttackingAfterMoving = false;

    private bool lookAtPlayer = false;
    private bool hitPlayer = false;

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

    public event Action onBarrelHit;

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
        dashAttackHitBox.gameObject.SetActive(false);
    }

    private void Update() 
    {
        if(lookAtPlayer) LookPlayer(true);

        if(isAttacking) attackTimer += Time.deltaTime;

        if (isJumping)
        {    
            float time = attackTimer / jumpTimeLength;
            movementVelocity.y = jumpAttackCurve.Evaluate(time) * jumpHeight;
            movementVelocity.x = moveDirection.x * jumpAttackHorizontalVelocity;
            movementVelocity.z = moveDirection.z * jumpAttackHorizontalVelocity;
            if(time > clubBehindBackTimeFraction && !isJumpClubBehind)
            {
                Instantiate(preAttackSFXPrefab, transform.position, Quaternion.identity);
                isJumpClubBehind = true;
                animator.SetTrigger("ClubBack");
                clubHitBox.SetupHitBox(this.gameObject, jumpAttackDamage);
                clubHitBox.gameObject.SetActive(true);
  
            }
            if(time > clubAttackTimeFraction && !isJumpClubUsed)
            {
                Instantiate(attackSFXPrefab, transform.position, Quaternion.identity);
                isJumpClubUsed = true;
                animator.SetTrigger("ClubAttack");
                clubHitBox.gameObject.SetActive(true);
  
            }
            if (time > 1f)
            {
                isJumping = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
                isJumpClubUsed = false;
                isJumpClubBehind = false;
                Invoke("DeactivateHitBox", 0.4f);
            }
        }

        if(isStepping)
        {
            float time = attackTimer / stepAwayTimeLength;
            movementVelocity.x = moveDirection.x * stepAwayVelocity * stepBackCurve.Evaluate(time);
            movementVelocity.z = moveDirection.z * stepAwayVelocity * stepBackCurve.Evaluate(time);
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
            StartCoroutine(DashRoutine());
            float time = attackTimer / dashDuration;
            movementVelocity.x = moveDirection.x * dashVelocity * dashCurve.Evaluate(time);
            movementVelocity.z = moveDirection.z * dashVelocity * dashCurve.Evaluate(time);
            if (time > 1f || hitPlayer)
            {
                dashAttackHitBox.gameObject.SetActive(false);
                isDashing = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
            }
        }

        if(isSpinning)
        {
            //LookPlayer(false);
            float time = attackTimer / spinAttackTimeLength;
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            movementVelocity.x = direction.x * spinAttackVelocity;
            movementVelocity.z = direction.z * spinAttackVelocity;
            if (time > 1f || hitPlayer)
            {
                isSpinning = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
                clubHitBox.onHit -= HitPlayer;
                clubHitBox.gameObject.SetActive(false);
            }
        }

        if(isUsingClub)
        {
            isUsingClub = false;
            StartCoroutine(ClubRoutine());
        }

        if(isMovingTowardsPlayer)
        {
            Vector3 direction = player.transform.position - transform.position;
            LookPlayer(true);
            direction.y = 0f;
            direction.Normalize();
            movementVelocity.x = direction.x * moveTowardsPlayerAttackVelocity;
            movementVelocity.z = direction.z * moveTowardsPlayerAttackVelocity;
            if(Vector3.Distance(transform.position, player.transform.position) <= distanceToPlayerToAttack)
            {
                Instantiate(preAttackSFXPrefab, transform.position, Quaternion.identity);
                animator.SetTrigger("MoveToPlayerAttack");
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
                clubHitBox.SetupHitBox(this.gameObject, moveAttackDamage);
                clubHitBox.gameObject.SetActive(true);
                attackTimer = 0f;
                isMovingTowardsPlayer = false;
                isAttackingAfterMoving = true;
            }
        }

        if(isAttackingAfterMoving)
        {
            float time = attackTimer / moveTowardsPlayerAttackDuration;
            if (time > 1f)
            {
                isAttackingAfterMoving = false;
                attackTimer = 0f;
                AttackFinished();
                movementVelocity.x = 0f;
                movementVelocity.z = 0f;
                Debug.Log("Finish");
                clubHitBox.gameObject.SetActive(false);
            }
        }

        movementVelocity.y += -9.81f * Time.deltaTime;
        if (controller.isGrounded && movementVelocity.y < 0 && !isJumping)
        {
            movementVelocity.y = 0f;
        }

        controller.Move(movementVelocity * Time.deltaTime);
    }

    private void DeactivateHitBox()
    {
        clubHitBox.gameObject.SetActive(false);
    }

    private IEnumerator ClubRoutine()
    {
        Instantiate(preAttackSFXPrefab, transform.position, Quaternion.identity);
        clubHitBox.SetupHitBox(this.gameObject, clubAttackDamage);
        clubHitBox.gameObject.SetActive(false);
        lookAtPlayer = true;
        animator.SetTrigger("ClubBack");
        yield return new WaitForSeconds(hitboxActivationTime);
        animator.SetTrigger("ClubAttack");
        Instantiate(attackSFXPrefab, transform.position, Quaternion.identity);
        clubHitBox.gameObject.SetActive(true);
        lookAtPlayer = false;
        yield return new WaitForSeconds(hitboxDeactivationTime-hitboxActivationTime);
        clubHitBox.gameObject.SetActive(false);
        yield return new WaitForSeconds(clubAttackTimeLength - hitboxDeactivationTime-hitboxActivationTime);
        AttackFinished();
    }

    private IEnumerator DashRoutine()
    {
        dashAttackHitBox.SetupHitBox(this.gameObject, dashAttackDamage);
        dashAttackHitBox.gameObject.SetActive(true);
        dashAttackHitBox.onHit += HitPlayer;
        yield return new WaitForSeconds(dashDuration);
        dashAttackHitBox.gameObject.SetActive(false);
        dashAttackHitBox.onHit -= HitPlayer;
    }

    public void LookPlayer(bool immediate)
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0f;
        lookDirection.Normalize();
        if(immediate) transform.forward = lookDirection;
        else transform.forward = Vector3.Lerp(transform.forward, lookDirection, Time.deltaTime*lookRotationVelocity);
    }

    public void MoveTowardsPlayerAttack()
    {
        if(isAttacking) return;
        isAttacking = true;
        isMovingTowardsPlayer = true;
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
        clubHitBox.SetupHitBox(this.gameObject, 10f);
        clubHitBox.gameObject.SetActive(true);
        clubHitBox.onHit += HitPlayer;
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
        Debug.Log("Advancing");
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

    public void LaunchBarrel()
    {
        if(isAttacking) return;
        hitBarrel = true;
        isAttacking = true;
        GameObject barrel = Instantiate(barrelPrefab, barrelSpawnPoint.position, Quaternion.identity);
        HitBox hitbox = barrel.GetComponentInChildren<HitBox>();
        hitbox.SetupHitBox(this.gameObject, barrelDamage);
        hitbox.gameObject.SetActive(true);
        hitbox.onHit += ResetRageOnBarrelHit;
        Destroy(barrel, barrelLifeTime);
        Rigidbody rb = barrel.GetComponent<Rigidbody>();
        rb.AddForce(barrelLaunchForce * Vector3.up, ForceMode.Impulse);
        StartCoroutine(HitBarrelRoutine(rb));
    }

    private void ResetRageOnBarrelHit(GameObject hitObject)
    {
        if(hitObject == player)
        {
            onBarrelHit?.Invoke();
        }
    }

    private void HitPlayer(GameObject hit)
    {
        if(hit == player)
        {
            hitPlayer = true;
        }
    }

    public void ResetHitPlayer()
    {
        hitPlayer = false;
    }

    public bool HasHitPlayer()
    {
        return hitPlayer;
    }

    public void DontHitBarrel()
    {
        hitBarrel = false;
    }

    private IEnumerator HitBarrelRoutine(Rigidbody barrelRB)
    {
        Instantiate(preAttackSFXPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(hitBarrelWaitTime);
        if(!hitBarrel)
        {
            hitBarrel = true;
            isAttacking = false;
            yield break;
        }
        Instantiate(attackSFXPrefab, transform.position, Quaternion.identity);
        Vector3 direction;
        direction = player.transform.position - transform.position;
        Vector3 playerVelocity = player.GetComponent<Mover>().GetVelocity();
        direction += playerVelocity * Time.deltaTime;
        direction.y = 0f;
        direction.Normalize();
        barrelRB.AddForceAtPosition(barrelHitForce * direction, barrelRB.transform.position + Vector3.down * 0.2f, ForceMode.Impulse);  
        isAttacking = false; 
    }

    public int GetCurrentStage()
    {
        return stage;
    }
}

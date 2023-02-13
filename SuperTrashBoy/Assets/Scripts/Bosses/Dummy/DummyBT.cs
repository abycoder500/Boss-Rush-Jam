using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTrashBoy.BehaviorTrees;
using System;

public class DummyBT : BTUser
{
    [Header("SFX")]
    [SerializeField] private SFXObject laughSFXPrefab;
    [SerializeField] private SFXObject enrageSFXPrefab;
    [SerializeField] private SFXObject enragePlusSFXPrefab;
    [SerializeField] private SFXObject attackSFXPrefab;
    [SerializeField] private SFXObject comeToLifeSFXPrefab;
    [SerializeField] private SFXObject deathSFXPrefab;
    [SerializeField] private SFXObject hitSFXPrefab;
    [SerializeField] private SFXObject moveSFXPrefab;
    [SerializeField] private SFXObject preAttackSFXPrefab;
    [Space]
    [Space]
    [Header("Other parameters")]
    [SerializeField] private string bossName = "The Dummy";
    [SerializeField] private WeaponUpgradePickable weaponUpgradePickablePrefab;
    [SerializeField] private ParticleSystem rageParticles = null;
    [SerializeField] private ParticleSystem aliveParticles = null;
    [SerializeField] private ParticleSystem weaponParticles = null;
    [SerializeField] private float aliveParticlesDuration = 1f;
    [SerializeField] private Animator animator;
    [SerializeField] private float enterTimeDuration = 2f;
    [SerializeField] private int numberOfHitsToLeaveIdle = 5;
    [SerializeField] private int rageLevelToAdvanceStage1 = 3;
    [SerializeField] private int rageLevelToAdvanceStage2 = 4;
    [SerializeField] private int rageLevelToAdvanceStage3 = 5;
    [SerializeField] private float enrageTimeLength = 1f;
    [SerializeField] private float projectileDetectionRange = 2f;
    [SerializeField] private float playerAttackDetectionRange = 2f;
    [SerializeField] private float waitTimeBetweenAttacks = 5f;
    [SerializeField] private float waitTimeBeforeSpin = 1f;
    [SerializeField] private float waitTimeAfterSpin = 3f;
    [SerializeField] private float distanceToTriggerMoveToPlayerAttack = 10f;
    [SerializeField] private float shieldTime = 0.4f;
    [SerializeField] private float timeOnGroundAfterSpin = 2.5f;
    [SerializeField] private float waitToBeHitTime = 2f;
    [SerializeField] private float laughTimeAfterHit = 2f;
    [SerializeField] private float deathTime = 2f;
    [SerializeField] private NotificationUI.Notification deathNotification;

    private HitReceivedCounter hitReceivedCounter;
    private Fighter playerFighter;
    private DummyFighter dummyFighter;
    private AudioManager audioManager;
    private NotificationUI notificationUI;

    private int rage = 0;
    private float enterTimer = 0f;
    private float waitToBeHitTimer = 0f;
    private float deathTimer = 0f;
    private bool isAttacking = false;
    private bool isHitAfterSpin = false;
    private bool isIdleSetUp = false;
    private bool entranceTriggered = false;
    private bool canAdvanceStage = true;
    private bool isDead = false;

    private BehaviorTree stage1DependencyCondition = new BehaviorTree("Stage1 dep");
    private BehaviorTree stage2DependencyCondition = new BehaviorTree("Stage2 dep");
    private BehaviorTree stage3DependencyCondition = new BehaviorTree("Stage3 dep");
    private BehaviorTree waitForAttackDependency = new BehaviorTree("Wait for attack dep");
    private BehaviorTree checkForHitAfterSpinTree = new BehaviorTree("Check for hit during spin");

    public event Action<string> onActivate;
    public event Action<float> onRageChange;
    public event Action onAlive;
    public event Action onDie;

    protected override void Awake() 
    {
        base.Awake();

        hitReceivedCounter = GetComponent<HitReceivedCounter>();    
        dummyFighter = GetComponent<DummyFighter>();
        audioManager = FindObjectOfType<AudioManager>();
        notificationUI = FindObjectOfType<NotificationUI>();
    }

    protected override void Start()
    {
        playerFighter = player.GetComponent<Fighter>();
        rageParticles.gameObject.SetActive(false);

        base.Start();

        //Common stuff---------------------------------------------------------------------------
        Wait waitForNextAttack = new Wait("Wait to attack", waitTimeBetweenAttacks);
        Wait waitAfterSpin = new Wait("Wait after spin", waitTimeAfterSpin);
        Wait waitBeforeSpin = new Wait("Wait after spin", waitTimeBeforeSpin);
        Inverter waitForNextAttackInverter = new Inverter("Wait inverter");

        Leaf findPlayerLocation = new Leaf("Find Player Location", FindPlayerLocation);
        Leaf checkForRangeAttack = new Leaf("Check for range attack", CheckForIncomingRangeAttack);
        Leaf checkForMeleeAttack = new Leaf("Check for melee attack", CheckForMeleeAttack);
        Leaf resetHits = new Leaf("Reset hits", ResetHitCounter);
        Leaf resetStageAdvance = new Leaf("Reset Stage Advance", ResetStageAdvance);
        Leaf resetAnimatorTriggers = new Leaf("Reset Animator triggers", ResetAnimatorTriggers);

        Leaf hitAfterSpin = new Leaf("Check hit during spin", HitAfterSpin);
        Leaf checkHitAfterSpin = new Leaf("Check hit after spin", CheckHitAfterSpin);
        Leaf spin = new Leaf("spin attack", Spin);
        Leaf playRageBeforeSpin = new Leaf("Rage before spin", EnrageBeforeSpin);
        Leaf checkPlayerDistanceLessThan = new Leaf("Check Player Distance", () => IsPlayerDistanceLessThan(distanceToTriggerMoveToPlayerAttack));
        Leaf moveTowardsPlayerAttack = new Leaf("Move towards Player Attack", MoveTowardsPlayerAttack);

        Sequence spinSequence = new Sequence("Spin sequence");
        Selector checkForPlayerDistanceSelector = new Selector("Check for player distance");

        Loop checkForPlayerAttacks = new Loop("Check for player attack", waitForAttackDependency);
        
        checkForPlayerAttacks.AddChild(checkForRangeAttack);
        checkForPlayerAttacks.AddChild(findPlayerLocation);
        checkForPlayerAttacks.AddChild(checkForMeleeAttack);

        waitForNextAttackInverter.AddChild(waitForNextAttack);
        waitForAttackDependency.AddChild(waitForNextAttackInverter);
        checkForHitAfterSpinTree.AddChild(hitAfterSpin);

        spinSequence.AddChild(playRageBeforeSpin);
        spinSequence.AddChild(waitBeforeSpin);
        spinSequence.AddChild(spin);
        spinSequence.AddChild(waitAfterSpin);
        spinSequence.AddChild(checkHitAfterSpin);

        checkForPlayerDistanceSelector.AddChild(checkPlayerDistanceLessThan);
        checkForPlayerDistanceSelector.AddChild(moveTowardsPlayerAttack);
        checkForPlayerDistanceSelector.AddChild(checkForPlayerAttacks);
        //---------------------------------------------------------------------------------------------


        //MAIN TREE---------------------------------------------------------------------------------------
        Sequence mainSequence = new Sequence("Main Sequence");
        //-----------------------------------------------------------------------------------------------------------

        //IDLE STAGE
        Leaf stayIdle = new Leaf("Stay Idle at start", StayIdle);
        Leaf enter = new Leaf("Enter animation", Enter);
        
        //-----------------------------------------------------------------------------------------------------------
        //STAGE 1
        
        //Stage1 condition 
        Leaf stage1RageOverComeLimit = new Leaf("Stage 1 Rage Check", () => RageOverComeLimit(rageLevelToAdvanceStage1));
        Leaf waitToBeHitAfterJumpAttack = new Leaf("Wait after jump", () => WaitToBeHit(waitToBeHitTime));
        stage1DependencyCondition.AddChild(stage1RageOverComeLimit);
  
        Loop stage1Loop = new Loop("Stage 1 loop", checkForHitAfterSpinTree);
        Loop stage1AttacksLoop = new Loop("Stage 1 attacks loop", stage1DependencyCondition);

        Sequence jumpAttackSequence = new Sequence("Jump attackSequence");
        Sequence dashAttackSequence = new Sequence("Dash Attack Sequence");

        RSelector stage1AttackSelector = new RSelector("Stage1 attack selector");

       
        Leaf jumpAttack = new Leaf("JumpAttack", JumpAttack);
        Leaf dashAttack = new Leaf("DashAttack", DashAttack);
        Leaf stepAway = new Leaf("StepAway", StepAway);
        //-----------------------------------------------------------------------------------------------------------

        //STAGE 2
        
        //Stage1 condition 
        Leaf Stage2RageOverComeLimit = new Leaf("Stage 2 Rage Check", () => RageOverComeLimit(rageLevelToAdvanceStage2));
        stage2DependencyCondition.AddChild(Stage2RageOverComeLimit);
  
        Loop stage2Loop = new Loop("Stage 2 loop", checkForHitAfterSpinTree);
        Loop stage2AttacksLoop = new Loop("Stage 2 attacks loop", stage2DependencyCondition);

        Sequence clubAttackSequence = new Sequence("club attackSequence");
        Leaf clubAttack = new Leaf("Club attack", ClubAttack);

        RSelector stage2AttackSelector = new RSelector("Stage2 attack selector");


        //-----------------------------------------------------------------------------------------------------------

        //STAGE 3

        //Stage1 condition 
        Leaf stage3RageOverComeLimit = new Leaf("Stage 1 Rage Check", () => RageOverComeLimit(rageLevelToAdvanceStage3));
        stage3DependencyCondition.AddChild(stage3RageOverComeLimit);

        Loop stage3Loop = new Loop("Stage 3 loop", checkForHitAfterSpinTree);
        Loop stage3AttacksLoop = new Loop("Stage 3 attacks loop", stage3DependencyCondition);

        Sequence launchAndHitSequence = new Sequence("launch and hit attackSequence");
        Leaf launchBin = new Leaf("Launch and hit attack", LaunchBin);

        RSelector stage3AttackSelector = new RSelector("Stage3 attack selector");


        //-----------------------------------------------------------------------------------------------------------

        //Final sequence
        Leaf die = new Leaf("Die", Die);
        


        //Build tree--------------------------------------------------------------------------------------------------------
        jumpAttackSequence.AddChild(findPlayerLocation);
        jumpAttackSequence.AddChild(jumpAttack);
        jumpAttackSequence.AddChild(resetHits);
        jumpAttackSequence.AddChild(waitToBeHitAfterJumpAttack);

        dashAttackSequence.AddChild(findPlayerLocation);
        dashAttackSequence.AddChild(stepAway);
        dashAttackSequence.AddChild(findPlayerLocation);
        dashAttackSequence.AddChild(dashAttack);

        clubAttackSequence.AddChild(findPlayerLocation);
        clubAttackSequence.AddChild(clubAttack);
        clubAttackSequence.AddChild(resetHits);
        clubAttackSequence.AddChild(waitToBeHitAfterJumpAttack);

        launchAndHitSequence.AddChild(launchBin);
        launchAndHitSequence.AddChild(findPlayerLocation);

        stage1AttackSelector.AddChild(jumpAttackSequence);
        stage1AttackSelector.AddChild(dashAttackSequence);
        //stage1AttackSelector.AddChild(launchAndHitSequence);

        stage2AttackSelector.AddChild(jumpAttackSequence);
        stage2AttackSelector.AddChild(dashAttackSequence);
        stage2AttackSelector.AddChild(clubAttackSequence);

        stage3AttackSelector.AddChild(jumpAttackSequence);
        stage3AttackSelector.AddChild(dashAttackSequence);
        stage3AttackSelector.AddChild(clubAttackSequence);
        stage3AttackSelector.AddChild(launchAndHitSequence);

        stage1AttacksLoop.AddChild(checkForPlayerAttacks);
        stage1AttacksLoop.AddChild(findPlayerLocation);
        stage1AttacksLoop.AddChild(checkForPlayerDistanceSelector);
        stage1AttacksLoop.AddChild(resetAnimatorTriggers);
        stage1AttacksLoop.AddChild(stage1AttackSelector);

        stage2AttacksLoop.AddChild(checkForPlayerAttacks);
        stage2AttacksLoop.AddChild(findPlayerLocation);
        stage2AttacksLoop.AddChild(checkForPlayerDistanceSelector);
        stage2AttacksLoop.AddChild(resetAnimatorTriggers);
        stage2AttacksLoop.AddChild(stage2AttackSelector);

        stage3AttacksLoop.AddChild(checkForPlayerAttacks);
        stage3AttacksLoop.AddChild(findPlayerLocation);
        stage3AttacksLoop.AddChild(checkForPlayerDistanceSelector);
        stage3AttacksLoop.AddChild(resetAnimatorTriggers);
        stage3AttacksLoop.AddChild(stage3AttackSelector);

        stage1Loop.AddChild(resetStageAdvance);
        stage1Loop.AddChild(stage1AttacksLoop);
        stage1Loop.AddChild(spinSequence);

        stage2Loop.AddChild(resetStageAdvance);
        stage2Loop.AddChild(stage2AttacksLoop);
        stage2Loop.AddChild(spinSequence);

        stage3Loop.AddChild(resetStageAdvance);
        stage3Loop.AddChild(stage3AttacksLoop);
        stage3Loop.AddChild(spinSequence);

        mainSequence.AddChild(stayIdle);
        mainSequence.AddChild(enter);
        mainSequence.AddChild(stage1Loop);
        mainSequence.AddChild(stage2Loop);
        mainSequence.AddChild(stage3Loop);
        mainSequence.AddChild(die);

        bt.AddChild(mainSequence);

        bt.PrintTree();
        //------------------------------------------------------------------------------------

    }

    private void OnEnable() 
    {
        dummyFighter.GetHitBox().onHit += ResetRage;    
        dummyFighter.onBarrelHit += ResetRage;
    }

    private void OnDisable() 
    {
        dummyFighter.GetHitBox().onHit -= ResetRage;
        dummyFighter.onBarrelHit -= ResetRage; 
    }

    private Node.Status StayIdle()
    {
        if(!isIdleSetUp)
        {
            isIdleSetUp = true;
            hitReceivedCounter.onHit += PlayHitAnimationAtIdle;
        }
        Debug.Log(hitReceivedCounter.GetHitReceivedNumber());
        if (hitReceivedCounter.GetHitReceivedNumber() < numberOfHitsToLeaveIdle)
        {
            return Node.Status.RUNNING;
        }
        hitReceivedCounter.onHit -= PlayHitAnimationAtIdle; 
        hitReceivedCounter.ResetHits();
        return Node.Status.SUCCESS;
    }

    private void PlayHitAnimationAtIdle()
    {
        animator.SetTrigger("HitOnIdle");
    }

    private Node.Status EnrageBeforeSpin()
    {
        animator.SetTrigger("EnrageBeforeSpin");
        Instantiate(enragePlusSFXPrefab, transform.position, Quaternion.identity);
        return Node.Status.SUCCESS;
    }

    private Node.Status Enter()
    {
        if(!entranceTriggered)
        {
            Instantiate(comeToLifeSFXPrefab, transform.position, Quaternion.identity);
            onAlive?.Invoke();
            if(aliveParticles != null)
            {
                aliveParticles.Play();
                weaponParticles.Play();
            }
            onActivate?.Invoke(bossName);
            animator.SetTrigger("Enter");
            audioManager.PlayDummyBossMusic();
            entranceTriggered = true;
            player.GetComponent<PlayerController>().TakeKnockBack(0f, transform);
        }
        if(enterTimer <= enterTimeDuration)
        {
            dummyFighter.LookPlayer(false);
            enterTimer += Time.deltaTime;
            return Node.Status.RUNNING;
        }
        enterTimer = 0f;
        return Node.Status.SUCCESS;
    }

    private Node.Status Die()
    {
        if(!isDead)
        {
            onDie?.Invoke();
            deathTimer = 0f;
            isDead = true;
            notificationUI.ShowNotification(deathNotification);
            animator.SetTrigger("Die");
            Instantiate(deathSFXPrefab, transform.position, Quaternion.identity);
        }
        deathTimer += Time.deltaTime;
        if(deathTimer <= deathTime)
        {
            return Node.Status.RUNNING;
        }
        else
        {
            this.gameObject.SetActive(false);
            WeaponUpgradePickable upgrade = Instantiate(weaponUpgradePickablePrefab, transform.position, Quaternion.identity);
            upgrade.transform.parent = null;
            upgrade.Spawn();
            return Node.Status.SUCCESS;
        }
        
    }

    private Node.Status RageOverComeLimit(int limit)
    {
        if(rage >= limit) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    private Node.Status ResetHitCounter()
    {
        hitReceivedCounter.ResetHits();
        return Node.Status.SUCCESS;
    }

    private Node.Status ResetStageAdvance()
    {
        canAdvanceStage = true;
        return Node.Status.SUCCESS;
    }

    private Node.Status ResetAnimatorTriggers()
    {
        animator.ResetTrigger("BackToIdleAfterClub");
        animator.ResetTrigger("BackToIdle");
        animator.ResetTrigger("BackToIdleAfterLaugh");
        return Node.Status.SUCCESS;
    }

    private Node.Status WaitToBeHit(float time)
    {
        waitToBeHitTimer += Time.deltaTime;
        if(waitToBeHitTimer >= time)
        {
            waitToBeHitTimer = 0f;
            animator.SetTrigger("BackToIdleAfterClub");
            return Node.Status.SUCCESS;
        }
        if(hitReceivedCounter.GetHitReceivedNumber() >= 1)
        {
            Debug.Log("Wait to be hit enrage");
            Enrage();
            waitToBeHitTimer = 0f;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    private Node.Status IsPlayerDistanceLessThan(float range)
    {
        Debug.Log("check distance");
        if (Vector3.Distance(playerLocation, transform.position) < range)
        {
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }

    private Node.Status HitAfterSpin()
    {
        if(isHitAfterSpin && canAdvanceStage) 
        {
            isHitAfterSpin = false;
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status CheckHitAfterSpin()
    {
        if(hitReceivedCounter.GetHitReceivedNumber() >= 1)
        {
            if(canAdvanceStage)
            {
                Debug.Log("Success stage!");
                isHitAfterSpin = true;
                dummyFighter.AdvanceStage();                           
            }
            else
            {
                Debug.Log("Can't advance");
                //isHitAfterSpin = false;                      
            }
        }
        ResetRage(false);
        return Node.Status.SUCCESS;
        
    }

    private Node.Status CheckForIncomingRangeAttack()
    {
        dummyFighter.LookPlayer(false);
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, projectileDetectionRange, Vector3.down, 0f);
        foreach (RaycastHit hit in hits)
        {
            if(hit.transform.TryGetComponent<Projectile>(out Projectile projectile))
            {
                Destroy(projectile.gameObject);
                animator.SetTrigger("Protect");
                ResetRage(false);
            }
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status CheckForMeleeAttack()
    {
        dummyFighter.LookPlayer(false);
        if(playerFighter.IsAttackingMelee())
        {
            if(IsPlayerWithinRange(playerAttackDetectionRange))
            {
                Debug.Log("Keep Player From Attacking");
                animator.SetTrigger("Protect");
                ResetRage(false);
                Pause(shieldTime, null);
                player.GetComponent<PlayerController>().TakeKnockBack(0f,this.transform);
                return Node.Status.SUCCESS; 
            }
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status JumpAttack()
    {
        if(!isAttacking) 
        {
            dummyFighter.JumpForwardAttack(playerLocation - transform.position);
            isAttacking = true;
            dummyFighter.LookPlayer(true);
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) return Node.Status.RUNNING;
            else
            {
                isAttacking = false;
                return Node.Status.SUCCESS;
            }
        }
    }
    private Node.Status MoveTowardsPlayerAttack()
    {
        if (!isAttacking)
        {
            Instantiate(attackSFXPrefab, transform.position, Quaternion.identity);
            dummyFighter.MoveTowardsPlayerAttack();
            animator.SetTrigger("Dash");
            isAttacking = true;
            dummyFighter.LookPlayer(true);
            return Node.Status.RUNNING;
        }
        else
        {
            if (dummyFighter.IsAttacking()) return Node.Status.RUNNING;
            else
            {
                isAttacking = false;
                return Node.Status.FAILURE;
            }
        }
    }

    private Node.Status StepAway()
    {
        if(!isAttacking) 
        {
            Instantiate(preAttackSFXPrefab, transform.position, Quaternion.identity);
            animator.SetTrigger("StepAwayPosition");
            dummyFighter.StepAway(transform.position - playerLocation);
            dummyFighter.LookPlayer(true);
            isAttacking = true;
            hitReceivedCounter.ResetHits();
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) 
            {
                if(hitReceivedCounter.GetHitReceivedNumber() >= 1)
                {
                    hitReceivedCounter.ResetHits();
                    Enrage();
                    return Node.Status.FAILURE;
                }
                return Node.Status.RUNNING;
            }
            else
            {
                isAttacking = false;
                return Node.Status.SUCCESS;
            }
        }
    }

    private Node.Status DashAttack()
    {
        if(!isAttacking) 
        {
            Instantiate(attackSFXPrefab, transform.position, Quaternion.identity);
            animator.SetTrigger("DashPosition");
            dummyFighter.LookPlayer(true);
            dummyFighter.DashAttack(playerLocation - transform.position);
            isAttacking = true;
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) return Node.Status.RUNNING;
            else
            {
                isAttacking = false;
                if(dummyFighter.HasHitPlayer())
                {
                    dummyFighter.ResetHitPlayer();
                    animator.SetTrigger("BackToIdle");
                    ResetRage(true);
                }
                else
                {
                    Enrage();
                }
                return Node.Status.SUCCESS;
            }
        }
    }

    private Node.Status ClubAttack()
    {
        if(!isAttacking) 
        {
            dummyFighter.ClubAttack();
            isAttacking = true;
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) return Node.Status.RUNNING;
            else
            {
                isAttacking = false;
                //Enrage();
                return Node.Status.SUCCESS;
            }
        }
    }

    private Node.Status LaunchBin()
    {
        if (!isAttacking)
        {
            dummyFighter.LaunchBarrel();
            isAttacking = true;
            return Node.Status.RUNNING;
        }
        else
        {
            if (hitReceivedCounter.GetHitReceivedNumber() >= 1) 
            {
                Enrage();
                hitReceivedCounter.ResetHits();
                dummyFighter.DontHitBarrel();
                isAttacking = false;
                return Node.Status.SUCCESS;
            }
            if (dummyFighter.IsAttacking()) return Node.Status.RUNNING;
            else 
            {
                isAttacking = false;
                return Node.Status.SUCCESS;
            }
        }
        
    }

    private Node.Status Spin()
    {
        if(!isAttacking) 
        {
            dummyFighter.SpinAttack(playerLocation - transform.position);
            hitReceivedCounter.ResetHits();
            isAttacking = true;
            animator.SetBool("Spin", true);
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) 
            {
                return Node.Status.RUNNING;
            }
            else
            {
                if (dummyFighter.HasHitPlayer())
                {
                    Debug.Log("Hit");
                    dummyFighter.ResetHitPlayer();
                    ResetRage(true);
                    animator.SetBool("Spin", false);
                    isAttacking = false;
                    canAdvanceStage = false;
                }
                else
                {
                    Debug.Log("tired");
                    isAttacking = false;
                    hitReceivedCounter.ResetHits();
                    animator.SetTrigger("Tired");
                    Pause(timeOnGroundAfterSpin, () => animator.SetBool("Spin", false));
                }
                return Node.Status.SUCCESS;
            }
        }
    }

    private void ResetRage(GameObject playerObject)
    {
        if(playerObject == player) ResetRage(true);
    }

    private void ResetRage(bool laugh)
    {
        rage = 0;
        onRageChange?.Invoke(GetRageFraction());
        rageParticles.gameObject.SetActive(false);
        if (laugh) 
        {
            animator.SetTrigger("Laugh");
            Instantiate(laughSFXPrefab, transform.position, Quaternion.identity);        
        }
        Pause(laughTimeAfterHit, StopLaugh);
    }

    private void ResetRage()
    {
        ResetRage(true);
    }

    private void Enrage()
    {
        rage++;
        onRageChange?.Invoke(GetRageFraction());
        rageParticles.gameObject.SetActive(true);
        Instantiate(enrageSFXPrefab, transform.position, Quaternion.identity);
        StartCoroutine(ResetEnrage());
        if(rage == 1)
        {
            rageParticles.startSpeed = 1f;
        }
        if(rage == 2)
        {
            rageParticles.startSpeed = 5f;
        }
        if(rage == 3)
        {
            rageParticles.startSpeed = 15f;
        }
    }

    private void StopLaugh()
    {
        Debug.Log("Stop");
        paused = false;
        animator.SetTrigger("BackToIdleAfterLaugh");
    }

    private IEnumerator ResetEnrage()
    {
        Pause(true);
        animator.SetTrigger("Enrage");
        dummyFighter.LookPlayer(true);
        yield return new WaitForSeconds(enrageTimeLength);
        Pause(false);
    }

    public float GetRageFraction()
    {
        int stage = dummyFighter.GetCurrentStage();
        float fraction = 0f;

        if(stage == 1) fraction = (float)rage / rageLevelToAdvanceStage1;
        else if (stage == 2) fraction = (float)rage / rageLevelToAdvanceStage2;
        else fraction = (float)rage / rageLevelToAdvanceStage3;

        Debug.Log("final " + fraction);
        return fraction;

    }

}

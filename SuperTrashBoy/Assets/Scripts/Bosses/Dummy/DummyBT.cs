using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTrashBoy.BehaviorTrees;
using System;

public class DummyBT : BTUser
{
    [SerializeField] private ParticleSystem rageParticles = null;
    [SerializeField] private Animator animator;
    [SerializeField] private int numberOfHitsToLeaveIdle = 5;
    [SerializeField] private int rageLevelToAdvanceStage1 = 3;
    [SerializeField] private int rageLevelToAdvanceStage2 = 4;
    [SerializeField] private int rageLevelToAdvanceStage3 = 5;
    [SerializeField] private float enrageTimeLength = 1f;
    [SerializeField] private float projectileDetectionRange = 2f;
    [SerializeField] private float playerAttackDetectionRange = 2f;
    [SerializeField] private float waitTimeBetweenAttacks = 5f;
    [SerializeField] private float waitTimeAfterSpin = 3f;
    [SerializeField] private float distanceToTriggerMoveToPlayerAttack = 10f;

    private HitReceivedCounter hitReceivedCounter;
    private Fighter playerFighter;
    private DummyFighter dummyFighter;

    private int rage = 0;
    private bool isAttacking = false;
    private bool isHitAfterSpin = false;

    private BehaviorTree stage1DependencyCondition = new BehaviorTree("Stage1 dep");
    private BehaviorTree stage2DependencyCondition = new BehaviorTree("Stage2 dep");
    private BehaviorTree stage3DependencyCondition = new BehaviorTree("Stage3 dep");
    private BehaviorTree waitForAttackDependency = new BehaviorTree("Wait for attack dep");
    private BehaviorTree checkForHitAfterSpinTree = new BehaviorTree("Check for hit during spin");

    protected override void Awake() 
    {
        base.Awake();

        hitReceivedCounter = GetComponent<HitReceivedCounter>();    
        dummyFighter = GetComponent<DummyFighter>();
    }

    protected override void Start()
    {
        playerFighter = player.GetComponent<Fighter>();
        rageParticles.gameObject.SetActive(false);

        base.Start();

        //Common stuff---------------------------------------------------------------------------
        Wait waitForNextAttack = new Wait("Wait to attack", waitTimeBetweenAttacks);
        Wait waitAfterSpin = new Wait("Wait after spin", waitTimeAfterSpin);
        Inverter waitForNextAttackInverter = new Inverter("Wait inverter");

        Leaf findPlayerLocation = new Leaf("Find Player Location", FindPlayerLocation);
        Leaf checkForRangeAttack = new Leaf("Check for range attack", CheckForIncomingRangeAttack);
        Leaf checkForMeleeAttack = new Leaf("Check for melee attack", CheckForMeleeAttack);

        Leaf hitAfterSpin = new Leaf("Check hit during spin", HitAfterSpin);
        Leaf checkHitAfterSpin = new Leaf("Check hit after spin", CheckHitAfterSpin);
        Leaf spin = new Leaf("spin attack", Spin);

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
        
        //-----------------------------------------------------------------------------------------------------------
        //STAGE 1
        
        //Stage1 condition 
        Leaf Stage1RageOverComeLimit = new Leaf("Stage 1 Rage Check", () => RageOverComeLimit(rageLevelToAdvanceStage1));
        stage1DependencyCondition.AddChild(Stage1RageOverComeLimit);
  
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
        Leaf Stage3RageOverComeLimit = new Leaf("Stage 1 Rage Check", () => RageOverComeLimit(rageLevelToAdvanceStage3));
        stage3DependencyCondition.AddChild(Stage3RageOverComeLimit);

        Loop stage3Loop = new Loop("Stage 3 loop", checkForHitAfterSpinTree);
        Loop stage3AttacksLoop = new Loop("Stage 3 attacks loop", stage3DependencyCondition);

        Sequence launchAndHitSequence = new Sequence("launch and hit attackSequence");
        Leaf launchBarrel = new Leaf("Launch and hit attack", LaunchBarrel);

        RSelector stage3AttackSelector = new RSelector("Stage3 attack selector");


        //-----------------------------------------------------------------------------------------------------------


        //Build tree--------------------------------------------------------------------------------------------------------
        jumpAttackSequence.AddChild(findPlayerLocation);
        jumpAttackSequence.AddChild(jumpAttack);

        dashAttackSequence.AddChild(findPlayerLocation);
        dashAttackSequence.AddChild(stepAway);
        dashAttackSequence.AddChild(findPlayerLocation);
        dashAttackSequence.AddChild(dashAttack);

        clubAttackSequence.AddChild(findPlayerLocation);
        clubAttackSequence.AddChild(clubAttack);

        launchAndHitSequence.AddChild(launchBarrel);
        launchAndHitSequence.AddChild(findPlayerLocation);

        stage1AttackSelector.AddChild(jumpAttackSequence);
        stage1AttackSelector.AddChild(dashAttackSequence);

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
        stage1AttacksLoop.AddChild(stage1AttackSelector);

        stage2AttacksLoop.AddChild(checkForPlayerAttacks);
        stage2AttacksLoop.AddChild(findPlayerLocation);
        stage2AttacksLoop.AddChild(checkForPlayerDistanceSelector);
        stage2AttacksLoop.AddChild(stage2AttackSelector);

        stage3AttacksLoop.AddChild(checkForPlayerAttacks);
        stage3AttacksLoop.AddChild(findPlayerLocation);
        stage3AttacksLoop.AddChild(checkForPlayerDistanceSelector);
        stage3AttacksLoop.AddChild(stage3AttackSelector);

        stage1Loop.AddChild(stage1AttacksLoop);
        stage1Loop.AddChild(spinSequence);

        stage2Loop.AddChild(stage2AttacksLoop);
        stage2Loop.AddChild(spinSequence);

        stage3Loop.AddChild(stage3AttacksLoop);
        stage3Loop.AddChild(spinSequence);

        mainSequence.AddChild(stayIdle);
        mainSequence.AddChild(stage1Loop);
        mainSequence.AddChild(stage2Loop);
        mainSequence.AddChild(stage3Loop);

        bt.AddChild(mainSequence);

        bt.PrintTree();
        //------------------------------------------------------------------------------------

    }

    private void OnEnable() 
    {
        dummyFighter.GetHitBox().onHit += ResetRage;    
    }

    private void OnDisable() 
    {
        dummyFighter.GetHitBox().onHit -= ResetRage;    
    }

    private Node.Status StayIdle()
    {
        Debug.Log(hitReceivedCounter.GetHitReceivedNumber());
        if (hitReceivedCounter.GetHitReceivedNumber() < numberOfHitsToLeaveIdle)
        {
            return Node.Status.RUNNING;
        } 
        hitReceivedCounter.ResetHits();
        return Node.Status.SUCCESS;
    }

    private Node.Status RageOverComeLimit(int limit)
    {
        if(rage >= limit) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
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
        if(isHitAfterSpin) 
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
            Debug.Log("Success stage!");
            isHitAfterSpin = true;
            dummyFighter.AdvanceStage();
        }
        ResetRage();
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
                ResetRage();
            }
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status CheckForMeleeAttack()
    {
        dummyFighter.LookPlayer(false);
        Debug.Log("waiting");
        if(playerFighter.IsAttackingMelee())
        {
            if(IsPlayerWithinRange(playerAttackDetectionRange))
            {
                Debug.Log("Keep Player From Attacking");
                animator.SetTrigger("Protect");
                ResetRage();
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
            dummyFighter.MoveTowardsPlayerAttack();
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
                    ResetRage();
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
                Enrage();
                return Node.Status.SUCCESS;
            }
        }
    }

    private Node.Status LaunchBarrel()
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
            isAttacking = true;
            animator.SetBool("Spin", true);
            return Node.Status.RUNNING;
        }
        else
        {
            if(dummyFighter.IsAttacking()) 
            {
                if(dummyFighter.HasHitPlayer())
                {
                    dummyFighter.ResetHitPlayer();
                    ResetRage();
                }
                return Node.Status.RUNNING;
            }
            else
            {
                isAttacking = false;
                hitReceivedCounter.ResetHits();
                animator.SetBool("Spin", false);
                return Node.Status.SUCCESS;
            }
        }
    }

    private void ResetRage(GameObject playerObject)
    {
        if(playerObject == player) ResetRage();
    }

    private void Enrage()
    {
        rage++;
        rageParticles.gameObject.SetActive(true);
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

    private void ResetRage()
    {
        rage = 0;
        rageParticles.gameObject.SetActive(false);
    }

    private IEnumerator ResetEnrage()
    {
        Pause(true);
        animator.SetTrigger("Enrage");
        dummyFighter.LookPlayer(true);
        yield return new WaitForSeconds(enrageTimeLength);
        Pause(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTrashBoy.BehaviorTrees;
using System;

public class DummyBT : BTUser
{
    [SerializeField] private int numberOfHitsToLeaveIdle = 5;
    [SerializeField] private int rageLevelToAdvanceStage1 = 3;
    [SerializeField] private float projectileDetectionRange = 2f;
    [SerializeField] private float playerAttackDetectionRange = 2f;

    private HitReceivedCounter hitReceivedCounter;
    private Fighter playerFighter;

    private int rage = 0;

    private BehaviorTree stage1DependencyCondition = new BehaviorTree("Stage1 dep");

    protected override void Awake() 
    {
        base.Awake();

        hitReceivedCounter = GetComponent<HitReceivedCounter>();    
    }

    protected override void Start()
    {
        playerFighter = player.GetComponent<Fighter>();

        base.Start();

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
  
        Loop stage1Loop = new Loop("Stage 1 sequence", stage1DependencyCondition);

        RSelector stage1AttackSelector = new RSelector("Stage1 attack selector");

        Leaf checkForRangeAttack = new Leaf("Check for range attack", CheckForIncomingRangeAttack);
        Leaf checkForMeleeAttack = new Leaf("Check for melee attack", CheckForMeleeAttack);

        Leaf attack1 = new Leaf("Attack1", Attack1);
        Leaf attack2 = new Leaf("Attack2", Attack2);
        //-----------------------------------------------------------------------------------------------------------

        //Common stuff---------------------------------------------------------------------------
        Leaf findPlayerLocation = new Leaf("Find Player Location", FindPlayerLocation);
        //---------------------------------------------------------------------------------------------

        //Build tree--------------------------------------------------------------------------------------------------------
        stage1Loop.AddChild(checkForRangeAttack);
        stage1Loop.AddChild(findPlayerLocation);
        stage1Loop.AddChild(checkForMeleeAttack);

        stage1AttackSelector.AddChild(attack1);
        stage1AttackSelector.AddChild(attack2);
        stage1Loop.AddChild(stage1AttackSelector);

        mainSequence.AddChild(stayIdle);
        mainSequence.AddChild(stage1Loop);

        bt.AddChild(mainSequence);

        bt.PrintTree();
        //------------------------------------------------------------------------------------

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

    private Node.Status Stage1()
    {
        Debug.Log("Stage1");
        return Node.Status.RUNNING;
    }

    private Node.Status RageOverComeLimit(int limit)
    {
        if(rage >= limit) return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    private Node.Status CheckForIncomingRangeAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, projectileDetectionRange, Vector3.down, 0f);
        foreach (RaycastHit hit in hits)
        {
            if(hit.transform.TryGetComponent<Projectile>(out Projectile projectile))
            {
                Destroy(projectile.gameObject);
                rage = 0;
            }
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status CheckForMeleeAttack()
    {
        if(playerFighter.IsAttackingMelee())
        {
            if(IsPlayerWithinRange(playerAttackDetectionRange))
            {
                Debug.Log("Keep Player From Attacking");
                rage = 0;
            }
        }
        return Node.Status.SUCCESS;
    }

    private Node.Status Attack1()
    {
        Debug.Log("attack1!");
        return Node.Status.SUCCESS;
    }

    private Node.Status Attack2()
    {
        Debug.Log("attack2!");
        return Node.Status.SUCCESS;
    }

}

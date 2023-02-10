using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuController : MonoBehaviour
{
    private bool mInAttack = false;

    //Variables for choosing attacks
    public int handChance = 100;  //Higher values are less likely
    public int roarChance = 100;  //Higher values are less likely
    public int throwChance = 100;  //Higher values are less likely
    public int moveChance = 100;  //Higher values are less likely
    public int timeWeightingValue = 300; //Higher values mean longer gaps between attacks
    public int lowMoveThreshold = 2;
    public int highMoveThreshold = 4;
    private int attacksSinceMove = 0;
    private int mTimeSinceLastAttack = 0;

    //List all the states the boss could be in
    private enum states
    {
        neutral,
        handSlam,
        throwingTrash,
        roar,
        repositioning,
        none
    }

    private states mCurrentState;

    // Start is called before the first frame update
    void Start()
    {
        mCurrentState = states.none;
    }

    private void FixedUpdate()
    {
        HandleState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleState()
    {
        switch (mCurrentState)
        {
            case states.none:
                ChangeState(states.neutral);
                break;

            case states.neutral:
                HandleNeutral();
                break;

            case states.handSlam:
                //TODO
                ChangeState(states.neutral);
                break;

            case states.roar:
                //TODO
                ChangeState(states.neutral);
                break;

            case states.throwingTrash:
                //TODO
                ChangeState(states.neutral);
                break;

            case states.repositioning:
                //TODO
                ChangeState(states.neutral);
                break;

            default:
                ChangeState(states.none);
                break;
        }
    }

    private void HandleNeutral()
    {
        NeutralChooseNextState();
    }

    private void NeutralChooseNextState()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mTimeSinceLastAttack = 0;
            mInAttack = true;
        }

        mTimeSinceLastAttack++;

        //Roll for hand
        if (RollForAttack(handChance, true))
        {
            ChangeState(states.handSlam);
            mInAttack = false;
            return;
        }

        //Roll for roar
        if (RollForAttack(roarChance, true))
        {
            ChangeState(states.roar);
            mInAttack = false;
            return;
        }

        //Roll for throw
        if (RollForAttack(throwChance, true))
        {
            ChangeState(states.throwingTrash);
            mInAttack = false;
            return;
        }

        //Set up movement being less likely until a couple of attacks and more likely after a few
        float adjustment = 1.0f;
        if (attacksSinceMove < lowMoveThreshold)
            adjustment = 1.5f;
        else if (attacksSinceMove >= highMoveThreshold)
            adjustment = 0.5f;
        //Roll for move
        if (RollForAttack(moveChance, true, adjustment))
        {
            ChangeState(states.repositioning);
            mInAttack = false;
            return;
        }
        //If we've not hit any of the rolls, stay in movement, just return
    }

    bool RollForAttack(float prob, bool timeWeighting, float adjustementFactor=1.0f)
    {
        float attackProb = prob;
        if (timeWeighting)
            attackProb = attackProb * (timeWeightingValue / mTimeSinceLastAttack);
        if (Random.Range(0, attackProb) < 1) //Roll for the attack
            return true; else return false;
    }

    void ChangeState(states state)
    {
        //Funcion added to allow processing of states
        if (state != states.neutral && state != states.repositioning)
            attacksSinceMove++;
        else if (state == states.repositioning)
            attacksSinceMove = 0;

        Debug.Log("Changing state to " + state);

        mCurrentState = state;
    }
}

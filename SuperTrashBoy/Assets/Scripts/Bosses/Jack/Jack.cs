using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack : MonoBehaviour
{
    //This class should be spawned in knowing what phase it is in. A new instance of the boss is spawned for every main phase.

    public int barrelChance = 100;  //Higher values are less likely
    public int hammerChance = 100;
    public int spitChance = 100;

    private bool mInAttack = false;
    private bool mHasTakenDamage = false;
    private int mAttacksAfterDamageTaken = 0;

    //Variables for choosing attack
    private int mTimeSinceLastAttack = 0;

    //List all the states the boss could be in
    private enum states
    {
        neutral,
        throwingBarrels,
        hammerSwing,
        spinSpit,
        endState,
        none
    }

    private states mCurrentState;
    //private states mNextState;  //mNextState can be added if you need to know what state to go to next while in a state

    // Start is called before the first frame update
    void Start()
    {
        mCurrentState = states.neutral;
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
                mCurrentState = states.neutral;
                break;

            case states.neutral:
                HandleNeutral();
                break;

            default:
                mCurrentState = states.none;
                break;
        }
    }

    private void HandleNeutral()
    {
        if (IsPlayerLOS())
        {
            MoveTowardsPlayer();
        }
        NeutralChooseNextState();
    }

    private void NeutralChooseNextState()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mTimeSinceLastAttack = 0;
        }

        // Work out what attack to do next based on weightings from player position, time since previous attack and so on
        float playerDistance = GetDistanceFromPlayer();

        //Roll for hammer
        int hammerProb = hammerChance - mTimeSinceLastAttack + (int)playerDistance;  //Less likely when far
        if (hammerProb < 1) //Weighted to less than one, the attack is certain
        {
            mCurrentState = states.hammerSwing;
            return;
        }
        else if (Random.Range(0, hammerProb) == 0) //Roll for the attack
        {
            mCurrentState = states.hammerSwing;
            return;
        }

        //Roll for barrels
        int barrelProb = barrelChance - mTimeSinceLastAttack - (int)playerDistance; //More likely when far
        if (barrelProb < 1) //Weighted to less than one, the attack is certain
        {
            mCurrentState = states.throwingBarrels;
            return;
        }
        else if (Random.Range(0, hammerProb) == 0) //Roll for the attack
        {
            mCurrentState = states.throwingBarrels;
            return;
        }

        //Roll for spit
        int spitProb = spitChance - mTimeSinceLastAttack;
        if (spitProb < 1) //Weighted to less than one, the attack is certain
        {
            mCurrentState = states.spinSpit;
            return;
        }
        else if (Random.Range(0, hammerProb) == 0) //Roll for the attack
        {
            mCurrentState = states.spinSpit;
            return;
        }

        //If we've not hit any of the rolls, stay in movement, just return
    }

    public void SetDamageTaken()
    {
        mHasTakenDamage = true;
    }


    //-------------Functions below are TODO!-----------

    private bool IsPlayerLOS()
    {
        //Draw a ray from the boss to the player, and return false if it hits anything else
        //Physics.Raycast...

        return true;
    }

    private float GetDistanceFromPlayer()
    {
        //Draw a ray from the boss to the player, and return false if it hits anything else
        //Physics.Raycast...

        return 50;
    }

    private void MoveTowardsPlayer()
    {

    }
}
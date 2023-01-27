using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleFSM : MonoBehaviour
{
    private bool mInAttack = false;

    //List all the states the boss could be in
    private enum states
    {
        state1,
        state2,
        none
    }

    private states mCurrentState;
    //private states mNextState;  //mNextState can be added if you need to know what state to go to next while in a state

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
                PickNextState();
                break;

            case states.state1:
                HandleState1();
                break;

            case states.state2:
                HandleState2();
                break;

            default:
                mCurrentState = states.none;
                break;
        }
    }

    private void PickNextState()
    {
        //This was used to pick a random attack, but more complex state selection logic could be put here
        //Or different functions could be called depending on state
        bool valid = false;
        while (valid == false)
        {
            //Make sure not to do the same attack twice
            int nextAttack = Random.Range(0, 1);
            if (nextAttack != (int)mCurrentState)
            {
                valid = true;
                mCurrentState = (states)nextAttack;
            }
        }
    }

    private void HandleState1()
    {
        if (mInAttack == false)
        {
            //Do initialisation for the state here
            mInAttack = true;
        }

        bool done = true; //Put here as an example, this check would probably be done with a member variable
        if (!done)
        {
            //Do whatever the boss should do in this state
            //After that has finished, set done to true
        }
        else
        {
            mInAttack = false; //Set variable for initialisation of next state
            //Either pick the next state, or go straight into a known state if that transition is always wanted
            PickNextState();
            //mCurrentState = x;
        }
    }

    private void HandleState2()
    {
        if (mInAttack == false)
        {
            //Do initialisation for the state here
            mInAttack = true;
        }

        bool done = true; //Put here as an example, this check would probably be done with a member variable
        if (!done)
        {
            //Do whatever the boss should do in this state
            //After that has finished, set done to true
        }
        else
        {
            mInAttack = false; //Set variable for initialisation of next state
            //Either pick the next state, or go straight into a known state if that transition is always wanted
            PickNextState();
            //mCurrentState = x;
        }
    }
}

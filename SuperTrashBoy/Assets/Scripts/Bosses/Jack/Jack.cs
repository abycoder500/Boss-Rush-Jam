using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jack : MonoBehaviour
{
    //This class should be spawned in knowing what phase it is in. A new instance of the boss is spawned for every main phase.

    public int barrelChance = 100;  //Higher values are less likely
    public int hammerChance = 100;
    public int spitChance = 100;
    public int timeWeightingValue = 300; //Higher values mean longer gaps between attacks
    [Range(0f, 100f)]
    public float closeThreshold = 10f; //Range less than which the player is considered close
    [Range(0f, 100f)]
    public float farThreshold = 80f;   //Range greater than which the player is considered far
    public float distanceWeighting = 1.5f;  //Amount by which distance will effect the attack chosen


    private bool mInAttack = false;
    private bool mHasTakenDamage = false;
    private int mAttacksAfterDamageTaken = 0;

    //Variables for choosing attack
    private int mTimeSinceLastAttack = 0;

    protected GameObject player;

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
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            Debug.LogError("No player gameobject found");
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

            case states.throwingBarrels:
                mCurrentState = states.neutral;
                Debug.Log("Throwing Barrels");
                break;

            case states.hammerSwing:
                mCurrentState = states.neutral;
                Debug.Log("Hammer Swing");
                break;

            case states.spinSpit:
                mCurrentState = states.neutral;
                Debug.Log("Spin spit");
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
            mInAttack = true;
        }

        mTimeSinceLastAttack++;

        // Work out what attack to do next based on weightings from player position, time since previous attack and so on
        float playerDistance = GetDistanceFromPlayer();

        //Roll for hammer
        float hammerProb = hammerChance;
        hammerProb = hammerProb * (timeWeightingValue/mTimeSinceLastAttack);
        if (playerDistance < closeThreshold)
            hammerProb /= distanceWeighting;
        else if (playerDistance > farThreshold)
            hammerProb *= distanceWeighting;
        if (Random.Range(0, hammerProb) < 1) //Roll for the attack
        {
            mCurrentState = states.hammerSwing;
            mInAttack = false;
            return;
        }

        //Roll for barrels
        float barrelProb = barrelChance;
        barrelProb = barrelProb * (timeWeightingValue / mTimeSinceLastAttack);
        if (playerDistance < closeThreshold)
            barrelProb *= distanceWeighting;
        else if (playerDistance > farThreshold)
            barrelProb /= distanceWeighting;
        if (Random.Range(0, barrelProb) < 1) //Roll for the attack
        {
            mCurrentState = states.throwingBarrels;
            mInAttack = false;
            return;
        }

        //Roll for spit
        float spitProb = spitChance;
        spitProb = spitProb * (timeWeightingValue / mTimeSinceLastAttack);
        if (Random.Range(0, spitProb) < 1) //Roll for the attack
        {
            mCurrentState = states.spinSpit;
            mInAttack = false;
            return;
        }

        //If we've not hit any of the rolls, stay in movement, just return
    }

    public void SetDamageTaken()
    {
        mHasTakenDamage = true;
    }

    private bool IsPlayerLOS()
    {
        //Draw a ray from the boss to the player, and return false if it hits anything else
        Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit);
        if (hit.collider.gameObject != player.GetComponent<Collider>().gameObject)
            return false;
        else
            return true;
    }

    private float GetDistanceFromPlayer()
    {
        float distance = Vector3.Magnitude(player.transform.position - transform.position);
        return distance;
    }


    //-------------Functions below are TODO!-----------

    private void MoveTowardsPlayer()
    {

    }
}
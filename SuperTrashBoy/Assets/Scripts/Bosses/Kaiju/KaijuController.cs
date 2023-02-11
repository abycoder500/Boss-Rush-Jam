using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuController : MonoBehaviour
{
    private bool mInAttack = false;

    //GameObjects for attacks
    public GameObject fallingTrash;
    public GameObject wind;

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

    //Throw trash variables:
    public float trashDamage = 10;
    public int noOfTrashPiles = 3;
    public float timeBetweenTrashThrows = 0.5f;
    public int noPilesThrown = 0;
    public float trashSpawnHeight = 50f;

    //Wind variables
    public float windTime = 5.0f;
    public float windStrength = 18f;
    public float windHeight = 9.5f;
    private GameObject windInst = null;

    //Variables for flow control
    public float mCurrentAttackTime = 0;

    private GameObject player;
    public GameObject ground;

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
        player = GameObject.FindGameObjectWithTag("Player");
        if (ground == null)
        {
            Debug.LogError("No ground object specified");
        }
    }

    private void FixedUpdate()
    {
        //Kaiju should always be looking at the middle of the arena
        LookAtCenter();
        HandleState();
    }

    private void LookAtCenter()
    {
        transform.LookAt(ground.transform);
        //Make sure Jack isn't tilting up or down
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y);
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
                Roar();
                break;

            case states.throwingTrash:
                ThrowTrash();
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
        mInAttack = false; //Added redundancy

        mCurrentState = state;
    }

    //------Attacks-------

    void ThrowTrash()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mInAttack = true;
            mCurrentAttackTime = Time.time;
            noPilesThrown = 0;
        }

        if (Time.time > mCurrentAttackTime + timeBetweenTrashThrows)
        {
            if (noPilesThrown >= noOfTrashPiles)
            {
                //End state, closing logic goes here
                mInAttack = false;
                ChangeState(states.neutral);
            }
            else
            {
                //Throw a trash pile
                SpawnTrashPile();
                noPilesThrown++;
                mCurrentAttackTime = Time.time;
            }
        }
    }

    private void SpawnTrashPile()
    {
        Vector2 pos = Random.insideUnitCircle * ground.transform.localScale.x;
        GameObject pile = Instantiate(fallingTrash, new Vector3(pos.x, trashSpawnHeight, pos.y), Quaternion.identity);
        pile.GetComponent<FallingTrash>().damage = trashDamage;
    }

    private void Roar()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mInAttack = true;
            mCurrentAttackTime = Time.time;
            windInst = Instantiate(wind, ground.transform.position, transform.rotation);
            windInst.transform.position = new Vector3(windInst.transform.position.x, windHeight, windInst.transform.position.z);
        }

        if (Time.time > mCurrentAttackTime + windTime)
        {
            Destroy(windInst);
            mInAttack = false;
            ChangeState(states.neutral);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuController : MonoBehaviour
{
    [SerializeField] string bossName = "The Kaiju";
    [SerializeField] Animator animator;

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

    //Swipe variables
    public float swipeDamage = 20f;


    //Reposition variables
    public float waitMaximum = 5.0f;
    public float withdrawSpeed = 1.0f;
    public float returnSpeed = 1.0f;
    public float distanceFromCenter = 30.0f;
    public float retreatDistance = 70.0f;
    private Vector2 targetPos;
    private float waitTime;
    private bool movingBack = false;
    private bool waitingToReturn = false;
    private bool movingForward = false;

    //Variables for flow control
    private float mCurrentAttackTime = 0;
    private float roarTime = 1.5f;
    private float swipeTime = 4.5f;
    private bool rotationSet = false;

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
        disarmingRoar,
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
        animator = GetComponentInChildren<Animator>();

        //Start by disarming the player
        ChangeState(states.disarmingRoar);
    }

    private void FixedUpdate()
    {
        //Kaiju should always be looking at the middle of the arena
        LookAtCenter();
        HandleState();
    }

    private void LookAtCenter()
    {

        if (mCurrentState == states.handSlam)
        {
            if (!rotationSet)
            {
                rotationSet = true;
                transform.LookAt(player.transform);
                //Make sure Jack isn't tilting up or down
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y + 50);
            }
        }
        else
        {
            transform.LookAt(ground.transform);
            transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y);
        }


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
                HandSlam();
                break;

            case states.roar:
                Roar();
                break;

            case states.disarmingRoar:
                DisarmingRoar();
                break;

            case states.throwingTrash:
                ThrowTrash();
                break;

            case states.repositioning:
                Reposition();
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
            animator.SetTrigger("isSlowRoar");
        }

        if (Time.time > mCurrentAttackTime + windTime)
        {
            animator.ResetTrigger("isSlowRoar");
            Destroy(windInst);
            mInAttack = false;
            ChangeState(states.neutral);
        }
    }

    private void HandSlam()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mInAttack = true;
            mCurrentAttackTime = Time.time;
            animator.SetTrigger("isSwipeL");
            rotationSet = false;
        }

        if (Time.time > mCurrentAttackTime + swipeTime)
        {
            animator.ResetTrigger("isSwipeL");
            Destroy(windInst);
            mInAttack = false;
            ChangeState(states.neutral);
        }
    }

    private void DisarmingRoar()
    {
        if (!mInAttack)
        {
            mInAttack = true;
            player.GetComponent<Fighter>().UnequipWeapon();
            animator.SetTrigger("isRoar");
            //TODO: put sound here
            mCurrentAttackTime = Time.time;
        }

        if (Time.time > mCurrentAttackTime + roarTime)
        {
            mInAttack = false;
            animator.ResetTrigger("isRoar");
            ChangeState(states.neutral);
        }
    }

    private void Reposition()
    {
        if (!mInAttack)
        {
            //We've hit this point for the first time
            mInAttack = true;
            targetPos = Random.insideUnitCircle.normalized;

            LookAtCenter();
            waitTime = Random.Range(0, waitMaximum);
            movingBack = true;
            waitingToReturn = false;
            movingForward = false;
        }

        if (movingBack)
        {
            transform.position -= transform.forward * withdrawSpeed;
            if ((new Vector2(ground.transform.position.x, ground.transform.position.z) 
                - new Vector2 (transform.position.x, transform.position.z)).magnitude > retreatDistance)
            {
                movingBack = false;
                waitingToReturn = true;
                mCurrentAttackTime = Time.time;
            }
        }

        if (waitingToReturn)
        {
            if (Time.time > mCurrentAttackTime + waitTime)
            {
                Vector2 horizontalPos = new Vector2(ground.transform.position.x, ground.transform.position.z) + targetPos * retreatDistance;
                transform.position = new Vector3(horizontalPos.x, transform.position.y, horizontalPos.y);
                LookAtCenter();
                waitingToReturn = false;
                movingForward = true;
            }
        }

        if (movingForward)
        {
            transform.position += transform.forward * returnSpeed;
            if ((new Vector2(ground.transform.position.x, ground.transform.position.z)
                - new Vector2(transform.position.x, transform.position.z)).magnitude < distanceFromCenter)
            {
                movingForward = false;
                mInAttack = false;
                ChangeState(states.neutral);
            }
        }
    }
}

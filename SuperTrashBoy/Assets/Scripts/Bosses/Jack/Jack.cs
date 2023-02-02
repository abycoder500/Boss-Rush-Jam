using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Jack : MonoBehaviour
{
    //This class should be spawned in knowing what phase it is in. A new instance of the boss is spawned for every main phase.
    [SerializeField] string bossName = "Jack in the box";
    [SerializeField] Animator animator;
    [SerializeField] HitBox hammerHitbox;

    //Attacks
    public GameObject wave;

    //Materials for coloured attacks:
    public Material redAttack;
    public Material blueAttack;
    public Material yellowAttack;
    public Material purpleAttack;

    //Variables for choosing attacks
    public int barrelChance = 100;  //Higher values are less likely
    public int hammerChance = 100;
    public int spitChance = 100;
    public int timeWeightingValue = 300; //Higher values mean longer gaps between attacks
    [Range(0f, 100f)]
    public float closeThreshold = 10f; //Range less than which the player is considered close
    [Range(0f, 100f)]
    public float farThreshold = 80f;   //Range greater than which the player is considered far
    public float distanceWeighting = 1.5f;  //Amount by which distance will effect the attack chosen
    private int mTimeSinceLastAttack = 0;

    //Variables for attack parameters
    public float waveTime = 1f; //Time in seconds the wave will last
    public float waveSpeed = 0.1f; //Speed of the wave
    public float waveDamage = 10f;
    public float hammerAttackDamage = 20f;

    //Variables for flow control
    private bool mInAttack = false;
    private bool mHasTakenDamage = false;
    private int mAttacksAfterDamageTaken = 0;
    public bool hammerDown = false;
    private bool mAnimationFinished = false;    //Set true when an animation has finished, set back to false
                                                //when that's handled

    protected GameObject player;

    public event Action<string> onActivate;

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

        animator = GetComponent<Animator>();

        //Todo: put this where it actually happens
        onActivate?.Invoke(bossName);
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
                HammerAttack();
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

    //Helper functions

    public void SetDamageTaken()
    {
        mHasTakenDamage = true;
    }

    //-----Animation functions

    public void AnimationFinished()
    {
        mAnimationFinished = true;
    }

    public void SetHammerDown()
    {
        hammerDown = true;
    }

    //-----End of Animation functions

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

    //Attack functions

    private void HammerAttack()
    {
        if (!mInAttack)
        {
            Debug.Log("Hammer Swing");
            //Set up
            LookAtPlayer();
            animator.SetTrigger("isHammerAttack");
            mInAttack = true;
            hammerHitbox.SetupHitBox(this.gameObject, hammerAttackDamage);
            hammerHitbox.gameObject.SetActive(true);
        }

        if (hammerDown)
        {
            SpawnHammerWave();
            hammerDown = false;
        }


        if (mAnimationFinished)
        {
            hammerHitbox.gameObject.SetActive(false);
            animator.ResetTrigger("isHammerAttack");
            mAnimationFinished = false;
            //Leaving the state
            mInAttack = false;
            mCurrentState = states.neutral;       
        }
    }

    private void SpawnHammerWave()
    {
        GameObject waveInstance = Instantiate(wave, transform);
        waveInstance.SetActive(true);
        waveInstance.GetComponent<WaveAttackController>().SetSpeedAndLifetime(waveSpeed, waveTime);
        HitBox waveHitbox = waveInstance.GetComponent<HitBox>();
        waveHitbox.SetupHitBox(gameObject, waveDamage);
        //Set attack colour
        MeshRenderer[] rend = waveInstance.gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in rend)
        {
            if (render.gameObject != gameObject)
                render.material = redAttack;
        }
    }

    private void LookAtPlayer()
    {
        transform.LookAt(player.transform);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y);
    }

    //-------------Functions below are TODO!-----------

    private void MoveTowardsPlayer()
    {
        LookAtPlayer();
    }
}
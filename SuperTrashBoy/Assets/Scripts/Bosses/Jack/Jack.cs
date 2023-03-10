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
    public GameObject barrel;
    public GameObject bullet;

    //Materials for coloured attacks:
    private Material[] normalAttacks;
    private Material finalAttack;

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
    public float jackMovementSpeed = 0.1f;
    //Hammer
    public float waveTime = 1f; //Time in seconds the wave will last
    public float waveSpeed = 0.1f; //Speed of the wave
    public float waveDamage = 10f;
    public float hammerAttackDamage = 20f;
    private float slamAnimTime = 1.5f;
    private float hammerDownTime = 0.5f;
    //Barrels
    public float barrelSpawnHeight = 50f;
    public int noOfBarrels = 5;
    public float barrelSpawnDelay = 0.5f;
    public float barrelDamage = 20f;
    //Spit
    public float spitTime = 5f;
    public float spitRate = 0.2f;
    public float spinSpeed = 10f;
    public float bulletDamage = 5f;
    public float bulletSpeed = 1f;
    public float bulletLifetime = 1f;
    public Vector3 bulletOffset = new Vector3(0, 2);

    public int attacksForPhaseChange = 3;

    //Variables for flow control
    private bool mInAttack = false;
    private bool mHasTakenDamage = false;
    private int mAttacksAfterDamageTaken = 0;
    //private bool mAnimationFinished = false;    //Set true when an animation has finished, set back to false
    //when that's handled
    private bool hammerDown = false;
    private float mAttackStartTime;
    private float barrelThrowStart = 0;
    private int barrelsThrown = 0;
    private float spitSpinStart = 0;
    private float lastBulletTime = 0;

    private bool mNoHealth = false;
    private bool oneLastAttackDone = false;

    private Material lastAttack;

    protected GameObject player;
    private FightManager fightManager;
    public Health health;

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

        fightManager = FindObjectOfType<FightManager>();
        if (fightManager == null)
            Debug.LogError("No fight manager found");
        else
        {
            normalAttacks = fightManager.attackMaterials;
            finalAttack = fightManager.finalAttackMaterial;
        }

        animator = GetComponentInChildren<Animator>();

        health.onTakeDamage += SetDamageTaken;
    }

    public void GetActivated()
    {
        onActivate?.Invoke(bossName);
    }

    public void SetHealth(float value)
    {
        health.currentHealth = value;
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
                ThrowBarrels();
                break;

            case states.hammerSwing:
                HammerAttack();
                break;

            case states.spinSpit:
                SpinSpit();
                break;

            default:
                mCurrentState = states.none;
                break;
        }
    }

    private void HandleNeutral()
    {
        if (mAttacksAfterDamageTaken >= attacksForPhaseChange)
        {
            if (oneLastAttackDone == true || mNoHealth == false)  //Make sure there is always a purple attack after running out of health
            {
                fightManager.SeekPhase(lastAttack, health.GetCurrentHealth(), mNoHealth);
                Destroy(gameObject);
            }
        }
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

        if (!IsPlayerLOS()) //Don't attack if we can't see the player
            return;

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
            animator.ResetTrigger("isMoving");
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
            animator.ResetTrigger("isMoving");
            return;
        }

        //Roll for spit
        float spitProb = spitChance;
        spitProb = spitProb * (timeWeightingValue / mTimeSinceLastAttack);
        if (Random.Range(0, spitProb) < 1) //Roll for the attack
        {
            mCurrentState = states.spinSpit;
            mInAttack = false;
            animator.ResetTrigger("isMoving");
            return;
        }

        //If we've not hit any of the rolls, stay in movement, just return
    }

    //Helper functions

    public void SetDamageTaken(float i, Transform t)
    {
        if (t.gameObject != gameObject)
        {
            mHasTakenDamage = true;
            if (health.GetComponent<Health>().currentHealth <= 0)
                mNoHealth = true;
        }
    }
    private bool IsPlayerLOS()
    {
        //Draw a ray from the boss to the player, and return false if it hits anything else
        Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit);
        if (hit.collider == null)
            return false;
        else if (hit.collider.gameObject == null)
            return false;
        else if (hit.collider.gameObject != player.GetComponent<Collider>().gameObject)
            return false;
        else
            return true;
    }

    private float GetDistanceFromPlayer()
    {
        float distance = Vector3.Magnitude(player.transform.position - transform.position);
        return distance;
    }

    //-----Animation functions

    public void AnimationFinished()
    {
        //mAnimationFinished = true;
    }

    public void SetHammerDown()
    {
        hammerDown = true;
    }

    //-----End of Animation functions

    //Attack functions
    private void HammerAttack()
    {
        if (!mInAttack)
        {
            Debug.Log("Hammer Swing");
            //Set up
            LookAtPlayer();
            //animator.SetTrigger("isHammerAttack");
            animator.Play("jack_rig|jack_slam");
            mInAttack = true;
            hammerDown = false;
            hammerHitbox.SetupHitBox(gameObject, hammerAttackDamage);
            hammerHitbox.gameObject.SetActive(true);
            mAttackStartTime = Time.time;
        }

        if (Time.time > mAttackStartTime + hammerDownTime && hammerDown == false)
        {
            SpawnHammerWave();
            hammerDown = true;
        }

        if (Time.time > mAttackStartTime + slamAnimTime)
        {
            hammerHitbox.gameObject.SetActive(false);
            animator.ResetTrigger("isHammerAttack");
            //Leaving the state
            mInAttack = false;
            mCurrentState = states.neutral;
            if (mHasTakenDamage)
                mAttacksAfterDamageTaken++;
        }
    }

    private void SpawnHammerWave()
    {
        GameObject waveInstance = Instantiate(wave, transform.position, transform.rotation);
        waveInstance.transform.Rotate(new Vector3(90, 0, 0));
        waveInstance.transform.position += new Vector3(0, 0.4f, 0);
        waveInstance.SetActive(true);
        waveInstance.GetComponent<WaveAttackController>().SetSpeedAndLifetime(waveSpeed, waveTime);
        HitBox waveHitbox = waveInstance.GetComponent<HitBox>();
        waveHitbox.SetupHitBox(gameObject, waveDamage);
        //Set attack colour
        MeshRenderer[] rend = waveInstance.gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in rend)
        {
            if (render.gameObject != gameObject)
                render.material = ChooseMaterial();
        }
    }

    private void ThrowBarrels()
    {
        if (!mInAttack)
        {
            Debug.Log("Barrel Throw");
            //Set up
            LookAtPlayer();
            animator.SetTrigger("isSwinging");
            mInAttack = true;
            barrelThrowStart = Time.time;
            barrelsThrown = 0;
            ChooseMaterial();
        }

        if (Time.time > barrelThrowStart + barrelSpawnDelay)
        {
            if (barrelsThrown < noOfBarrels)
            {
                SpawnBarrel();
                barrelsThrown++;
                barrelThrowStart = Time.time;
            }
            else
            {
                //We've spawned all the barrels we need to, end the attack
                animator.ResetTrigger("isSwinging");
                mCurrentState = states.neutral;
                mInAttack = false;
                if (mHasTakenDamage)
                    mAttacksAfterDamageTaken++;
            }
        }
    }

    private void SpawnBarrel()
    {
        Vector3 spawnPos = player.transform.position + Vector3.up * barrelSpawnHeight;
        GameObject barrelInstance = Instantiate(barrel, spawnPos, Quaternion.identity);
        barrelInstance.GetComponent<BarrelExplode>().SetupAttack(gameObject, barrelDamage);
        barrelInstance.GetComponent<MeshRenderer>().material = lastAttack;
    }

    private void SpinSpit()
    {
        if (!mInAttack)
        {
            Debug.Log("Spin Spit");
            //Set up
            LookAtPlayer();
            //animator.SetTrigger("isHammerAttack");
            mInAttack = true;
            spitSpinStart = Time.time;
            lastBulletTime = Time.time;
            ChooseMaterial();
        }

        if (Time.time > lastBulletTime + spitRate)
        {
            SpitBullet();
        }

        gameObject.transform.Rotate(new Vector3(0, spinSpeed));

        if (Time.time > spitSpinStart + spitTime)
        {
            //Attack finished
            LookAtPlayer();
            mCurrentState = states.neutral;
            mInAttack = false;
            if (mHasTakenDamage)
                mAttacksAfterDamageTaken++;
        }
    }

    private void SpitBullet()
    {
        GameObject bulletInst = Instantiate(bullet, transform.position + bulletOffset, transform.rotation);
        bulletInst.GetComponent<Bullet>().SetUpBullet(bulletLifetime, bulletSpeed, bulletDamage, gameObject);
        bulletInst.GetComponent<Renderer>().material = lastAttack;
    }

    private void LookAtPlayer()
    {
        transform.LookAt(player.transform);
        //Make sure Jack isn't tilting up or down
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y);
    }

    private void MoveTowardsPlayer()
    {
        transform.position += transform.forward * jackMovementSpeed;
        animator.SetTrigger("isMoving");
        LookAtPlayer();
    }

    private Material ChooseMaterial()
    {
        if (mNoHealth && mAttacksAfterDamageTaken == attacksForPhaseChange)
        {
            lastAttack = finalAttack;
            oneLastAttackDone = true;
            return finalAttack;
        }
        else
        {
            int i = Random.Range(0, normalAttacks.Length);
            lastAttack = normalAttacks[i];
            return normalAttacks[i];
        }
    }

    public string GetBossName()
    {
        return bossName;
    }
}
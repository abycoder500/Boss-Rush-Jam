using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    [SerializeField] private GameObject jack;
    [SerializeField] private GameObject miniJack;
    [SerializeField] private GameObject box;

    [SerializeField] private Transform startSpot;
    [SerializeField] public Transform[] spawnSpots;
    [SerializeField] private Transform finalSpot;

    [SerializeField] public Material[] attackMaterials;
    [SerializeField] public Material finalAttackMaterial;

    [SerializeField] public Material[] miniMaterials;   //Must be ordered the same as the attack materials
    [SerializeField] public Material finalMiniMaterial;
    [SerializeField] public DialogueTrigger beforeFightDialogue;

    [SerializeField] private float startingHealth = 200;
    private float currentHealth;
    private Jack jackInstance;
    private AudioManager audioManager;

    public event Action<string> onActivate;

    private void Awake() 
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {      
        currentHealth = startingHealth;
        GameObject jackInst = Instantiate(jack, startSpot.position, startSpot.rotation);
        jackInstance = jackInst.GetComponent<Jack>();
        jackInstance.SetHealth(currentHealth);
        jackInstance.enabled = false;
        beforeFightDialogue.StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartTheFight()
    {
        jackInstance.enabled = true;
        jackInstance.GetActivated();
        onActivate?.Invoke(jackInstance.GetBossName());
        
        if(audioManager == null) return;
        Debug.Log($"FightManager got audioManager: {audioManager.name}");
        audioManager.PlayJackBossMusic();
        
    }

    public void SeekPhase(Material lastAttack, float remainingHealth, bool isFinal)
    {
        if (audioManager != null)
        {
            Debug.Log($"FightManager got audioManager: {audioManager.name}");
            audioManager.StopMusic(null);
            audioManager.PlayJackSeekMusic();
        }

        //Destroy all miniJacks
        MiniDummySpit[] minis = FindObjectsOfType<MiniDummySpit>();
        for (int i = 0; i < minis.Length; i++)
        {
            Destroy(minis[i].gameObject.transform.parent.gameObject);
        }

        int lastAttackIndex = -1;
        if (!isFinal)
        {
            for (int i = 0; i < attackMaterials.Length; i++)
            {
                if (lastAttack == attackMaterials[i])
                {
                    lastAttackIndex = i;
                    break;
                }
            }
        }

        if (remainingHealth > 10)
            currentHealth = remainingHealth;
        else
            currentHealth = 10;

        //Spawn the correct box
        int randLocation;
        if (isFinal)
        {
            //Spawn the end box on finalSpot here
            randLocation = -1;  //Allow boxes to spawn in all other spots
            GameObject boxInst = Instantiate(box, finalSpot.position, finalSpot.rotation);
            boxInst.GetComponentInChildren<ClosedBoxScript>().SetFinalBox();
            boxInst.GetComponentInChildren<Renderer>().material = finalMiniMaterial;
            //To do: Make box smaller
        }
        else
        {
            randLocation = UnityEngine.Random.Range(0, spawnSpots.Length);
            GameObject boxInst = Instantiate(box, spawnSpots[randLocation].position, spawnSpots[randLocation].rotation);
            if(boxInst!=null)
            {   
                boxInst.GetComponentInChildren<ClosedBoxScript>().SetUpBox(false);
                boxInst.GetComponentInChildren<Renderer>().material = miniMaterials[lastAttackIndex];
            }
        }

        //Spawn the other boxes
        for (int i = 0; i < spawnSpots.Length; i++)
        {
            if (i != randLocation)
            {
                bool valid = false;
                int j = 0;
                while (valid == false)
                {
                    j = UnityEngine.Random.Range(0, attackMaterials.Length);
                    if (attackMaterials[j] != lastAttack)
                        valid = true;
                }
                GameObject fakeBoxInst = Instantiate(box, spawnSpots[i].position, spawnSpots[i].rotation);
                fakeBoxInst.GetComponentInChildren<ClosedBoxScript>().SetUpBox(true);
                fakeBoxInst.GetComponentInChildren<Renderer>().material = miniMaterials[j];
            }
        }
    }

    public void SpawnMiniJack(Transform spot)
    {
        Instantiate(miniJack, spot.position, spot.rotation);
    }

    public void SpawnJack(Transform spot)
    {
        if (audioManager != null)
        {
            Debug.Log($"FightManager got audioManager: {audioManager.name}");
            audioManager.StopMusic(null);
            audioManager.PlayJackBossMusic();
        }
        GameObject jackInst = Instantiate(jack, spot.position, spot.rotation);
        jackInst.GetComponent<Jack>().SetHealth(currentHealth);
    }

    public void AwakeBoxes()
    {
        ClosedBoxScript[] boxes = FindObjectsOfType<ClosedBoxScript>();
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].Activate();
        }
    }
    public void RemoveBoxes()
    {
        ClosedBoxScript[] boxes = FindObjectsOfType<ClosedBoxScript>();
        for (int i = 0; i < boxes.Length; i++)
        {
            Destroy(boxes[i].gameObject);
        }
    }

    private void HandleEndOfFight()
    {
        Debug.Log("You win!");
    }
}

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

    [SerializeField] private float startingHealth = 200;
    private float currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        GameObject jackInst = Instantiate(jack, startSpot.position, startSpot.rotation);
        jackInst.GetComponent<Jack>().SetHealth(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SeekPhase(Material lastAttack, float remainingHealth, bool isFinal)
    {

        currentHealth = remainingHealth;
        //Spawn the correct box
        int randLocation;
        if (isFinal)
        {
            //Spawn the end box on finalSpot here
            randLocation = -1;  //Allow boxes to spawn in all other spots
            GameObject boxInst = Instantiate(box, finalSpot.position, finalSpot.rotation);
            boxInst.GetComponent<ClosedBoxScript>().SetFinalBox();
            boxInst.GetComponent<Renderer>().material = lastAttack;
        }
        else
        {
            randLocation = Random.Range(0, spawnSpots.Length);
            GameObject boxInst = Instantiate(box, spawnSpots[randLocation].position, spawnSpots[randLocation].rotation);
            boxInst.GetComponent<ClosedBoxScript>().SetUpBox(false);
            boxInst.GetComponent<Renderer>().material = lastAttack;
        }

        //Spawn the other boxes
        for (int i = 0; i< spawnSpots.Length; i++)
        {
            if (i != randLocation)
            {
                bool valid = false;
                int j = 0;
                while (valid == false)
                {
                    j = Random.Range(0, attackMaterials.Length);
                    if (attackMaterials[j] != lastAttack)
                        valid = true;
                }
                GameObject fakeBoxInst = Instantiate(box, spawnSpots[i].position, spawnSpots[i].rotation);
                fakeBoxInst.GetComponent<ClosedBoxScript>().SetUpBox(true);
                fakeBoxInst.GetComponent<Renderer>().material = attackMaterials[j];
            }
        }
    }

    public void SpawnMiniJack(Transform spot)
    {
        Instantiate(miniJack, spot.position, spot.rotation);
    }

    public void SpawnJack(Transform spot)
    {
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
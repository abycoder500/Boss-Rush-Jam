using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrash : MonoBehaviour
{
    //[SerializeField] float explodeHeight = -4f;

    public GameObject destructablePile;
    public float damage;
    public int spawnChance = 3;

    public float cullingHeight = -20;

    private GameObject ground;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<HitBox>().SetupHitBox(gameObject, damage);
        ground = GameObject.FindGameObjectWithTag("Ground");
    }

    private void FixedUpdate()
    {
        if (transform.position.y < cullingHeight)
            Destroy(gameObject);

        return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == ground)
        {
            SpawnTrash(collision);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnTrash(Collision collision)
    {
        //Roll to see if this spawns a trash pile
        if (Random.Range(0, spawnChance) != 0)
            return;
        if (GetComponentInChildren<HitBox>() != null)
            GetComponentInChildren<HitBox>().gameObject.SetActive(false);
        if (destructablePile!= null)
        {
            //To do: Make sure the trash pile isn't embedded in the ground (unless that looks good?)
            Instantiate(destructablePile, collision.GetContact(0).point, Quaternion.identity);
        }
    }

    private void SpawnTrash()
    {
        if (GetComponentInChildren<HitBox>() != null)
            GetComponentInChildren<HitBox>().gameObject.SetActive(false);
        if (destructablePile != null)
        {
            Instantiate(destructablePile, transform.position, Quaternion.identity);
        }
    }
}

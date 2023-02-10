using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTrash : MonoBehaviour
{
    [SerializeField] float explodeHeight = -4f;

    public GameObject destructablePile;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<HitBox>().SetupHitBox(gameObject, damage);
    }

    private void FixedUpdate()
    {
        return;
        /*
        //for turning into trash by height, not collision
        if (transform.position.y < explodeHeight)
        {
            SpawnTrash();
            Destroy(gameObject);
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
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

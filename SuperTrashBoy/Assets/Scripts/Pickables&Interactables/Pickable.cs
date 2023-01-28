using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Pickable : MonoBehaviour
{
    [SerializeField] private bool interactWithPlayerOnly = true;
    [SerializeField] private float lifeTime = -1;

    private Rigidbody rb;
    private Collider coll;
    
    public UnityEvent OnPicked;

    protected GameObject player;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();   
        coll = GetComponent<Collider>(); 
    }

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (lifeTime > 0)
        {
            Destroy(this.gameObject, lifeTime);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(interactWithPlayerOnly)
        {
            if (other.gameObject == player)
            {
                Collect();
            }
        }
        else
        {
            Collect();
        }    
    }

    protected virtual void Collect()
    {
        OnPicked?.Invoke();
        Destroy(this.gameObject);
    }

    public void Spawn(Vector3 force)
    {
        StartCoroutine(SpawnWithGravityRoutine(force));
    }

    private IEnumerator SpawnWithGravityRoutine(Vector3 force)
    {
        coll.isTrigger = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);
        yield return new WaitForSeconds(2f);
        rb.useGravity = false;
        rb.isKinematic = true;
        coll.isTrigger = true;
        

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickable : Pickable
{
    [SerializeField] private float healAmount;

    private Rigidbody rb;
    private Collider coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }


    protected override void Collect()
    {
        if (player == null) return;

        Health playerHealth = player.GetComponent<Health>();

        if(playerHealth.TryHeal(healAmount)) base.Collect();
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

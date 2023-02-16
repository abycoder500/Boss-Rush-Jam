using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    private GameObject instigator;
    private Weapon weapon;
    private float damage;
    private bool collide = true;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void Start() 
    {
        Destroy(gameObject, lifeTime);    
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!collide) return;
        Debug.Log("collided with " + other.name);
        if(other.gameObject == this) return;
        if(other.gameObject == instigator) return;
        if(other.gameObject == weapon.gameObject) return;

        if(other.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.TryTakeDamage(damage, instigator.transform);
            Debug.Log("Hit!");
        }

        Destroy(this.gameObject);
    }

    public void Launch(Vector3 launchForce, float damage, GameObject instigator, Weapon weapon)
    {
        this.instigator = instigator; 
        this.damage = damage;
        this.weapon = weapon;
        rb.AddForce(launchForce, ForceMode.Impulse);
    }

    public void Launch(Vector3 launchForce, float damage, GameObject instigator, Weapon weapon, bool collide)
    {
        this.collide = collide;
        Launch(launchForce, damage, instigator, weapon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;

    private GameObject instigator;
    private float damage;

    private void Start() 
    {
        ActivateHitBox(false, null, 0f);    
    }

    public void ActivateHitBox(bool activate, GameObject instigator, float damage)
    {
        Debug.Log("Activated!");
        boxCollider.enabled = activate;
        Debug.Log(boxCollider.enabled);
        this.instigator = instigator;
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Entered something");
        if (other.gameObject == instigator) return;
        if (other.gameObject == this.gameObject) return;    

        if (other.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.TakeDamage(damage);
            Debug.Log("attacked!");
        }
    }
}

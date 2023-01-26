using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private GameObject instigator;
    private float damage;

    public void SetupHitBox(GameObject instigator, float damage)
    {
        this.instigator = instigator;
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject == instigator) return;
        if (other.gameObject == this.gameObject) return;    

        if (other.TryGetComponent<HitReceivedCounter>(out HitReceivedCounter counter))
        {
            counter.Hit();
            if(other.GetComponent<Health>() != null) gameObject.SetActive(false);
        }

        if (other.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.TakeDamage(damage);
            Debug.Log("attacked!");
            gameObject.SetActive(false);
        }
        
    }
}

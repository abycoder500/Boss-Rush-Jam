using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    private GameObject instigator;
    private float damage;

    public event Action<GameObject> onHit;
    public UnityEvent OnHit;

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
            Debug.Log("hit");
            counter.Hit();
            if(other.GetComponent<Health>() == null) gameObject.SetActive(false);
        }

        if (other.TryGetComponent<Health>(out Health targetHealth))
        {
            Debug.Log("Health");
            if(targetHealth.TryTakeDamage(damage, instigator.transform))
            {
                gameObject.SetActive(false);
                onHit?.Invoke(other.gameObject);
                OnHit?.Invoke();
            }
        }
    }
}

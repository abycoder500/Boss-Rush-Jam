using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float startingHealth = 100f;
    [SerializeField] bool dieOnOneHit = false;
    [SerializeField] bool canBeHitByPlayer = true;

    private GameObject player;

    private float maxHealth;
    public float currentHealth;

    public event Action<float, Transform> onTakeDamage;
    public event Action<float> onHeal;
    public event Action onDie;

    public UnityEvent OnDeath;

    void Awake()
    {
        maxHealth = startingHealth;
        currentHealth = maxHealth;  

        player = GameObject.FindGameObjectWithTag("Player");      
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetHealthFraction()
    {
        return currentHealth/maxHealth;
    }

    public bool TryTakeDamage(float damageAmount, Transform damager)
    {
        if(!canBeHitByPlayer && damager.gameObject == player) return false;

        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
        onTakeDamage?.Invoke(damageAmount, damager);

        if (currentHealth <= 0f || dieOnOneHit) 
        {
            onDie?.Invoke();
            OnDeath?.Invoke();
            gameObject.SetActive(false);
        }
        return true;
    }

    public bool TryHeal(float healAmount)
    {
        if(currentHealth >= maxHealth) return false;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        onHeal?.Invoke(healAmount);
        return true;
    }
}

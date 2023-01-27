using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] float startingHealth = 100f;

    private float maxHealth;
    public float currentHealth;

    public event Action<float, Transform> onTakeDamage;
    public event Action<float> onHeal;
    public event Action onDie;

    public UnityEvent OnDeath;

    void Start()
    {
        maxHealth = startingHealth;
        currentHealth = maxHealth;        
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(float damageAmount, Transform damager)
    {
        currentHealth = Mathf.Max(0f, currentHealth - damageAmount);
        onTakeDamage?.Invoke(damageAmount, damager);

        if (currentHealth <= 0f) 
        {
            onDie?.Invoke();
            OnDeath?.Invoke();
        }
    }

    public bool TryHeal(float healAmount)
    {
        if(currentHealth >= maxHealth) return false;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        onHeal?.Invoke(healAmount);
        return true;
    }
}

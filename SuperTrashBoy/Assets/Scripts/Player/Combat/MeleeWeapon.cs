using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float hitBoxActivationTime;
    [SerializeField] private float hitBoxDeactivationTime;
    [SerializeField] private float attackEndTime; 
    [SerializeField] private HitBox hitBox;

    private void Start() 
    {
        hitBox.gameObject.SetActive(false);    
    }

    public override void Attack(GameObject instigator, Action AttackFinished)
    {
        hitBox.SetupHitBox(instigator, damage);
        StartCoroutine(HitBoxRoutine(AttackFinished));  
    }

    private IEnumerator HitBoxRoutine(Action AttackFinished)
    {
        yield return new WaitForSeconds(hitBoxActivationTime);
        hitBox.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitBoxDeactivationTime - hitBoxActivationTime);
        hitBox.gameObject.SetActive(false);
        yield return new WaitForSeconds(attackEndTime - hitBoxDeactivationTime - hitBoxActivationTime);
        AttackFinished();
    }
}

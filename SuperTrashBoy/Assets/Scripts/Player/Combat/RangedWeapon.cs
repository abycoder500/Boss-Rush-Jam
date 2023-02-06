using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MeleeWeapon
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float timeBetweenFireAttacks = 2f;

    public void Fire(GameObject instigator, Action AttackFinished)
    {
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint);
        projectile.transform.parent = null;
        projectile.Launch(launchForce * transform.forward, damage, instigator, this);
        StartCoroutine(ResetAttack(AttackFinished, timeBetweenFireAttacks)); 
    }

    private IEnumerator ResetAttack(Action AttackFinished, float time)
    {
        yield return new WaitForSeconds(time);
        AttackFinished();
    }
}

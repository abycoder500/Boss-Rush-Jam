using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float timeBetweenAttacks = 2f;

    public override void Attack(GameObject instigator, Action AttackFinished)
    {
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoint);
        projectile.transform.parent = null;
        projectile.Launch(launchForce * transform.forward, damage, instigator, this);
        StartCoroutine(ResetAttack(AttackFinished));
    }

    private IEnumerator ResetAttack(Action AttackFinished)
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        AttackFinished();
    }
}
